using UnityEngine;
using UnityEngine.SceneManagement;
using TravesiaColombia.Core;

public class GameOverController : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject hudCanvas;
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject inventoryCanvas;

    [Header("Escenas")]
    [SerializeField] private string mainMenuSceneName = "SampleScene";

    private bool gameOverShown = false;

    private void Start()
    {
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(false);
        }

        gameOverShown = false;
    }

    private void OnEnable()
    {
        EventBus.Subscribe<PlayerDied>(OnPlayerDied);
        EventBus.Subscribe<GameOver>(OnGameOver);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<PlayerDied>(OnPlayerDied);
        EventBus.Unsubscribe<GameOver>(OnGameOver);
    }

    private void OnPlayerDied(PlayerDied e)
    {
        ShowGameOver();
    }

    private void OnGameOver(GameOver e)
    {
        ShowGameOver();
    }

    private void ShowGameOver()
    {
        if (gameOverShown)
        {
            return;
        }

        gameOverShown = true;

        if (hudCanvas != null)
        {
            hudCanvas.SetActive(false);
        }

        if (pauseCanvas != null)
        {
            pauseCanvas.SetActive(false);
        }

        if (inventoryCanvas != null)
        {
            inventoryCanvas.SetActive(false);
        }

        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);
        }

        if (AudioManager.Instance != null)
        {
            // Si ya tienes sonido de muerte total en AudioManager, aquí lo llamamos.
            // AudioManager.Instance.PlayGameOver();
        }

        Time.timeScale = 0f;
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

    public void QuitGame()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}