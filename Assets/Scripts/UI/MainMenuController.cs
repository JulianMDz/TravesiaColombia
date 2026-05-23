using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject worldSelectorPanel;

    [Header("Scene Names")]
    [SerializeField] private string worldSelectorSceneName = "WorldSelector";
    [SerializeField] private string gameplaySceneName = "Selva_Test";

    public void OnClickPlay()
    {
        // Opción A: abrir selector dentro de la misma escena
        if (worldSelectorPanel != null)
        {
            mainMenuPanel.SetActive(false);
            worldSelectorPanel.SetActive(true);
            return;
        }

        // Opción B: cargar una escena de selector de mundos
        if (!string.IsNullOrEmpty(worldSelectorSceneName))
        {
            SceneManager.LoadScene(worldSelectorSceneName);
        }
    }

    public void OnClickOptions()
    {
        if (optionsPanel != null)
        {
            mainMenuPanel.SetActive(false);
            optionsPanel.SetActive(true);
        }
    }

    public void OnClickBackFromOptions()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
            mainMenuPanel.SetActive(true);
        }
    }

    public void OnClickStartSelvaDirectly()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void OnClickExit()
    {
        Debug.Log("Saliendo del juego...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}