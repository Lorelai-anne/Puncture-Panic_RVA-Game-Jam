using UnityEngine;

public class BedHiss : MonoBehaviour
{

    private GameManager gameManager;
    private AudioSource audioSource;

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        if (gameManager == null) {
            Debug.Log("Failed to find the GameManager component!");
            Application.Quit();
        }

        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null) {
            Debug.Log("Failed to find the AudioSource component!");
            Application.Quit();
        }
        
    }

    void Update()
    {
        if (gameManager.IsRunning()) {
            if (!audioSource.isPlaying) {
                audioSource.Play();
            }
        }
        else if (audioSource.isPlaying) {
            audioSource.Stop();
        }
    }
}
