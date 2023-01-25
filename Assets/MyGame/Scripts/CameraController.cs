using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    [Range(0, 3)]
    private float speed = 0.1f;
    [Range(0, 3)]
    [SerializeField]
    private float amplitude = 0.1f;
    void Update()
    {
        transform.position = new Vector3(0, Mathf.Sin(Time.time * speed) * amplitude, -10);
    }
}
