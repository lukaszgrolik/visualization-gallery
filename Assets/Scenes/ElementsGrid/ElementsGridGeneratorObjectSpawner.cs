using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementsGridGeneratorObjectSpawner : MonoBehaviour
{
    [System.Serializable]
    class PrefabItem
    {
        public List<GameObject> prefabs;
        public float minValue;
        public float maxValue;
    }

    [SerializeField] private List<PrefabItem> prefabs;

    private System.Random prng;

    public void Setup(IReadOnlyElementsGridGenerator gridGen)
    {
        prng = new System.Random(gridGen.Seed);
    }

    // public GameObject Spawn(IReadOnlyElementsGridGenerator gridGen, Vector3 pos, float noiseValue)
    public GameObject Spawn(IReadOnlyElementsGridGenerator gridGen, float noiseValue)
    {
        var foundPrefabs = new List<GameObject>();
        foreach (var item in prefabs)
        {
            if (noiseValue >= item.minValue && noiseValue <= item.maxValue)
            {
                var index = Mathf.FloorToInt(Utils.RandomRange(prng, 0, item.prefabs.Count));
                var selectedPrefab = item.prefabs[index];

                foundPrefabs.Add(selectedPrefab);
            }
        }

        GameObject prefab = null;
        // Debug.Log($"foundPrefabs.Count: {foundPrefabs.Count}");
        if (foundPrefabs.Count == 0)
        {
            prefab = null;
        }
        else if (foundPrefabs.Count == 1)
        {
            prefab = foundPrefabs[0];
        }
        else
        {
            var index = Mathf.FloorToInt(Utils.RandomRange(prng, 0, foundPrefabs.Count));
            // if (index == foundPrefabs.Count) index = foundPrefabs.Count - 1;
            prefab = foundPrefabs[index];
        }

        return prefab;
        // var obj = Instantiate(prefab, pos, Quaternion.identity, gridGen.Container);

        // obj.transform.localScale = gridGen.ElementSize.With(y: gridGen.ElementSize.y * noiseValue);

        // var meshRend = obj.GetComponentInChildren<MeshRenderer>();
        // var tempMat = new Material(meshRend.sharedMaterial);

        // Color.RGBToHSV(gridGen.DefaultColor, out var h, out var s, out var l);
        // var hue = gridGen.ColorNoiseMode == ElementsGridGenerator._ColorNoiseMode.Hue ? noiseValue : h;
        // var sat = gridGen.ColorNoiseMode == ElementsGridGenerator._ColorNoiseMode.Sat ? noiseValue : s;
        // var light = gridGen.ColorNoiseMode == ElementsGridGenerator._ColorNoiseMode.Light ? noiseValue : l;
        // var color = Color.HSVToRGB(hue, sat, light);
        // tempMat.SetColor("_Color", color);

        // meshRend.sharedMaterial = tempMat;
    }
}