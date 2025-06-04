using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public Slider m_VolumeSlider;
    public float m_VolumeSliderValue;

    //public SettingsContainer settings;

   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
        m_VolumeSlider.value = PlayerPrefs.GetFloat("AudioVolume", 100f);
        //settings.volume = m_VolumeSlider.value;
        

    }

   

    // Update is called once per frame
    void Update()
    {
       

    }

   

        

    public void ChangeSlider(float value)
    {
        m_VolumeSliderValue = value;
        m_VolumeSlider.value = PlayerPrefs.GetFloat("AudioVolume", m_VolumeSliderValue);
        //settings.volume = m_VolumeSlider.value;
    }
}
