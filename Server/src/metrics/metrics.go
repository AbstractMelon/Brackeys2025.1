package metrics

import (
	"sync"
	"time"
)

type Metrics struct {
    mu sync.RWMutex

    // Connection metrics
    totalConnections     int64
    currentConnections   int64
    failedConnections    int64
    
    // Room metrics
    totalRoomsCreated   int64
    currentRooms        int64
    
    // Message metrics
    messagesProcessed   int64
    messagesFailed      int64
    
    // Performance metrics
    avgProcessingTime   float64
    peakConnections     int64
    lastUpdateTime      time.Time
}

func NewMetrics() *Metrics {
    return &Metrics{
        lastUpdateTime: time.Now(),
    }
}

func (m *Metrics) RecordConnection() {
    m.mu.Lock()
    defer m.mu.Unlock()
    
    m.totalConnections++
    m.currentConnections++
    
    if m.currentConnections > m.peakConnections {
        m.peakConnections = m.currentConnections
    }
}

func (m *Metrics) RecordDisconnection() {
    m.mu.Lock()
    defer m.mu.Unlock()
    
    m.currentConnections--
}

func (m *Metrics) RecordFailedConnection() {
    m.mu.Lock()
    defer m.mu.Unlock()
    
    m.failedConnections++
}

func (m *Metrics) RecordRoomCreated() {
    m.mu.Lock()
    defer m.mu.Unlock()
    
    m.totalRoomsCreated++
    m.currentRooms++
}

func (m *Metrics) RecordRoomClosed() {
    m.mu.Lock()
    defer m.mu.Unlock()
    
    m.currentRooms--
}

func (m *Metrics) RecordMessageProcessed(processingTime time.Duration) {
    m.mu.Lock()
    defer m.mu.Unlock()
    
    m.messagesProcessed++
    
    // Update average processing time
    oldTotal := m.avgProcessingTime * float64(m.messagesProcessed-1)
    newTotal := oldTotal + float64(processingTime.Microseconds())
    m.avgProcessingTime = newTotal / float64(m.messagesProcessed)
}

func (m *Metrics) RecordFailedMessage() {
    m.mu.Lock()
    defer m.mu.Unlock()
    
    m.messagesFailed++
}

func (m *Metrics) GetSnapshot() map[string]interface{} {
    m.mu.RLock()
    defer m.mu.RUnlock()
    
    return map[string]interface{}{
        "total_connections":      m.totalConnections,
        "current_connections":    m.currentConnections,
        "failed_connections":     m.failedConnections,
        "total_rooms_created":    m.totalRoomsCreated,
        "current_rooms":          m.currentRooms,
        "messages_processed":     m.messagesProcessed,
        "messages_failed":        m.messagesFailed,
        "avg_processing_time_us": m.avgProcessingTime,
        "peak_connections":       m.peakConnections,
        "uptime_seconds":        time.Since(m.lastUpdateTime).Seconds(),
    }
}