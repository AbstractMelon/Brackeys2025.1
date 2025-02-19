package handlers

import (
	"bufio"
	"encoding/json"
	"log"
	"net"

	"vampriretcp/config"
	"vampriretcp/models"
	"vampriretcp/utils"
)

type ClientHandler struct {
    conn   net.Conn
    state  *models.ServerState
    config *config.ServerConfig
}

func NewClientHandler(conn net.Conn, state *models.ServerState, config *config.ServerConfig) *ClientHandler {
    return &ClientHandler{
        conn:   conn,
        state:  state,
        config: config,
    }
}

func (h *ClientHandler) Handle() {
    defer h.conn.Close()

    h.state.Mu.Lock()
    clientID := h.state.NextClientID
    h.state.NextClientID++
    h.state.ClientIDs[h.conn] = clientID
    h.state.Mu.Unlock()

    log.Printf("Client %d connected", clientID)

    defer h.cleanupClient(clientID)

    reader := bufio.NewReader(h.conn)

    for {
        data, err := reader.ReadString('\n')
        if err != nil {
            log.Printf("Client %d read error: %v", clientID, err)
            break
        }
        data = data[:len(data)-1]

        var msg models.Message
        if err := json.Unmarshal([]byte(data), &msg); err != nil {
            log.Printf("Client %d JSON unmarshal error: %v", clientID, err)
            continue
        }

        h.handleMessage(msg, clientID)
    }
}

func (h *ClientHandler) handleMessage(msg models.Message, clientID int) {
    switch msg.Action {
    case "createRoom":
        h.handleCreateRoom(clientID)
    case "joinRoom":
        h.handleJoinRoom(msg.RoomCode, clientID)
    case "listRooms":
        h.handleListRooms(clientID)
    case "broadcast":
        h.handleBroadcast(msg, clientID)
    default:
        h.sendError("Unknown action")
        log.Printf("Client %d sent unknown action: %s", clientID, msg.Action)
    }
}

func (h *ClientHandler) handleCreateRoom(clientID int) {
    roomCode := utils.GenerateRoomCode(h.config.RoomCodeLength)
    h.state.Mu.Lock()
    h.state.Rooms[roomCode] = append(h.state.Rooms[roomCode], h.conn)
    h.state.ClientRooms[h.conn] = roomCode
    h.state.Mu.Unlock()

    response := models.Response{
        Action:   "roomCreated",
        RoomCode: roomCode,
    }
    h.sendResponse(response)
    log.Printf("Client %d created room %s", clientID, roomCode)
}

func (h *ClientHandler) handleJoinRoom(roomCode string, clientID int) {
    h.state.Mu.Lock()
    if conns, ok := h.state.Rooms[roomCode]; ok {
        if len(conns) >= h.config.MaxPlayersRoom {
            h.state.Mu.Unlock()
            h.sendError("Room is full")
            return
        }
        h.state.Rooms[roomCode] = append(conns, h.conn)
        h.state.ClientRooms[h.conn] = roomCode
        h.state.Mu.Unlock()

        response := models.Response{
            Action:   "joinedRoom",
            RoomCode: roomCode,
        }
        h.sendResponse(response)
        log.Printf("Client %d joined room %s", clientID, roomCode)
    } else {
        h.state.Mu.Unlock()
        h.sendError("Room not found")
        log.Printf("Client %d attempted to join non-existent room %s", clientID, roomCode)
    }
}

func (h *ClientHandler) handleListRooms(clientID int) {
    h.state.Mu.Lock()
    roomCodes := make([]string, 0, len(h.state.Rooms))
    for code := range h.state.Rooms {
        roomCodes = append(roomCodes, code)
    }
    h.state.Mu.Unlock()

    response := models.Response{
        Action: "roomsList",
        Value:  roomCodes,
    }
    h.sendResponse(response)
    log.Printf("Client %d listed rooms", clientID)
}

func (h *ClientHandler) handleBroadcast(msg models.Message, clientID int) {
    h.state.Mu.Lock()
    roomCode, inRoom := h.state.ClientRooms[h.conn]
    var clients []net.Conn
    if inRoom {
        clients = make([]net.Conn, len(h.state.Rooms[roomCode]))
        copy(clients, h.state.Rooms[roomCode])
    }
    h.state.Mu.Unlock()

    if !inRoom {
        h.sendError("Not in a room")
        log.Printf("Client %d attempted to broadcast without being in a room", clientID)
        return
    }

    broadcastMsg := models.Response{
        Action:  "broadcast",
        Message: msg.Message,
        Value:   msg.Value,
        From:    clientID,
    }

    for _, client := range clients {
        if client != h.conn {
            sendToClient(client, broadcastMsg)
        }
    }
}

func (h *ClientHandler) cleanupClient(clientID int) {
    h.state.Mu.Lock()
    defer h.state.Mu.Unlock()

    delete(h.state.ClientIDs, h.conn)

    if roomCode, ok := h.state.ClientRooms[h.conn]; ok {
        if conns, ok := h.state.Rooms[roomCode]; ok {
            for i, c := range conns {
                if c == h.conn {
                    h.state.Rooms[roomCode] = append(conns[:i], conns[i+1:]...)
                    break
                }
            }
            if len(h.state.Rooms[roomCode]) == 0 {
                log.Printf("Room %s deleted", roomCode)
                delete(h.state.Rooms, roomCode)
            }
        }
        delete(h.state.ClientRooms, h.conn)
    }

    log.Printf("Client %d disconnected", clientID)
}

func (h *ClientHandler) sendResponse(response models.Response) {
    sendToClient(h.conn, response)
}

func (h *ClientHandler) sendError(message string) {
    response := models.Response{
        Action:  "error",
        Message: message,
    }
    h.sendResponse(response)
    log.Printf("Sent error: %s", message)
}

func sendToClient(conn net.Conn, response models.Response) {
    jsonData, err := json.Marshal(response)
    if err != nil {
        log.Printf("Error marshaling response: %v", err)
        return
    }
    jsonData = append(jsonData, '\n')
    conn.Write(jsonData)
    log.Printf("Sent response: %v", response)
}