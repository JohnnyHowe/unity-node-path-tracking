using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DrawLineBetweenChildren : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;

    void Update()
    {
        _lineRenderer.positionCount = transform.childCount;             
        for (int i = 0; i < transform.childCount; i++) {
            _lineRenderer.SetPosition(i, transform.GetChild(i).position);
        }
    }
}
