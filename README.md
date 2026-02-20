# AMAZE (Unity)

Amaze is a small swipe-based puzzle game: move the ball across the grid and **paint all existing cells** to complete the level.

## Tech stack

- Unity (C#)
- DOTween (animations)

## Project structure

- `Assets/Scenes/`
  - `GameScene.unity` — main gameplay scene
  - `LevelEditorScene.unity` — level editor scene
- `Assets/Scripts/` — code
  - `Configs/` — data models + ScriptableObject types
  - `LevelEditor/` — level editor scripts
  - `Editor/` — editor-only tools (menu items)
- `Assets/GameResources/`
  - `Configs/` — `GameSettings.asset`, `LevelsConfig.asset`
  - `Prefabs/` — gameplay prefabs (`Ball`, `Cell`)
- `Assets/LevelEditorResources/`
  - `CellEditor.prefab` — editor cell prefab
- `Assets/Resources/LevelsExport.json` — optional example JSON container with levels (can be imported into `LevelsDataConfig` via editor tools)

## Gameplay architecture

- **Game bootstrap**
  - `GameManager` starts a level and tracks progress in level by `UIManager`.
- **Level selection / progress**
  - `LevelProgressor` reads and stores the current level index via `PlayerPrefs` (`Level` key).
  - `LevelsConfigr` stores data of levels. 
  - "Assets/Resources/LevelsExport" is a JUST EXAMPLE of an import/export file from the level editor windows build. You don`t if you edit levels in unity.
- **Grid generation**
  - `GridManager.SetLevel(LevelData)` generates a 2D grid of `CellView` instances.
  - `BallSpawner` instantiates and initializes `BallController`.
  - You can change cell size and spacing for all cells.
- **Input**
  - `InputController` emits `OnSwipeEvent(Vector2Int)`.
  - In `UNITY_EDITOR || UNITY_STANDALONE` it uses mouse swipes; otherwise it uses touch.
- **Movement / painting**
  - `BallController` moves using DOTween and paints cells on the path via `CellView.PaintFilled()`.
  - Ball moves with a trail.
  - Each painted cell notifies `GameManager` to update UI progress.
- **UI**
  - `UIManager` updates progress bar/text, shows the win panel + Restart Button.

## Levels data

Levels are represented by `LevelData`:

- `Width`, `Height` represent Colomns and Rows 
- `Cells` — flat array (`CellType.Exist` / `CellType.Empty`)
- `StartPosition` — `Vector2Int`(0, 0) -(`Width`-1, `Height`-1). If a start cell is empty, first available cell become a start cell.

Runtime gameplay levels are getting from `LevelsDataConfig` (ScriptableObject) via `GetLevel(index)` by DeepCopy.

## Level Editor

The Level Editor is implemented in `Assets/Scripts/LevelEditor/` and works in:

- Unity Editor (`UNITY_EDITOR`)
- **Windows standalone builds** (`UNITY_STANDALONE_WIN`).In this case you should export result to json and in editor by tool (describe above) add this to the config.

This is enforced by compile guards in:

- `LevelEditorManager`
- `CellViewEditor`
- `LevelsDataConfig.GetAllLevels()`

### Editor capabilities

- Create a new level by size (width/height)
- Load/edit/save a level by index
- Toggle cells between `Exist` and `Empty` by clicking
- Set `StartPosition` (click a cell after pressing the set-start button)
- Validation on Save (cannot save invalid/unsolvable levels)

### Import / Export levels

`LevelsRuntimeJsonTool` supports runtime import/export:

- **Export**: `ExportToJson(LevelData[] levels)` writes `LevelsRuntime.json` to:
  - `Application.persistentDataPath/LevelsRuntime.json`
- **Import**: `ImportFromJson()` reads from the same location

**Important:** the Level Editor export result can be used to **update the levels configuration** (e.g. by taking the exported JSON as the new source of truth and synchronizing it back into `LevelsDataConfig` / your project’s level config pipeline).

## How to run

- **Game**
  - Open `Assets/Scenes/GameScene.unity` and press Play.
- **Level Editor**
  - Open `Assets/Scenes/LevelEditorScene.unity` and press Play.
  - You can also build a **Windows standalone** player and run the Level Editor there (it is enabled under `UNITY_STANDALONE_WIN`).

## Controls

- **Gameplay**
  - Swipe (mouse drag in Editor / touch on device) to move the ball in 4 directions.
  - The ball moves until it hits an obstacle or the grid boundary.
- **Level Editor**
  - Click on cells to toggle `Exist` / `Empty`.
  - Use the UI buttons to create a new level, set start position, load/save.

## Build notes

- Add both scenes to Build Settings:
  - `Assets/Scenes/GameScene.unity`
  - `Assets/Scenes/LevelEditorScene.unity`
- Android: enable ARM64 before building the `.apk`.

## Future improvements

- **DI**
  - The project is too small for zenject, but something like 'ServiceLocator' is good if the game will be increased.
- **Level editor**
  - Undo/redo
- **Graphic**
  - Make a fake 3d view
  - Make ability to change cells and ball theme (color/ sprite)
- Fix minor bug when you do multiple clicks on restart button and multiple scrolling on the screen during one time. If you are doing it during some seconds, the ball stops respond. Relaunch fixes it.
