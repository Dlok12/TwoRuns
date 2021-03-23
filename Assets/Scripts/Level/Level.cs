using System.Collections.Generic;
using UnityEngine;

namespace TwoRuns
{
    public class Level : MonoBehaviour
    {
        [SerializeField] private GameObject p1Control;
        [SerializeField] private GameObject p2Control;

        [SerializeField] private GameObject pipePrefab;

        // Level objects
        private Queue<GameObject> _pipes;
        private GameObject _lastPipe;

        // params
        private float _pipeWidth = 11f;
        private int _pipeCount = 3;
        private float _levelSpeed = 0.5f;

        private float _pipeSpawnPoint;

        // Cache vars
        private Transform _p1Transform;
        private Transform _p2Transform;

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
        public void SetP2Position(int position)
        {
            _p2Transform.rotation = Quaternion.Euler(0, 0, position);
        }

        private void Start()
        {
            _pipeSpawnPoint = transform.position.z + _pipeCount * _pipeWidth;
            _pipes = new Queue<GameObject>();

            if (_p1Transform != null)
                _p1Transform = p1Control.transform;
            if (_p2Transform != null)
                _p2Transform = p2Control.transform;

            // Build level
            GameObject pipe = null;
            for (int i = 0; i < _pipeCount; i++)
            {
                pipe = Instantiate(pipePrefab, transform);

                pipe.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - i * _pipeWidth);
                _pipes.Enqueue(pipe);
            }
            _lastPipe = pipe;
        }
        private void FixedUpdate()
        {
            // Move pipes
            foreach (GameObject pipe in _pipes)
            {
                pipe.transform.position += new Vector3(0, 0, -_levelSpeed);
            }

            // Move first pipe to the end of queue if space in the end is aviable
            while (_lastPipe.transform.position.z < _pipeSpawnPoint - _pipeWidth)
            {
                float deltaZ = _lastPipe.transform.position.z - (_pipeSpawnPoint - _pipeWidth);
                _lastPipe = _pipes.Peek();
                _pipes.Dequeue();

                _pipes.Enqueue(_lastPipe);
                _lastPipe.transform.position = new Vector3(transform.position.x, transform.position.y, _pipeSpawnPoint + deltaZ);
            }
        }
    }
}
