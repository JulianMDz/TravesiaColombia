using UnityEngine;

public class FinishLevelTrigger : MonoBehaviour
{
    [SerializeField] private GameObject finishLevelCanvas;
    [SerializeField] private GameObject hudCanvas;

    private bool levelFinished = false;

    private void Start()
    {
        if (finishLevelCanvas != null)
        {
            finishLevelCanvas.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (levelFinished)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            levelFinished = true;
            ShowFinishLevel();
        }
    }

    private void ShowFinishLevel()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayLevelComplete();
        }

        if (finishLevelCanvas != null)
        {
            finishLevelCanvas.SetActive(true);
        }

        if (hudCanvas != null)
        {
            hudCanvas.SetActive(false);
        }

        Time.timeScale = 0f;
    }
}