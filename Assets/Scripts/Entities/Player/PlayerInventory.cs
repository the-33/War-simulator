using EPOOutline;
using NUnit.Framework;
using StarterAssets;
using System.Collections.Generic;
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

    public float timeToUseComputer;
    public float timeToPlaceBomb;

    public GameObject c4Prefab;

    public List<GameObject> bombsPlaced = new();

    private bool placingBomb = false;
    private bool hackingComputer = false;

    public string sceneNightName;
    public FadeSceneLoader sceneLoader;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _interact = GetComponent<PlayerInteract>();
        _input = GetComponent<StarterAssetsInputs>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_interact.GetCurrentInteractable() == null) ;
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
                        bombsPlaced.Add(Instantiate(c4Prefab, _interact.collisionPoint, Quaternion.LookRotation(_interact.hitNormal)));
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
                _interact.GetCurrentInteractable().SetActive(false);
            }
        }
        else if (_interact.GetCurrentInteractable().CompareTag(TentTag))
        {
            if (_input.interact && hasComputer)
            {
                hasWaited = true;
                _interact.GetCurrentInteractable().GetComponent<Outlinable>().enabled = false;
                sceneLoader.LoadScene(sceneNightName);
            }
        }
    }
}
