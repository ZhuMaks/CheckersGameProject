Ось готовий, відформатований та ідеальний **README.md**, який можна просто вставити в GitHub.
Він підтримує Markdown, має стилізовані секції, бейджі, emoji — виглядає професійно.

---

#  **Checkers Game Project (Console Edition)**

*A fully-featured console checkers game written in C#, with bot AI, move serialization, and game replay.*

---

##  **About the Project**

This is a **console-based Checkers game**, built in **C# (.NET)** with clean architecture and complete game logic.
The project includes:

✔ Human vs Human mode
✔ Human vs Bot (4 difficulty levels)
✔ Move history serialization to JSON
✔ Game replay system
✔ Event-driven architecture (delegates + events)
✔ Pretty console board with colors
✔ Full rules: mandatory captures, multi-captures, crowning, win detection

This project demonstrates **OOP design**, **serialization**, **AI logic**, and **event-based programming**.

---

##  **Project Structure**

```
CheckersGameProject/
│
├── Game/
│   ├── GameManager.cs
│   ├── Board.cs
│   ├── Move.cs
│   ├── MoveValidator.cs
│
├── Pieces/
│   ├── Piece.cs
│   ├── CheckerPiece.cs
│   ├── KingPiece.cs
│   └── PieceColor.cs
│
├── Players/
│   ├── IPlayer.cs
│   ├── HumanPlayer.cs
│   ├── BotPlayer.cs
│   └── BotDifficulty.cs
│
├── GameRecords/
│   ├── MoveRecord.cs
│
└── Program.cs
```

---

#  **Features**

###  **1. Game Modes**

At launch, the player chooses:

```
1 — Player vs Player
2 — Player vs Bot
3 — Replay Previous Game
```

###  **2. Bot AI with Difficulty Settings**

| Difficulty | Description                    |
| ---------- | ------------------------------ |
| Easy       | Random legal moves             |
| Medium     | Chooses simple favorable moves |
| Hard       | Evaluates future moves         |
| Impossible | Optimized minimax-style logic  |

###  **3. Beautiful Console Board**

* Dark/light squares
* Colored pieces
* Highlighted moves
* Crowning indicators

###  **4. JSON Move Serialization**

After every game, the file:

```
last_game_moves.json
```

is generated automatically and contains:

* Player names
* Colors
* Start/end coordinates
* Captured pieces
* All moves in chronological order

###  **5. Replay System**

The replay engine:

* Reconstructs the board
* Plays each move step-by-step
* Displays board after each move
* Waits for Enter to continue

---

#  **Installation & Running**

###  Requirements

* .NET SDK 6 / 7 / 8
* Windows, Linux, or macOS console

### ▶ Run the game

Clone the repository:

```
git clone https://github.com/YOUR_USERNAME/CheckersGameProject.git
```

Navigate:

```
cd CheckersGameProject
```

Run:

```
dotnet run
```

---

#  **How It Works (Short Explanation)**

###  Delegates & Events

GameManager exposes events:

```csharp
MoveMade
PlayerChanged
GameEnded
```

They allow tracking actions and expanding functionality without modifying core logic.

### 📦 Serialization

JSON serialization:

```csharp
JsonSerializer.Serialize()
JsonSerializer.Deserialize()
```

Stores moves and reloads them for replay.

###  Game Logic

Includes:

* Mandatory captures
* Multi-jumps
* King/Queen logic
* Endgame detection
* Board validation
