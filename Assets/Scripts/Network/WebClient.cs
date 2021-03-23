using System;
using System.Collections;
using System.IO;
using System.Net;
using UnityEngine;
using WebSocketSharp;

namespace TwoRuns
{
    public class WebClient : MonoBehaviour
    {
        public static WebClient instance = null;
        /// <summary>
        /// Where WS opened execute OnOpenEventHandler in Unity thread; After - loads Hub
        /// </summary>
        public static event EventHandler OnOpenEventHandler;
        /// <summary>
        /// Where WS closed execute OnCloseEventHandler in Unity thread; After - loads Login if it is not SinglePlayer
        /// </summary>
        public static event EventHandler OnCloseEventHandler;
        /// <summary>
        /// On WS message parse messages bytes and execute OnMessageEventHandler in Unity thread
        /// </summary>
        public static event EventHandler OnMessageEventHandler;

        public static bool IsFirstConnection { get => _firstConnection; }

        public static byte playerNumber = 0;

        private static string _token;
        private static WebSocket _ws;
        private static bool _firstConnection = true;

        // JSON serialize
        private class AuthPayloads
        {
            public string email;
            public string password;

            public AuthPayloads(string email, string password)
            {
                this.email = email;
                this.password = password;
            }
        }
        private class AuthResponse
        {
            public string last_login;
            public string token;
            public int exp;
            public string uid;
        }

        /// <summary>
        /// Open WebSocket
        /// </summary>
        /// <returns>
        /// Connect status
        /// </returns>
        public static bool Connect()
        {
            if (_token.IsNullOrEmpty())
            {
                return false;
            }
            try
            {
                _ws = new WebSocket(string.Format(Consts.URL_WS, _token));
                _ws.OnOpen += OnWsOpen;
                _ws.OnClose += OnWsClose;
                _ws.OnMessage += OnWsMessage;

                _ws.Connect();
                if (_ws.IsAlive && _firstConnection)
                    _firstConnection = false;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }

            return true;
        }
        /// <summary>
        /// Close WebSocket
        /// </summary>
        public static void Disconnect()
        {
            if (_ws != null && _ws.IsAlive)
            {
                _ws.Close();
            }
        }

        /// <summary>
        /// Where WS opened execute OnOpenEventHandler in Unity thread; After - loads Hub
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnWsOpen(object sender, EventArgs e)
        {
            Debug.Log("Connected!");

            ThreadHelper.RunInUnityThread(() =>
            {
                if (OnOpenEventHandler != null)
                {
                    OnOpenEventHandler(sender, e);
                }
                SceneLoader.LoadHub();
            });
        }
        /// <summary>
        /// Where WS closed execute OnCloseEventHandler in Unity thread; After - loads Login if it is not SinglePlayer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnWsClose(object sender, CloseEventArgs e)
        {
            Debug.Log("Disconnected.");

            ThreadHelper.RunInUnityThread(() =>
            {
                if (OnCloseEventHandler != null)
                {
                    OnCloseEventHandler(sender, e);
                }
                if (!SceneLoader.IsSingleplayer())
                {
                    SceneLoader.LoadLogin();
                }
            });
        }
        /// <summary>
        /// On WS message parse messages bytes and execute OnMessageEventHandler in Unity thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnWsMessage(object sender, MessageEventArgs e)
        {
            ThreadHelper.RunInUnityThread(() =>
            {
                Debug.Log($"Response: {(int)e.RawData[0]}");
                ParseBytes(e.RawData);

                if (OnMessageEventHandler != null)
                {
                    OnMessageEventHandler(sender, e);
                }
            });
        }

        /// <summary>
        /// Send bytes by WS
        /// </summary>
        /// <param name="bytes">Message</param>
        /// <returns>Send status</returns>
        public static bool WsSend(byte[] bytes)
        {
            bool connected = _ws.IsAlive || Connect();
            if (connected)
                _ws.Send(bytes);
            return connected;
        }
        /// <summary>
        /// Player is loaded
        /// </summary>
        /// <returns>Message sended</returns>
        public static bool SendReady()
        {
            return WsSend(new byte[] { Consts.PLAYER_READY });
        }
        /// <summary>
        /// Send current player position
        /// </summary>
        /// <param name="postion"></param>
        /// <returns>Message sended</returns>
        public static bool SendPostion(int postion)
        {
            return WsSend(new byte[] { Consts.PLAYER_POSITION, (byte)postion });
        }

        /// <summary>
        /// Get token by http connection and save token in PlayerPrefs
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool Authorize(string login, string password)
        {
            try
            {
                // http request
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Consts.URL_AUTHORIZE);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = Consts.AUTHORIZE_TIMEOUT_MS;
                
                // Send login and password
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(JsonUtility.ToJson(new AuthPayloads(login, password)));
                }

                // http response
                HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    // Get token
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        AuthResponse result = JsonUtility.FromJson<AuthResponse>(streamReader.ReadToEnd());
                        _token = result.token;

                        //  Save token in PlayerPrefs
                        ThreadHelper.RunInUnityThread(() =>
                        {
                            PlayerPrefs.SetString(Consts.PLAYER_PREFS_KEY_TOKEN, _token);
                            PlayerPrefs.Save();
                        });
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }

            return true;
        }

        private IEnumerator ClientUpdate()
        {
            while (true)
            {
                //if (SceneLoader.IsOnlineScene() && _ws != null && LevelBuilder.instance != null && _ws.IsAlive)
                //{
                //    _ws.Send(new byte[] { Consts.PLAYER_POSITION, (byte)(LevelBuilder.instance.GetPlayerPosition() / 2) });
                //    /*Debug.Log(LevelBuilder.instance.GetPlayerPosition());
                //    LevelBuilder.instance.SetPlayer2Position(LevelBuilder.instance.GetPlayerPosition() + 45);*/
                //}
                yield return new WaitForSecondsRealtime(Consts.CLIENT_TICK_S);
            }
        }

        /// <summary>
        /// Parse message from WS
        /// </summary>
        /// <param name="bytes"></param>
        private static void ParseBytes(byte[] bytes)
        {
            //switch (bytes[0])
            //{
            //    case Consts.GAME_READY:
            //        SceneLoader.LoadOnline();
            //        playerNumber = bytes[1];
            //        break;

            //    case Consts.GAME_OVER:
            //        SceneLoader.LoadHub();
            //        break;

            //    case Consts.GAME_SET_BARRIER:
            //        if (LevelBuilder.instance.Builded)
            //            LevelBuilder.instance.SetBarrier(bytes[1]);
            //        break;

            //    case Consts.PLAYER_POSITION:
            //        Debug.Log($"{bytes[0]}, {bytes[1]}, {bytes[2]}; (len={bytes.Length})");
            //        if (LevelBuilder.instance.Builded)
            //        {
            //            if (bytes[1] != playerNumber)
            //            {
            //                LevelBuilder.instance.SetPlayer2Position(bytes[2] * 2);
            //            }
            //        }
            //        break;
            //}
        }

        private void Start()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);

            _token = PlayerPrefs.GetString(Consts.PLAYER_PREFS_KEY_TOKEN); // Load token in PlayerPrefs

            StartCoroutine(ClientUpdate());
        }
        private void OnApplicationQuit()
        {
            Disconnect();
        }
    }
}

