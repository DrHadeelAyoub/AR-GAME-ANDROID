using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderClue : MonoBehaviour
{
    // Clue object
    Clue clue;

    // Raw image
    RawImage rawImage;

    // Standard material
    public Material standardMaterial;

    // Multiply blend material
    public Material multiplyBlendMaterial;

    // Start is called before the first frame update
    void Start()
    {
        clue = gameObject.GetComponent<Clue>();
        rawImage = gameObject.GetComponent<RawImage>();

        // // Setup materials with shaders
        // standardMaterial = new Material(Shader.Find("Standard"));
        // // Render mode transparent
        // standardMaterial.SetFloat("_Mode", 3);

        // // Multiply blend material
        // multiplyBlendMaterial =
        //     new Material(Shader.Find("Custom/MultiplyBlend"));

        GenerateClue();
    }

    // Update is called once per frame
    void Update()
    {
        GenerateClue();
    }

    void GenerateClue()
    {
        // If the clue has been found, render the coloured asset
        // otherwise render the outline
        if (clue.found)
        {
            rawImage.texture = clue.AssetColor;
            rawImage.material = standardMaterial;
        }
        else
        {
            rawImage.texture = clue.AssetOutline;
            rawImage.material = multiplyBlendMaterial;
            rawImage.color = new Color(1, 1, 1, 1);
        }
    }
}
