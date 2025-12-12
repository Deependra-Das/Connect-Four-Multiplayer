# Connect-Four-Multiplayer

## Overview
Multiplayer Connect 4 is a multiplayer version of the classic Connect 4 game, where two players compete in real-time, to connect four discs in a row — either horizontally, vertically, or diagonally — in order to win. The objective is simple: strategize and drop your discs into the grid, aiming to align four discs while preventing your opponent from doing the same.

Players can manually host a lobby, join hosted lobby randomly, and play with others over a network. The game uses Unity’s Netcode for GameObjects and Unity Lobby Service to manage multiplayer interactions and lobbies.

---

## Features

- Real-time Multiplayer: Two players can connect and compete against each other in a public lobby.

- Automatic Turn Management: The game automatically handles player turns, ensuring a smooth experience.

- Victory Detection: The first player to align four discs in a row (horizontally, vertically, or diagonally) wins the match.

- Public Lobbies: All lobbies are public, and players can manually host a lobby or join random lobbies created by other players.

- Simple and Intuitive Gameplay: Drop discs into a grid and try to connect four discs in a row — either horizontally, vertically, or diagonally — in order to win
  
---

## Implementation Details

### Design Patterns

Singleton Pattern: Used for managing global systems like GameManager to manage the game's state & act as mediator for the Services, EventBusManager to manage the events, SceneLoader to load specific scnenes both locally & on network. This ensures there's only one instance handling these operations across the game.

Service Locator Pattern: Centralized management of game services like Board, Disk etc. The Service Locator helps decouple the game components, making it easier to manage services and dependencies across different systems.

Observer Pattern: Used for event-driven interactions for managing various gameplay events. Components can "subscribe" to events (like Player taking Turn for gameplay) and automatically update themselves when these events occur.

State Machine Pattern: Used to manage different game states (MainMenu, Lobby, Gameplay, GameOver) by clearly defining transitions between each state. This ensures smooth handling of required services for each state & logic, as the game responds to user inputs and events based on its current state.

### Scriptable Objects

Used to store and manage data independently of game objects, making them ideal for managing data for Board, Disk etc. This allows for easy customization and modification of attributes directly from the Unity Editor without needing to modify code.

### Multiplayer & Network Services

Unity Netcode: Unity Netcode enables multiplayer functionality by synchronizing data across clients and servers, allowing real-time communication and interaction in networked games.

Unity Lobby Service: The Unity Lobby Service helps manage player matchmaking and game sessions, allowing players to create, join, and manage lobbies before entering the game.

Unity Relay Service: The Unity Relay service facilitate multiplayer connectivity in games without requiring developers to manage dedicated game servers or players to deal with network complexities like IP addresses and port forwarding.

---

## Architecture Block Diagram

---

## How to Play

---

## Playable build

---

## Gameplay Video


