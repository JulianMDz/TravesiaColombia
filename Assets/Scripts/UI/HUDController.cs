using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Vidas")]
    [SerializeField] private Image[] heartImages;
    [SerializeField] private TMP_Text vidaText;

    [Header("Tiempo y Score")]
    [SerializeField] private TMP_Text tiempoText;
    [SerializeField] private TMP_Text scoreText;

    [Header("Recolectables")]
    [SerializeField] private TMP_Text plumaText;
    [SerializeField] private TMP_Text hongoText;
    [SerializeField] private TMP_Text esmeraldaText;

    private int currentLives = 3;
    private int currentScore = 0;
    private int currentPlumas = 0;
    private int currentHongos = 0;
    private int currentEsmeraldas = 0;

    private float timer = 0f;
    private bool timerRunning = true;

    private void Start()
    {
        UpdateLives(3);
        UpdateScore(0);
        UpdatePlumas(0);
        UpdateHongos(0);
        UpdateEsmeraldas(0);
    }

    private void Update()
    {
        if (timerRunning)
        {
            timer += Time.deltaTime;
            UpdateTimerText();
        }
    }

    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);

        if (tiempoText != null)
        {
            tiempoText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
        }
    }

    public void UpdateLives(int lives)
    {
        currentLives = lives;

        if (vidaText != null)
        {
            vidaText.text = "VIDA";
        }

        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] != null)
            {
                heartImages[i].enabled = i < currentLives;
            }
        }
    }

    public void UpdateScore(int score)
    {
        currentScore = score;

        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }
    }

    public void UpdatePlumas(int amount)
    {
        currentPlumas = amount;

        if (plumaText != null)
        {
            plumaText.text = currentPlumas.ToString();
        }
    }

    public void UpdateHongos(int amount)
    {
        currentHongos = amount;

        if (hongoText != null)
        {
            hongoText.text = currentHongos.ToString();
        }
    }

    public void UpdateEsmeraldas(int amount)
    {
        currentEsmeraldas = amount;

        if (esmeraldaText != null)
        {
            esmeraldaText.text = currentEsmeraldas.ToString();
        }
    }

    public void AddPluma()
    {
        currentPlumas++;
        UpdatePlumas(currentPlumas);
    }

    public void AddHongo()
    {
        currentHongos++;
        UpdateHongos(currentHongos);
    }

    public void AddEsmeralda()
    {
        currentEsmeraldas++;
        UpdateEsmeraldas(currentEsmeraldas);
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateScore(currentScore);
    }

    public void LoseLife()
    {
        currentLives--;
        currentLives = Mathf.Clamp(currentLives, 0, heartImages.Length);
        UpdateLives(currentLives);
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    public float GetCurrentTime()
    {
        return timer;
    }
}