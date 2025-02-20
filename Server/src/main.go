package main

import (
	"bufio"
	"encoding/json"
	"fmt"
	"log"
	"net"
	"os"
	"runtime"
	"strings"

	"vampiretcp/config"
	"vampiretcp/handlers"
	"vampiretcp/models"
	"vampiretcp/utils"
)

func setupLogging(cfg *config.ServerConfig) {
    if cfg.LogFile != "" {
        f, err := os.OpenFile(cfg.LogFile, os.O_RDWR|os.O_CREATE|os.O_APPEND, 0666)
        if err != nil {
            log.Fatalf("Error opening log file: %v", err)
        }
        log.SetOutput(f)
    }

    if cfg.EnableDebug {
        log.SetFlags(log.Ldate | log.Ltime | log.Lshortfile)
    }
}

func loadConfig() *config.ServerConfig {
    cfg := config.DefaultConfig()
    
    // Try to load from config file
    file, err := os.Open("config.json")
    if err == nil {
        defer file.Close()
        decoder := json.NewDecoder(file)
        if err := decoder.Decode(cfg); err != nil {
            log.Printf("Error reading config file: %v, using defaults", err)
        }
    }
    
    return cfg
}

func dumpState(state *models.ServerState) {
    state.Mu.Lock()
    defer state.Mu.Unlock()

    fmt.Println("Connected players:")
    for conn, id := range state.ClientIDs {
        fmt.Printf("%d - %s\n", id, conn.RemoteAddr().String())
    }

    fmt.Println("Created rooms:")
    for roomCode, conns := range state.Rooms {
        fmt.Printf("%s - %d players\n", roomCode, len(conns))
    }
    log.Println("State dumped")
}

func showUsage(state *models.ServerState) {
    var mem runtime.MemStats
    runtime.ReadMemStats(&mem)
    fmt.Printf("\rUsage: %s players connected, %s rooms, Memory usage: %s, CPU usage: %s \n",
        utils.FormatNumber(len(state.ClientIDs)),
        utils.FormatNumber(len(state.Rooms)),
        utils.FormatBytes(mem.Alloc),
        utils.FormatBytes(mem.Sys))
    log.Println("Usage stats displayed")
}

func handleCommands(state *models.ServerState) {
    scanner := bufio.NewScanner(os.Stdin)
    for scanner.Scan() {
        command := strings.TrimSpace(scanner.Text())
        switch command {
        case "dump":
            dumpState(state)
        case "help":
            fmt.Println("Available commands:")
            fmt.Println("  dump   - dump the current state of the server")
            fmt.Println("  help   - show this help")
            fmt.Println("  stop   - stop the server")
            fmt.Println("  usage  - show the usage statistics")
        case "stop":
            log.Println("Server stopping...")
            os.Exit(0)
        case "usage":
            showUsage(state)
        default:
            fmt.Println("Unknown command. Type 'help' for available commands.")
            log.Printf("Unknown command: %s", command)
        }
    }
}

func main() {
    cfg := loadConfig()
    setupLogging(cfg)
    
    state := models.NewServerState()
    
    addr := fmt.Sprintf("%s:%d", cfg.Host, cfg.Port)
    listener, err := net.Listen("tcp", addr)
    if err != nil {
        log.Fatal(err)
    }
    defer listener.Close()

    log.Printf("Server listening on %s", listener.Addr().String())

    // Start command handler
    go handleCommands(state)

    // Accept connections
    for {
        conn, err := listener.Accept()
        if err != nil {
            log.Printf("Accept error: %v", err)
            continue
        }
        clientHandler := handlers.NewClientHandler(conn, state, cfg)
        go clientHandler.Handle()
    }
}