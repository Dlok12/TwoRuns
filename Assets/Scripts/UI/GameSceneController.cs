using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TwoRuns
{
    class GameSceneController : MonoBehaviour
    {
        [SerializeField] private PlayerPhysics playerPhysics;
        [SerializeField] private KeyCode left = KeyCode.A;
        [SerializeField] private KeyCode right = KeyCode.D;
        [SerializeField] private KeyCode menu = KeyCode.Escape;

        [SerializeField] private Text scoresText;

        private int _fps = 0;

        private IEnumerator UpdateFps()
        {
            while (true)
            {
                _fps = (int)(1 / Time.deltaTime);
                yield return new WaitForSeconds(0.5f);
            }
        }
        private void Start()
        {
            StartCoroutine(UpdateFps());
        }
        private void FixedUpdate()
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
                if (SceneLoader.IsOnline)
                {
                    WebClient.Disconnect();
                }
                else
                {
                    SceneLoader.LoadLogin();
                }
            }

            Player.IncrementScores();
        }
        private void Update()
        {
            scoresText.text = $"Highscore: {Player.Highscore}\nScores: {Player.Scores}\n" +
                $"P{WebClient.playerNumber} FPS: {_fps} / {(int)(1 / Time.fixedDeltaTime)}";
        }
    }
}
