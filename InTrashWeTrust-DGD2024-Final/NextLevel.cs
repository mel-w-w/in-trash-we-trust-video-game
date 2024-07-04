using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class NextLevel : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*
        if (collision.gameObject.name == "LR")
        {
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();


            if (player.foodCollected >= player.foodRequired)
            {
                // go to next level
                // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                audioPlayer.Play();
                Debug.Log("Going to Next Level");
            } else
            {
                // warningText.text = "Not Enough Food Collected"; // Set the warning text
                warningText.gameObject.SetActive(true); // Make the TextMeshPro object visible
                Debug.Log("Not Enough Food Collected");
                StartCoroutine(HideWarningAfterDelay(5));
            }
        }
        */
    }
    /*
    public TextMeshProUGUI warningText;
    public AudioSource audioPlayer;

    // Start is called before the first frame update
    private void Start()
    {
        warningText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "LR")
        {
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();


            if (player.foodCollected >= player.foodRequired)
            {
                // go to next level
                // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                audioPlayer.Play();
                Debug.Log("Going to Next Level");
            } else
            {
                warningText.text = "Not Enough Food Collected"; // Set the warning text
                warningText.gameObject.SetActive(true); // Make the TextMeshPro object visible
                Debug.Log("Not Enough Food Collected");
                StartCoroutine(HideWarningAfterDelay(5));
            }
        }
    }

    private IEnumerator HideWarningAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay
        warningText.gameObject.SetActive(false); // Hide the TextMeshPro object
    }
    */
}
