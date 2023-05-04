using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectSpawner.Core
{
    public class RandomWalk
    {
        private readonly float stepX;

        // private readonly List<float> listX = new List<float>();
        // private readonly List<float> listY = new List<float>();
        private readonly List<Vector2> positions = new List<Vector2>();

        // private float currentX = 0f;
        // private float currentY = 0f;
        private Vector2 currentPos = Vector2.zero; public Vector2 CurrentPos => currentPos;

        public RandomWalk(float stepX = 1f)
        {
            this.stepX = stepX;

            positions.Add(currentPos);
        }

        public void Tick()
        {
            var stepY = Random.value >= .5 ? -1 : 1;
            currentPos += new Vector2(stepX, stepY);

            positions.Add(currentPos);
        }

        void Split()
        {

        }
    }

    public class RandomPath
    {
        private readonly Vector2 startPos;
        // private readonly Vector2 endPos;
        private readonly float distance;
        private readonly int segments;
        private readonly float splitChance;

        private List<Vector2> path = new List<Vector2>();

        public RandomPath(
            Vector2 startPos,
            // Vector2 endPos,
            float distance,
            int segments,
            float splitChance
        )
        {
            this.startPos = startPos;
            // this.endPos = endPos;
            this.distance = distance;
            this.segments = segments;
            this.splitChance = splitChance;
        }

        public void Init()
        {
            path.Add(Vector2.zero);

            // split in segments
            // var distance = Vector2.Distance(startPos, endPos);
            var segmentLength = distance / segments;

            for (int i = 0; i < segments; i++)
            {
                var progress = i + 1 / segments;

                // var pos = Vector2.Lerp(startPos, endPos, progress);
                // path.Add(pos);
            }
        }

        public void Tick()
        {

        }

        void Split()
        {

        }
    }
}

namespace ObjectSpawner.Main
{
    public class ObjectSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private Transform container;
        [SerializeField] private float interval = .2f;

        private Core.RandomWalk randomWalk;
        private float lastSpawnTime = -Mathf.Infinity;

        // private float currentPosX = 0;

        // Start is called before the first frame update
        void Start()
        {
            randomWalk = new Core.RandomWalk(
                stepX: 5f
            );
        }

        // Update is called once per frame
        void Update()
        {
            if (Time.time >= lastSpawnTime + interval)
            {
                randomWalk.Tick();

                SpawnObject();

                lastSpawnTime = Time.time;
            }
        }

        void SpawnObject()
        {
            var posDisplacement = (Random.insideUnitCircle * 1f).ToVector3();
            var pos = new Vector3(randomWalk.CurrentPos.x, 0, randomWalk.CurrentPos.y) + posDisplacement;
            var rot = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

            var obj = Instantiate(prefab, pos, rot, container);

            obj.transform.localScale *= Random.Range(.75f, 1.5f);

            // currentPosX += 5f;
        }
    }
}
