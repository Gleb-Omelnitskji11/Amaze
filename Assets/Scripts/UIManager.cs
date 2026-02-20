using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Amaze
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _progressText;
        [SerializeField] private Scrollbar _progressScrollbar;
        [SerializeField] private GameObject _winPanel;
        [SerializeField] private Button _restartButton;
        [SerializeField] private TMP_Text _currentLevelText;
        private Tween _progressTween;
        private Tween _textTween;
        private float _currentVisualProgress;

        private void Awake()
        {
            _restartButton.onClick.AddListener(Restart);
        }

        private void OnDestroy()
        {
            _restartButton.onClick.RemoveAllListeners();
        }

        public void UpdateProgress(float percent)
        {
            percent = Mathf.Clamp(percent, 0f, 100f);

            float targetNormalized = percent / 100f;

            _progressTween?.Kill();
            _textTween?.Kill();

            _progressTween = DOTween.To(
                    () => _progressScrollbar.size,
                    x => _progressScrollbar.size = x,
                    targetNormalized,
                    0.25f)
                .SetEase(Ease.OutCubic);

            _textTween = DOTween.To(
                    () => _currentVisualProgress,
                    x =>
                    {
                        _currentVisualProgress = x;
                        _progressText.text = Mathf.RoundToInt(x) + "%";
                    },
                    percent,
                    0.25f)
                .SetEase(Ease.OutCubic);
        }

        public void ShowWin()
        {
            _winPanel.SetActive(true);
        }

        public void Restart()
        {
            ResetInstant();
            OnRestartClicked?.Invoke();
            _winPanel.SetActive(false);
        }

        public void SetLevel(int level)
        {
            _currentLevelText.text = level.ToString();
        }

        private void ResetInstant()
        {
            _progressTween?.Kill();
            _textTween?.Kill();

            _currentVisualProgress = 0f;

            _progressScrollbar.size = 0f;
            _progressText.text = "0%";
        }

        public event Action OnRestartClicked;
    }
}