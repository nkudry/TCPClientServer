from datetime import datetime
import socket
import threading

#Variables for holding information about connections
connections = []
total_connections = 0

#Define buffer size
BUFFER_SIZE = 1024

#Display logs in proper format
def Log(str):
    time = datetime.now().strftime("%d-%m-%Y %H:%M")
    print(f"[{time}] [Server] {str}")

# Check if string is null or empty
def IsNullOrEmpty(x):
    nullorempty = False
    if not(x):
        nullorempty = True
    if x.isspace():
        nullorempty = True    
    return nullorempty 
    
#Client class, new instance is created for each connected client which holds all the information
class Client(threading.Thread):
    def __init__(self, socket, address, id, name, signal):
        threading.Thread.__init__(self)
        self.socket = socket
        self.address = address
        self.id = id
        self.name = name
        self.signal = signal
    
    def __str__(self):
        return str(self.address[0]) + ":" + str(self.address[1])
    
    #Attempt to get data from client
    #If unable to, assume client has disconnected and remove him from server data
    #If able to and we get data back, print it in the server and send it back
    def run(self):
        while self.signal:
            try:
                data = self.socket.recv(BUFFER_SIZE)
                if not IsNullOrEmpty(data):
                    message = str(data.decode("utf-8"));
                    Log(f"[{self}] Client has sent a message: \"{message}\", returning \"{message}\" to Client")
                    self.socket.sendall(f"{message}".encode(encoding='utf-8'))
                else:
                    Log(f"[{self}] Client has sent an empty message")
                    self.socket.sendall(f"[Error] Received empty message".encode(encoding='utf-8'))
            except:
                Log(f"[{self}] Client has disconnected")
                self.signal = False
                connections.remove(self)
                break


#Wait for new connections
def newConnections(socket):
    while True:
        sock, address = socket.accept()
        global total_connections
        client = Client(sock, address, total_connections, "Name", True);
        connections.append(client)
        connections[len(connections) - 1].start()
        Log(f"[{client}] Client has connected")
        total_connections += 1

def main():
    host = "0.0.0.0"
    port = 54321
    
    Log(f"Starting at {host}:{port}")

    #Create new server socket
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    sock.bind((host, port))
    sock.listen(5)

    #Create new thread to wait for connections
    newConnectionsThread = threading.Thread(target = newConnections, args = (sock,))
    newConnectionsThread.start()
    
main()