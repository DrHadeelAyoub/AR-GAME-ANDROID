using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceRandom : MonoBehaviour
{
    public float includeRadius = 10;
    public float excludeRadius = 3;

    Vector3 originalPosition;

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

        // Check if the clue has a min or max placement distance
        if (currentClue.minPlacementDistance != 0)
            excludeRadius = currentClue.minPlacementDistance;

        if (currentClue.maxPlacementDistance != 0)
            includeRadius = currentClue.maxPlacementDistance;

        // Store the original position 
        originalPosition = transform.position;

        // Select a random angle and distance
        float angle = Random.Range(0, 2 * Mathf.PI);
        float distance = Random.Range(excludeRadius, includeRadius);

        // Calculate the new position
        Vector3 newPosition = new Vector3(
            originalPosition.x + Mathf.Cos(angle) * distance,
            originalPosition.y,
            originalPosition.z + Mathf.Sin(angle) * distance
        );

        // Set the new position
        transform.position = newPosition;
    }

    void OnDrawGizmos()
    {
        // Draw a circle at the new position
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(originalPosition, excludeRadius);

        // Draw a circle at the new position
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(originalPosition, includeRadius);
    }
}
