package main

import (
	"encoding/json"
	"fmt"
	"io"
	"log"
	"math/rand"
	"net"
	"strings"
	"sync"
	"time"
)

type message struct {
	Action  string      `json:"action"`
	Message string      `json:"message,omitempty"`
	Value   interface{} `json:"value,omitempty"`
	From    int         `json:"from,omitempty"`
}

var (
	rooms       = make(map[string][]net.Conn)
	clientRooms = make(map[net.Conn]string)
	clientIds   = make(map[net.Conn]int)
	nextClientId int
	roomLock    = sync.RWMutex{}
	clientsLock = sync.RWMutex{}
)

func generateRoomCode(length int) string {
	rand.Seed(time.Now().UnixNano())
	b := make([]byte, length)
	for i := range b {
		b[i] = letterBytes[rand.Intn(len(letterBytes))]
	}
	return string(b)
}

var letterBytes = []byte("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")

func handleClient(conn net.Conn) {
	defer conn.Close()
	clientsLock.Lock()
	clientId := nextClientId
	clientIds[conn] = clientId
	nextClientId++
	clientsLock.Unlock()
	log.Printf("Client %d connected at %s", clientId, time.Now().Format(time.RFC3339))

	defer func() {
		// Cleanup on disconnect
		clientsLock.Lock()
		roomCode, inRoom := clientRooms[conn]
		delete(clientRooms, conn)
		delete(clientIds, conn)
		clientsLock.Unlock()

		if inRoom {
			roomLock.Lock()
			if clients, ok := rooms[roomCode]; ok {
				for i, c := range clients {
					if c == conn {
						newClients := append(clients[:i], clients[i+1:]...)
						if len(newClients) == 0 {
							delete(rooms, roomCode)
						} else {
							rooms[roomCode] = newClients
						}
						break
					}
				}
			}
			roomLock.Unlock()
		}
		log.Printf("Client %d disconnected", clientId)
	}()

	for {
		buf := make([]byte, 1024)
		n, err := conn.Read(buf)
		if err != nil {
			if err != io.EOF {
				log.Printf("Error reading from client %d: %v", clientId, err)
			}
			break
		}

		var msg message
		err = json.Unmarshal(buf[:n], &msg)
		if err != nil {
			continue
		}

		switch msg.Action {
		case "createRoom":
			roomCode := generateRoomCode(6)
			roomLock.Lock()
			rooms[roomCode] = append(rooms[roomCode], conn)
			roomLock.Unlock()
			clientsLock.Lock()
			clientRooms[conn] = roomCode
			clientsLock.Unlock()
			log.Printf("Client %d created room %s at %s", clientId, roomCode, time.Now().Format(time.RFC3339))
			conn.Write([]byte(fmt.Sprintf(`{"action": "roomCreated", "room_code": "%s"}`, roomCode)))

		case "joinRoom":
			roomCode := msg.Message
			roomLock.Lock()
			if clients, ok := rooms[roomCode]; ok {
				rooms[roomCode] = append(clients, conn)
				roomLock.Unlock()
				clientsLock.Lock()
				clientRooms[conn] = roomCode
				clientsLock.Unlock()
				log.Printf("Client %d joined room %s at %s", clientId, roomCode, time.Now().Format(time.RFC3339))
				conn.Write([]byte(fmt.Sprintf(`{"action": "joinedRoom", "room_code": "%s"}`, roomCode)))
			} else {
				roomLock.Unlock()
				conn.Write([]byte(`{"action": "error", "message": "Room not found"}`))
			}

		case "listRooms":
			publicRooms := make([]string, 0, len(rooms))
			roomLock.RLock()
			for roomCode := range rooms {
				publicRooms = append(publicRooms, roomCode)
			}
			roomLock.RUnlock()
			conn.Write([]byte(fmt.Sprintf(`{"action": "roomsList", "rooms": %s}`, json.RawMessage(`["`+strings.Join(publicRooms, `","`)+`"]`))))

		case "broadcast":
			clientsLock.RLock()
			roomCode := clientRooms[conn]
			clientsLock.RUnlock()
			if roomCode != "" {
				broadcastMessage := message{
					Action:  "broadcast",
					Message: msg.Message,
					Value:   msg.Value,
					From:    clientId,
				}
				roomLock.RLock()
				clients := make([]net.Conn, len(rooms[roomCode]))
				copy(clients, rooms[roomCode])
				roomLock.RUnlock()
				for _, clientConn := range clients {
					if clientConn != conn {
						jsonBytes, err := json.Marshal(broadcastMessage)
						if err != nil {
							log.Println(err)
						} else {
							clientConn.Write(jsonBytes)
						}
					}
				}
				log.Printf("Client %d broadcasted message in room %s at %s", clientId, roomCode, time.Now().Format(time.RFC3339))
			} else {
				conn.Write([]byte(`{"action": "error", "message": "Not in a room"}`))
			}
		default:
			conn.Write([]byte(`{"action": "error", "message": "Unknown action"}`))
		}
	}
}

func main() {
	listener, err := net.Listen("tcp", "127.0.0.1:8888")
	if err != nil {
		log.Fatal(err)
	}
	defer listener.Close()

	log.Println("Server started!")

	for {
		conn, err := listener.Accept()
		if err != nil {
			log.Fatal(err)
		}
		go handleClient(conn)
	}
}