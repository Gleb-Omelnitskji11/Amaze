using System;
using UnityEngine;

namespace Amaze
{
    public class InputController : MonoBehaviour
    {
        private Vector2 _startTouch;
        private const float SwipeThreshold = 50f;
        
        public static InputController Instance;

        private void Start()
        {
            Instance = this;
            Input.multiTouchEnabled = false;
        }

        private void Update()
        {
#if UNITY_EDITOR || UNITY_STANDALONE

            if (Input.GetMouseButtonDown(0))
                _startTouch = Input.mousePosition;

            if (Input.GetMouseButtonUp(0))
                ProcessSwipe((Vector2)Input.mousePosition);

#else

            if (Input.touchCount == 0)
                return;

            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
                _startTouch = touch.position;

            if (touch.phase == TouchPhase.Ended)
                ProcessSwipe(touch.position);

#endif
        }

        private void ProcessSwipe(Vector2 endPosition)
        {
            Vector2 delta = endPosition - _startTouch;

            if (delta.sqrMagnitude < SwipeThreshold * SwipeThreshold)
                return;

            Vector2Int swipe;

            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                swipe = delta.x > 0 ? Vector2Int.right : Vector2Int.left;
            else
                swipe = delta.y > 0 ? Vector2Int.up : Vector2Int.down;

            OnSwipeEvent?.Invoke(swipe);
        }
        
        public event Action<Vector2Int> OnSwipeEvent;
    }
}
