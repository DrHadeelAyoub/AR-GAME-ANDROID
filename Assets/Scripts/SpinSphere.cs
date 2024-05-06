using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinSphere : MonoBehaviour
{
    // Rotations per second
    public float speed = 0.10f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Rotate the sphere around the x axis
        transform.Rotate(Vector3.right, speed * Time.deltaTime);
    }
}
