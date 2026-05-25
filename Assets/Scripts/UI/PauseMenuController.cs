using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class PauseMenuController : MonoBehaviour
{
    [Header("Panel de pausa")]
    [SerializeField] private GameObject pauseCanvas;

    [Header("Escenas")]
    [SerializeField] private string mainMenuSceneName = "SampleScene";

    [Header("Textos de pausa — asignar en Inspector")]
    [SerializeField] private TMP_Text pauseScoreText;
    [SerializeField] private TMP_Text pauseTiempoText;
    [SerializeField] private TMP_Text pausePlumaText;
    [SerializeField] private TMP_Text pauseHongoText;
    [SerializeField] private TMP_Text pauseEsmeraldaText;
    [SerializeField] private Image[] pauseHeartImages;

    [Header("Referencia al HUD")]
    [SerializeField] private HUDController hudController;

    private bool isPaused = false;

    private void Start()
    {
        if (pauseCanvas != null)
            pauseCanvas.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;

        // Habilitar Enhanced Touch para Android
#if ENABLE_INPUT_SYSTEM
    UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Enable();
#endif
    }

    private void Update()
    {
        bool escapePressed = false;

#if ENABLE_INPUT_SYSTEM
    if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        escapePressed = true;
#endif

        // Back button Android — funciona con ambos Input Systems
        if (Input.GetKeyDown(KeyCode.Escape))
            escapePressed = true;

        if (escapePressed)
            TogglePause();
    }

    public void TogglePause()
    {
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    public void PauseGame()
    {
        isPaused = true;

        // Actualizar datos antes de mostrar el panel
        RefreshPauseUI();

        if (pauseCanvas != null)
            pauseCanvas.SetActive(true);

        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;

        if (pauseCanvas != null)
            pauseCanvas.SetActive(false);

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

    // ── Actualiza los textos del panel de pausa con datos del HUD ────────────
    private void RefreshPauseUI()
    {
        if (hudController == null) return;

        // Score
        if (pauseScoreText != null)
            pauseScoreText.text = hudController.GetCurrentScore().ToString();

        // Tiempo
        if (pauseTiempoText != null)
        {
            float t = hudController.GetCurrentTime();
            int min = Mathf.FloorToInt(t / 60f);
            int sec = Mathf.FloorToInt(t % 60f);
            pauseTiempoText.text = min.ToString("00") + ":" + sec.ToString("00");
        }

        // Recolectables
        if (pausePlumaText != null)
            pausePlumaText.text = hudController.GetCurrentPlumas().ToString();

        if (pauseHongoText != null)
            pauseHongoText.text = hudController.GetCurrentHongos().ToString();

        if (pauseEsmeraldaText != null)
            pauseEsmeraldaText.text = hudController.GetCurrentEsmeraldas().ToString();

        // Vidas (corazones)
        if (pauseHeartImages != null)
        {
            int lives = hudController.GetCurrentLives();
            for (int i = 0; i < pauseHeartImages.Length; i++)
            {
                if (pauseHeartImages[i] != null)
                    pauseHeartImages[i].enabled = i < lives;
            }
        }
    }
}