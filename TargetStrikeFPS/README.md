# TargetStrike FPS 🎯

A polished first-person target shooting game built in Unity (C#). Shoot targets, rack up points, beat the clock.

---

## Quick Setup

### Requirements
- Unity 2022.3 LTS or newer
- TextMeshPro package (install via Package Manager)
- Universal Render Pipeline (optional — works with Built-in too)

### Import Steps
1. Open Unity Hub → **New Project** → 3D (Core)
2. Copy the `Assets/Scripts/` folder into your project's `Assets/` directory
3. Install **TextMeshPro** via `Window > Package Manager`
4. Follow the Scene Setup guide below

---

## Scene Hierarchy

```
[Scene: TrainingArena]
├── _MANAGERS
│   ├── GameManager          → GameManager.cs
│   ├── ScoreManager         → ScoreManager.cs
│   ├── TimerManager         → TimerManager.cs
│   ├── UIManager            → UIManager.cs (Canvas attached)
│   └── AudioManager         → AudioManager.cs
│
├── ENVIRONMENT
│   ├── Ground               → Plane (scale 20,1,20), Material: GroundMat
│   ├── WallNorth            → Cube (pos: 0,5,10, scale: 20,10,0.5)
│   ├── WallSouth            → Cube (pos: 0,5,-10, scale: 20,10,0.5)
│   ├── WallEast             → Cube (pos: 10,5,0, scale: 0.5,10,20)
│   ├── WallWest             → Cube (pos: -10,5,0, scale: 0.5,10,20)
│   ├── Ceiling              → Cube (pos: 0,10,0, scale: 20,0.5,20)
│   └── DirectionalLight     → Light component, Rotation: (50,-30,0)
│
├── PLAYER
│   └── Player               → CharacterController + PlayerController.cs
│       └── PlayerCamera     → Camera + GunController.cs
│           └── GunModel     → 3D model or placeholder cube
│               └── MuzzlePoint → Empty for muzzle flash
│
├── TARGETS
│   ├── Target_Normal_01     → Target.cs  (pos: 5, 0.5, 5)
│   ├── Target_Normal_02     → Target.cs  (pos: -5, 0.5, 8)
│   ├── Target_Normal_03     → Target.cs  (pos: 0, 0.5, 7)
│   ├── Target_Moving_01     → MovingTarget.cs (pos: 3, 0.5, 6)
│   ├── Target_Moving_02     → MovingTarget.cs (pos: -3, 0.5, 9)
│   └── Target_Bonus_01      → BonusTarget.cs  (pos: 0, 0.5, 10)
│
└── UI (Canvas — Screen Space Overlay)
    ├── HUDPanel
    │   ├── ScorePanel       → TMP: "0"
    │   ├── TimerPanel       → TMP: "2:00"
    │   ├── AccuracyPanel    → TMP: "0.0%"
    │   ├── HitsPanel        → TMP: "0"
    │   └── Crosshair        → Image (dot/crosshair sprite)
    ├── StartScreen
    ├── PauseMenu
    └── GameOverScreen
```

---

## Target Prefab Setup

### Normal Target
```
TargetRoot (Target.cs)
├── Pole  → Cylinder (scale: 0.05, 0.5, 0.05)
└── Board → Cylinder (scale: 0.4, 0.05, 0.4)  ← add Collider + Layer: "Target"
    └── RedMaterial
```
- `scoreValue`: 10
- `respawnDelay`: 2

### Moving Target
Same structure, use `MovingTarget.cs` instead. Set:
- `moveRange`: 3–5
- `moveSpeed`: 1.5–2.5
- `scoreValue`: 20

### Bonus Target
Same structure, use `BonusTarget.cs`. Set:
- `scoreValue`: 50
- `activeTime`: 3
- `minSpawnDelay`: 5, `maxSpawnDelay`: 12

---

## Layer Setup
1. Edit > Project Settings > Tags and Layers
2. Add Layer: **"Target"** (Layer 8)
3. Assign all target colliders to Layer "Target"
4. In `GunController`, set `targetLayer` to "Target"

---

## Script Reference

| Script | Attach To | Purpose |
|--------|-----------|---------|
| `PlayerController.cs` | Player root | WASD + jump + sprint + mouse look |
| `GunController.cs` | Player camera | Raycast shooting, fire rate, effects |
| `Target.cs` | Normal target | Hit detection, respawn, score event |
| `MovingTarget.cs` | Moving target | Extends Target with sine movement |
| `BonusTarget.cs` | Bonus target | Timed appearance, glow effect |
| `ScoreManager.cs` | _MANAGERS | Score / accuracy tracking, PlayerPrefs |
| `TimerManager.cs` | _MANAGERS | 120s countdown, fires EndGame |
| `GameManager.cs` | _MANAGERS | Start / pause / end / restart flow |
| `UIManager.cs` | Canvas | Wires all UI panels and text |
| `AudioManager.cs` | _MANAGERS | Music + SFX with volume control |
| `FloatingScorePopup.cs` | Prefab (world space) | Animated +N popup above target |

---

## Key Controls

| Key | Action |
|-----|--------|
| WASD | Move |
| Mouse | Look |
| Left Mouse Button | Shoot |
| Left Shift | Sprint |
| Space | Jump |
| Esc | Pause / Resume |

---

## Scoring

| Event | Points |
|-------|--------|
| Normal Target hit | +10 |
| Moving Target hit | +20 |
| Bonus Target hit | +50 |
| Miss | −2 |

High score persists via `PlayerPrefs`.

---

## Build Instructions
1. File > Build Settings
2. Add the TrainingArena scene
3. Platform: PC / Mac / Linux Standalone
4. Build → choose output folder

---

## Optional Enhancements (from spec)
- **Ammo System** — add `int ammo` field to `GunController`, decrement on shot, trigger reload animation
- **Health System** — add `HealthManager.cs`, show HP bar, enemies can damage player
- **Multiple Weapons** — create `WeaponBase.cs` parent class, extend for Pistol / Rifle / Shotgun
- **Leaderboard** — store top-5 scores as JSON in `Application.persistentDataPath`
- **Difficulty Levels** — expose `fireRate`, `targetSpeed`, `bonusFrequency` to a `DifficultyConfig` ScriptableObject

---

## Folder Structure

```
TargetStrikeFPS/
└── Assets/
    ├── Scripts/           ← All C# game scripts (this package)
    ├── Scenes/            ← TrainingArena.unity
    ├── Prefabs/           ← Target_Normal, Target_Moving, Target_Bonus, ScorePopup
    ├── Materials/         ← TargetRed, TargetBlue, TargetGold, Ground, Wall
    ├── Audio/             ← gunshot.wav, hit.wav, miss.wav, music.ogg
    └── Animations/        ← GunRecoil.anim (optional)
```
