using UnityEngine;

public class SettingsGrabber : MonoBehaviour
{
    public AudioListener AudioListener;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SettingsContainer settingsContainer = GameObject.Find("SettingsContainer").GetComponent<SettingsContainer>();

        if (settingsContainer != null)
        {
            AudioListener.volume = settingsContainer.volume;
            print("Volume set to" + AudioListener.volume);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
