using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject uiCanvas; // Reference to your UI canvas
    public GameObject gameOverPanel; // Reference to your Game Over panel

    public bool checkAlive;

    void Start()
    {
        // Ensure the UI canvas is initially disabled
        uiCanvas.SetActive(false);
        gameOverPanel.SetActive(false);
        checkAlive = true;
    }

    void Update()
    {
        // Check for Esc key press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Toggle the visibility of the UI canvas
            TogglePauseMenu();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            // Restart the current scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (checkAlive)
        {
            Debug.Log("Player is dead");
            checkAlive = false;
            StartCoroutine(utility.DelayedAction(2f, () =>
            {
                ShowGameOverPanel();
            }));
        }
    }

    public void ShowGameOverPanel()
    {
        // Show the Game Over panel
        Time.timeScale = 0;
        gameOverPanel.SetActive(true);
    }

    void TogglePauseMenu()
    {
        // Check if the game is paused
        bool isPaused = !uiCanvas.activeSelf;

        // Toggle the visibility of the UI canvas
        uiCanvas.SetActive(isPaused);

        // Pause or resume the game time
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void ResumeButtonClicked()
    {
        // Hide the UI canvas
        TogglePauseMenu();
    }

    public void RestartButtonClicked()
    {
        // Restart the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    public void ExitButtonClicked()
    {
        // Exit the game
        Application.Quit();
    }
}