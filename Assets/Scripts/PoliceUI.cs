using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Import this for working with UI elements

public class PoliceUI : MonoBehaviour
{
    [SerializeField] private GameObject policeText;
    [SerializeField] private Image policeBG;  // Change this to Image since it's a UI element
    [SerializeField] private float duration;  // Total duration for which the UI stays active
    [SerializeField] private float flashInterval = 0.5f;  // Interval at which the background color switches
    private bool activated = false;

    private void Start()
    {
        policeText.SetActive(false);
        policeBG.gameObject.SetActive(false);
    }

    public void activatePoliceUI()
    {
        if (!activated)
        {
            activated = true;
            StartCoroutine(PoliceUICoroutine());
            GameObject.FindAnyObjectByType<UIManager>().GetComponent<AudioSource>().volume = 0.2f;
            GameObject.FindAnyObjectByType<UIManager>().Play(GameObject.FindWithTag("Player").GetComponent<BusAudioHandler>().PoliceAlert);
        }
    }

    IEnumerator PoliceUICoroutine()
    {
        policeText.SetActive(true);
        policeBG.gameObject.SetActive(true);  // Activate the background

        float elapsedTime = 0f;
        bool isRed = true;

        // Flash the background color between red and blue
        while (elapsedTime < duration)
        {
            // Switch between red and blue colors
            if (isRed)
            {
                policeBG.color = Color.red;
            }
            else
            {
                policeBG.color = Color.blue;
            }

            // Toggle the flag
            isRed = !isRed;

            // Wait for the flash interval
            yield return new WaitForSeconds(flashInterval);

            // Increase the elapsed time by the interval
            elapsedTime += flashInterval;
        }

        // After the flashing effect, deactivate the UI elements
        policeText.SetActive(false);
        policeBG.gameObject.SetActive(false);
    }
}
