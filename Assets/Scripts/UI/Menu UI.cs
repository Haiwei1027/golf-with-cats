using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
        Resident.onConnected += OnConnected;
        Resident.onDisconnected += OnDisconnect;
        Resident.onJoinTown += OnJoinLobby;
        Resident.onLeaveTown += OnLeaveLobby;
        Resident.onStartGame += OnStartGame;
    }

    private void Start()
    {
        Initialise();

        Listen();
        Resident.Disconnect();
    }

    public void OnStartGame()
    {

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
        }
        else
        {

        }
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

    public void OnLeaveLobby()
    {
        connectUI.SetActive(false);
        preLobbyUI.SetActive(true);
        inLobbyUI.SetActive(false);
    }

}
