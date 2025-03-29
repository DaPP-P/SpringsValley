using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;


public class UIManager : MonoBehaviour
{

    public static bool UIOpen = false;

    public static GameObject openUI;


    public GameObject menuCanvas; // Reference to your UI canvas
    public GameObject gameOverPanel; // Reference to your Game Over panel
    public PlayerMovement playerMovement;

    // Canvases
    public GameObject inventoryCanvas;

    public GameObject questCanvas;

    public bool isAlive;

    static public bool isPaused;

  
    void Start()
    {
        // Ensure the UI canvas is initially disabled
        menuCanvas.SetActive(false);
        gameOverPanel.SetActive(false);
        inventoryCanvas.SetActive(false);
        isAlive = true;
    }

    void Update()
    {

        if (UIOpen)
        {
            UIIsOpen();
        }

        // Check for Esc key press
        if (Input.GetKeyDown(KeyCode.Escape) && isAlive)
        {
            if (UIOpen) {
                CloseUI();
            } else {
                TogglePauseMenu();  
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab) && isAlive)
        {
            if (UIOpen) {
                CloseUI();
            } else {
                ToggleInventoryMenu();  
            }
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
        openUI = menuCanvas;
        UIOpen = isPaused;

        if (isPaused) {
            playerMovement.attackable = false;
        }

        // Pause or resume the game time
        Time.timeScale = isPaused ? 0 : 1;
    }

    void CloseUI()
    {
        if (openUI != null)
        {
            openUI.SetActive(false); // Hide the currently open UI
            openUI = null; // Clear reference
        }

        isPaused = false; // Reset pause state
        UIOpen = false;
        Time.timeScale = 1; // Resume game time

        StartCoroutine(AttackableCoolDown()); // Reactivate attacks after closing UI
    }

    void UIIsOpen()
    {

        isPaused = true; // Reset pause state
        UIOpen = true;
        Time.timeScale = 0; // Resume game time

        StartCoroutine(AttackableCoolDown()); // Reactivate attacks after closing UI
    }

    void ToggleInventoryMenu()
    {
        // Check if the game is paused
        isPaused = !inventoryCanvas.activeSelf;
        UIOpen = isPaused;
        openUI = inventoryCanvas;

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

    public void InventoryButtonClicked()
    {
        questCanvas.SetActive(false);
        inventoryCanvas.SetActive(true);
        openUI = inventoryCanvas;
    }

    public void QuestButtonClicked()
    {
        inventoryCanvas.SetActive(false);
        questCanvas.SetActive(true);
        openUI = questCanvas;
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