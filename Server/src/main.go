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
			clientRooms[conn] = roomCode
			roomLock.Unlock()
			log.Printf("Client %d created room %s at %s", clientId, roomCode, time.Now().Format(time.RFC3339))
			conn.Write([]byte(fmt.Sprintf(`{"action": "roomCreated", "room_code": "%s"}`, roomCode)))

		case "joinRoom":
			roomCode := msg.Message
			roomLock.RLock()
			if _, ok := rooms[roomCode]; ok {
				rooms[roomCode] = append(rooms[roomCode], conn)
				clientRooms[conn] = roomCode
				roomLock.RUnlock()
				log.Printf("Client %d joined room %s at %s", clientId, roomCode, time.Now().Format(time.RFC3339))
				conn.Write([]byte(fmt.Sprintf(`{"action": "joinedRoom", "room_code": "%s"}`, roomCode)))
			} else {
				roomLock.RUnlock()
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
			roomCode := clientRooms[conn]
			if roomCode != "" {
				broadcastMessage := message{
					Action:  "broadcast",
					Message: msg.Message,
					Value:   msg.Value,
					From:    clientId,
				}
				roomLock.RLock()
				for _, clientConn := range rooms[roomCode] {
					if clientConn != conn {
						jsonBytes, err := json.Marshal(broadcastMessage)
						if err != nil {
							log.Println(err)
						} else {
							clientConn.Write(jsonBytes)
						}
					}
				}
				roomLock.RUnlock()
				log.Printf("Client %d broadcasted message in room %s at %s", clientId, roomCode, time.Now().Format(time.RFC3339))
			} else {
				conn.Write([]byte(`{"action": "error", "message": "Not in a room"}`))
			}
		default:
			conn.Write([]byte(`{"action": "error", "message": "Unknown action"}`))
		}
	}
}

// main starts a TCP server on localhost:8888 that accepts incoming connections
// and starts a new goroutine to handle each client connection.
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