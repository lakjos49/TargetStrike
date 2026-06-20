# TargetStrike FPS

A 3D First-Person Shooter (FPS) game built in Unity using C#. Players engage in combat, eliminate enemies, earn points through precision shooting, collect ammunition, and survive increasingly difficult enemy waves.

---

## Overview

TargetStrike FPS is a single-player survival shooter designed to demonstrate core game development concepts including:

* First-person player movement
* Raycast shooting mechanics
* Enemy AI behavior
* Body-part damage system
* Score tracking
* Ammo management
* Wave-based survival gameplay
* Health and armor systems
* Interactive UI and HUD

The project was developed as a portfolio-ready Unity game showcasing gameplay programming and object-oriented design principles.

---

## Features

### Player System

* WASD movement
* Mouse look
* Sprinting
* Jumping
* Smooth FPS controls

### Weapon System

* Assault rifle
* Raycast shooting
* Reload mechanics
* Muzzle flash effects
* Hit markers

### Enemy AI

* Patrol behavior
* Player detection
* Chase system
* Combat engagement
* Enemy health management

### Body-Part Damage

Different body parts apply different damage multipliers:

| Body Part | Multiplier |
| --------- | ---------- |
| Head      | 4x         |
| Chest     | 1x         |
| Arms      | 0.75x      |
| Legs      | 0.5x       |

### Scoring System

* Normal Kill: +100
* Headshot Bonus: +50
* Elite Enemy Bonus
* Kill Streak Rewards

### Survival Mechanics

* Health System
* Armor System
* Ammo Management
* Ammo Pickups
* Health Packs

### Wave Survival

* Progressive enemy waves
* Increasing difficulty
* Dynamic enemy spawning

### Statistics Tracking

* Total Score
* Kills
* Headshots
* Accuracy Percentage
* Kill Streaks
* Survival Time

---

## Technologies Used

* Unity 6 / Unity LTS
* C#
* Unity Physics
* NavMesh AI
* TextMeshPro
* Unity UI System

---

## Project Structure

```text
Assets/
│
├── Scripts/
│   ├── Core/
│   │   ├── GameManager.cs
│   │   ├── AudioManager.cs
│   │   └── UIManager.cs
│   │
│   ├── Player/
│   │   ├── PlayerController.cs
│   │   └── GunController.cs
│   │
│   ├── Enemy/
│   │   ├── EnemyAI.cs
│   │   ├── EnemyHealth.cs
│   │   └── BodyPart.cs
│   │
│   ├── Combat/
│   │   └── FloatingScorePopup.cs
│   │
│   └── Systems/
│       ├── ScoreManager.cs
│       ├── TimerManager.cs
│       └── WaveManager.cs
│
├── Prefabs/
├── Scenes/
├── Materials/
├── Audio/
└── UI/
```

---

## Controls

| Action | Key               |
| ------ | ----------------- |
| Move   | WASD              |
| Jump   | Space             |
| Sprint | Left Shift        |
| Shoot  | Left Mouse Button |
| Reload | R                 |
| Pause  | ESC               |

---

## Gameplay Loop

1. Spawn into the combat arena.
2. Search for enemy targets.
3. Eliminate enemies to earn points.
4. Collect ammunition and health pickups.
5. Survive increasingly difficult enemy waves.
6. Achieve the highest score possible before being eliminated.

---

## Screenshots

Add screenshots of:

* Main Menu
* Gameplay
* Enemy Combat
* Headshot System
* Scoreboard
* Game Over Screen

---

## Future Improvements

* Multiple weapon types
* Weapon upgrades
* Multiplayer mode
* Boss battles
* Save system
* Achievement system
* Online leaderboard
* Additional maps

---

## Installation

### Clone Repository

```bash
git clone https://github.com/YOUR_USERNAME/TargetStrike-FPS.git
```

### Open in Unity

1. Open Unity Hub.
2. Click Open Project.
3. Select the project folder.
4. Open the Main Scene.
5. Press Play.

---

## Learning Outcomes

This project demonstrates:

* Game Programming
* Object-Oriented Programming
* Unity Development
* AI Navigation
* Combat Systems
* UI Development
* Event-Driven Design
* Gameplay Architecture

---

## Author

Developed as a Unity FPS game project for learning game development, gameplay programming, and interactive system design.
