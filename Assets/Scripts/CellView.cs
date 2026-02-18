using UnityEngine;

public class CellView : MonoBehaviour
{
    [SerializeField]
    private Renderer _rend;
    
    [SerializeField]
    private bool _painted;

    public Vector2Int GridPosition;// { get; private set; }
    public Vector3 Position => transform.position;
    
    private static GameSettings GameSettings;

    public static void Setup(GameSettings gameSettings)
    {
        GameSettings = gameSettings;
    }

    public void Init(Vector2Int pos, CellType type)
    {
        GridPosition = pos;

        if (type == CellType.Empty)
        {
            _rend.material.color = GameSettings.WallColor;
        }
        else
        {
            _rend.material.color = GameSettings.UnpaintedColor;
        }
    }

    public void PaintFilled()
    {
        if (_painted) return;

        _painted = true;
        _rend.material.color = GameSettings.PaintedColor;
        GameManager.Instance.AddPaintedCell();
    }

    public void PaintUnpainted()
    {
        if (!_painted) return;
        
        _painted = false;
        _rend.material.color = GameSettings.UnpaintedColor;
    }

    public bool IsPainted() => _painted;
}
