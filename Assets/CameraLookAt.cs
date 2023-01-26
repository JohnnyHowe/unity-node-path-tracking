using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookAt : MonoBehaviour
{
    [SerializeField] private Transform target; 

    void Update()
    {
        transform.LookAt(target.position);        
    }
}
