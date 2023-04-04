# minimal-api
.net 6.0 minimal API demo





## Docker 설치

```bash
$ sudo apt install docker.io
$ sudo systemctl enable --now docker
$ sudo usermod -aG docker ubuntu
$ sudo apt install docker-compose
```



### api docker 설정

`minimal-api.Dockerfile`

```yaml
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MinimalAPIsDemo.csproj", "./"]
RUN dotnet restore "./minimal-api.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "minimal-api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "minimal-api.csproj" -c Release -o /app/publish

FROM base AS final

LABEL maintainer="sbjeon <sbjeon@gampit.co.kr>"

WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS http://*:5001

RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf

ENTRYPOINT ["dotnet", "MinimalAPIsDemo.dll"]

```





### nginx docker 설정

`nginx.Dockerfile`

```yaml
FROM nginx:latest

COPY nginx.conf /etc/nginx/nginx.conf
```

`nginx.conf`

```yaml
worker_processes auto;

events { worker_connections 2048; }

http {
  sendfile on;

  upstream web-api {
    server api_1:5001;
    # server api_2:5001;
    # server api_3:5001;
  }

  server {
    listen 80;

    location / {
      proxy_pass         http://web-api;
      proxy_headers_hash_max_size 512;
      proxy_headers_hash_bucket_size 128; 
      proxy_redirect     off;
      proxy_http_version 1.1;
      proxy_cache_bypass $http_upgrade;
      proxy_set_header   Upgrade $http_upgrade;
      proxy_set_header   Connection keep-alive;
      proxy_set_header   Host $host:$server_port;
      proxy_set_header   X-Real-IP $remote_addr;
      proxy_set_header   X-Forwarded-Proto https;
      proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
      proxy_set_header   X-Forwarded-Host $server_name;
      proxy_set_header   X-Forwarded-Ssl on;
    }
  }
}
```



### Docker-compose

```bash
$ sudo -i
$ /usr/local/bin/docker-compose up -d
```



- Path: `/home/ubuntu/minimal-api`

```yaml
version: "3.7"

services:
  nginx:
    depends_on:
      - api_1
    #   - api_2
    #   - api_3
    build:
      context: ./nginx
      dockerfile: nginx.Dockerfile
    environment:
      - VIRTUAL_HOST=minimal-api.gampit.co.kr
      - VIRTUAL_PORT=5298
    restart: "no"

  api_1:
    build:
      context: ./src
      dockerfile: minimal-api.Dockerfile
    expose:
      - "5001"
    restart: "no"


networks:
  default:
    external:
      name: nginx-proxy
```





