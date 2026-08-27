using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public static bool UIOpen { get; private set; }
    public static GameObject openUI { get; private set; }
    public static bool isPaused { get; private set; }

    public GameObject menuCanvas;
    public GameObject gameOverPanel;
    public PlayerMovement playerMovement;
    public GameObject inventoryCanvas;
    public GameObject questCanvas;
    public bool isAlive = true;

    private bool gameOver;
    private Coroutine controlsCooldown;

    private void Awake()
    {
        Instance = this;
        ResetStaticState();

        if (playerMovement == null)
        {
            playerMovement = FindFirstObjectByType<PlayerMovement>();
        }
    }

    private void Start()
    {
        SetActive(menuCanvas, false);
        SetActive(gameOverPanel, false);
        SetActive(inventoryCanvas, false);
        SetActive(questCanvas, false);
        isAlive = true;
        gameOver = false;
        SetPaused(false);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            ResetStaticState();
            Time.timeScale = 1f;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartButtonClicked();
            return;
        }

        if (gameOver || !isAlive)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (UIOpen)
            {
                CloseUI();
            }
            else
            {
                OpenInternal(menuCanvas);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (UIOpen)
            {
                CloseUI();
            }
            else
            {
                OpenInternal(inventoryCanvas);
            }
        }
    }

    public static bool OpenUI(GameObject uiRoot)
    {
        if (Instance == null || uiRoot == null || Instance.gameOver)
        {
            return false;
        }

        Instance.OpenInternal(uiRoot);
        return true;
    }

    public static void CloseCurrentUI()
    {
        if (Instance != null)
        {
            Instance.CloseUI();
        }
    }

    public void ShowGameOverPanel()
    {
        gameOver = true;
        isAlive = false;

        if (openUI != null && openUI != gameOverPanel)
        {
            openUI.SetActive(false);
        }

        SetActive(gameOverPanel, true);
        openUI = gameOverPanel;
        UIOpen = gameOverPanel != null;
        SetPaused(true);
        SetPlayerControls(false);
    }

    public void ResumeButtonClicked()
    {
        PlayAcceptSound();
        CloseUI();
    }

    public void RestartButtonClicked()
    {
        PlayAcceptSound();
        PlayerLoot.ResetInventory();
        Quest.ResetAllQuests();
        ResetStaticState();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitButtonClicked()
    {
        Application.Quit();
    }

    public void InventoryButtonClicked()
    {
        PlayAcceptSound();
        OpenInternal(inventoryCanvas);
    }

    public void QuestButtonClicked()
    {
        PlayAcceptSound();
        OpenInternal(questCanvas);
    }

    public void CloseUI()
    {
        if (gameOver)
        {
            return;
        }

        if (openUI != null)
        {
            openUI.SetActive(false);
        }

        openUI = null;
        UIOpen = false;
        SetPaused(false);

        if (controlsCooldown != null)
        {
            StopCoroutine(controlsCooldown);
        }

        controlsCooldown = StartCoroutine(EnableControlsAfterClick());
    }

    private void OpenInternal(GameObject uiRoot)
    {
        if (uiRoot == null || gameOver)
        {
            return;
        }

        if (openUI != null && openUI != uiRoot)
        {
            openUI.SetActive(false);
        }

        openUI = uiRoot;
        openUI.SetActive(true);
        UIOpen = true;
        SetPaused(true);
        SetPlayerControls(false);
    }

    private void SetPaused(bool paused)
    {
        isPaused = paused;
        Time.timeScale = paused ? 0f : 1f;
    }

    private void SetPlayerControls(bool enabled)
    {
        if (playerMovement != null)
        {
            playerMovement.SetControlsEnabled(enabled);
        }
    }

    private IEnumerator EnableControlsAfterClick()
    {
        yield return new WaitForSecondsRealtime(0.15f);
        if (!gameOver && !UIOpen)
        {
            SetPlayerControls(true);
        }

        controlsCooldown = null;
    }

    private void PlayAcceptSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.acceptSound, transform.position);
        }
    }

    private static void SetActive(GameObject target, bool active)
    {
        if (target != null)
        {
            target.SetActive(active);
        }
    }

    private static void ResetStaticState()
    {
        UIOpen = false;
        openUI = null;
        isPaused = false;
    }
}
