using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoRuns
{
    public class LevelGenerator : MonoBehaviour
    {
        [SerializeField] private float generatorTickSeconds = .1f;

        [SerializeField] private int platformCount = 16;

        [SerializeField] private float pBarrier = 0.25f; // probability
        [SerializeField] private float pFence = 0.05f;
        [SerializeField] private float pWall = 0.06f;
        [SerializeField] private float pWallLong = 0.01f;
        [SerializeField] private float pSpine2 = 0.02f;
        [SerializeField] private float pSpine3 = 0.005f;

        private Hindrance[] hindrances;
        private static LevelGenerator _instance = null;

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

            hindrances = new Hindrance[]
            {
                new SingleBarrier(pBarrier),
                new Fence(pFence),
                new Wall(pWall),
                new WallLong(pWallLong),
                new Spine2(pSpine2),
                new Spine3(pSpine3)
            };

            StartCoroutine(GeneratorLoop());
        }
        private IEnumerator GeneratorLoop()
        {
            while (true)
            {
                foreach (var h in hindrances)
                {
                    if (h.Generate())
                    {
                        Queue<List<int>> q = h.GetHindrancePositionsQueue(Random.Range(0, platformCount), platformCount);
                        while (q.Count > 0)
                        {
                            foreach (var pos in q.Dequeue())
                            {
                                Level.CurrentLevel.AddHindrance(pos);
                            }
                            yield return new WaitForSeconds(generatorTickSeconds);
                        }
                        break;
                    }
                }

                yield return new WaitForSeconds(generatorTickSeconds);
            }
        }
    }

    public abstract class Hindrance
    {
        public float probability;
        public Hindrance(float probability)
        {
            this.probability = probability;
        }
        public virtual bool Generate()
        {
            return Random.Range(0.0f, 1.0f) < probability;
        }
        public abstract Queue<List<int>> GetHindrancePositionsQueue(int startPosition, int platformCount);
    }
    public class SingleBarrier : Hindrance
    {
        public SingleBarrier(float probability) : base(probability)
        {
            base.probability = probability;
        }
        public override Queue<List<int>> GetHindrancePositionsQueue(int startPosition, int platformCount)
        {
            Queue<List<int>> pattern = new Queue<List<int>>(1);
            List<int> barrier = new List<int>(1);
            barrier.Add(startPosition % platformCount);
            pattern.Enqueue(barrier);

            return pattern;
        }
    }
    public class Fence : Hindrance
    {
        public Fence(float probability) : base(probability)
        {
            base.probability = probability;
        }
        public override Queue<List<int>> GetHindrancePositionsQueue(int startPosition, int platformCount)
        {
            Queue<List<int>> pattern = new Queue<List<int>>(1);
            List<int> fence = new List<int>();
            for (int i = 0; i < platformCount; i += 4)
            {
                fence.Add((startPosition + i) % platformCount);
            }
            pattern.Enqueue(fence);

            return pattern;
        }
    }
    public class Wall : Hindrance
    {
        public Wall(float probability) : base(probability)
        {
            base.probability = probability;
        }
        public override Queue<List<int>> GetHindrancePositionsQueue(int startPosition, int platformCount)
        {
            Queue<List<int>> pattern = new Queue<List<int>>(1);
            List<int> wall = new List<int>();
            for (int i = 0; i < platformCount / 2 - 4; i++)
            {
                wall.Add((startPosition + i) % platformCount);
                wall.Add((startPosition + i + platformCount / 2) % platformCount);
            }
            pattern.Enqueue(wall);

            return pattern;
        }
    }
    public class WallLong : Hindrance
    {
        const int LENGTH = 10;
        public WallLong(float probability) : base(probability)
        {
            base.probability = probability;
        }
        public override Queue<List<int>> GetHindrancePositionsQueue(int startPosition, int platformCount)
        {
            Queue<List<int>> wallLong = new Queue<List<int>>(LENGTH);
            for (int t = 0; t < LENGTH; t++)
            {
                List<int> wall = new List<int>();
                for (int i = 0; i < platformCount / 2; i++)
                {
                    wall.Add((startPosition + i) % platformCount);
                }
                wallLong.Enqueue(wall);
            }

            return wallLong;
        }
    }
    public class Spine2 : Hindrance
    {
        const int LENGTH = 30;
        public Spine2(float probability) : base(probability)
        {
            base.probability = probability;
        }
        public override Queue<List<int>> GetHindrancePositionsQueue(int startPosition, int platformCount)
        {
            Queue<List<int>> spine = new Queue<List<int>>(LENGTH);
            int k = Random.Range(-1, 2);
            for (int t = 0; t < LENGTH; t++)
            {
                List<int> spinebars = new List<int>();
                spinebars.Add((startPosition + platformCount + k * (t % platformCount)) % platformCount);
                spinebars.Add((startPosition + platformCount / 2 * 3 + k * (t % platformCount)) % platformCount);

                spine.Enqueue(spinebars);
            }

            return spine;
        }
    }
    public class Spine3 : Hindrance
    {
        const int LENGTH = 20;
        public Spine3(float probability) : base(probability)
        {
            base.probability = probability;
        }
        public override Queue<List<int>> GetHindrancePositionsQueue(int startPosition, int platformCount)
        {
            Queue<List<int>> spine = new Queue<List<int>>(LENGTH);
            int k = Random.Range(-1, 2);
            for (int t = 0; t < LENGTH; t++)
            {
                List<int> spinebars = new List<int>();
                spinebars.Add((startPosition + platformCount / 3 * 3 + k * (t % platformCount)) % platformCount);
                spinebars.Add((startPosition + platformCount / 3 * 4 + k * (t % platformCount)) % platformCount);
                spinebars.Add((startPosition + platformCount / 3 * 5 + k * (t % platformCount)) % platformCount);

                spine.Enqueue(spinebars);
            }

            return spine;
        }
    }
}
