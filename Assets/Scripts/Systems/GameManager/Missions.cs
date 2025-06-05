using Unity.VisualScripting;
using UnityEngine;

public class Missions : MonoBehaviour
{
    public Radio radio;

    public AudioClip PenDriveAudio = null;
    public AudioClip HackComputerAudio = null;
    public AudioClip WaitAudio = null;
    public AudioClip GrabBombAudio = null;
    public AudioClip PlaceBombAudio = null;
    public AudioClip EscapeAudio = null;
    public AudioClip MissionCompleteAudio = null;
    public AudioClip MinefieldAudio = null;

    private string PenDriveText = 
        "<size=36><b>Current mission</b></size>\n<size=12> </size>\n" +
        "Grab Pen Drive containing important information (\udb84\ude9e)\n" +
        "It is located in the coaster base of the enemy\n" +
        "Stealth is very important";
    private string HackComputerText = "";
    private string WaitText = "";
    private string GrabBombText = "";
    private string PlaceBombText = "";
    private string EscapeText = "";
    private string MissionCompleteText = "";
    [DoNotSerialize] public string MinefieldText = "";

    public GameObject helicopter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PenDriveMission()
    {
        radio.TransmitirMensaje(PenDriveText, PenDriveAudio);
    }

    public void HackComputerMission()
    {
        radio.TransmitirMensaje(HackComputerText, HackComputerAudio);
    }

    public void WaitMission()
    {
        radio.TransmitirMensaje(WaitText, WaitAudio);
    }

    public void GrabBombMission()
    {
        radio.TransmitirMensaje(GrabBombText, GrabBombAudio);
    }

    public void PlaceBombMission()
    {
        radio.TransmitirMensaje(PlaceBombText, PlaceBombAudio);
    }

    public void EscapeMission()
    {
        radio.TransmitirMensaje(EscapeText, EscapeAudio);
        helicopter.SetActive(true);
        helicopter.GetComponent<Helicopter>().toggleHelicopterDust();

    }

    public void MissionComplete()
    {
        radio.TransmitirMensaje(MissionCompleteText, MissionCompleteAudio);
    }
}
