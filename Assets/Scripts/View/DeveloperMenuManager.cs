using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeveloperMenuManager : MonoBehaviour
{
    public GameObject developerMenu;
    public Slider volumeSlider;
    public float volumeLevel;

    private const string VolumePrefKey = "VolumeLevel";

    private void Start()
    {
        volumeLevel = PlayerPrefs.GetFloat(VolumePrefKey, 1.0f);
        volumeSlider.value = volumeLevel;

        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    private void OnVolumeChanged(float value)
    {
        volumeLevel = value;
        PlayerPrefs.SetFloat(VolumePrefKey, value);
        PlayerPrefs.Save();
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
