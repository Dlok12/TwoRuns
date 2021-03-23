using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace TwoRuns
{
    public class ThreadHelper : MonoBehaviour
    {
        public static ThreadHelper instance = null;
        private static Queue<Action> actionsQueue = new Queue<Action>();

        public static void RunInUnityThread(Action action)
        {
            if (instance == null)
                Debug.LogError("ERROR: ThreadHelper is null but called");

            actionsQueue.Enqueue(action);
        }
        public static void RunInNewThread(Action action)
        {
            if (instance == null)
                Debug.LogError("ERROR: ThreadHelper is null but called");

            Task.Run(action);
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
        }
        private void Update()
        {
            while (actionsQueue.Count > 0)
                actionsQueue.Dequeue()(); // calll action
        }
    }
}
