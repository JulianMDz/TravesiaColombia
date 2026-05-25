using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionsPanel;

    [Header("Volumen")]
    [SerializeField] private Slider volumeSlider;

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

    private float masterVolume = 1f;
    private float tempMasterVolume = 1f;

    private void Start()
    {
        musicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        sfxEnabled = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);

        tempMusicEnabled = musicEnabled;
        tempSfxEnabled = sfxEnabled;
        tempMasterVolume = masterVolume;

        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.wholeNumbers = false;
            volumeSlider.value = tempMasterVolume;

            volumeSlider.onValueChanged.AddListener(SetMasterVolume);
        }

        UpdateVisualState();
        ApplySavedAudioSettings();
    }

    public void SetMusicEnabled(bool enabled)
    {
        tempMusicEnabled = enabled;
        UpdateVisualState();
        ApplyTemporaryAudioSettings();
    }

    public void SetSfxEnabled(bool enabled)
    {
        tempSfxEnabled = enabled;
        UpdateVisualState();
        ApplyTemporaryAudioSettings();
    }

    public void SetMasterVolume(float volume)
    {
        tempMasterVolume = volume;
        ApplyTemporaryAudioSettings();
    }

    public void OnClickSave()
    {
        musicEnabled = tempMusicEnabled;
        sfxEnabled = tempSfxEnabled;
        masterVolume = tempMasterVolume;

        PlayerPrefs.SetInt("MusicEnabled", musicEnabled ? 1 : 0);
        PlayerPrefs.SetInt("SFXEnabled", sfxEnabled ? 1 : 0);
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.Save();

        ApplySavedAudioSettings();
        CloseOptions();
    }

    public void OnClickCancel()
    {
        tempMusicEnabled = musicEnabled;
        tempSfxEnabled = sfxEnabled;
        tempMasterVolume = masterVolume;

        if (volumeSlider != null)
        {
            volumeSlider.value = tempMasterVolume;
        }

        UpdateVisualState();
        ApplySavedAudioSettings();
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

    private void ApplyTemporaryAudioSettings()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMasterVolume(tempMasterVolume);
            AudioManager.Instance.SetMusicVolume(tempMusicEnabled ? 1f : 0f);
            AudioManager.Instance.SetSFXVolume(tempSfxEnabled ? 1f : 0f);
        }
        else
        {
            AudioListener.volume = tempMasterVolume;
        }
    }

    private void ApplySavedAudioSettings()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMasterVolume(masterVolume);
            AudioManager.Instance.SetMusicVolume(musicEnabled ? 1f : 0f);
            AudioManager.Instance.SetSFXVolume(sfxEnabled ? 1f : 0f);
        }
        else
        {
            AudioListener.volume = masterVolume;
        }

        Debug.Log("Configuración aplicada. Música: " + musicEnabled +
                  " | SFX: " + sfxEnabled +
                  " | Volumen: " + masterVolume);
    }
}