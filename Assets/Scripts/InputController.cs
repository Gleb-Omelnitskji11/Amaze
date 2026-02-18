using System;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private Vector2 _startTouch;
    
    public static InputController Instance;

    private void Start()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            _startTouch = Input.mousePosition;

        if (Input.GetMouseButtonUp(0))
        {
            Vector2 delta = (Vector2)Input.mousePosition - _startTouch;

            if (delta.magnitude < 50) return;
            Vector2Int swipe;

            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                swipe = delta.x > 0 ? Vector2Int.right : Vector2Int.left;
            }
            else
            {
                swipe = delta.y > 0 ? Vector2Int.up : Vector2Int.down;
            }
            
            OnSwipeEvent?.Invoke(swipe);
        }
    }
    
    public event Action<Vector2Int> OnSwipeEvent;
}
