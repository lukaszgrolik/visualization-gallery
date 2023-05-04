using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(RandomWalk))]
    public class RandomWalkEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var script = (RandomWalk)target;

            if (DrawDefaultInspector())
            {
                // if (script.autoUpdate)
                // {
                //     script.Generate();
                // }
            }

            if (GUILayout.Button("Generate"))
            {
                script.Generate();
            }

            if (GUILayout.Button("Clear"))
            {
                script.Clear();
            }
        }
    }
#endif

    public class RandomWalk : MonoBehaviour
    {
        [Header("Background")]

        [SerializeField] private MeshRenderer backgroundRend;
        [SerializeField] private Color backgroundColor = Color.white;

        [Header("Lines")]

        [SerializeField] private GameObject linePrefab;
        [SerializeField] private Transform container;

        [SerializeField] private int seed = 0;
        [SerializeField] private int walksCount = 10;
        [SerializeField] private int stepsCount = 100;
        [SerializeField] private Vector2 lineSizeRange = new Vector2(.1f, .1f);
        [SerializeField] private float lineSizeRound = 0f;
        [SerializeField] private Vector2 hueRange = new Vector2(0f, 1f);
        [SerializeField] private Vector2 saturationRange = new Vector2(.5f, .5f);
        [SerializeField] private Vector2 lightRange = new Vector2(.5f, .5f);

        [SerializeField] private float lastTickTime = -Mathf.Infinity;
        [SerializeField] private float tickInterval = .1f;

        private List<Core.RandomWalk> randomWalks = new List<Core.RandomWalk>();
        private List<LineRenderer> lineRends = new List<LineRenderer>();

        private System.Random prng;


        void Start()
        {
            Clear();

            this.prng = new System.Random(seed);

            for (int i = 0; i < walksCount; i++)
            {
                // var randomWalk = new Core.RandomWalk(prng, luck: (float)prng.NextDouble(), skill: (float)prng.NextDouble());
                var randomWalk = new Core.RandomWalk(prng);
                randomWalk.Init();

                randomWalks.Add(randomWalk);

                var lineObj = Instantiate(linePrefab, transform.position, Quaternion.identity, container);
                var lineRend = lineObj.GetComponentInChildren<LineRenderer>();

                lineRends.Add(lineRend);

                lineRend.colorGradient = GetGradient();

                // var positions = new Vector3[stepsCount];
                // positions[0] = new Vector3(0, 0, randomWalk.LastValue);

                // for (int j = 1; j < stepsCount; j++)
                // {
                //     positions[j] = new Vector3(j, 0, randomWalk.Tick());
                // }

                lineRend.positionCount = 1;
                lineRend.SetPosition(0, new Vector3(0, 0, randomWalk.LastValue));
            }
        }

        void Update()
        {
            if (Time.time >= lastTickTime + tickInterval)
            {
                for (int i = 0; i < walksCount; i++)
                {
                    var randomWalk = randomWalks[i];
                    var lineRend = lineRends[i];

                    randomWalk.Tick();

                    lineRend.positionCount += 1;

                    var index = lineRend.positionCount - 1;
                    var pos = new Vector3(index, 0, randomWalk.LastValue);

                    lineRend.SetPosition(index, pos);
                }

                lastTickTime = Time.time;
            }
        }

        public void Generate()
        {
            Clear();

            this.prng = new System.Random(seed);

            for (int i = 0; i < walksCount; i++)
            {
                // var randomWalk = new Core.RandomWalk(prng, luck: (float)prng.NextDouble(), skill: (float)prng.NextDouble());
                var randomWalk = new Core.RandomWalk(prng);
                randomWalk.Init();

                var lineObj = Instantiate(linePrefab, transform.position, Quaternion.identity, container);
                var lineRend = lineObj.GetComponentInChildren<LineRenderer>();
                // var tempMat = new Material(lineRend.sharedMaterial);
                // tempMat.SetColor();

                // lineRend.sharedMaterial = tempMat;

                var bgTempMat = new Material(backgroundRend.sharedMaterial);
                bgTempMat.SetColor("_Color", backgroundColor);
                backgroundRend.sharedMaterial = bgTempMat;

                lineRend.colorGradient = GetGradient();
                // lineRend.colorGradient.colorKeys = new GradientColorKey[]

                var positions = new Vector3[stepsCount];
                positions[0] = new Vector3(0, 0, randomWalk.LastValue);

                for (int j = 1; j < stepsCount; j++)
                {
                    positions[j] = new Vector3(j, 0, randomWalk.Tick());
                }

                var lineSize = Mathf.Lerp(lineSizeRange.x, lineSizeRange.y, (float)prng.NextDouble());
                if (lineSizeRound != 0f) lineSize = lineSize.RoundPrec(lineSizeRound);

                lineRend.startWidth = lineSize;
                lineRend.endWidth = lineSize;

                lineRend.positionCount = stepsCount;
                lineRend.SetPositions(positions);
            }
        }

        public void Clear()
        {
            for (int i = container.transform.childCount; i > 0; --i)
            {
                DestroyImmediate(container.transform.GetChild(0).gameObject);
            }
        }

        Gradient GetGradient()
        {
            var hue = RandomRange(hueRange.x, hueRange.y);
            var sat = RandomRange(saturationRange.x, saturationRange.y);
            var light = RandomRange(lightRange.x, lightRange.y);
            var color = Color.HSVToRGB(hue, sat, light);

            return ColorAsGradient(color);
        }

        Gradient ColorAsGradient(Color color)
        {
            var g = new Gradient();
            var gck = new GradientColorKey[2];
            var gak = new GradientAlphaKey[2];
            gck[0].color = color;
            gck[0].time = 0f;
            gak[0].alpha = 1f;
            gak[0].time = 0f;
            g.SetKeys(gck, gak);

            return g;
        }

        float RandomRange(float min, float max)
        {
            return Mathf.Lerp(min, max, (float)prng.NextDouble());
        }
    }
}