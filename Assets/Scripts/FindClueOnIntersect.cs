using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindClueOnIntersect : MonoBehaviour
{
    // Global game manager
    GameManager gameManager;

    // Current clue
    Clue currentClue;

    // Clue finding threshold
    public float clueFindingThreshold = 1.0f;

    // Clue found text
    public GameObject notifyText;

    // Audio source for clue found sound
    public AudioSource audioSource;

    bool clueFound;

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

        // Get the current active clue from the clue placement script on 
        // the manager
        currentClue = gameManager.currentClue;
    }

    // Update is called once per frame
    void Update()
    {
        CheckClueIntersection();
    }

    void CheckClueIntersection()
    {
        // Check if the AR camera is within the clue finding threshold
        if (Vector3.Distance(
                Camera.main.transform.position, transform.position) >
            clueFindingThreshold) return;

        // Check if the camera is looking at the clue
        Vector3 forward = Camera.main.transform.forward;
        Vector3 toClue = transform.position - Camera.main.transform.position;
        if (Vector3.Dot(forward, toClue) < 0) return;

        // Set the clue as found
        StartCoroutine(HandleClueFound());
    }

    IEnumerator HandleClueFound()
    {
        if (currentClue.found) yield return null;

        // Set the current clue as found
        currentClue.found = true;

        // Play the clue found sound from the start
        audioSource.time = 0.0f;
        audioSource.Play();

        // Set the text
        if (currentClue.dogClue)
        {
            notifyText.GetComponent<UnityEngine.UI.Text>().text = "You found " + currentClue.label + "!";
        }
        else
        {
            notifyText.GetComponent<UnityEngine.UI.Text>().text = "You found a clue!";
        }

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
