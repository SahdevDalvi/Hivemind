# AI-Driven Autonomous Civilization Simulation (Unity)

## Overview

This project is an AI-driven civilization simulation built in Unity where autonomous NPC citizens manage their own survival while a centralized Mayor AI responds dynamically to world events.

Each citizen operates using individual utility-based decision making (hunger, energy, happiness, money, health), while a higher-level AI mayor analyzes city conditions and reacts to crises such as famine, economic crashes, depression waves, and sleep deprivation.

The simulation demonstrates emergent behavior where:

* NPCs prioritize personal survival
* The mayor intervenes only during major events
* The civilization stabilizes or collapses based on collective decisions

This project was created as an experimental AI-driven systems simulation for game design and technical exploration.

---

## Core Features

### 1. Autonomous NPC Survival AI

Each NPC independently manages:

* Hunger
* Energy
* Happiness
* Money
* Health

Citizens:

* Eat when starving
* Rest when exhausted
* Work to earn money
* Socialize to recover happiness
* Ignore mayor commands if in personal survival danger

This ensures believable, self-preserving agents rather than blindly obedient units.

---

### 2. Mayor AI (LLM-Driven Leadership System)

A mayor AI powered through local LLM integration analyzes:

* Active world event
* Population status
* Average hunger, energy, happiness, money, health
* Survival risk

The mayor responds only when events occur and sends strategic emergency orders:

* Emergency food distribution
* Mandatory rest
* Economic mobilization
* City festivals

Mayor decisions are generated via structured JSON responses from the language model.

Fallback logic ensures simulation stability if the AI response fails.

---

### 3. Dynamic World Event System

Random events periodically affect the entire civilization:

| Event           | Effect                             |
| --------------- | ---------------------------------- |
| Famine          | Hunger increases across population |
| Economic Crash  | Money reduced                      |
| Depression Wave | Happiness drops                    |
| Sleep Crisis    | Energy drops                       |
| Economic Boom   | Money increases                    |
| Peace Period    | No crisis                          |

Events trigger mayor analysis and response.

---

### 4. Emergent Behavior Simulation

The system is designed so:

* NPCs prioritize survival over obedience
* Mayor influences but does not fully control citizens
* Poor decisions can lead to population collapse
* Smart decisions stabilize civilization
* Outcomes differ each simulation run

---

### 5. Real-Time Simulation Controls

Simulation speed can be adjusted:

| Key | Speed      |
| --- | ---------- |
| Q   | Normal     |
| W   | Fast       |
| E   | Ultra fast |

Manual override keys for testing:

| Key | Command       |
| --- | ------------- |
| 1   | Force work    |
| 2   | Force rest    |
| 3   | Festival      |
| 4   | Feed citizens |

---

## System Architecture

### Core Scripts

NPCBrain.cs
Handles autonomous citizen behavior, survival logic, movement, and actions.

CivilizationAI.cs
Mayor AI system that analyzes events and sends strategic orders using LLM responses.

EventManager.cs
Generates random world events and applies global stat changes.

ActivityZone.cs
Defines functional areas (work, rest, food, social) and manages occupancy.

UIDisplay.cs
Displays NPC stats and state in real time.

GameSpeed.cs
Controls simulation time scale.

---

## AI Decision Model

### Citizen Logic Priority

1. Immediate survival (hunger, energy, health)
2. Personal stability (money, happiness)
3. Productivity or social behavior
4. Mayor commands (only if safe)

### Mayor Logic Priority

1. Detect global crisis event
2. Analyze population averages
3. Choose response for current crisis only
4. Issue temporary emergency order
5. Return control to citizens

---

## Technical Stack

* Unity Engine (NavMesh navigation)
* C# (behavior systems)
* Local LLM integration via Ollama API
* TextMeshPro UI
* Coroutine-based simulation loops

---

## How to Run

1. Open project in Unity.
2. Ensure NavMesh is baked.
3. Start local LLM server (Ollama or compatible).
4. Set model name in CivilizationAI inspector.
5. Press Play.

Simulation will begin automatically with periodic world events.

---

## Project Structure

Assets/
Scripts/

* NPCBrain.cs
* CivilizationAI.cs
* EventManager.cs
* ActivityZone.cs
* UIDisplay.cs
* GameSpeed.cs

Scenes/

* Main simulation scene

UI/

* NPC stat UI
* Mayor dialogue UI
* Event notification UI

---

## Purpose

This project explores:

* Autonomous agent behavior
* Utility-based decision systems
* AI-assisted strategy control
* Emergent simulation design
* Integration of LLMs into gameplay systems

It demonstrates how multiple AI layers (individual and strategic) can interact within a living game world.

---

## Future Improvements

* Memory-based mayor decisions
* Relationship system between citizens
* Resource economy simulation
* Visual dashboards for city metrics
* Expanded disaster system
* Save/load simulation states

---

## License

This project is intended for educational and experimental use.
## Credits

### Development
- Game & AI System: Sahdev Dalvi

### Tools & Technologies
- Unity Engine
- Ollama (Local LLM runtime)
- Qwen Model (Alibaba)

### Assets
- House Prefabs: Mega Fantasy Props Pack by karboosx.
  Source: [[Unity Asset Store / Sketchfab / itch.io link]](https://assetstore.unity.com/packages/3d/environments/fantasy/mega-fantasy-props-pack-87811)
