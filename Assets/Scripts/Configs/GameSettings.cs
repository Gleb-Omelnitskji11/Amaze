using UnityEngine;

[CreateAssetMenu(menuName = "Configs/GameSettings")]
public class GameSettings : ScriptableObject
{
    public float MoveSpeed = 8f;
    public Color PaintedColor = Color.blue;
    public Color UnpaintedColor = Color.white;
    public Color WallColor = Color.white;
}
