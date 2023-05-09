using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Noise
{
    private int seed;
    private Vector2Int size;
    private float freq;
    private float amp;
    private Vector2 offset;
    private bool normalize;

    private List<List<float>> grid = new List<List<float>>();
    // private float min = float.MaxValue;
    // private float max = float.MinValue;

    public Noise(
        int seed,
        Vector2Int size,
        float freq,
        float amp,
        Vector2 offset,
        bool normalize
    )
    {
        this.seed = seed;
        this.size = size;
        this.freq = freq;
        this.amp = amp;
        this.offset = offset;
        this.normalize = normalize;

        for (int y = 0; y < size.y; y++)
        {
            var row = new List<float>();
            grid.Add(row);

            for (int x = 0; x < size.x; x++)
            {
                row.Add(default(float));
            }
        }

        // Generate(offset);
    }

    public void Generate(
        Vector2? offset = null
    )
    {
        if (offset != null) this.offset = (Vector2)offset;

        var prng = new System.Random(seed);
        var offsetX = prng.Next(-100000, 100000) + this.offset.x;
        var offsetY = prng.Next(-100000, 100000) + this.offset.y;

        var min = float.MaxValue;
        var max = float.MinValue;

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                var sampleX = (x + offsetX) * freq;
                var sampleY = (y + offsetY) * freq;
                var val = Mathf.PerlinNoise(sampleX, sampleY) * amp;

                grid[y][x] = val;

                if (val < min) min = val;
                if (val > max) max = val;
            }
        }

        if (normalize)
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    grid[y][x] = Utils.Remap(grid[y][x], min, max, 0, 1);
                }
            }
        }
    }

    public float Sample(int x, int y)
    {
        return grid[y][x];
    }
}