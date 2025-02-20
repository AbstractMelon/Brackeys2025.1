package config

type ServerConfig struct {
    Host            string `json:"host"`
    Port            int    `json:"port"`
    RoomCodeLength  int    `json:"room_code_length"`
    MaxPlayersRoom  int    `json:"max_players_room"`
    LogFile         string `json:"log_file"`
    EnableDebug     bool   `json:"enable_debug"`
}

func DefaultConfig() *ServerConfig {
    return &ServerConfig{
        Host:           "0.0.0.0",
        Port:           8888,
        RoomCodeLength: 6,
        MaxPlayersRoom: 100,
        LogFile:        "server.log",
        EnableDebug:    false,
    }
}
