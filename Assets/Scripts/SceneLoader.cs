using UnityEngine.SceneManagement;

namespace TwoRuns
{
    class SceneLoader
    {
        private static bool _isOnline = false;

        /// <summary>
        /// Is Online scene
        /// </summary>
        public static bool IsOnline
        {
            get => _isOnline;
        }
        public static void LoadLogin()
        {
            SceneManager.LoadScene(Consts.LOGIN_SCENE_NAME);
            _isOnline = false;
        }
        public static void LoadHub()
        {
            SceneManager.LoadScene(Consts.HUB_SCENE_NAME);
            _isOnline = false;
        }
        public static void LoadSingleplayer()
        {
            SceneManager.LoadScene(Consts.GAME_SCENE_NAME);
            _isOnline = false;
        }
        public static void LoadOnline()
        {
            SceneManager.LoadScene(Consts.GAME_SCENE_NAME);
            _isOnline = true;
        }

        public static void LoadTest()
        {
            SceneManager.LoadScene("TestScene");
            _isOnline = false;
        }
    }
}
