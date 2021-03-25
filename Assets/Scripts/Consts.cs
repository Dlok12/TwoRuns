namespace TwoRuns
{
    public class Consts
    {
        // Network
        public const string URL_AUTHORIZE = "http://192.168.1.37:8000/app/users/login/";
        public const string URL_WS = "ws://192.168.1.37:8000/ws/client/?token={0}";
        public const int AUTHORIZE_TIMEOUT_MS = 3000;
        public const float CLIENT_TICK_S = 0.05f;

        // Network bytes
        public const byte PLAYER_READY = 1;
        public const byte GAME_READY = 2;
        public const byte PLAYER_POSITION = 5;
        public const byte GAME_SET_BARRIER = 8;
        public const byte GAME_OVER = 101;

        // UI Message
        public const string MSG_CONN_ERROR = "Connection Error((9(";
        public const string MSG_CONN_START = "Connecting...";
        public const string MSG_GAME_SEARCH = "w8...";
        public const string MSG_GAME_FOUND = "Game found!";
        public const string MSG_GAME_NOT_FOUND = @"Game not found (\/)(oO)(\/)";

        // Scene names
        public const string LOGIN_SCENE_NAME = "LoginScene";
        public const string HUB_SCENE_NAME = "HubScene";
        public const string GAME_SCENE_NAME = "GameScene";

        public const string ONLINE_SCENE_NAME = "OnlineScene";

        // Player prefs
        public const string PLAYER_PREFS_LOGIN = "login";
        public const string PLAYER_PREFS_KEY_TOKEN = "token";
        public const string PLAYER_PREFS_KEY_HIGHSCORE = "highscore";

        // Interpolation
        public const float K_LERP = 0.25f;
    }
}
