package handlers

import (
	"bufio"
	"encoding/json"
	"log"
	"net"

	"vampiretcp/config"
	"vampiretcp/logger"
	"vampiretcp/models"
	"vampiretcp/utils"
)

type ClientHandler struct {
	conn   net.Conn
	state  *models.ServerState
	config *config.ServerConfig
	logger *logger.Logger
}

func NewClientHandler(conn net.Conn, state *models.ServerState, config *config.ServerConfig, logger *logger.Logger) *ClientHandler {
	return &ClientHandler{
		conn:   conn,
		state:  state,
		config: config,
		logger: logger,
	}
}

func (h *ClientHandler) Handle() {
	defer h.conn.Close()

	h.state.Mu.Lock()
	clientID := h.state.NextClientID
	h.state.NextClientID++
	h.state.ClientIDs[h.conn] = clientID
	h.state.Mu.Unlock()

	h.logger.Info("Client %d connected", clientID)

	// Send the client their ID
	response := models.Response{
		Action:  "clientId",
		Message: "Your client ID is",
		Value:   clientID,
	}
	h.sendResponse(response)

	defer h.cleanupClient(clientID)

	reader := bufio.NewReader(h.conn)

	for {
		data, err := reader.ReadString('\n')
		if err != nil {
			h.logger.Warning("Client %d read error: %v", clientID, err)
			break
		}
		data = data[:len(data)-1]

		var msg models.Message
		if err := json.Unmarshal([]byte(data), &msg); err != nil {
			h.logger.Warning("Client %d JSON unmarshal error: %v", clientID, err)
			continue
		}

		h.handleMessage(msg, clientID)
	}
}

func (h *ClientHandler) handleMessage(msg models.Message, clientID int) {
	switch msg.Action {
	case "createRoom":
		h.handleCreateRoom(clientID, msg.IsPublic)
	case "joinRoom":
		h.handleJoinRoom(msg.RoomCode, clientID)
	case "listRooms":
		h.handleListRooms(clientID)
	case "broadcast":
		h.handleBroadcast(msg, clientID)
	default:
		h.sendError("Unknown action")
		h.logger.Warning("Client %d sent unknown action: %s", clientID, msg.Action)
	}
}

func (h *ClientHandler) handleCreateRoom(clientID int, public bool) {
	roomCode := utils.GenerateRoomCode(h.config.RoomCodeLength)
	h.state.Mu.Lock()
	h.state.Rooms[roomCode] = append(h.state.Rooms[roomCode], h.conn)
	h.state.ClientRooms[h.conn] = roomCode
	h.state.PublicRooms[roomCode] = public
	h.state.Mu.Unlock()

	response := models.Response{
		Action:   "roomCreated",
		RoomCode: roomCode,
		Value:    public,
	}
	h.sendResponse(response)
	h.logger.Info("Client %d created room %s", clientID, roomCode)
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
		h.logger.Info("Client %d joined room %s", clientID, roomCode)
	} else {
		h.state.Mu.Unlock()
		h.sendError("Room not found")
		h.logger.Warning("Client %d attempted to join non-existent room %s", clientID, roomCode)
	}
}

func (h *ClientHandler) handleListRooms(clientID int) {
	h.state.Mu.Lock()
	publicRooms := make([]string, 0, len(h.state.PublicRooms))
	for roomCode, isPublic := range h.state.PublicRooms {
		if isPublic {
			publicRooms = append(publicRooms, roomCode)
		}
	}
	h.state.Mu.Unlock()

	response := models.Response{
		Action: "roomsList",
		Value:  publicRooms,
	}
	h.sendResponse(response)
	h.logger.Info("Client %d listed rooms", clientID)
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
		h.logger.Warning("Client %d attempted to broadcast without being in a room", clientID)
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
				h.logger.Info("Room %s deleted", roomCode)
				delete(h.state.Rooms, roomCode)
				delete(h.state.PublicRooms, roomCode)
			} else {
				leaveMsg := models.Response{
					Action:  "leave",
					Message: "A user has left the room",
					From:    clientID,
				}
				for _, client := range h.state.Rooms[roomCode] {
					sendToClient(client, leaveMsg)
				}
			}
		}
		delete(h.state.ClientRooms, h.conn)
	}

	h.logger.Info("Client %d disconnected", clientID)
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
	h.logger.Warning("Sent error: %s", message)
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
