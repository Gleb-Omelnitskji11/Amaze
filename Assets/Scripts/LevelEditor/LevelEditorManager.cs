using System;
using TMPro;
using Amaze;
using Amaze.Configs;
using UnityEngine;
using UnityEngine.UI;

namespace Amaze.LevelEditor
{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    public class LevelEditorManager : MonoBehaviour
    {
        public static LevelEditorManager Instance;

        [SerializeField] private GridManager _grid;
        [SerializeField] private BallSpawner _ballSpawner;
        [SerializeField] private LevelsDataConfig _levelsConfig;

        [SerializeField] private TMP_InputField _levelIndex;
        [SerializeField] private TMP_InputField _width;
        [SerializeField] private TMP_InputField _height;

        [SerializeField] private Button _setStartButton;
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _loadButton;
        [SerializeField] private Button _createButton;
        [SerializeField] private Button _importButton;
        [SerializeField] private Button _exportButton;

        private LevelData _editingLevel;
        private LevelData[] _levels;
        private bool _isSettingStartPos;

        private void Awake()
        {
            Instance = this;

            _ballSpawner.Spawn(_grid);
            _levels = _levelsConfig.GetAllLevels();
            Subscribe();
        }

        private void Subscribe()
        {
            _loadButton.onClick.AddListener(LoadLevel);
            _saveButton.onClick.AddListener(SaveLevel);
            _setStartButton.onClick.AddListener(OnSetStartButtonClicked);
            _createButton.onClick.AddListener(CreateNewLevel);
            _importButton.onClick.AddListener(ImportLevels);
            _exportButton.onClick.AddListener(ExportLevels);
        }

        private void ToggleWall(CellViewEditor cellView)
        {
            if (_editingLevel == null) return;

            Vector2Int pos = cellView.GridPosition;
            CellType current = _editingLevel.GetCell(pos.x, pos.y);

            if (current == CellType.Empty)
            {
                _editingLevel.SetCell(pos.x, pos.y, CellType.Exist);
                cellView.PaintUnpainted();
            }
            else
            {
                _editingLevel.SetCell(pos.x, pos.y, CellType.Empty);
                cellView.PaintEmpty();
            }
        }

        private void OnSetStartButtonClicked()
        {
            _isSettingStartPos = true;
        }

        private void SetStartPosition(Vector2Int pos)
        {
            _editingLevel.StartPosition = pos;
            _isSettingStartPos = false;

            _grid.UpdateBallPosition(_ballSpawner.Ball);
        }

        public void OnCellClicked(CellViewEditor cellView)
        {
            if (_editingLevel == null) return;

            if (_isSettingStartPos)
            {
                SetStartPosition(cellView.GridPosition);
                return;
            }

            ToggleWall(cellView);
        }

        #region Save / Load

        private void SaveLevel()
        {
            if (_editingLevel == null) return;
            if (!int.TryParse(_levelIndex.text, out int index)) return;

            if (!LevelValidator.Validate(_editingLevel, out string error))
            {
                Debug.LogError("Level validation failed: " + error);
                return;
            }

            SetLevel(_editingLevel, ref index);
#if UNITY_EDITOR
            _levelsConfig.SetLevel(index, _editingLevel);
            UnityEditor.EditorUtility.SetDirty(_levelsConfig);
            UnityEditor.AssetDatabase.SaveAssets();
#endif

            Debug.Log($"Level {index} saved into LevelsDataConfig");
        }

        private void SetLevel(LevelData level, ref int index)
        {
            if (_levels == null || _levels.Length <= index)
            {
                index = _levels?.Length ?? 0;
                Array.Resize(ref _levels, index + 1);
            }

            _levels[index] = level;
        }

        private void LoadLevel()
        {
            if (!int.TryParse(_levelIndex.text, out int index)) return;

            LevelData level = _levels[index];

            if (level == null)
            {
                Debug.LogWarning("Level not found in config");
                return;
            }

            _editingLevel = level;

            _width.text = level.Width.ToString();
            _height.text = level.Height.ToString();

            ApplyLevelToGrid();
        }

        private void CreateNewLevel()
        {
            Int32.TryParse(_width.text, out int width);
            Int32.TryParse(_height.text, out int height);

            if (width <= 0 || height <= 0)
            {
                Debug.LogWarning("Width and Height must be greater than 0");
                return;
            }
            _editingLevel = new LevelData();
            _editingLevel.Width = width;
            _editingLevel.Height = height;
            _editingLevel.Cells = new CellType[width * height];
            _editingLevel.StartPosition = Vector2Int.zero;
            ApplyLevelToGrid();
        }

        private void ApplyLevelToGrid()
        {
            _grid.DestroyGrid();
            _grid.SetLevel(_editingLevel, _ballSpawner.Ball);
        }

        private void ExportLevels()
        {
            LevelsRuntimeJsonTool.ExportToJson(_levels);
        }

        private void ImportLevels()
        {
            _levels = LevelsRuntimeJsonTool.ImportFromJson() ?? System.Array.Empty<LevelData>();
        }

        #endregion
    }
#endif
}