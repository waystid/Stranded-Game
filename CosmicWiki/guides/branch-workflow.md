# Branch Workflow Guide

## Branch Structure

```
main                          ← stable releases only
 └── feature-base             ← integration branch (features merge here first)
      ├── features/001-world
      ├── features/002-items
      ├── features/003-camera-control
      ├── features/004-villagers
      ├── features/005-buildings-and-houses
      ├── features/006-tools
      └── features/007-character-creator
```

**Note:** `feature-base` is the umbrella integration branch.
Git cannot have both `features` (a branch) and `features/001-world` (a sub-branch) due to
ref namespace conflicts — hence the umbrella is named `feature-base`.

## Merge Strategy

1. Feature work happens on `features/NNN-name`
2. When feature is complete + verified: PR `features/NNN-name` → `feature-base`
3. When multiple features stable: PR `feature-base` → `main` for a release

## Feature Dependencies

| Feature | Depends On |
|---------|-----------|
| 001-world | — (foundation) |
| 002-items | — (independent) |
| 003-camera-control | — (independent) |
| 004-villagers | 001-world (NavMesh from terrain) |
| 005-buildings-and-houses | 001-world (grid snap) |
| 006-tools | 001-world (grid interaction) |
| 007-character-creator | — (independent) |

## Working on a Feature Branch

```bash
git checkout features/001-world
# ... make changes ...
git add Assets/Scripts/Environment/GridCell.cs
git commit -m "Add GridCell data structure with TerrainType enum"
git push
```

## Starting a New Session

```bash
git checkout features/001-world
git pull origin features/001-world
# Check feature-base for integration updates:
git merge feature-base  # if needed
```
