using UnityEngine;
using System;
using System.Collections;

public class FloatBehaviour : MonoBehaviour
{
    float originalY;

    // The strength of the float effect 
    public float floatStrength = 1;
    public float floatSpeed = 1;

    // Store the original Y position
    void Start()
    {
        this.originalY = this.transform.position.y;
    }

    void Update()
    {
        // Float up/down with a Sin()
        transform.position = new Vector3(
            transform.position.x,
            originalY +
                ((float)Math.Sin(Time.time / floatSpeed) * floatStrength),
            transform.position.z
        );
    }
}