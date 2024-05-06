using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindFinalClueOnIntersect : MonoBehaviour
{
    // Global game manager
    GameManager gameManager;

    // Current clue
    Clue currentClue;

    GameObject target;

    // Clue finding threshold
    public float clueFindingThreshold = 1.0f;

    // Clue found text
    public GameObject notifyText;

    bool animateTowardsTarget = false;

    // Audio source for clue found sound
    public AudioSource audioSource;

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
        if (animateTowardsTarget)
        {
            // If we get to the target, switch to the WinScene
            if (Vector3.Distance(transform.position, Camera.main.transform.position) < 3.0f)
            {
                // Switch to the win scene
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(
                    "WinScene",
                    UnityEngine.SceneManagement.LoadSceneMode.Additive
                ).completed += (AsyncOperation asyncOp) =>
                {
                    // Unload the current scene and the radar scene
                    UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(
                        UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
                    );

                    Debug.Log("Unloaded current scene");

                    UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("RadarScene");

                    Debug.Log("Unloaded RadarScene");

                    bool res = UnityEngine.SceneManagement.SceneManager.SetActiveScene(
                        UnityEngine.SceneManagement.SceneManager.GetSceneByName("WinScene")
                    );

                    Debug.Log("Set active scene to WinScene: " + res);
                };
            }

            // Move the clue towards the target unless the clue is more than 4m
            // from the camera
            if (Vector3.Distance(Camera.main.transform.position, transform.position) < 4.0f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    Camera.main.transform.position,
                    0.1f
                );
            }
        }
        else
        {
            CheckClueIntersection();
        }

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
        if (currentClue.finalClue)
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
