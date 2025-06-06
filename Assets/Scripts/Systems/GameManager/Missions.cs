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
    private string HackComputerText =
        "<size=36><b>Current mission</b></size>\n<size=12> </size>\n" +
        "Take pen drive to the computer in the village base (\ueea7)\n" +
        "It is located inside the hangar by the hospital\n" +
        "Try not to get spotted";
    private string WaitText =
        "<size=36><b>Current mission</b></size>\n<size=12> </size>\n" +
        "Wait until night inside the tent behind the Hangar(\udb81\udd08)\n" +
        "There are provisions and ammo inside the tent for you\n" +
        "Exit the hangar through the back door\n" +
        "Theres too many enemies wandering arround";
    private string GrabBombText = 
        "<size=36><b>Current mission</b></size>\n<size=12> </size>\n" +
        "Find the C4 stashed in the enemy's village \nbase and take it with you(\uf1e2)\n" +
        "Possible location: East of the village\n";
    private string PlaceBombText =
        "<size=36><b>Current mission</b></size>\n<size=12> </size>\n" +
        "Put the 6 C4s in the columns of the tower(\uf1e2)\n" +
        "Reach the second floor of the tower and place the bombs\n" +
        "Put at least one C4 in each column";
    private string EscapeText = 
        "<size=36><b>Current mission</b></size>\n<size=12> </size>\n" +
        "Get back to the beach\n" +
        "Get to the beach, an extraction vehicle is waiting (\uedfd)\n" +
        "The bombs are exploding soon";
    private string MissionCompleteText =
        "<size=36><b>Mission complete</b></size>\n<size=12> </size>\n" +
        "Good job, you're heading home now\n";
    public string MinefieldText { get; private set; } =
        "<color=red><size=36><b>WARNING \uea6c</b></size>\n<size=12> </size>\n" +
        "You are in a minefield, go back where you came from inmediately(\udb83\uddda)\n</color>";

    public GameObject helicopter;
    public GameObject enemysAfterHack;
    public GameObject enemysAfterPlacingBomb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(enemysAfterPlacingBomb);
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
        enemysAfterHack.SetActive(true);
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
        enemysAfterPlacingBomb.SetActive(true);
        helicopter.SetActive(true);
        helicopter.GetComponent<Helicopter>().toggleHelicopterDust();

    }

    public void MissionComplete()
    {
        radio.TransmitirMensaje(MissionCompleteText, MissionCompleteAudio);
    }
}
