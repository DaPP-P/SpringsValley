using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;


public class UIManager : MonoBehaviour
{
    public GameObject menuCanvas; // Reference to your UI canvas
    public GameObject gameOverPanel; // Reference to your Game Over panel
    public PlayerMovement playerMovement;


    public GameObject inventoryCanvas;
    public bool isAlive;

    static public bool isPaused;

    void Start()
    {
        // Ensure the UI canvas is initially disabled
        menuCanvas.SetActive(false);
        gameOverPanel.SetActive(false);
        inventoryCanvas.SetActive(false);
        playerMovement = FindObjectOfType<PlayerMovement>();
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

        if (Input.GetKeyDown(KeyCode.Tab) && isAlive)
        {
            ToggleInventoryMenu();
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
        isPaused = !menuCanvas.activeSelf;

        // Toggle the visibility of the UI canvas
        menuCanvas.SetActive(isPaused);

        if (isPaused) {
            playerMovement.attackable = false;
        }

        // Pause or resume the game time
        Time.timeScale = isPaused ? 0 : 1;

    }

    void ToggleInventoryMenu()
    {
        // Check if the game is paused
        isPaused = !inventoryCanvas.activeSelf;

        // Toggle the visibility of the UI canvas
        inventoryCanvas.SetActive(isPaused);

        if (isPaused) {
            playerMovement.attackable = false;
        }

        // Pause or resume the game time
        Time.timeScale = isPaused ? 0 : 1; 

        if (!isPaused)
        {
            StartCoroutine(AttackableCoolDown());
        }
    }

    public void ResumeButtonClicked()
    {
        TogglePauseMenu();
        StartCoroutine(ResetAttackableNextFrame());
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

    private IEnumerator ResetAttackableNextFrame()
    {
    yield return new WaitForEndOfFrame();
        if (playerMovement != null)
        {
        playerMovement.attackable = true;
        }
    }

    private IEnumerator AttackableCoolDown()
    {
        playerMovement.attackable = false;
        yield return new WaitForSeconds(0.2f);
        {
            playerMovement.attackable = true;

        }
    }
}