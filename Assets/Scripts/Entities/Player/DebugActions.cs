using StarterAssets;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugActions : MonoBehaviour
{
    public Canvas UI;
    public GameObject FreeCamera;
    public GameObject PlayerCamera;
    public GameObject WeaponCamera;

    public FirstPersonController playerController;

    private bool freeCameraEnabled = false;

    private void toggleUI(bool active)
    {
        if (UI != null && !freeCameraEnabled)
        {
            UI.enabled = active;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            toggleUI(!UI.enabled);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            toggleUI(freeCameraEnabled);
            freeCameraEnabled = !freeCameraEnabled;
            toggleUI(!freeCameraEnabled);
            FreeCamera.SetActive(freeCameraEnabled);
            PlayerCamera.SetActive(!freeCameraEnabled);
            WeaponCamera.SetActive(!freeCameraEnabled);
        }
    }
}
