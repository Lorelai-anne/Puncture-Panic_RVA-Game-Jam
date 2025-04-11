using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

//this script will go on the bed asset

public class Deflate : MonoBehaviour
{
    public float deflateSpeed = .01f; // Speed at which bed deflates or moves down
    public float inflateSpeed = .05f; // How much the bed moves up when button pressed
    public float startingInflation = .65f; // Starting inflation level

    public float inflatedY = 2f; // y-position when bed is fully inflated
    public float deflatedY =0f; // y-position when bed is fully deflated
    public float inflatedCamY = 6.75f; // y-position of the camera when bed is fully inflated
    public float deflatedCamY = 4.25f; // y-position of the camera when bed is fully deflated

    public float medCutOff = 0.3f;    
    public float hiCutOff = 0.8f;
    public Sprite deflatedSprite;
    public Sprite lowSprite;
    public Sprite medSprite;
    public Sprite hiSprite;
    public Sprite poppedSprite;

    public AudioClip deflatedClip;
    public AudioClip pumpClip;
    public AudioClip popClip;

    private float inflationLevel = 0.0f; // Current bed inflation
    private GameManager gameManager;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private Mom mom;

    private void Awake()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        if (gameManager == null) {
            Debug.Log("Failed to find the GameManager component!");
            Application.Quit();
        }

        mom = GameObject.Find("Door").GetComponent<Mom>();
        if (mom == null) {
            Debug.Log("Failed to find the Mom component!");
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

        deflateSpeed *= Time.deltaTime;

        Reset();
    }

    public void Reset() {
        inflationLevel = startingInflation;
    }

    private void Update()
    {
        if (gameManager.IsRunning()) {
            UpdateBed();
        }
    }

    private void UpdateBed()
    {
        // if mouse button is pressed
        //   GetMouseButtonDown only returns true for 
        //   the first frame the mouse is pressed
        if (Input.GetMouseButtonDown(0))
        {
            // update the current inflation level and position
            inflationLevel += inflateSpeed;
            Debug.Log("Inflating! " + inflationLevel);

            audioSource.clip = pumpClip;
            audioSource.Play();

            // call mom function to update volume
            mom.IncreaseVolume();

            // Bed is over-inflated and popped!
            if (inflationLevel >= 1) {
                OnBedPopped();
            }
            else {
                UpdateSprites();
            }
        }
        // else the mouse button was not pressed
        else
        {
            // deflate the mattress
            inflationLevel -= deflateSpeed;

            // if inflation level is zero of negative
            if(inflationLevel <= 0.0)
            {
                OnBedDeflated();
            }
            else {
                UpdateSprites();
            }
        }

        if (gameManager.IsRunning()) {
            // update mattress position based on inflation level
            float new_y = Mathf.Lerp(deflatedY, inflatedY, inflationLevel);
            transform.position = new Vector3(
                transform.position.x, new_y, transform.position.z);

            // update camera position
            new_y = Mathf.Lerp(deflatedCamY, inflatedCamY, inflationLevel);
            Camera.main.transform.position = new Vector3(
                Camera.main.transform.position.x, new_y, Camera.main.transform.position.z);
        }
    }

    private void UpdateSprites() {
        // swap sprites based on inflation level
        if (inflationLevel > hiCutOff) {
            spriteRenderer.sprite = hiSprite;
        }
        else if (inflationLevel > medCutOff) {
            spriteRenderer.sprite = medSprite;
        }
        else {
            spriteRenderer.sprite = lowSprite;
        }
    }

    private void OnBedPopped() {
        transform.position = new Vector3(
            transform.position.x, inflatedY, transform.position.z);
        
        audioSource.clip = popClip;
        audioSource.Play();

        spriteRenderer.sprite = poppedSprite;
        gameManager.SetGameOverFlag();
    }

    private void OnBedDeflated() {
        transform.position = new Vector3(
            transform.position.x, deflatedY, transform.position.z);
        
        audioSource.clip = deflatedClip;
        audioSource.Play();
        
        spriteRenderer.sprite = deflatedSprite;
        gameManager.SetGameOverFlag();
    }
}
