using EPOOutline;
using NUnit.Framework;
using StarterAssets;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

public class PlayerInventory : MonoBehaviour
{
    public bool hasPendrive = false;
    public bool hasComputer = false;
    public bool hasBomb = false;
    public bool hasWaited = false;

    public int bombsAmount;

    private PlayerInteract _interact;
    private StarterAssetsInputs _input;

    public LoadingCircle _loading;
    public VideoPlayer computerPlayer;
    public MeshRenderer computerScreen;

    public string PendriveTag;
    public string ComputerTag;
    public string BombTag;
    public string BombPlacementTag;
    public string TentTag;
    public string HelicopterTag;

    public float timeToUseComputer;
    public float timeToPlaceBomb;
    public float timeToBoardHelicopter;

    public GameObject c4Prefab;
    public Transform bombPlacement;

    public List<GameObject> bombsPlaced = new();

    private bool placingBomb = false;
    private bool hackingComputer = false;
    private bool boardingHelicopter = false;

    public string sceneNightName;
    public FadeSceneLoader sceneLoader;

    public Missions missions;

    public GameObject tower;

    public bool doingStuff;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _interact = GetComponent<PlayerInteract>();
        _input = GetComponent<StarterAssetsInputs>();
        missions.PenDriveMission();
    }

    public void explodeTower()
    {
        foreach(var c4 in bombsPlaced)
        {
            c4.GetComponent<c4>().explode();
        }

        tower.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        doingStuff = (placingBomb || hackingComputer || boardingHelicopter);

        if (hasBomb && bombsAmount <= 0)
        {
            missions.EscapeMission();
            hasBomb = false;
        }

        if (_interact.GetCurrentInteractable() == null) { }
        else if (_interact.GetCurrentInteractable().CompareTag(BombPlacementTag) && hasBomb && bombsAmount > 0 && !placingBomb && hasWaited)
        {
            if (_input.interact)
            {
                _input.lockPlayer(true);
                placingBomb = true;
                _loading.CheckConditionForTime(() => _input.interact && (_interact.GetCurrentInteractable() != null && _interact.GetCurrentInteractable().CompareTag(BombPlacementTag)), timeToPlaceBomb,
                    () =>
                    {
                        _input.unlockPlayer();
                        GameObject bomb = Instantiate(c4Prefab, _interact.collisionPoint, Quaternion.LookRotation(_interact.hitNormal));
                        bomb.transform.parent = bombPlacement;
                        bombsPlaced.Add(bomb);
                        bombsAmount--;
                        placingBomb = false;
                    },
                    () =>
                    {
                        _input.unlockPlayer();
                        placingBomb = false;
                    }
                    );
            }
        }
        else if (_interact.GetCurrentInteractable().CompareTag(PendriveTag))
        {
            if (_input.interact)
            {
                hasPendrive = true;
                _interact.GetCurrentInteractable().SetActive(false);
                missions.HackComputerMission();
            }
        }
        else if (_interact.GetCurrentInteractable().CompareTag(ComputerTag) && hasPendrive && !hackingComputer && !hasComputer)
        {
            if (_input.interact)
            {
                _input.lockPlayer(true);
                hackingComputer = true;
                computerScreen.enabled = true;
                computerPlayer.Play();
                _loading.CheckConditionForTime(() => _input.interact && (_interact.GetCurrentInteractable() != null && _interact.GetCurrentInteractable().CompareTag(ComputerTag)), timeToUseComputer,
                    () =>
                    {
                        _input.unlockPlayer();
                        hasComputer = true;
                        missions.WaitMission();
                        hackingComputer = false;
                    },
                    () =>
                    {
                        _input.unlockPlayer();
                        computerPlayer.Stop();
                        computerScreen.enabled = false;
                        hackingComputer = false;
                    }
                    );
            }
        }
        else if (_interact.GetCurrentInteractable().CompareTag(BombTag))
        {
            if (_input.interact && hasWaited)
            {
                hasBomb = true;
                bombsAmount = 6;
                missions.PlaceBombMission();
                _interact.GetCurrentInteractable().SetActive(false);
            }
        }
        else if (_interact.GetCurrentInteractable().CompareTag(TentTag))
        {
            if (_input.interact && hasComputer)
            {
                hasWaited = true;
                _interact.GetCurrentInteractable().GetComponent<Outlinable>().enabled = false;
                sceneLoader.LoadScene(sceneNightName, missions.GrabBombMission);
            }
        }
        else if (_interact.GetCurrentInteractable().CompareTag(HelicopterTag))
        {
            if (_input.interact && !boardingHelicopter)
            {
                _input.lockPlayer(true);
                boardingHelicopter = true;

                _loading.CheckConditionForTime(() => _input.interact && (_interact.GetCurrentInteractable() != null && _interact.GetCurrentInteractable().CompareTag(HelicopterTag)), timeToBoardHelicopter,
                    () =>
                    {
                        _input.unlockPlayer();
                        _interact.GetCurrentInteractable().GetComponent<Helicopter>().doAnimation();
                    },
                    () =>
                    {
                        _input.unlockPlayer();
                        boardingHelicopter = false;
                    }
                );
            }
        }
    }
}
