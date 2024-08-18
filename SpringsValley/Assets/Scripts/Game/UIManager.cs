using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject uiCanvas; // Reference to your UI canvas
    public GameObject gameOverPanel; // Reference to your Game Over panel

    public bool isAlive;

    static public bool isPaused;

    void Start()
    {
        // Ensure the UI canvas is initially disabled
        uiCanvas.SetActive(false);
        gameOverPanel.SetActive(false);
        isAlive = true;
    }

    void Update()
    {
        // Check for Esc key press
        if (Input.GetKeyDown(KeyCode.Escape) && isAlive)
        {
            // Toggle the visibility of the UI canvas
            TogglePauseMenu();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            // Restart the current scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
        isPaused = !uiCanvas.activeSelf;

        // Toggle the visibility of the UI canvas
        uiCanvas.SetActive(isPaused);

        // Pause or resume the game time
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void ResumeButtonClicked()
    {
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