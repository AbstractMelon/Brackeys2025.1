package main

import (
	"bufio"
	"encoding/json"
	"fmt"
	"log"
	"math/rand"
	"net"
	"os"
	"runtime"
	"strings"
	"sync"
	"time"
)

type ServerState struct {
	mu           sync.Mutex
	rooms        map[string][]net.Conn
	clientRooms  map[net.Conn]string
	clientIDs    map[net.Conn]int
	nextClientID int
}

const charset = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"

func init() {
	rand.Seed(time.Now().UnixNano())
}

func generateRoomCode(length int) string {
	b := make([]byte, length)
	for i := range b {
		b[i] = charset[rand.Intn(len(charset))]
	}
	return string(b)
}

func handleClient(conn net.Conn, state *ServerState) {
	defer conn.Close()

	state.mu.Lock()
	clientID := state.nextClientID
	state.nextClientID++
	state.clientIDs[conn] = clientID
	state.mu.Unlock()

	log.Printf("Client %d connected", clientID)

	defer func() {
		state.mu.Lock()
		defer state.mu.Unlock()

		delete(state.clientIDs, conn)

		if roomCode, ok := state.clientRooms[conn]; ok {
			if conns, ok := state.rooms[roomCode]; ok {
				for i, c := range conns {
					if c == conn {
						state.rooms[roomCode] = append(conns[:i], conns[i+1:]...)
						break
					}
				}
				if len(state.rooms[roomCode]) == 0 {
					log.Printf("Room %s deleted", roomCode)
					delete(state.rooms, roomCode)
				}
			}
			delete(state.clientRooms, conn)
		}

		log.Printf("Client %d disconnected", clientID)
	}()

	reader := bufio.NewReader(conn)

	for {
		data, err := reader.ReadString('\n')
		if err != nil {
			log.Printf("Client %d read error: %v", clientID, err)
			break
		}
		data = data[:len(data)-1]

		var msg map[string]interface{}
		if err := json.Unmarshal([]byte(data), &msg); err != nil {
			log.Printf("Client %d JSON unmarshal error: %v", clientID, err)
			continue
		}

		action, ok := msg["action"].(string)
		if !ok {
			log.Printf("Client %d sent invalid action", clientID)
			continue
		}

		switch action {
		case "createRoom":
			roomCode := generateRoomCode(6)
			state.mu.Lock()
			state.rooms[roomCode] = append(state.rooms[roomCode], conn)
			state.clientRooms[conn] = roomCode
			state.mu.Unlock()

			response := map[string]interface{}{
				"action":    "roomCreated",
				"room_code": roomCode,
			}
			sendResponse(conn, response)
			log.Printf("Client %d created room %s", clientID, roomCode)

		case "joinRoom":
			roomCode, ok := msg["room_code"].(string)
			if !ok {
				sendError(conn, "Invalid room code")
				continue
			}

			state.mu.Lock()
			if conns, ok := state.rooms[roomCode]; ok {
				state.rooms[roomCode] = append(conns, conn)
				state.clientRooms[conn] = roomCode
				state.mu.Unlock()

				response := map[string]interface{}{
					"action":    "joinedRoom",
					"room_code": roomCode,
				}
				sendResponse(conn, response)
				log.Printf("Client %d joined room %s", clientID, roomCode)
			} else {
				state.mu.Unlock()
				sendError(conn, "Room not found")
				log.Printf("Client %d attempted to join non-existent room %s", clientID, roomCode)
			}

		case "listRooms":
			state.mu.Lock()
			roomCodes := make([]string, 0, len(state.rooms))
			for code := range state.rooms {
				roomCodes = append(roomCodes, code)
			}
			state.mu.Unlock()

			response := map[string]interface{}{
				"action": "roomsList",
				"rooms":  roomCodes,
			}
			sendResponse(conn, response)
			log.Printf("Client %d listed rooms", clientID)

		case "broadcast":
			state.mu.Lock()
			roomCode, inRoom := state.clientRooms[conn]
			var clients []net.Conn
			if inRoom {
				clients = make([]net.Conn, len(state.rooms[roomCode]))
				copy(clients, state.rooms[roomCode])
			}
			state.mu.Unlock()

			if !inRoom {
				sendError(conn, "Not in a room")
				log.Printf("Client %d attempted to broadcast without being in a room", clientID)
				continue
			}

			message, _ := msg["message"].(string)
			value := msg["value"]

			broadcastMsg := map[string]interface{}{
				"action":  "broadcast",
				"message": message,
				"value":   value,
				"from":    clientID,
			}

			for _, client := range clients {
				if client != conn {
					sendResponse(client, broadcastMsg)
				}
			}
			// log.Printf("Client %d broadcasted message in room %s", clientID, roomCode)

		default:
			sendError(conn, "Unknown action")
			log.Printf("Client %d sent unknown action: %s", clientID, action)
		}
	}
}

