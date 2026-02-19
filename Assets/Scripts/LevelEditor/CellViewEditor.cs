using DG.Tweening;
using UnityEngine;

public class CellViewEditor : CellView
{
    public void OnMouseDown()
    {
        LevelEditorManager.Instance.OnCellClicked(this);
    }
    
    public override void PaintFilled()
    {
        if (_painted) return;

        _painted = true;
        _rend.material.color = GameSettings.PaintedColor;
    }
}
