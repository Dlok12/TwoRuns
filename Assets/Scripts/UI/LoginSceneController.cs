using UnityEngine;
using UnityEngine.UI;

namespace TwoRuns
{
    public class LoginSceneController : MonoBehaviour
    {
        [SerializeField] private InputField loginInputField;
        [SerializeField] private InputField passwordInputField;
        [SerializeField] private Toggle toggleRemember;
        [SerializeField] private Text messageText;
        [SerializeField] private Button buttonLogin;

        public void OnLoginButtonClick()
        {
            messageText.text = Consts.MSG_CONN_START;
            buttonLogin.interactable = false;

            if (!string.IsNullOrWhiteSpace(loginInputField.text) &&
                !string.IsNullOrWhiteSpace(passwordInputField.text))
            {
                if (toggleRemember.isOn)
                    PlayerPrefs.SetString(Consts.PLAYER_PREFS_LOGIN, loginInputField.text); // Save login

                ThreadHelper.RunInNewThread(() => // Connect in other thread
                {
                    bool connected =
                        WebClient.Authorize(loginInputField.text, passwordInputField.text) && // Get token
                        WebClient.Connect(); // Connect to WS by token

                    if (!connected)
                    {
                        ThreadHelper.RunInUnityThread(() => // in Unity thread
                        {
                            buttonLogin.interactable = true;
                            messageText.text = Consts.MSG_CONN_ERROR;
                        });
                    }
                });
            }
        }
        public void OnSingleplayerClick()
        {
            SceneLoader.LoadSingleplayer();
        }

        private void Start()
        {
            string login = PlayerPrefs.GetString(Consts.PLAYER_PREFS_LOGIN); // Set login text
            if (!string.IsNullOrEmpty(login))
                loginInputField.text = login;

            if (WebClient.IsFirstConnection) // AutoConnect by loaded token
            {
                buttonLogin.interactable = false;
                ThreadHelper.RunInNewThread(() =>
                {
                    bool connected = WebClient.Connect();

                    if (!connected)
                    {
                        ThreadHelper.RunInUnityThread(() =>
                        {
                            buttonLogin.interactable = true;
                        });
                    }
                });
            }
        }
    }
}
