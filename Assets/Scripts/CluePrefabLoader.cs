using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CluePrefabLoader : MonoBehaviour
{
    // The prefab for the clue
    GameObject prefab;

    // Global game manager
    GameManager gameManager;

    void Start()
    {
        // Get the current game manager instance
        gameManager =
            GameObject.Find("GameManager").GetComponent<GameManager>();

        if (gameManager == null)
        {
            Debug.LogError("GameManager not found");
            return;
        }

        // Get the current active clue from the clue placement script on 
        // the manager
        Clue currentClue = gameManager.currentClue;

        if (currentClue == null)
        {
            Debug.LogError("Current clue not found");
            return;
        }

        if (currentClue.prefab == null)
        {
            Debug.LogError("Prefab not found for clue");
            return;
        }
        // Load the prefab as a child of the current game object
        prefab = Instantiate(
            currentClue.prefab, transform.position, Quaternion.identity, transform);
        prefab.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }
}
