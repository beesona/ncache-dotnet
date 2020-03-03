# nCache-dotnet
nCache-dotnet- A simple caching example using Containerized dotnetcore 3.1 and Redis

### Dependencies to run locally
- Dotnet Core 3.1+
- Docker 19.0.3.1

### Running Locally
**Note**: If using Docker Desktop for Windows, make sure you are pointing to Linux Containers.
```
git clone https://github.com/beesona/ncache-dotnet.git
cd ncache-dotnet
dotnet restore
docker container run --name=redis-1 -p 6379:6379 redis
dotnet run
```
- Navigate to https://localhost:5001 to access the page.
- Any src changes will need to stop the express server (CTRL+C) and rerun ```dotnet run```

### Deploying Containers and Running
**Note**: If using Docker Desktop for Windows, make sure you are pointing to Linux Containers.
```
cd <project-root-dir>
dotnet publish -c Release
docker-compose up
```
- Navigate to localhost:5000 to access the page. (**NOTE**: The docker-compose.yml points the dotnetcore app to 5000 instead of 5001)
- Any changes will require the following (ran from the project root)
```
docker-compose down
dotnet publish -c Release
docker-compose build
docker-compose up
```

### Using the application
There are two endpoints (currently) available for caching data.
1. /cache/{key}?={value}
  - use this endpoint to get account demographic data.
  - Example: **http://localhost:5000/cache/foo?value=bar**
    - Calling **http://localhost:5000/cache/foo** after setting will return **bar**.
2. /fromurl/{uriEncodedUrl}
  - use this endpoint to cache and return any data serialized from the provided URL.
  - Example: **http://localhost:5000/fromurl/http%3A%2F%2Fdev.intsvc.nelnet.net%2FHistoryNote%2Fapi%2Fv1%2Fhistorynotes%2F99%2F1%2F003823158%2Fabeeson%3FrequestId%3D5302fe94-7596-41f6-84a8-2977f5c3eecf**
