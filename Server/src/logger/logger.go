package logger

import (
	"fmt"
	"io"
	"log"
	"os"
	"path/filepath"
	"runtime"
	"time"
)

type Logger struct {
    debugMode bool
    logFile   *os.File
}

func NewLogger(filename string, debug bool) (*Logger, error) {
    var file *os.File
    var err error


    if filename != "" {
        // Create logs directory if it doesn't exist
        dir := filepath.Dir(filename)
        if err := os.MkdirAll(dir, 0755); err != nil {
            return nil, fmt.Errorf("failed to create log directory: %v", err)
        }

        // Open log file
        file, err = os.OpenFile(filename, os.O_RDWR|os.O_CREATE|os.O_APPEND, 0666)
        if err != nil {
            return nil, fmt.Errorf("failed to open log file: %v", err)
        }

        // Configure standard logger
        log.SetOutput(io.MultiWriter(file, os.Stdout))
    } else {
        log.SetOutput(os.Stdout)
    }

    if debug {
        log.SetFlags(log.Ldate | log.Ltime | log.Lshortfile)
    } else {
        log.SetFlags(log.Ldate | log.Ltime)
    }

    return &Logger{
        debugMode: debug,
        logFile:   file,
    }, nil
}

func (l *Logger) Close() error {
    if l.logFile != nil {
        return l.logFile.Close()
    }
    return nil
}

func (l *Logger) Debug(format string, v ...interface{}) {
    if l.debugMode {
        _, file, line, _ := runtime.Caller(1)
        log.Printf("[DEBUG] "+format+" (%s:%d)", append(v, filepath.Base(file), line)...)
    }
}

func (l *Logger) Info(format string, v ...interface{}) {
    log.Printf("[INFO] "+format, v...)
}

func (l *Logger) Warning(format string, v ...interface{}) {
    log.Printf("[WARN] "+format, v...)
}

func (l *Logger) Error(format string, v ...interface{}) {
    log.Printf("[ERROR] "+format, v...)
}

func (l *Logger) Fatal(format string, v ...interface{}) {
    log.Printf("[FATAL] "+format, v...)
    os.Exit(1)
}

func (l *Logger) RotateLogs() error {
    if l.logFile == nil {
        return nil
    }

    // Close current log file
    l.logFile.Close()

    // Generate timestamp for the old log file
    timestamp := time.Now().Format("20060102150405")
    oldPath := l.logFile.Name()
    newPath := fmt.Sprintf("%s.%s", oldPath, timestamp)

    // Rename current log file
    if err := os.Rename(oldPath, newPath); err != nil {
        return fmt.Errorf("failed to rotate log file: %v", err)
    }

    // Remove old log files
    dir := filepath.Dir(oldPath)
    files, err := os.ReadDir(dir)
    if err != nil {
        return fmt.Errorf("failed to read log directory: %v", err)
    }

    for _, file := range files {
        if file.Name() != filepath.Base(oldPath) && file.Name() != filepath.Base(newPath) {
            if err := os.Remove(filepath.Join(dir, file.Name())); err != nil {
                return fmt.Errorf("failed to remove old log file: %v", err)
            }
        }
    }

    // Open new log file
    file, err := os.OpenFile(oldPath, os.O_RDWR|os.O_CREATE|os.O_APPEND, 0666)
    if err != nil {
        return fmt.Errorf("failed to create new log file: %v", err)
    }

    l.logFile = file
    log.SetOutput(io.MultiWriter(file, os.Stdout))
    return nil
}