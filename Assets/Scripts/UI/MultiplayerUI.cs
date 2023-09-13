using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Class responsible for updating the multiplayer UI in response to events
/// </summary>
public class MultiplayerUI : MonoBehaviour
{
    public static MultiplayerUI Instance { get; private set; }

    [Header("Connect UI")]
    [SerializeField] GameObject connectPanel;
    [SerializeField] TMP_InputField usernameField;
    [Header("Lobby UI")]
    [SerializeField] GameObject lobbyPanel;
    [SerializeField] TMP_InputField townField;
    [Header("Town UI")]
    [SerializeField] GameObject townPanel;
    [SerializeField] TMP_Text usernameList;
    [SerializeField] TMP_Text roomIdLabel;

    private void UpdateNames(List<ResidentRecord> residents)
    {
        usernameList.text = "";
        foreach (ResidentRecord resident in residents)
        {
            usernameList.text += resident.Username + "\n";
        }
    }

    public void Awake()
    {
        Instance = this;
    }

    public void Connect()
    {
        Resident.Connect(usernameField.text);
    }

    public void JoinTown()
    {
        Resident.JoinTown(townField.text);
    }

    public void Start()
    {
        OnDisconnected();
        Resident.Instance.LetterHandler.onConnected += OnConnected;
        Resident.Instance.LetterHandler.onDisconnected += OnDisconnected;
        Resident.Instance.LetterHandler.onJoinTown += OnJoinTown;
        Resident.Instance.LetterHandler.onLeaveTown += OnLeaveTown;
        Resident.Instance.LetterHandler.onStartGame += OnStartGame;
    }

    public void OnConnected()
    {
        connectPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        townPanel.SetActive(false);
    }

    public void OnDisconnected()
    {
        Debug.Log("Disconnected");
        connectPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        townPanel.SetActive(false);
    }

    public void OnJoinTown(int newResidentID)
    {
        connectPanel.SetActive(false);
        lobbyPanel.SetActive(false);
        UpdateNames(Resident.Instance.Town.Residents);
        roomIdLabel.text = $"Room ID: {Resident.Instance.Town.Id}";
        townPanel.SetActive(true);

        if (newResidentID == Resident.Instance.record.Id)
        {
            // self join
            
        }
        else
        {
            // another player

        }

    }

    public void OnLeaveTown(int leftResidenId)
    {
        if (leftResidenId == Resident.Id)
        {
            connectPanel.SetActive(false);
            lobbyPanel.SetActive(true);
            townPanel.SetActive(false);
        }
        else
        {

        }
    }

    public void OnStartGame()
    {
        connectPanel.SetActive(false);
        lobbyPanel.SetActive(false);
        townPanel.SetActive(false);
    }
}
