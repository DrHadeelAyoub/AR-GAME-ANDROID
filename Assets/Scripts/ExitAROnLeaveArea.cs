using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitAROnLeaveArea : MonoBehaviour
{
    // Global game manager
    GameManager gameManager;

    // Clue finding threshold
    public float clueDistanceThreshold = 15.0f;

    // Clue found text
    public GameObject notifyText;

    // Start is called before the first frame update
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
    }

    // Update is called once per frame
    void Update()
    {
        CheckClueDistance();
    }

    // Check if the AR camera is far enough away from the clue
    void CheckClueDistance()
    {
        // Check if the AR camera is within the clue finding threshold
        if (Vector3.Distance(
                Camera.main.transform.position, transform.position) <
            clueDistanceThreshold) return;

        // Set the clue as found
        StartCoroutine(HandleLeaveArea());
    }

    // Coroutine to handle leaving the area
    IEnumerator HandleLeaveArea()
    {
        // Set the text
        notifyText.GetComponent<UnityEngine.UI.Text>().text = "You lost the clue...";


        // Scale up the text over 1 second
        float scale = 0.0f;
        while (scale < 1.0f)
        {
            scale += Time.deltaTime;
            notifyText.transform.localScale =
                new Vector3(scale, scale, scale);
            yield return null;
        }

        // Wait for 3 seconds
        yield return new WaitForSeconds(3);

        // Set the game state back to Radar
        gameManager.ReturnFromAR();
    }
}
