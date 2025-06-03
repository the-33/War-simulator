using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public Slider m_VolumeSlider;
    public float m_VolumeSliderValue;

    //public Slider m_SensititySlider;
    public float m_SensititySliderValue = 100f;
    //public Transform m_Playerbody;
    private float xRotation = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_SensititySliderValue = PlayerPrefs.GetFloat("CurrentSensitivity", 100);
        m_VolumeSlider.value = PlayerPrefs.GetFloat("AudioVolume", 50f);
        AudioListener.volume = m_VolumeSlider.value;
        //m_SensititySlider.value = m_SensititySliderValue / 10;

    }

   

    // Update is called once per frame
    void Update()
    {
        PlayerPrefs.SetFloat("CurrentSensitivity", 100);
        float mouseX = Input.GetAxis("Mouse X") * m_SensititySliderValue * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * m_SensititySliderValue * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        //m_Playerbody.Rotate(Vector3.up, mouseX);

    }

    public void AdjustSpeed (float speed)
    {
        m_SensititySliderValue = speed * 10;
    }

        

    public void ChangeSlider(float value)
    {
        m_VolumeSliderValue = value;
        m_VolumeSlider.value = PlayerPrefs.GetFloat("AudioVolume", m_VolumeSliderValue);
        AudioListener.volume = m_VolumeSlider.value;
    }
}
