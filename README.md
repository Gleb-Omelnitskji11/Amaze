# AMAZE (Unity)

Amaze is a small swipe-based puzzle game: move the ball across the grid and **paint all existing cells** to complete the level.

## Tech stack

- Unity (C#)
- DOTween (animations)

## Project structure

- `Assets/Scenes/`
  - `GameScene.unity` — main gameplay scene
  - `LevelEditorScene.unity` — level editor scene
- `Assets/Scripts/` — runtime code
  - `Configs/` — level/settings data models + ScriptableObject configs
  - `LevelEditor/` — level editor runtime scripts
- `Assets/Resources/LevelsExport.json` — example JSON container with levels from level editor win build (it should be added by Tool->Import and remove this file after). 
If you use LevelEditor only in unity, you not need in it (it updates config by button `Save`)

## Gameplay architecture

- **Game bootstrap**
  - `GameManager` starts a level and tracks progress in level by `UIManager`.
- **Level selection / progress**
  - `LevelProgressor` reads and stores the current level index via `PlayerPrefs` (`Level` key).
- **Grid generation**
  - `GridManager.SetLevel(LevelData)` generates a 2D grid of `CellView` instances.
  - `GridManager` also instantiates and initializes `BallController`.
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

- Load/edit/save a level by index
- Toggle cells between `Exist` and `Empty`byclicking
- Set `StartPosition` (you should click on the certain cell after click on the button)
- Apply current level data to the grid

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

## Future improvements

- **Refactor `GridManager` into components**
  - Split responsibilities (grid generation, coordinate/layout, ball lifecycle, cell lifecycle) into separate components/services.
- **Factories + object pooling**
  - Introduce factories and object pools for `CellView` and `BallController` to reduce `Instantiate/Destroy` overhead (especially when regenerating grids / switching levels).
  - Add a capability to change a theme (colors or sprites)
- **Graphic**
  - Make a fake 3d view
- **Di**
  - Replace singletons with zenject/Service locator/VContainer
- Fix minor bug when you do multiple clicks on restart button and multiple scrolling on the screen during one time. If you are doing it during some seconds, the ball stops respond. Relaunch fixes it.
