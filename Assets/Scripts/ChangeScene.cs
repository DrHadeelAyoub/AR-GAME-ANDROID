using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void SwitchToSceneName(string sceneName, bool unloadCurrent = false)
    {
        Debug.Log("Switching to scene: " + sceneName);

        // Load the scene and set it as active
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(
            sceneName,
            UnityEngine.SceneManagement.LoadSceneMode.Additive
        ).completed += (AsyncOperation asyncOp) =>
        {
            if (unloadCurrent)
            {
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
                );

                Debug.Log("Unloaded current scene");
            }

            bool res = UnityEngine.SceneManagement.SceneManager.SetActiveScene(
                UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName)
            );

            Debug.Log("Set active scene to " + sceneName + ": " + res);
        };
    }

    // Push the radar scene
    public void PushRadarScene()
    {
        SwitchToSceneName("RadarScene");
    }

    // Switch to the radar scene
    public void SwitchToRadarScene()
    {
        SwitchToSceneName("RadarScene", unloadCurrent: true);
    }

    // Push AR scene
    public void PushARScene()
    {
        SwitchToSceneName("ARScene");
    }

    // Switch to AR scene
    public void SwitchToARScene()
    {
        SwitchToSceneName("ARScene", unloadCurrent: true);
    }

    // Push instructions
    public void PushInstructionScene()
    {
        Debug.Log("Switching to scene");
        SwitchToSceneName("Instructions");
    }

    // Switch to instructions
    public void SwitchToInstructionScene()
    {
        SwitchToSceneName("Instructions", unloadCurrent: true);
    }

    // Push win game scene
    public void PushWinGameScene()
    {
        SwitchToSceneName("WinGameScene");
    }

    // Switch to win game scene
    public void SwitchToWinGameScene()
    {
        SwitchToSceneName("WinGameScene", unloadCurrent: true);
    }

    // Push lose game scene
    public void PushLoseGameScene()
    {
        SwitchToSceneName("LoseGameScene");
    }

    // Switch to lose game scene
    public void SwitchToLoseGameScene()
    {
        SwitchToSceneName("LoseGameScene", unloadCurrent: true);
    }

    // Quit the game
    public void QuitGame()
    {
        Application.Quit();
    }

    // Quit to menu
    public void QuitToMenu()
    {
        SwitchToSceneName("Menu", unloadCurrent: true);
    }
}
