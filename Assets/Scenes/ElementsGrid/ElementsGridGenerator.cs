using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ElementsGridGenerator))]
public class ElementsGridGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var script = (ElementsGridGenerator)target;

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

static class Utils
{
    static public float Remap(float v, float inMin, float inMax, float outMin, float outMax)
    {
        return Mathf.Lerp(outMin, outMax, Mathf.InverseLerp(inMin, inMax, v));
    }

    static public float RandomRange(System.Random prng, float min, float max)
    {
        return Mathf.Lerp(min, max, (float)prng.NextDouble());
    }
}

class Noise
{
    private List<List<float>> grid = new List<List<float>>();
    private float min = float.MaxValue;
    private float max = float.MinValue;

    public Noise(
        int seed,
        int width,
        int height,
        float freq,
        float amp,
        Vector2 offset,
        bool normalize
    )
    {
        System.Random prng = new System.Random(seed);
        float offsetX = prng.Next(-100000, 100000) + offset.x;
        float offsetY = prng.Next(-100000, 100000) + offset.y;

        for (int y = 0; y < height; y++)
        {
            var row = new List<float>();
            grid.Add(row);

            for (int x = 0; x < width; x++)
            {
                var sampleX = (x + offsetX) * freq;
                var sampleY = (y + offsetY) * freq;
                var val = Mathf.PerlinNoise(sampleX, sampleY) * amp;

                row.Add(val);

                if (val < min) min = val;
                if (val > max) max = val;
            }
        }

        if (normalize)
        {
            for (int z = 0; z < height; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    grid[z][x] = Utils.Remap(grid[z][x], min, max, 0, 1);
                }
            }
        }
    }

    public float Sample(int x, int y)
    {
        return grid[y][x];
    }
}

public interface IReadOnlyElementsGridGenerator
{
    // Transform Container { get; }
    // Vector3 ElementSize { get; }
    // ElementsGridGenerator._ColorNoiseMode ColorNoiseMode { get; }
    // Color DefaultColor { get; }
    int Seed { get; }
}

public class ElementsGridGenerator : MonoBehaviour, IReadOnlyElementsGridGenerator
{
    public enum _ColorNoiseMode
    {
        Hue,
        Sat,
        Light,
    }

    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform container; public Transform Container => container;
    [SerializeField] private Vector2Int gridSize = Vector2Int.one * 10;

    [Header("Rotation")]

    [SerializeField] private bool randomRotation = false;
    [SerializeField] private float rotationPrecision = 0f;

    [Header("Size")]

    [SerializeField] private Vector3 baseElementSize = Vector3.one; public Vector3 BaseElementSize => baseElementSize;
    [SerializeField] private Vector3 elementSize = Vector3.one; public Vector3 ElementSize => elementSize;

    [Header("Color")]

    [SerializeField] private _ColorNoiseMode colorNoiseMode = _ColorNoiseMode.Hue; public _ColorNoiseMode ColorNoiseMode => colorNoiseMode;
    [SerializeField] private Color defaultColor = Color.HSVToRGB(0, .5f, .5f); public Color DefaultColor => defaultColor;

    [Header("Noise")]

    [SerializeField] private int seed = 0; public int Seed => seed;
    [SerializeField] private float freq = 1f;
    [SerializeField] private float amp = 1f;
    [SerializeField] private Vector2 offset = Vector2.zero;
    [SerializeField] private bool normalize = true;
    [SerializeField] private float remapNoiseMin = 0f;
    [SerializeField] private float remapNoiseMax = 1f;
    [SerializeField] private float noisePrecision = 0f;

    public void Generate()
    {
        Clear();

        var noise = new Noise(
            width: gridSize.x,
            height: gridSize.y,
            seed: seed,
            freq: freq,
            amp: amp,
            offset: offset,
            normalize: normalize
        );

        System.Random prng = new System.Random(seed);

        var objectSpawner = GetComponent<ElementsGridGeneratorObjectSpawner>();
        if (objectSpawner) objectSpawner.Setup(this);

        for (int z = 0; z < gridSize.y; z++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                var offset = new Vector3(.5f, 0, .5f);
                var pos = transform.position + new Vector3(x, 0, z) + offset;

                var noiseVal = noise.Sample(x, z);

                if (remapNoiseMin != 0 || remapNoiseMax != 1)
                {
                    noiseVal = Utils.Remap(noiseVal, remapNoiseMin, remapNoiseMax, 0, 1);
                }

                var val = noisePrecision == 0 ? noiseVal : RoundPrec(noiseVal, noisePrecision);

                if (val == 0) continue;

                GameObject prefabObject = null;
                if (objectSpawner)
                {
                    var obj = objectSpawner.Spawn(this, val);
                    if (obj == null) continue;

                    prefabObject = obj;
                }
                else
                {
                    prefabObject = prefab;
                }

                var rot = Quaternion.identity;
                if (randomRotation)
                {
                    var rotVal = Utils.RandomRange(prng, 0f, 360f);
                    if (rotationPrecision != 0) rotVal = rotVal.RoundPrec(rotationPrecision);

                    rot = Quaternion.Euler(0f, rotVal, 0f);
                }

                var objInst = Instantiate(prefabObject, pos, rot, container);
                // var localScale = obj.transform.localScale;

                var scale = baseElementSize;
                if (elementSize.x != 0) scale = scale.With(x: elementSize.x * val);
                if (elementSize.y != 0) scale = scale.With(y: elementSize.y * val);
                if (elementSize.z != 0) scale = scale.With(z: elementSize.z * val);
                objInst.transform.localScale = scale;

                var meshRend = objInst.GetComponentInChildren<MeshRenderer>();
                var tempMat = new Material(meshRend.sharedMaterial);
                // var color = Color.HSVToRGB(val, .5f, .5f);

                Color.RGBToHSV(defaultColor, out var h, out var s, out var l);
                var hue = colorNoiseMode == _ColorNoiseMode.Hue ? val : h;
                var sat = colorNoiseMode == _ColorNoiseMode.Sat ? val : s;
                var light = colorNoiseMode == _ColorNoiseMode.Light ? val : l;
                var color = Color.HSVToRGB(hue, sat, light);
                tempMat.SetColor("_Color", color);

                meshRend.sharedMaterial = tempMat;
            }
        }
    }

    public void Clear()
    {
        for (int i = container.transform.childCount; i > 0; --i)
        {
            DestroyImmediate(container.transform.GetChild(0).gameObject);
        }
    }

    float RoundPrec(float val, float prec)
    {
        return Mathf.Round(val / prec) * prec;
    }
}
