using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearAfterTime : MonoBehaviour
{
    // Text component
    Component notifyText;

    // Ready to disappear?
    bool readyToDisappear = false;

    // Delay time before disappearing
    public float delayTime = 5.0f;

    // Time to fade out the text
    public float disappearTime = 1.0f;

    void Start()
    {
        notifyText = gameObject.GetComponent<UnityEngine.UI.Text>();

        // Start the coroutine to disappear after delayTime
        StartCoroutine(StartDisappear());
    }

    void Update()
    {
        // If ready to disappear, decrease the alpha value of the text color
        if (readyToDisappear)
        {
            Color textColor = ((UnityEngine.UI.Text)notifyText).color;
            textColor.a -= Time.deltaTime / disappearTime;
            ((UnityEngine.UI.Text)notifyText).color = textColor;
        }
    }

    // Coroutine to disappear after delayTime
    IEnumerator StartDisappear()
    {
        // Wait for delayTime
        yield return new WaitForSeconds(delayTime);
        readyToDisappear = true;

        // Destroy the object after disappearTime because the alpha value of 
        // the text color is 0
        yield return new WaitForSeconds(disappearTime);
        Destroy(gameObject);
    }
}
