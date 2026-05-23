using UnityEngine;

public class OptionsController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionsPanel;

    [Header("Music Buttons")]
    [SerializeField] private GameObject musicOnButton;
    [SerializeField] private GameObject musicOffButton;

    [Header("SFX Buttons")]
    [SerializeField] private GameObject sfxOnButton;
    [SerializeField] private GameObject sfxOffButton;

    private bool musicEnabled = true;
    private bool sfxEnabled = true;

    private bool tempMusicEnabled = true;
    private bool tempSfxEnabled = true;

    private void Start()
    {
        musicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        sfxEnabled = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;

        tempMusicEnabled = musicEnabled;
        tempSfxEnabled = sfxEnabled;

        UpdateVisualState();
        ApplyAudioSettings();
    }

    public void SetMusicEnabled(bool enabled)
    {
        tempMusicEnabled = enabled;
        UpdateVisualState();
    }

    public void SetSfxEnabled(bool enabled)
    {
        tempSfxEnabled = enabled;
        UpdateVisualState();
    }

    public void OnClickSave()
    {
        musicEnabled = tempMusicEnabled;
        sfxEnabled = tempSfxEnabled;

        PlayerPrefs.SetInt("MusicEnabled", musicEnabled ? 1 : 0);
        PlayerPrefs.SetInt("SFXEnabled", sfxEnabled ? 1 : 0);
        PlayerPrefs.Save();

        ApplyAudioSettings();
        CloseOptions();
    }

    public void OnClickCancel()
    {
        tempMusicEnabled = musicEnabled;
        tempSfxEnabled = sfxEnabled;

        UpdateVisualState();
        CloseOptions();
    }

    private void CloseOptions()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }

        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }
    }

    private void UpdateVisualState()
    {
        if (musicOnButton != null)
        {
            musicOnButton.SetActive(tempMusicEnabled);
        }

        if (musicOffButton != null)
        {
            musicOffButton.SetActive(!tempMusicEnabled);
        }

        if (sfxOnButton != null)
        {
            sfxOnButton.SetActive(tempSfxEnabled);
        }

        if (sfxOffButton != null)
        {
            sfxOffButton.SetActive(!tempSfxEnabled);
        }
    }

    private void ApplyAudioSettings()
    {
        // Versión simple: si música y efectos están apagados, se silencia todo.
        // Si alguno está activo, se deja el audio general encendido.
        AudioListener.volume = (musicEnabled || sfxEnabled) ? 1f : 0f;

        Debug.Log("Configuración guardada. Música: " + musicEnabled + " | SFX: " + sfxEnabled);
    }
}