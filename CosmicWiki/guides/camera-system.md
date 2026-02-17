# Camera System Guide (Feature 003)

**Branch:** `features/003-camera-control`

## Overview

Three switchable camera modes with orbital snap rotation for ACNH-style 45° modes.
Builds on `ACNHCameraFollow.cs` from `feature/camera-controller`.

## Camera Modes

| Mode | Key | Distance | FOV | Notes |
|------|-----|----------|-----|-------|
| ACNH Fixed | Tab (cycle) | 15 | 60° | Default — Y=45°, follows player |
| Close Follow | Tab | 7 | 40° | Intimate view — still Y=45° |
| Cinematic | Tab | 30 | 55° | Free-orbit, right-click drag |

## Orbital Snap Rotation

Available in ACNH Fixed and Close Follow modes:
- **Q** — rotate CCW (NE → NW → SW → SE)
- **E** — rotate CW (NE → SE → SW → NW)
- Snap presets: 45°, 135°, 225°, 315° (NE, SE, SW, NW facing)
- Transition: `Quaternion.Lerp` over 0.3s

## Components

| Script | Role |
|--------|------|
| `CameraController.cs` | Mode state machine, transition logic |
| `ACNHCameraFollow.cs` | Refactored — mode-aware, orbital snap |

## Cinemachine Integration

Uses `Unity.Cinemachine` (already installed).
Each mode can use a separate `CinemachineVirtualCamera` with different settings.

## DevConsole Integration

Camera section in DevConsole:
- Mode toggle (dropdown or cycle button)
- Zoom slider (overrides distance)
- Snap angle display (read-only)

## Status

- [ ] CameraController.cs — mode state machine
- [ ] ACNHCameraFollow.cs — refactor for mode awareness
- [ ] Orbital snap rotation (Q/E)
- [ ] Cinematic free-orbit (right-click drag)
- [ ] DevConsole camera section
