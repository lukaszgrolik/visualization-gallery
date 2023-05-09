// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class ElementsGrid
// {
//     private Noise noise;
//     private Vector2 animOffset = Vector2.zero;

//     public void Generate()
//     {
//         noise = new Noise(
//             size: gridSize,
//             seed: seed,
//             freq: freq,
//             amp: amp,
//             offset: offset,
//             normalize: normalize
//         );
//         noise.Generate();
//     }

//     public void Update(float deltaTime)
//     {
//         animOffset += new Vector2(1f * Time.deltaTime, 0);
//         noise.Generate(offset: offset + animOffset);
//     }

//     private float NoiseValue(int x, int z)
//     {
//         var noiseValue = noise.Sample(x, z);

//         if (remapNoiseMin != 0 || remapNoiseMax != 1)
//         {
//             noiseValue = Utils.Remap(noiseValue, remapNoiseMin, remapNoiseMax, 0, 1);
//         }

//         if (noisePrecision != 0)
//         {
//             noiseValue = RoundPrec(noiseValue, noisePrecision);
//         }

//         return noiseValue;
//     }

//     private Vector3 ElementScale(float noiseValue)
//     {
//         var scale = baseElementSize;
//         if (elementSize.x != 0) scale = scale.With(x: elementSize.x * noiseValue);
//         if (elementSize.y != 0) scale = scale.With(y: elementSize.y * noiseValue);
//         if (elementSize.z != 0) scale = scale.With(z: elementSize.z * noiseValue);
//         return scale;
//     }
// }