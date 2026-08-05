 HashProcessingEngine

![.NET](https://img.shields.io/badge/.NET-10-purple)
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-Message%20Broker-orange)
![MariaDB](https://img.shields.io/badge/MariaDB-Database-blue)
![Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture-green)

**.NET 10 distributed backend application** that generates SHA1 hashes, processes them asynchronously using RabbitMQ, and stores processed results in MariaDB.

---

Overview

HashProcessingEngine is a distributed system built to explore asynchronous backend processing workflows.

The application receives requests to generate a large number of random SHA1 hashes, publishes each hash as a message through RabbitMQ, processes messages independently using a worker service, and stores the final results in MariaDB.

The project demonstrates:

- REST API development
- Clean Architecture
- Distributed application design
- Asynchronous message processing
- RabbitMQ communication
- Database persistence
- Repository pattern
- Swagger/OpenAPI documentation

---

System Architecture

The application is split into independent services that communicate through RabbitMQ.

```text
                    HTTP Request
                         |
                         |
                         v

              HashProcessingEngine.Api

                         |
                         |
                    RabbitMQ Queue

                         |
                         |
                         v

           HashProcessingEngine.Worker

                         |
                         |
                         v

                     MariaDB
```

This architecture allows:

- API requests to return immediately.
- Heavy processing to happen in the background.
- Independent scaling of API and worker services.

---

Applications

## HashProcessingEngine.Api

REST API responsible for receiving requests and publishing hash jobs.

Responsibilities:

- Accept hash generation requests.
- Validate input data.
- Generate SHA1 hashes.
- Publish messages to RabbitMQ.
- Provide hash processing statistics.
- Expose Swagger/OpenAPI documentation.

---

## HashProcessingEngine.Worker

Background worker responsible for consuming messages and persisting results.

Responsibilities:

- Consume RabbitMQ messages.
- Process hash messages.
- Store processed hashes into MariaDB.
- Run independently from the API.
- 

Processing Workflow

Example:

Client sends:

```http
POST /hashes
```

Request:

```json
{
  "count": 40000
}
```

The system:

1. Generates 40,000 random SHA1 hashes.
2. Publishes each hash as a RabbitMQ message.
3. Worker consumes messages asynchronously.
4. Worker stores processed hashes into MariaDB.
5. API provides processing statistics.

---

 API Endpoints

## Generate Hashes

### POST `/hashes`

Example request:

```json
{
  "count": 40000
}
```

Example response:

```json
{
  "message": "Hash generation started",
  "count": 40000
}
```

---

## Get Hash Statistics

### GET `/hashes`

Example response:

```json
[
  {
    "date": "2026-08-05",
    "count": 40000
  }
]
```

---

Swagger / OpenAPI

The API includes Swagger documentation.

After starting the application:

```
http://localhost:5235/swagger
```

Swagger provides:

- Interactive API testing
- Endpoint documentation
- Request examples
- Response schemas
- OpenAPI specification

---

Technologies

## Backend

- .NET 10
- ASP.NET Core Web API
- C#
- Background Services

## Messaging

- RabbitMQ
- RabbitMQ.Client

## Database

- MariaDB
- Dapper
- Repository Pattern

## Infrastructure

- Docker
- Swagger/OpenAPI
- Clean Architecture

---

Configuration

Configuration is managed through:

```
appsettings.json
```

Example:

```json
{
  "HashGeneration": {
    "DefaultCount": 40000,
    "MaximumCount": 60000
  },
  "RabbitMq": {
    "HostName": "localhost",
    "Port": 5672
  },
  "Database": {
    "ConnectionString": "Server=localhost;Database=HashProcessing;User=root;Password=password;"
  }
}
```

---

 Running RabbitMQ with Docker

Start RabbitMQ:

```bash
docker run -d \
--hostname hash-rabbitmq \
--name hashprocessing-rabbitmq \
-p 5672:5672 \
-p 15672:15672 \
rabbitmq:3-management
```

RabbitMQ Management:

```
http://localhost:15672
```

Credentials:

```
Username: guest
Password: guest
```

---

Database Setup

Create MariaDB database:

```sql
CREATE DATABASE HashProcessing;
```

Example table:

```sql
CREATE TABLE Hashes
(
    Id INT AUTO_INCREMENT PRIMARY KEY,
    HashValue VARCHAR(40) NOT NULL,
    CreatedAt DATETIME NOT NULL
);
```

---

 Running the Application

## 1. Clone repository

```bash
git clone https://github.com/YOUR_USERNAME/HashProcessingEngine.git
```

---

## 2. Start RabbitMQ

```bash
docker start hashprocessing-rabbitmq
```

---

## 3. Run API

```bash
cd src/HashProcessingEngine.Api

dotnet run
```

API:

```
http://localhost:5235
```

---

## 4. Run Worker

Open another terminal:

```bash
cd src/HashProcessingEngine.Worker

dotnet run
```

Worker will start consuming RabbitMQ messages.

---

Testing

The solution contains:

```
HashProcessingEngine.Tests
```

Tests cover:

- Application services
- Business logic
- Validation rules
- Processing workflows

Run tests:

```bash
dotnet test
```

---

Future Improvements

Planned improvements:
    - Authentication
    - Secure secrets management
    - Structured logging
    - Metrics and monitoring

---
