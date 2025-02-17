import asyncio
import json
import random
import string

rooms = {}
client_rooms = {}
client_ids = {}
next_client_id = 1


def generate_room_code(length=6):
    return "".join(random.choices(string.ascii_uppercase + string.digits, k=length))


async def handle_client(reader, writer):
    global next_client_id
    client_id = next_client_id
    next_client_id += 1
    client_ids[writer] = client_id
    print(f"Client {client_id} connected.")

    try:
        while True:
            data = await reader.readline()
            if not data:
                break
            message_str = data.decode().strip()
            if not message_str:
                continue

            try:
                message = json.loads(message_str)
            except json.JSONDecodeError:
                continue

            action = message.get("action")
            if action == "createRoom":
                room_code = generate_room_code()
                rooms.setdefault(room_code, []).append(writer)
                client_rooms[writer] = room_code
                response = {"action": "roomCreated", "room_code": room_code}
                writer.write((json.dumps(response) + "\n").encode())
                await writer.drain()
                print(f"Client {client_id} created room {room_code}.")

            elif action == "joinRoom":
                room_code = message.get("room_code")
                if room_code in rooms:
                    rooms[room_code].append(writer)
                    client_rooms[writer] = room_code
                    response = {"action": "joinedRoom", "room_code": room_code}
                    writer.write((json.dumps(response) + "\n").encode())
                    await writer.drain()
                    print(f"Client {client_id} joined room {room_code}.")
                else:
                    response = {"action": "error", "message": "Room not found"}
                    writer.write((json.dumps(response) + "\n").encode())
                    await writer.drain()

            elif action == "listRooms":
                public_rooms = list(rooms.keys())
                response = {"action": "roomsList", "rooms": public_rooms}
                writer.write((json.dumps(response) + "\n").encode())
                await writer.drain()
                print(f"Client {client_id} requested list of rooms.")

            elif action == "broadcast":
                room_code = client_rooms.get(writer)
                print(room_code)
                if room_code:
                    broadcast_message = {
                        "action": "broadcast",
                        "message": message.get("message", ""),
                        "value": message.get("value"),
                        "from": client_id,
                    }
                    for client_writer in rooms.get(room_code, []):
                        if client_writer != writer:
                            try:
                                client_writer.write(
                                    (json.dumps(broadcast_message) + "\n").encode()
                                )
                                await client_writer.drain()
                            except ConnectionResetError:
                                continue
                    print(
                        f"Client {client_id} broadcasted message in room {room_code}."
                    )
                else:
                    response = {"action": "error", "message": "Not in a room"}
                    writer.write((json.dumps(response) + "\n").encode())
                    await writer.drain()
            else:
                response = {"action": "error", "message": "Unknown action"}
                writer.write((json.dumps(response) + "\n").encode())
                await writer.drain()

    except (ConnectionResetError, asyncio.IncompleteReadError):
        print(f"Client {client_id} disconnected abruptly.")
    except Exception as e:
        print(f"Error with client {client_id}: {e}")
    finally:
        room_code = client_rooms.get(writer)
        if room_code and writer in rooms.get(room_code, []):
            rooms[room_code].remove(writer)
            if not rooms[room_code]:
                del rooms[room_code]
        if writer in client_ids:
            print(f"Client {client_ids[writer]} disconnected.")
            del client_ids[writer]
        try:
            writer.close()
            await writer.wait_closed()
        except ConnectionResetError:
            pass


async def main():
    server = await asyncio.start_server(handle_client, "127.0.0.1", 8888)
    addrs = ", ".join(str(sock.getsockname()) for sock in server.sockets)
    print(f"Serving on {addrs}")
    async with server:
        await server.serve_forever()


if __name__ == "__main__":
    asyncio.run(main())
