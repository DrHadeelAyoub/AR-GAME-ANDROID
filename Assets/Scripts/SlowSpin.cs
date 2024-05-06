using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowSpin : MonoBehaviour
{
    // Spin speed
    public float speedDegrees = 30f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Slowly rotate, speed times per second
        transform.Rotate(0, speedDegrees * Time.deltaTime, 0);
    }
}
