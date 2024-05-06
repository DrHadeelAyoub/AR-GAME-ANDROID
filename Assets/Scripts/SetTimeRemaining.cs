using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTimeRemaining : MonoBehaviour
{
    // Global game manager
    public GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        if (gameManager == null)
        {
            gameManager =
                GameObject.Find("GameManager").GetComponent<GameManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        float timeRemaining = Mathf.Floor(gameManager.timeRemaining);

        // If the time remaining is less than or equal to 0, set the text to 
        // "00:00"
        if (timeRemaining <= 0.0f)
        {
            GetComponent<UnityEngine.UI.Text>().text = "00:00";
        }

        // Update this object's text to show the remaining time in mm:ss
        GetComponent<UnityEngine.UI.Text>().text =
            Mathf.Floor(timeRemaining / 60.0f).ToString("00") + ":" +
            (timeRemaining % 60.0f).ToString("00");
    }
}
