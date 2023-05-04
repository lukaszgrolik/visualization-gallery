using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPoint : MonoBehaviour
{
    [SerializeField] private float lineLength = 5f;

    void OnDrawGizmos()
    {
        var pos = transform.position;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(pos.With(x: pos.x - lineLength / 2), pos.With(x: pos.x + lineLength / 2));

        // Gizmos.color = Color.red;
        // Gizmos.DrawLine(pos.With(x: pos.x - lineLength / 2), pos.With(x: pos.x + lineLength / 2));
    }
}
