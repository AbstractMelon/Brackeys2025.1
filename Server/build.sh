#!/bin/bash

# Create build directories if they don't exist
mkdir -p builds/linux
mkdir -p builds/windows

# Build for Linux
GOOS=linux GOARCH=amd64 go build -o builds/linux/server ./src/main.go

# Build for Windows
GOOS=windows GOARCH=amd64 go build -o builds/windows/server.exe ./src/main.go

echo "Builds completed successfully."


