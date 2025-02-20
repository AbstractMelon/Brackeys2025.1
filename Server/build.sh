#!/bin/bash

# Navigate into the source directory
cd src

# Create build directories if they don't exist
mkdir -p ../builds/linux
mkdir -p ../builds/windows

# Build for Linux
GOOS=linux GOARCH=amd64 go build -o ../builds/linux/server main.go

# Build for Windows
GOOS=windows GOARCH=amd64 go build -o ../builds/windows/server.exe main.go

echo "Builds completed successfully."

# Navigate out and create archives
cd ../builds
tar -czf linux.tar.gz linux
zip -r windows.zip windows
cd ..

