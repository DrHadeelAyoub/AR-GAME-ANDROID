using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(
            "Menu",
            UnityEngine.SceneManagement.LoadSceneMode.Additive
        ).completed += (AsyncOperation asyncOp) =>
        {
            bool res = UnityEngine.SceneManagement.SceneManager.SetActiveScene(
                UnityEngine.SceneManagement.SceneManager.GetSceneByName("Menu")
            );

            Debug.Log("Set active scene to Menu " + res);
        };
    }

    // Update is called once per frame
    void Update()
    {

    }
}
