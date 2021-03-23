using UnityEngine;
using UnityEngine.UI;

namespace TwoRuns
{
    public class HubSceneController : MonoBehaviour
    {
        [SerializeField] private Text messageText;
        [SerializeField] private Button buttonReady;

        public void OnReadyButtonClick()
        {
            messageText.text = Consts.MSG_GAME_SEARCH;
            buttonReady.interactable = false;

            ThreadHelper.RunInNewThread(() =>
            {
                bool gameReady = WebClient.SendReady();

                ThreadHelper.RunInUnityThread(() => // in Unity thread
                {
                    buttonReady.interactable = true;
                    if (gameReady)
                    {
                        messageText.text = Consts.MSG_GAME_FOUND; // load online scene in WebClient
                    }
                    else
                    {
                        messageText.text = Consts.MSG_GAME_NOT_FOUND;
                    }
                });
            });
        }
        public void OnSingleplayerButtonClick()
        {
            SceneLoader.LoadSingleplayer();
        }
    }
}
