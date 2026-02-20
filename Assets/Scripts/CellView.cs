using System;
using DG.Tweening;
using Amaze.Configs;
using UnityEngine;

namespace Amaze
{
    public class CellView : MonoBehaviour
    {
        [SerializeField]
        protected Renderer _rend;
    
        [SerializeField]
        protected bool _painted;

        public Vector2Int GridPosition;// { get; private set; }
        public Vector3 Position => transform.position;
        
        protected static GameSettings GameSettings;

        public static void Setup(GameSettings gameSettings)
        {
            GameSettings = gameSettings;
        }

        public event Action<CellView> OnPainted;

        public void Init(Vector2Int pos, CellType type)
        {
            GridPosition = pos;

            _painted = false;
            if (_rend != null)
                _rend.material.DOKill();

            if (type == CellType.Empty)
            {
                _rend.material.color = GameSettings.EmptyColor;
            }
            else
            {
                _rend.material.color = GameSettings.UnpaintedColor;
            }
        }

        public virtual void PaintFilled()
        {
            if (_painted) return;

            _painted = true;
            _rend.material.DOColor(GameSettings.PaintedColor, 0.5f);
            OnPainted?.Invoke(this);
        }

        public void PaintUnpainted()
        {
            _painted = false;
            _rend.material.color = GameSettings.UnpaintedColor;
        }
    
        public void PaintEmpty()
        {
            _painted = false;
            _rend.material.color = GameSettings.EmptyColor;
        }

        public void Deactivate()
        {
            _painted = false;
            gameObject.SetActive(false);
        }

        public bool IsPainted() => _painted;
    }
}
