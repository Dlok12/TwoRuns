using UnityEngine.SceneManagement;

namespace TwoRuns
{
    class SceneLoader
    {
        public static void LoadLogin()
        {
            SceneManager.LoadScene(Consts.LOGIN_SCENE_NAME);
        }
        public static void LoadHub()
        {
            SceneManager.LoadScene(Consts.HUB_SCENE_NAME);
        }
        public static void LoadSingleplayer()
        {
            SceneManager.LoadScene(Consts.SINGLEPLAYER_SCENE_NAME);
        }
        public static void LoadOnline()
        {
            SceneManager.LoadScene(Consts.ONLINE_SCENE_NAME);
        }
        public static bool IsSingleplayer()
        {
            return SceneManager.GetActiveScene().name == Consts.SINGLEPLAYER_SCENE_NAME;
        }
        public static bool IsOnlineScene()
        {
            return SceneManager.GetActiveScene().name == Consts.ONLINE_SCENE_NAME;
        }
    }
}
