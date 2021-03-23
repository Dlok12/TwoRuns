using UnityEngine;
using UnityEngine.UI;

namespace TwoRuns
{
    class SingleplayerSceneController : MonoBehaviour
    {
        [SerializeField] private PlayerPhysics playerPhysics;
        [SerializeField] private KeyCode left = KeyCode.A;
        [SerializeField] private KeyCode right = KeyCode.D;
        [SerializeField] private KeyCode menu = KeyCode.Escape;

        void FixedUpdate()
        {
            if (Input.GetKey(left))
            {
                playerPhysics.MoveLeft();
            }
            if (Input.GetKey(right))
            {
                playerPhysics.MoveRight();
            }
            if (Input.GetKey(menu))
            {
                if (SceneLoader.IsOnlineScene())
                {
                    WebClient.Disconnect();
                }
                else
                {
                    SceneLoader.LoadLogin();
                }
            }
        }
    }
}
