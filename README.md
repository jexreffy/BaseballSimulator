# Baseball Simulator

A VR baseball batting game built in Unity. Step up to the plate, swing a physics-driven bat, and see how far you can hit 30 pitched balls — including high-value gold balls and point-deducting red balls. Home runs trigger confetti and bonus points, and your high score is saved between sessions.

## Gameplay

Each game pitches **30 balls** at the player. Ball type is randomised per pitch:

| Ball | Frequency | Score multiplier |
|---|---|---|
| White | 60% | 1× distance |
| Gold | 20% | 2× distance |
| Red | 20% | −1× distance |

**Scoring:** Points = distance the ball travels × ball multiplier. Hit a ball into the home run zone for a flat **+50 bonus points** and a confetti celebration. Red balls deduct points, so let them go if you can.

**Bat hit zones:** The bat has three zones that affect how much power transfers to the ball — hitting the sweet spot matters.

A beacon light appears on balls that land 30+ units away so you can see where your best hits landed.

## Controls (VR)

| Input | Action |
|---|---|
| **Hold trigger (1 second)** | Start a new game |
| **Grip button** | Switch between two bat models |
| **Swing** | Hit the ball — tracked by controller motion |

## Tech Stack

- **Engine:** Unity 2019.2.17f1
- **Language:** C#
- **VR Framework:** VRTK (Virtual Reality Toolkit)
- **Platforms:** Oculus (Quest / Rift) · OpenVR / SteamVR
- **UI:** TextMeshPro

## Project Structure

```
Assets/Jexreffy/
  Scripts/
    GameController.cs   # Game lifecycle, ball spawning, scoring, high scores
    Ball.cs             # Physics, fair/foul detection, home run zone, beacons
    BatController.cs    # VR controller input, bat switching, game-start trigger
    SaveState.cs        # Binary serialisation for persistent high scores
  Scenes/
    MainScene.unity     # Single gameplay scene
  Prefabs/              # WhiteBall, GoldBall, RedBall prefabs
  Audio/                # Hit, home run, and high score sound effects
```

## Setup

1. Install **Unity 2019.2.17f1** (exact version required — see `ProjectSettings/ProjectVersion.txt`)
2. Connect a supported VR headset (Oculus or SteamVR compatible) with controllers
3. Open the project in Unity and load `Assets/Jexreffy/Scenes/MainScene.unity`
4. Press **Play** in the editor (with VR device connected), or build and deploy to your target platform

### Platform notes
- **Oculus Quest:** Build target Android, Oculus XR Plugin included
- **Oculus Rift / SteamVR:** Build target Standalone, OpenVR plugin included
