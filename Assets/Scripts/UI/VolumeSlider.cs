using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public Slider m_VolumeSlider;
    public float m_VolumeSliderValue;

    public SettingsContainer settings;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("AudioVolume", -1f);
        if (savedVolume == -1f)
        {
            PlayerPrefs.SetFloat("AudioVolume", 100f);
            savedVolume = 100f;
        }

        m_VolumeSlider.value = savedVolume;
        m_VolumeSliderValue = savedVolume;
        settings.volume = m_VolumeSlider.value;
    }

    // Update is called once per frame
    void Update()
    {
       

    }

    public void ChangeSlider(float value)
    {
        m_VolumeSliderValue = value;
        PlayerPrefs.SetFloat("AudioVolume", m_VolumeSliderValue);
        settings.volume = m_VolumeSlider.value;
    }
}
