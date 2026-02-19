using DG.Tweening;
using UnityEngine;

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

    public void Init(Vector2Int pos, CellType type)
    {
        GridPosition = pos;

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
        GameManager.Instance.AddPaintedCell();
    }

    public void PaintUnpainted()
    {
        if (!_painted) return;
        
        _painted = false;
        _rend.material.color = GameSettings.UnpaintedColor;
    }
    
    public void PaintEmpty()
    {
        _painted = false;
        _rend.material.color = GameSettings.EmptyColor;
    }

    public bool IsPainted() => _painted;
}
