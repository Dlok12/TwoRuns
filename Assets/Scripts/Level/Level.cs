using System.Collections.Generic;
using UnityEngine;

namespace TwoRuns
{
    public class Level : MonoBehaviour
    {
        public bool LevelLoaded { get => _loaded; }
        public float levelSpeed = 0.5f;

        [SerializeField] private GameObject p1Control;
        [SerializeField] private GameObject p2Control;

        [SerializeField] private GameObject chunkPrefab;
        [SerializeField] private GameObject hindrancePrefab;

        // Level objects
        private Queue<GameObject> _chunks = new Queue<GameObject>();
        private Queue<GameObject> _hindrances = new Queue<GameObject>();
        private GameObject _lastChunk;

        // params
        private float _chunkLength = 64f;
        private float _chunkRadius = 10f;
        private float _polygonesCount = 16;
        private int _chunkCount = 3;
        private float _chunkSpawnPoint;

        // Cache vars
        private Transform _p1Transform;
        private Transform _p2Transform;

        private static Level _instance = null;
        private bool _loaded = false;
        private float _p2Position = 0;

        public static Level CurrentLevel
        {
            get => _instance;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Position in degrees</returns>
        public int GetP1Position()
        {
            return (int)_p1Transform.rotation.eulerAngles.z;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="position">Position in degrees</param>
        public void SetP1Postion(int position)
        {
            _p1Transform.rotation = Quaternion.Euler(0, 0, position);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="position">Position in degrees</param>
        public void SetP2Position(int position)
        {
            //_p2Transform.rotation = Quaternion.Euler(0, 0, position);
            _p2Position = position;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="postion">Number of platform</param>
        public void AddHindrance(int postion)
        {
            float angle = postion * Mathf.PI * 2 / _polygonesCount;
            float x = Mathf.Cos(angle) * _chunkRadius;
            float y = Mathf.Sin(angle) * _chunkRadius;
            float z = _chunkSpawnPoint;

            GameObject h = Instantiate(
                hindrancePrefab,
                new Vector3(x, y, z),
                Quaternion.Euler(0, 0, (angle + Mathf.PI / 2) * Mathf.Rad2Deg),
                transform);
            _hindrances.Enqueue(h);
        }


        private void InterpolateP2Rotation()
        {
            float lastPos = _p2Transform.rotation.eulerAngles.z;
            float nextPos = _p2Position;
            float t = Consts.K_LERP;

            if (lastPos - nextPos > 300)
                lastPos -= 360;
            else if (nextPos - lastPos > 300)
                nextPos -= 360;

            if (Mathf.Abs(lastPos - nextPos) < 10)
                t *= Mathf.Abs(lastPos - nextPos) / 10f;

            _p2Transform.rotation = Quaternion.Euler(
                Vector3.Lerp(new Vector3(0, 0, lastPos), new Vector3(0, 0, nextPos), t));
        }
        private void SetOnline()
        {
            Destroy(gameObject.GetComponent<LevelGenerator>());
            
            new GameObject("WebClient").AddComponent<WebClient>();
            p2Control.SetActive(true);
        }
        private void SetSingleplayer()
        {
            gameObject.AddComponent<LevelGenerator>();
            p2Control.SetActive(false);
        }

        private void Start()
        {
            if (_instance != null)
            {
                Destroy(this);
            }
            else
            {
                _instance = this;
            }

            // Cache vars
            _p1Transform = p1Control.transform;
            _p2Transform = p2Control.transform;

            // Set online / single
            if (SceneLoader.IsOnline)
                SetOnline();
            else
                SetSingleplayer();

            _chunkSpawnPoint = transform.position.z + (_chunkCount - 1) * _chunkLength;

            // Build level
            GameObject chunk = null;
            for (int i = 0; i < _chunkCount; i++)
            {
                chunk = Instantiate(chunkPrefab, transform);

                chunk.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - i * _chunkLength);
                _chunks.Enqueue(chunk);
            }
            _lastChunk = chunk;
            _loaded = true;
        }
        private void FixedUpdate()
        {
            // Move chunks
            foreach (GameObject chunk in _chunks)
            {
                chunk.transform.position += new Vector3(0, 0, -levelSpeed);
            }

            // Move first chunk to the end of queue if space in the end is available
            while (_lastChunk.transform.position.z < _chunkSpawnPoint - _chunkLength)
            {
                float deltaZ = _lastChunk.transform.position.z - (_chunkSpawnPoint - _chunkLength);
                _lastChunk = _chunks.Peek();
                _chunks.Dequeue();

                _chunks.Enqueue(_lastChunk);
                _lastChunk.transform.position = new Vector3(transform.position.x, transform.position.y, _chunkSpawnPoint + deltaZ);
            }

            // Move hindrances
            foreach (GameObject h in _hindrances)
            {
                h.transform.position += new Vector3(0, 0, -levelSpeed);
            }
            // Destroy hindrance if it in destroy point
            if (_hindrances.Count > 0)
            {
                GameObject hindrance = _hindrances.Peek();
                while (hindrance.transform.position.z < _chunkSpawnPoint - _chunkLength * _chunkCount)
                {
                    _hindrances.Dequeue();
                    Destroy(hindrance);

                    hindrance = _hindrances.Peek();
                }
            }

            InterpolateP2Rotation();
        }
    }
}
