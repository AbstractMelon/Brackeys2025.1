package models

import (
	"net"
	"sync"
)

type ServerState struct {
	Mu           sync.Mutex
	Rooms        map[string][]net.Conn
	PublicRooms  map[string]bool
	ClientRooms  map[net.Conn]string
	ClientIDs    map[net.Conn]int
	NextClientID int
}

func NewServerState() *ServerState {
	return &ServerState{
		Rooms:        make(map[string][]net.Conn),
		PublicRooms:  make(map[string]bool),
		ClientRooms:  make(map[net.Conn]string),
		ClientIDs:    make(map[net.Conn]int),
		NextClientID: 1,
	}
}

type Message struct {
	Action   string      `json:"action"`
	RoomCode string      `json:"room_code,omitempty"`
	IsPublic bool        `json:"publicRoom,omitempty"`
	Message  string      `json:"message,omitempty"`
	Value    interface{} `json:"value,omitempty"`
	From     int         `json:"from,omitempty"`
	Audio    []byte      `json:"audio,omitempty"`
}

type Response struct {
	Action   string      `json:"action"`
	RoomCode string      `json:"room_code,omitempty"`
	Message  string      `json:"message,omitempty"`
	Value    interface{} `json:"value,omitempty"`
	From     int         `json:"from,omitempty"`
	Error    string      `json:"error,omitempty"`
}

type AudioBroadcastMessage struct {
	Action string `json:"action"`
	From   int    `json:"from"`
	Audio  []byte `json:"audio"`
}

