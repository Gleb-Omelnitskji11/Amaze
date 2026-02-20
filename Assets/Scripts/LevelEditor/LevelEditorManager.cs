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
        [SerializeField] private Button _newButton;
        [SerializeField] private Button _applyButton;
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
            _applyButton.onClick.AddListener(ApplyLevelToGrid);
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

            SetLevel(index, _editingLevel);
#if UNITY_EDITOR
            _levelsConfig.SetLevel(index, _editingLevel);
            UnityEditor.EditorUtility.SetDirty(_levelsConfig);
            UnityEditor.AssetDatabase.SaveAssets();
#endif

            Debug.Log($"Level {index} saved into LevelsDataConfig");
        }

        public void SetLevel(int index, LevelData level)
        {
            if (_levels == null || _levels.Length <= index)
            {
                System.Array.Resize(ref _levels, index + 1);
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

        public void ApplyLevelToGrid()
        {
            _grid.DestroyGrid();
            _grid.SetLevel(_editingLevel, _ballSpawner.Ball);
        }

        public void ExportLevels()
        {
            LevelsRuntimeJsonTool.ExportToJson(_levels);
        }

        public void ImportLevels()
        {
            _levels = LevelsRuntimeJsonTool.ImportFromJson();
        }

        #endregion
    }
#endif
}