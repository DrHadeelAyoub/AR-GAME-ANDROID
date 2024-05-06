using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignCameraToCompass : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Get the current compass direction
        float compassDirection = Input.compass.trueHeading;

        Debug.Log("Compass direction: " + compassDirection);

        // Rotate the camera to align with the compass direction
        Camera.main.transform.rotation = Quaternion.Euler(0, compassDirection, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
