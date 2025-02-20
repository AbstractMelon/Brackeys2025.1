package heartbeat

import (
	"encoding/json"
	"log"
	"net"
	"time"

	"vampiretcp/models"
)

type HeartbeatMonitor struct {
    clients        map[net.Conn]time.Time
    timeout        time.Duration
    checkInterval  time.Duration
}

func NewHeartbeatMonitor(timeout, interval time.Duration) *HeartbeatMonitor {
    return &HeartbeatMonitor{
        clients:       make(map[net.Conn]time.Time),
        timeout:      timeout,
        checkInterval: interval,
    }
}

func (h *HeartbeatMonitor) Start() {
    ticker := time.NewTicker(h.checkInterval)
    go func() {
        for range ticker.C {
            h.checkConnections()
        }
    }()
}

func (h *HeartbeatMonitor) RegisterClient(conn net.Conn) {
    h.clients[conn] = time.Now()
}

func (h *HeartbeatMonitor) UpdateClient(conn net.Conn) {
    h.clients[conn] = time.Now()
}

func (h *HeartbeatMonitor) RemoveClient(conn net.Conn) {
    delete(h.clients, conn)
}

func (h *HeartbeatMonitor) checkConnections() {
    now := time.Now()
    for conn, lastSeen := range h.clients {
        if now.Sub(lastSeen) > h.timeout {
            log.Printf("Client %v timed out", conn.RemoteAddr())
            conn.Close()
            h.RemoveClient(conn)
        }
    }
}

func (h *HeartbeatMonitor) SendHeartbeat(conn net.Conn) error {
    response := models.Response{
        Action: "heartbeat",
        Value:  time.Now().Unix(),
    }
    
    jsonData, err := json.Marshal(response)
    if err != nil {
        return err
    }
    
    jsonData = append(jsonData, '\n')
    _, err = conn.Write(jsonData)
    return err
}