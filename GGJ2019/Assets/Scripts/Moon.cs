using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moon : MonoBehaviour
{
    public float rotationSpeed = 5f;
    public float maxGravDist = 8f;
    public float maxGravity = 0.75f;

    void FixedUpdate()
    {
        transform.Rotate(0, 0, Time.deltaTime * rotationSpeed);
    }
}
