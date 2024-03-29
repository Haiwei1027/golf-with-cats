using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField] Button quitButton;

    [Header("Connect")]
    [SerializeField] GameObject connectUI;

    [SerializeField] Button connectButton;
    [SerializeField] TMP_InputField nameInputField;

    [Header("Pre Lobby")]
    [SerializeField] GameObject preLobbyUI;

    [SerializeField] Button hostButton;
    [SerializeField] TMP_InputField lobbyIdInputField;
    [SerializeField] Button joinButton;

    [Header("In Lobby")]
    [SerializeField] GameObject inLobbyUI;

    [SerializeField] Button startButton;
    [SerializeField] TMP_Text lobbyIdLabel;
    [SerializeField] Transform playerList;
    [SerializeField] Button leaveButton;

    private void Initialise()
    {
        quitButton.onClick.AddListener(() => { Application.Quit(); });
        connectButton.onClick.AddListener(() => { Resident.Connect(nameInputField.text); });
        hostButton.onClick.AddListener(() => { Resident.CreateTown(); });
        joinButton.onClick.AddListener(() => { Resident.JoinTown(lobbyIdInputField.text); });
        startButton.onClick.AddListener(() => { Resident.StartGame(); });
        leaveButton.onClick.AddListener(() => { Resident.LeaveTown(); });
    }

    private void Listen()
    {
        Resident.Instance.LetterHandler.onConnected += OnConnected;
        Resident.Instance.LetterHandler.onDisconnected += OnDisconnect;
        Resident.Instance.LetterHandler.onJoinTown += OnJoinLobby;
        Resident.Instance.LetterHandler.onLeaveTown += OnLeaveLobby;
        Resident.Instance.LetterHandler.onStartGame += OnStartGame;
    }

    private void Awake()
    {
        Initialise();
        
    }

    private void Start()
    {
        Listen();
        OnDisconnect();
        //Resident.Disconnect();
    }

    public void OnStartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void OnDisconnect()
    {
        connectUI.SetActive(true);
        preLobbyUI.SetActive(false);
        inLobbyUI.SetActive(false);
    }

    public void OnConnected()
    {
        connectUI.SetActive(false);
        preLobbyUI.SetActive(true);
        inLobbyUI.SetActive(false);
    }

    public void OnJoinLobby(int newPlayerId)
    {
        if (newPlayerId == Resident.Id)
        {
            connectUI.SetActive(false);
            preLobbyUI.SetActive(false);
            inLobbyUI.SetActive(true);

            lobbyIdLabel.text = $"Room Id:{Resident.Instance.Town.Id}";
        }
        else
        {

        }
        UpdateLobbyUI();
    }

    public void OnLeaveLobby(int leftResidentId)
    {
        if (leftResidentId == Resident.Id)
        {
            connectUI.SetActive(false);
            preLobbyUI.SetActive(true);
            inLobbyUI.SetActive(false);
        }
        else
        {

        }
        UpdateLobbyUI();
    }

    public void UpdateLobbyUI()
    {
        for (int i = 0; i < Resident.Instance.record.Town.Population; i++)
        {
            playerList.GetChild(i).gameObject.SetActive(true);
            Transform child = playerList.GetChild(i);
            child.GetComponentInChildren<TMP_Text>().text = Resident.Instance.record.Town.Residents[i].Username;
            foreach (Image image in child.GetComponentsInChildren<Image>())
            {
                image.color = PlayerColour.Get(Resident.Instance.record.Town.Residents[i].ColourId);
            }
        }
        if (Resident.Instance.record.Town.Population < playerList.childCount)
        {
            for (int i = Resident.Instance.record.Town.Population; i < playerList.childCount; i++)
            {
                playerList.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

}