func sendResponse(conn net.Conn, response map[string]interface{}) {
	jsonData, err := json.Marshal(response)
	if err != nil {
		log.Printf("Error marshaling response: %v", err)
		return
	}
	jsonData = append(jsonData, '\n')
	conn.Write(jsonData)
	log.Printf("Sent response: %v", response)
}

func sendError(conn net.Conn, message string) {
	sendResponse(conn, map[string]interface{}{
		"action":  "error",
		"message": message,
	})
	log.Printf("Sent error: %s", message)
}

func dumpState(state *ServerState) {
	state.mu.Lock()
	defer state.mu.Unlock()

	fmt.Println("Connected players:")
	for conn, id := range state.clientIDs {
		fmt.Printf("%d - %s\n", id, conn.RemoteAddr().String())
	}

	fmt.Println("Created rooms:")
	for roomCode, conns := range state.rooms {
		fmt.Printf("%s - %d players\n", roomCode, len(conns))
	}
	log.Println("State dumped")
}

func usage(state *ServerState) {
	var mem runtime.MemStats
	runtime.ReadMemStats(&mem)
	fmt.Printf("\rUsage: %s players connected, %s rooms, Memory usage: %s, CPU usage: %s \n", 
		formatNumber(len(state.clientIDs)), 
		formatNumber(len(state.rooms)), 
		formatBytes(mem.Alloc), 
		formatBytes(mem.Sys))
	log.Println("Usage stats displayed")
}

func formatNumber(n int) string {
	return fmt.Sprintf("%d", n)
}

func formatBytes(b uint64) string {
	const unit = 1024
	if b < unit {
		return fmt.Sprintf("%d B", b)
	}
	div, exp := unit, 0
	for n := b / unit; n >= unit; n /= unit {
		div *= unit
		exp++
	}
	return fmt.Sprintf("%.1f %ciB", float64(b)/float64(div), "KMGTPE"[exp])
}

func main() {
	state := &ServerState{
		rooms:        make(map[string][]net.Conn),
		clientRooms:  make(map[net.Conn]string),
		clientIDs:    make(map[net.Conn]int),
		nextClientID: 1,
	}

	listener, err := net.Listen("tcp", "127.0.0.1:8888")
	if err != nil {
		log.Fatal(err)
	}
	defer listener.Close()

	log.Printf("Server listening on %s", listener.Addr().String())

	go func() {
		scanner := bufio.NewScanner(os.Stdin)
		for scanner.Scan() {
			command := strings.TrimSpace(scanner.Text())
			switch command {
			case "dump":
				dumpState(state)
			case "help":
				fmt.Println("dump - dump the current state of the server")
				fmt.Println("help - show this help")
				fmt.Println("stop - stop the server")
				fmt.Println("usage - show the usage of the server")
			case "stop":
				log.Println("Server stopping...")
				os.Exit(0)
			case "usage":
				usage(state)
			default:
				fmt.Println("Unknown command")
				log.Printf("Unknown command: %s", command)
			}
		}
	}()

	for {
		conn, err := listener.Accept()
		if err != nil {
			log.Printf("Accept error: %v", err)
			continue
		}
		go handleClient(conn, state)
	}
}

