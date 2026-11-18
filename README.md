# Elemental VR Fighter

We're building a 1v1 VR arena fighter based on elemental combat for CS 417.

## Getting started

`main` currently contains the folder structure only. The Unity starter is on `daryl/unity-scaffold`; it includes the arena and movement/health components, not a playable multiplayer match yet.

1. Check out `daryl/unity-scaffold`.
2. Open the repository folder in Unity Hub with **Unity 6000.5.6f1**.
3. Open `Assets/ElementalFighter/Shared/Scenes/ElementalArena.unity`.

The project uses URP, OpenXR 1.17.1, XR Interaction Toolkit 3.5.1, and Input System 1.20.0.

## Project structure

Under `Assets/ElementalFighter/`:

- `Daryl/`: movement, dodging, and health
- `Giovanni/`: elemental combat
- `Suhaas/`: combos and ultimates
- `Ethan/`: multiplayer and rounds
- `Shared/Scenes/`: shared arena
