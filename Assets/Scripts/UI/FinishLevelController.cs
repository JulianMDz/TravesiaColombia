using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLevelController : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName = "SampleScene";

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