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

class GridElement
{
    private readonly IReadOnlyElementsGridGenerator elementsGridGenerator;
    private readonly GameObject obj; public GameObject Object => obj;
    private readonly Material material; public Material Material => material;

    public bool IsActive => obj.activeSelf;

    public GridElement(IReadOnlyElementsGridGenerator elementsGridGenerator, GameObject obj, Material material)
    {
        this.elementsGridGenerator = elementsGridGenerator;
        this.obj = obj;
        this.material = material;
    }

    public void Toggle()
    {
        obj.SetActive(!obj.activeSelf);
    }

    public void UpdateScale(float noiseValue)
    {
        var scale = ElementScale(noiseValue);
        obj.transform.localScale = scale;
    }

    public void UpdateColor(float noiseValue)
    {
        Color.RGBToHSV(elementsGridGenerator.DefaultColor, out var h, out var s, out var l);
        var hue = elementsGridGenerator.ColorNoiseMode == ElementsGridGenerator._ColorNoiseMode.Hue ? noiseValue : h;
        var sat = elementsGridGenerator.ColorNoiseMode == ElementsGridGenerator._ColorNoiseMode.Sat ? noiseValue : s;
        var light = elementsGridGenerator.ColorNoiseMode == ElementsGridGenerator._ColorNoiseMode.Light ? noiseValue : l;
        var color = Color.HSVToRGB(hue, sat, light);

        material.SetColor("_Color", color);
    }

    public Vector3 ElementScale(float noiseValue)
    {
        var scale = elementsGridGenerator.BaseElementSize;
        if (elementsGridGenerator.ElementSize.x != 0) scale = scale.With(x: elementsGridGenerator.ElementSize.x * noiseValue);
        if (elementsGridGenerator.ElementSize.y != 0) scale = scale.With(y: elementsGridGenerator.ElementSize.y * noiseValue);
        if (elementsGridGenerator.ElementSize.z != 0) scale = scale.With(z: elementsGridGenerator.ElementSize.z * noiseValue);
        return scale;
    }
}

public interface IReadOnlyElementsGridGenerator
{
    // Transform Container { get; }
    Vector3 BaseElementSize { get; }
    Vector3 ElementSize { get; }
    ElementsGridGenerator._ColorNoiseMode ColorNoiseMode { get; }
    Color DefaultColor { get; }
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

    // class GridCell
    // {
    //     public Vector2Int position;
    // }

    // private List<GridCell> cells = new List<GridCell>();

    private Noise noise;
    private Vector2 animOffset = Vector2.zero;

    // private Dictionary<GridCell, GameObject> gameObjects = new Dictionary<GridCell, GameObject>();
    private List<List<GridElement>> spawnedElements = new List<List<GridElement>>();

    void Start()
    {
        Generate();
    }

    void Update()
    {
        animOffset += new Vector2(1f * Time.deltaTime, 0);
        noise.Generate(offset: offset + animOffset);

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int z = 0; z < gridSize.y; z++)
            {
                var el = spawnedElements[x][z];

                if (el != null)
                {
                    var noiseValue = NoiseValue(x, z);
                    // obj.transform.localScale += new Vector3(0, .5f * Time.deltaTime, 0);
                    if (noiseValue == 0)
                    {
                        if (el.IsActive) el.Toggle();
                    }
                    else
                    {
                        if (el.IsActive == false) el.Toggle();

                        el.UpdateScale(noiseValue);
                        el.UpdateColor(noiseValue);
                    }
                }
            }
        }
    }

    public void Generate()
    {
        Clear();

        for (int x = 0; x < gridSize.x; x++)
        {
            var col = new List<GridElement>();
            spawnedElements.Add(col);

            for (int z = 0; z < gridSize.y; z++)
            {
                // col[z] = null;
                col.Add(null);
            }
        }

        noise = new Noise(
            size: gridSize,
            seed: seed,
            freq: freq,
            amp: amp,
            offset: offset,
            normalize: normalize
        );
        noise.Generate();

        System.Random prng = new System.Random(seed);

        var objectSpawner = GetComponent<ElementsGridGeneratorObjectSpawner>();
        if (objectSpawner) objectSpawner.Setup(this);

        for (int z = 0; z < gridSize.y; z++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                var el = SpawnElement(x, z, prng, noise, objectSpawner);

                if (el != null)
                {
                    spawnedElements[x][z] = el;
                }
            }
        }
    }

    GridElement SpawnElement(int x, int z, System.Random prng, Noise noise, ElementsGridGeneratorObjectSpawner objectSpawner)
    {
        var offset = new Vector3(.5f, 0, .5f);
        var pos = transform.position + new Vector3(x, 0, z) + offset;

        var noiseValue = NoiseValue(x, z);
        if (noiseValue == 0) return null;

        GameObject prefabObject = null;
        if (objectSpawner)
        {
            var obj = objectSpawner.Spawn(this, noiseValue);
            if (obj == null) return null;

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

        var meshRend = objInst.GetComponentInChildren<MeshRenderer>();
        var tempMat = new Material(meshRend.sharedMaterial);

        meshRend.sharedMaterial = tempMat;

        var el = new GridElement(this, objInst, tempMat);

        el.UpdateScale(noiseValue);
        el.UpdateColor(noiseValue);

        return el;
    }

    public float NoiseValue(int x, int z)
    {
        var noiseValue = noise.Sample(x, z);

        if (remapNoiseMin != 0 || remapNoiseMax != 1)
        {
            noiseValue = Utils.Remap(noiseValue, remapNoiseMin, remapNoiseMax, 0, 1);
        }

        if (noisePrecision != 0)
        {
            noiseValue = RoundPrec(noiseValue, noisePrecision);
        }

        return noiseValue;
    }

    public void Clear()
    {
        spawnedElements.Clear();

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
