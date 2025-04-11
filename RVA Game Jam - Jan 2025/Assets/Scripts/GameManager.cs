using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static float nightLength = 60.0f;

    private bool running = false;
    public static bool gameOverFlag = false;
    public static bool won = false;
    private Deflate bedDeflate;
    private Mom mom;
    //public static float timer = 0.0f;
    [SerializeField] GameObject momAsset;
    [SerializeField] TextMeshProUGUI generalText;

    private void Awake()
    {
        momAsset.SetActive(false);
        bedDeflate = GameObject.Find("Bed").GetComponent<Deflate>();
        if (bedDeflate == null) {
            Debug.Log("Failed to find the Bed component!");
            Application.Quit();
        }

        mom = GameObject.Find("Door").GetComponent<Mom>();
        if (mom == null) {
            Debug.Log("Failed to find the Mom component!");
            Application.Quit();
        }
    }

    private void Update() {

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Debug.Log("Escape Key Pressed!");
            Application.Quit();
        }

        // check if player lasted the whole night and won
        if (running && Timer.remainingTime == 0) {
            running = false;
            won = true;
            Timer.remainingTime = 0;

            Debug.Log("Game Over! You Won :)");
            generalText.text = "You won!";
            Application.Quit();
        }

        // check if the game over flag was set
        if (gameOverFlag) {
            momAsset.SetActive(true);
            running = false;
            Debug.Log("Running is now False");
            gameOverFlag = false;
            Timer.remainingTime = 0;
            
            // TODO Show game over text
            Debug.Log("Game Over! You Lost :(");
            generalText.text = "Mom has caught you! Game Over";
            Application.Quit();
            //EditorApplication.ExitPlaymode();
        }

        // restart the game if the user clicks
        else if (!running && Timer.remainingTime > 1 && Input.GetMouseButtonDown(0)) {
            Debug.Log("Resetting game!");
            generalText.text = "Don't bother mom and keep the bed inflated!";
            ResetGame();
            running = true;
        }

        //timer += Time.deltaTime;
        //if (Mathf.Repeat(timer, 1.0f) < 0.001) {
            //Debug.Log("Time: " + timer);
        //}
    }

    public void SetGameOverFlag() {
        Debug.Log("Game over flag is set!");
        gameOverFlag = true;
    }

    public void SetWon(bool value) {
        won = value;
    }

    public bool IsRunning() {
        return running;
    }

    private void ResetGame() {
        running = true;
        gameOverFlag = false;
        won = false;
        Timer.remainingTime = 60f;

        bedDeflate.Reset();
        mom.Reset();
    }
}
