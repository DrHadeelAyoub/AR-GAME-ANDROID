using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitBuddyScene : MonoBehaviour
{
    // Global game manager
    GameManager gameManager;

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

        // Wait for 5 seconds and then exit the buddy scene
        StartCoroutine(WaitAndExit());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator WaitAndExit()
    {
        // Wait for 5 seconds
        yield return new WaitForSeconds(7);

        // Exit the buddy scene
        ExitBuddy();
    }

    // Exit the buddy scene
    public void ExitBuddy()
    {
        // Switch to the radar scene
        gameManager.ReturnFromBuddy();
    }
}
