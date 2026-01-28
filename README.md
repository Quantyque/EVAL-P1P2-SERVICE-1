# 🏰 Castle of Whispers - Quests Microservice

This service manages the **Quests & Progression** system for the "Castle of Whispers" MMORPG.
It is built with **.NET 8**, **Vertical Slice Architecture**, and follows **Event-Driven Architecture** principles.

## 🛠️ Tech Stack
*   **Framework**: .NET 8 (ASP.NET Core Web API)
*   **Database**: PostgreSQL (via **Marten** as Document DB + Event Store)
*   **Messaging**: RabbitMQ (via **Wolverine**)
*   **Architecture**: Vertical Slice Architecture + CQRS (MediatR)

## 🚀 Key Features
1.  **Distributed Transactions**: Consumes `DungeonCompleted` events from the Game service to update player progress.
2.  **Idempotency**: Ensures events are processed exactly once per quest/player.
3.  **Targeting (Criteria)**: Quests can be generic or targeted to specific dungeons (`QuestDefinition.Criteria`).
4.  **Simulation Mode**: Includes `/debug` endpoints to simulate external events without requiring the full Game microservice.

## 🏃‍♂️ How to Run

### 1. Infrastructure
Start the required databases and message broker:
```bash
docker-compose up -d
```

### 2. Run the Service
```bash
dotnet run --project EVAL-P1P2-SERVICE-1
```
The API will be available at `http://localhost:5301`.
Swagger UI: `http://localhost:5301/swagger`

### 3. Simulation & Testing
You can simulate gameplay events using the Debug API (or the provided PowerShell script):

**Simulate Dungeon Completion:**
```http
POST /debug/dungeon-completed
Content-Type: application/json

{
  "eventId": "uuid-v4",
  "playerId": "player-1",
  "dungeonId": "dungeon-123",
  "completedAt": "2024-01-28T12:00:00Z"
}
```
This will trigger the `DungeonCompletedHandler`, find matching active quests, and update the player's progress.

## 📂 Project Structure
*   **API**: Endpoints (Controllers/Minimal APIs)
*   **Application**: Business Logic (Commands, Queries, Handlers)
*   **Domain**: Entities (`QuestDefinition`, `PlayerQuest`) and Enums
*   **Infrastructure**: Database (Marten) and Messaging (Wolverine) configuration
