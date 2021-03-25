using UnityEngine;

namespace TwoRuns
{
    public class Player
    {
        private static bool _died = false;
        private static int _scores = 0;
        public static int Highscore
        {
            get
            {
                return PlayerPrefs.GetInt(Consts.PLAYER_PREFS_KEY_HIGHSCORE);
            }
        }
        public static int Scores
        {
            get => _scores;
        }
        public static bool Died
        {
            get => _died;
            set
            {
                _died = value;
                if (_died)
                    ResetScores();
            }
        }
        public static void IncrementScores()
        {
            if (!_died)
                _scores++;
        }
        /// <summary>
        /// Save scores if it's highscore and set current scores to zero
        /// </summary>
        public static void ResetScores()
        {
            if (Highscore < _scores)
            {
                PlayerPrefs.SetInt(Consts.PLAYER_PREFS_KEY_HIGHSCORE, _scores);
                PlayerPrefs.Save();
            }
            _scores = 0;
        }
    }
}
