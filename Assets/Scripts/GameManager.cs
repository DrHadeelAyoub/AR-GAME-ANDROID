using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Loading,
    Radar,
    ClueHunt,
    FinalClueHunt,
    GameOver
}

enum CompassType
{
    Compass,
    Dog
}

public class GameManager : MonoBehaviour
{
    // Make this a singleton
    public static GameManager Instance;

    // Clue map object
    public ClueMap clueMap;

    // Clue finding threshold (m)
    public double clueFindingThreshold = 10.0d;

    // Game state
    public GameState gameState = GameState.Loading;

    public Clue currentClue;

    // Time remaining in the game (s)
    public float timeRemaining = 20.0f * 60.0f;

    public GameObject dogCompassContainer;
    public GameObject compassContainer;
    public GameObject dogCompassArrow;
    public GameObject compassArrow;

    // Radar type
    CompassType compassType = CompassType.Compass;

    // Start is called before the first frame update
    void Start()
    {
        // Start the GPS location service
        StartCoroutine(GPSLocationService());

        // Ensure the correct compass type is enabled
        SwitchCompassType();
    }

    public IEnumerator GPSLocationService()
    {
        // Check if the user has location service enabled.
        if (!Input.location.isEnabledByUser)
            Debug.Log("Location not enabled on device or app does not have permission to access location");

        // Starts the location service.
        Input.location.Start(0.5f, 0.5f);
        Input.compass.enabled = true;

        // Waits until the location service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing &&
            maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // If the service didn't initialize in 20 seconds this cancels location
        // service use.
        if (maxWait < 1)
        {
            Debug.Log("Timed out");
            yield break;
        }

        // If the connection failed this cancels location service use.
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Unable to determine device location");
            yield break;
        }

