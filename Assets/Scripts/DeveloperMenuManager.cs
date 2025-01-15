using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeveloperMenuManager : MonoBehaviour
{
    public GameObject developerMenu; // The developer menu UI
    public Slider volumeSlider;      // Slider for adjusting a value (e.g., volume)
    public  float volumeLevel; // Public variable to store the slider value

    private const string VolumePrefKey = "VolumeLevel"; // Key for PlayerPrefs

    private void Start()
    {
        // Load the saved volume value from PlayerPrefs
        volumeLevel = PlayerPrefs.GetFloat(VolumePrefKey, 1.0f); // Default to 1.0 if not set
        volumeSlider.value = volumeLevel;

        // Subscribe to the slider's value change event
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    private void OnVolumeChanged(float value)
    {
        volumeLevel = value; // Update the public variable
        PlayerPrefs.SetFloat(VolumePrefKey, value); // Save the value in PlayerPrefs
        PlayerPrefs.Save(); // Ensure the changes are saved immediately
    }

    public void EnableMenu()
    {
        developerMenu.SetActive(true);
    }

    public void DisableMenu()
    {
        developerMenu.SetActive(false);
    }

    public void LoadLevel1()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadLevel2()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadLevel3()
    {
        SceneManager.LoadScene(2);
    }

    public void LoadLevel4()
    {
        SceneManager.LoadScene(3);
    }

    public void LoadLevel5()
    {
        SceneManager.LoadScene(4);
    }
}
