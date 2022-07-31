# Simple Dockerized TCP Python server and .NET(C#) client

## Requirements
1. [Docker & Docker Compose](https://docs.docker.com/get-docker/) - to run using Docker
2. [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) and [Python 3](https://www.python.org/downloads/) - to run locally

## Run using Docker

1. Clone / Download this repository.

2. Open Terminal / Command Prompt at the project's root directory and type:
    ```
    docker-compose build --no-cache
    ```

    - Running **`TCPServer`** and **`TCPClient`** in separate terminals with real-time outputs and input:
    
        Starting **`TCPServer`** 
        ```
        docker compose up tcp-server --no-build
        ```

        Starting **`TCPClient`** 
        
        Open another Terminal / Command Prompt at the project's root directory and type:
        ```
        docker-compose run --rm tcp-client
        ```
        
    - Running **`TCPClient`** in single terminal with real-time output and input:
        ```
        docker compose up tcp-server --detach --no-build
        docker-compose run --rm tcp-client
        ```


#### Useful commands

- *Clone repository:*
```
git clone https://github.com/nkudry/TCPClientServer.git
```

- *Attaching to an existing running container:*
```
docker attach TCPClient
docker attach TCPServer
```

- *View container logs:*
```
docker logs -f TCPServer
docker logs -f TCPClient
```

- *Clean up the docker compose images and containers:*
```
docker compose down --rmi all
```


## Run using Visual Studio & Python 3

1. Clone / Download this repository.
2. Open Terminal / Command Prompt at `TCPServer` directory and type:
```
python server.py
```
The server will start on port `:54321`

3. Open `TCPClient.csproj` at `TCPClient` directory with Visual Studio 2022 and press Start (F5).
