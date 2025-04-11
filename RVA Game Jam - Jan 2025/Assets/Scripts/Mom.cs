using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// Controls the Mom sound effects and Door graphics

public class Mom : MonoBehaviour
{
    public float decreaseSpeed = 0.0007f;
    public float increaseSpeed = 0.05f;

    public float doorOpenCamY = 0.0f; // y-position of the camera when door opens

    public float slowFootstepsCutoff = 0.3f;
    public float fastFootstepsCutoff = 0.6f;
    public float doorCrackedCutoff = 0.7f;
    public Sprite doorClosedSprite;
    public Sprite doorCrackedSprite;
    public Sprite doorOpenSprite;
    public float doorClosedX = 5.0f; // x-position of sprite when door is closed
    public float doorOpenX = 2.5f; // x-position of sprite when door is open

    public AudioClip slowFootstepsClip;
    public AudioClip fastFootstepsClip;
    public AudioClip doorOpenClip;

    private float volumeLevel = 0.0f; // Current volume level
    private int prevVolumeState = 0; // Volume state from previous frame

    private GameManager gameManager;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    private void Awake()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        if (gameManager == null) {
            Debug.Log("Failed to find the GameManager component!");
            Application.Quit();
        }
        
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) {
            Debug.Log("Failed to find the SpriteRenderer component!");
            Application.Quit();
        }

        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null) {
            Debug.Log("Failed to find the AudioSource component!");
            Application.Quit();
        }

        decreaseSpeed *= Time.deltaTime;

        Reset();
    }

    public void Reset() {
        volumeLevel = 0.0f;
        
        // Reset position for closed doors
        transform.position = new Vector3(doorClosedX, transform.position.y, transform.position.z);
    }

    public void Update() {
        if (gameManager.IsRunning()) {
            volumeLevel = Mathf.Max(volumeLevel - decreaseSpeed, 0);
            // Debug.Log("Decreased Volume to " + volumeLevel);
            UpdateSpritesAndAudio();
        }
        else {
            audioSource.Stop();
        }
    }

    public void IncreaseVolume() {
        volumeLevel += increaseSpeed;

        Debug.Log("Increased Volume to " + volumeLevel);

        if (volumeLevel > 1) {
            Debug.Log("Volume over max!");
            gameManager.SetGameOverFlag();
            // Update position for open door sprite
            transform.position = new Vector3(doorOpenX, transform.position.y, transform.position.z);
            spriteRenderer.sprite = doorOpenSprite;

            // Update camera position
            Camera.main.transform.position = new Vector3(
                Camera.main.transform.position.x, doorOpenCamY, Camera.main.transform.position.z);
        }
        else {
            UpdateSpritesAndAudio();
        }
    }

    public void UpdateSpritesAndAudio() {
        //Debug.Log("Update sprites!");

        // scale the volume level by sqrt(x) to make it more dramatic
        float tempVolumeLevel = Mathf.Sqrt(volumeLevel);

        // door flung open, game over
        if (tempVolumeLevel > 1.0) {
            if (prevVolumeState != 0) {
                //Debug.Log("Game should no longer berunning");
                spriteRenderer.sprite = doorOpenSprite;
                audioSource.Stop();

                prevVolumeState = 0;
            }
        }
        // door cracked
        else if (tempVolumeLevel > doorCrackedCutoff) {
            if (prevVolumeState != 1) {
                // Change to door cracked sprite
                //Debug.Log("Volume over door cutoff");
                spriteRenderer.sprite = doorCrackedSprite;

                // Play door opening sound once
                audioSource.loop = false;
                audioSource.clip = doorOpenClip;
                audioSource.Play();

                prevVolumeState = 1;
            }
        }
        // fast footsteps
        else if (tempVolumeLevel > fastFootstepsCutoff) {
            if (prevVolumeState != 2) {
                // Change to door closed sprite
                spriteRenderer.sprite = doorClosedSprite;

                // Play fast footsteps
                audioSource.loop = true;
                audioSource.clip = fastFootstepsClip;
                audioSource.Play();
                
                prevVolumeState = 2;
            }
        }
        // slow footsteps
        else if (tempVolumeLevel > slowFootstepsCutoff) {
            if (prevVolumeState != 3) {
                // Change to door closed sprite
                spriteRenderer.sprite = doorClosedSprite;

                // Play slow footsteps
                audioSource.loop = true;
                audioSource.clip = slowFootstepsClip;
                audioSource.Play();
                
                prevVolumeState = 3;
            }
        }

        else {
            audioSource.Stop();
        }
    }

}