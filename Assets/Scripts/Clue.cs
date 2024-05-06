using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Data structure to store the clue information
public class Clue : MonoBehaviour
{
    // The location of the clue
    public Vector3d location;

    // The label of the clue
    public string label;

    // Flag to indicate if the clue is good or bad
    public bool good;

    // The 3D prefab asset to represent the clue
    public GameObject prefab;

    // The 2D image assets to represent the clue
    public Texture2D AssetColor;
    public Texture2D AssetOutline;

    // Whether the clue has been found
    public bool found;

    // Is the clue the dog clue
    public bool dogClue;

    // Is the clue the final clue
    public bool finalClue;

    // Clue AR ranges
    public float maxPlacementDistance = 10.0f;
    public float minPlacementDistance = 3.0f;
}
