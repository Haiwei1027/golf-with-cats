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
    }

    private void Start()
    {
        Initialise();

        Listen();
        Resident.Disconnect();
    }

    public void OnDisconnect()
    {

    }

    public void OnConnected()
    {

    }

    public void OnJoinLobby(int newPlayerId)
    {
        if (newPlayerId == Resident.Id)
        {

        }
        else
        {

        }
    }

    public void OnLeaveLobby()
    {

    }

}
