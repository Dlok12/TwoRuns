using UnityEngine;
using UnityEngine.EventSystems;

namespace TwoRuns
{
    public class PlayerButtonControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private PlayerPhysics playerPhysics;
        [SerializeField] private Moving moving;
        
        private enum Moving
        {
            left,
            right
        }
        private bool _isPressed = false;

        private void FixedUpdate()
        {
            if (_isPressed)
            {
                switch (moving)
                {
                    case Moving.left:
                        playerPhysics.MoveLeft();
                        break;
                    case Moving.right:
                        playerPhysics.MoveRight();
                        break;
                    default:
                        break;
                }
            }
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            _isPressed = true;
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            _isPressed = false;
        }
    }
}