        else
        {
            // If the connection succeeded
            Debug.Log("Successfully connected to GPS location service");
            gameState = GameState.Radar;
        }
    }

    // Switch compass type
    public void SwitchCompassType()
    {
        // Enable either the dog compass or the regular compass
        if (compassType == CompassType.Dog)
        {
            dogCompassContainer.SetActive(true);
            compassContainer.SetActive(false);
        }
        else
        {
            dogCompassContainer.SetActive(false);
            compassContainer.SetActive(true);
        }
    }

    // Calculate the bearing from the device's location to the clue
    double CalculateBearingToClueInDeg(
        Vector3d devicePosition,
        Vector3d cluePosition
    )
    {
        Vector3d directionToClue = cluePosition - devicePosition;

        return System.Math.Atan2(
            directionToClue.y,
            directionToClue.x
        ) * Mathf.Rad2Deg;
    }

    // Calculate the angle from the device's heading to the clue
    double CalculateAngleToClueInDeg(
        Vector3d devicePosition,
        double deviceHeading,
        Vector3d cluePosition
    )
    {
        double bearingToClueDeg = CalculateBearingToClueInDeg(
            devicePosition,
            cluePosition
        );
        return deviceHeading - bearingToClueDeg;
    }

    // Update is called once per frame
    void Update()
    {
        // If the location is still loading, return
        if (gameState == GameState.Loading)
            return;

        // Update the time remaining
        if (timeRemaining <= 0.0f && gameState != GameState.GameOver)
        {
            gameState = GameState.GameOver;

            // Switch to the LossScene
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(
                "LossScene",
                UnityEngine.SceneManagement.LoadSceneMode.Additive
            ).completed += (AsyncOperation asyncOp) =>
            {
                // Remove the current scene
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
                );

                // Remove the ARScene or FinalClueARScene if they are active
                if (UnityEngine.SceneManagement.SceneManager.GetSceneByName("ARScene").isLoaded)
                    UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("ARScene");

                if (UnityEngine.SceneManagement.SceneManager.GetSceneByName("FinalClueARScene").isLoaded)
                    UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("FinalClueARScene");

                UnityEngine.SceneManagement.SceneManager.SetActiveScene(
                    UnityEngine.SceneManagement.SceneManager.GetSceneByName("LossScene")
                );
            };

            return;
        }
        timeRemaining -= Time.deltaTime;

        if (gameState != GameState.Radar)
            return;

        // Enable either the dog compass or the regular compass
        SwitchCompassType();

        // Get the device's current GPS location
        Vector3d deviceLocation = new Vector3d(
            Input.location.lastData.latitude,
            Input.location.lastData.longitude,
            0.0d
        );

        // Get the device's current heading
        double trueHeadingDeg = Input.compass.magneticHeading;
        double headingAccuracy = Input.compass.headingAccuracy;

        // Log the location and heading
        Debug.Log("Location: " + deviceLocation.ToString("F6") +
                  " ; Heading: " + trueHeadingDeg.ToString("F6") +
                  " ; Accuracy: " + headingAccuracy.ToString("F6"));

        // Get the closest clue
        ClueDistance closestClue =
            clueMap.FindClosestClue(
                deviceLocation,
                filter: ClueFinderFilterOptions.Unfound
            );

        // Log the closest clue's distance
        Debug.Log("Closest clue (" + closestClue.clue.label +
                  ") is " + closestClue.distance.ToString("F6") + "m away");
        Debug.Log("Angle to closest clue: " +
                  CalculateAngleToClueInDeg(
                      deviceLocation,
                      trueHeadingDeg,
                      closestClue.clue.location
                  ).ToString("F6") + " deg");

        // If the closest clue is within 10m, notify the player
        if (closestClue.distance < clueFindingThreshold)
        {
            Debug.Log(closestClue.clue + " is within range");

            // Set the content of "NotifyText"
            GameObject notifyText = GameObject.Find("NotifyText");
            notifyText.GetComponent<UnityEngine.UI.Text>().text =
                closestClue.clue.label + " found";

            // Set the clue as current
            currentClue = closestClue.clue;

            Debug.Log("Current clue: " + currentClue.label);

            if (currentClue.finalClue)
            {
                // Run the final clue AR scene
                RunFinalClueARScene();
            }
            else
            {
                // Load the ARScene additively and set it as active
                RunARScene();
            }
        }
        else
        {
            // Clear the current clue
            currentClue = null;

            // Clear the content of "NotifyText"
            GameObject notifyText = GameObject.Find("NotifyText");
            notifyText.GetComponent<UnityEngine.UI.Text>().text = "Searching...";
        }

        // Rotate "Compass" object on canvas to point to the closest clue
        compassArrow.transform.rotation = Quaternion.Euler(
            0.0f,
            0.0f,
            (float)CalculateAngleToClueInDeg(
                deviceLocation,
                trueHeadingDeg,
                closestClue.clue.location
            )
        );

        // Rotate "Corgi" object around Y axis the same way
        dogCompassArrow.transform.rotation = Quaternion.Euler(
            0.0f,
            -(float)CalculateAngleToClueInDeg(
                deviceLocation,
                trueHeadingDeg,
                closestClue.clue.location
            ),
            0.0f
        );
    }

    // Asynchronously load the AR scene and set it as active in response to
    // the completed event
    void RunARScene()
    {
        // First check that clue hasn't been found before
        if (currentClue.found)
            return;

        // Vibrate the device
        Handheld.Vibrate();

        // Set the game state to ClueHunt
        gameState = GameState.ClueHunt;

        // Load the AR scene additively and set it as active
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(
            "ARScene",
            UnityEngine.SceneManagement.LoadSceneMode.Additive
        ).completed += (AsyncOperation asyncOp) =>
        {
            UnityEngine.SceneManagement.SceneManager.SetActiveScene(
                UnityEngine.SceneManagement.SceneManager.GetSceneByName("ARScene")
            );
        };
    }

    // Run the final clue AR scene
    public void RunFinalClueARScene()
    {
        // Vibrate the device
        Handheld.Vibrate();

        // Set the game state to FinalClueHunt
        gameState = GameState.FinalClueHunt;

        // Load the AR scene additively and set it as active
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(
            "FinalClueARScene",
            UnityEngine.SceneManagement.LoadSceneMode.Additive
        ).completed += (AsyncOperation asyncOp) =>
        {
            UnityEngine.SceneManagement.SceneManager.SetActiveScene(
                UnityEngine.SceneManagement.SceneManager.GetSceneByName("FinalClueARScene")
            );
        };
    }

    // Return to the radar view
    public void ReturnFromAR()
    {
        // Remove the ARScene or FinalClueARScene if they are active
        if (UnityEngine.SceneManagement.SceneManager.GetSceneByName("ARScene").isLoaded)
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("ARScene");

        if (UnityEngine.SceneManagement.SceneManager.GetSceneByName("FinalClueARScene").isLoaded)
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("FinalClueARScene");

        if (currentClue.finalClue && currentClue.found)
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("WinScene",
                UnityEngine.SceneManagement.LoadSceneMode.Additive).completed +=
                (AsyncOperation asyncOp) =>
                {
                    UnityEngine.SceneManagement.SceneManager.SetActiveScene(
                        UnityEngine.SceneManagement.SceneManager.GetSceneByName("WinScene")
                    );

                    UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("RadarScene");
                };
        }
        else if (currentClue.dogClue && currentClue.found ||
            compassType == CompassType.Dog)
        {
            compassType = CompassType.Dog;
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("BuddyScene",
                UnityEngine.SceneManagement.LoadSceneMode.Additive).completed +=
                (AsyncOperation asyncOp) =>
                {
                    UnityEngine.SceneManagement.SceneManager.SetActiveScene(
                        UnityEngine.SceneManagement.SceneManager.GetSceneByName("BuddyScene")
                    );
                };
        }
        else
        {
            compassType = CompassType.Compass;
            UnityEngine.SceneManagement.SceneManager.SetActiveScene(
                UnityEngine.SceneManagement.SceneManager.GetSceneByName(
                    "RadarScene")
);
        }

        // Set the game state back to Radar
        gameState = GameState.Radar;
    }

    // Return from the instruction scene
    public void ReturnFromBuddy()
    {
        // Set this as active
        UnityEngine.SceneManagement.SceneManager.SetActiveScene(
            UnityEngine.SceneManagement.SceneManager.GetSceneByName(
                "RadarScene")
        );

        // Unload the instruction scene
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("BuddyScene");

        // Set the game state back to Radar
        gameState = GameState.Radar;
    }

    public void DoWinScene()
    {
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(
            "FinalClueARScene");

        AsyncOperation sceneLoading = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(
            "WinScene",
            UnityEngine.SceneManagement.LoadSceneMode.Additive
        );

        sceneLoading.completed += (AsyncOperation asyncOp) =>
        {
            // Unload the current scene and the radar scene
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );

            Debug.Log("Unloaded current scene");

            if (UnityEngine.SceneManagement.SceneManager.GetSceneByName("RadarScene").isLoaded)
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("RadarScene");

            Debug.Log("Unloaded RadarScene");

            bool res = UnityEngine.SceneManagement.SceneManager.SetActiveScene(
                UnityEngine.SceneManagement.SceneManager.GetSceneByName("WinScene")
            );

            Debug.Log("Set active scene to WinScene: " + res);
        };
    }
}
