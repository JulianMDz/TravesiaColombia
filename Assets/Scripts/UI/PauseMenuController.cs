using UnityEngine;
using UnityEngine.SceneManagement;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PauseMenuController : MonoBehaviour
{
    [Header("Panel de pausa")]
    [SerializeField] private GameObject pauseCanvas;

    [Header("Escenas")]
    [SerializeField] private string mainMenuSceneName = "SampleScene";

    private bool isPaused = false;

    private void Start()
    {
        if (pauseCanvas != null)
        {
            pauseCanvas.SetActive(false);
        }

        Time.timeScale = 1f;
        isPaused = false;
    }

    private void Update()
    {
        bool escapePressed = false;

#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            escapePressed = true;
        }
#else
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            escapePressed = true;
        }
#endif

        if (escapePressed)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;

        if (pauseCanvas != null)
        {
            pauseCanvas.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;

        if (pauseCanvas != null)
        {
            pauseCanvas.SetActive(false);
        }

        Time.timeScale = 1f;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}