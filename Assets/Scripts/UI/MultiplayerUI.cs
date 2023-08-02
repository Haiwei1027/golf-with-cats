using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
        Resident.onConnected += OnConnected;
        Resident.onDisconnected += OnDisconnected;
        Resident.onJoinTown += OnJoinTown;
        Resident.onLeaveTown += OnLeaveTown;
    }
    public void OnConnected()
    {
        connectPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        townPanel.SetActive(false);
    }

    public void OnDisconnected()
    {
        connectPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        townPanel.SetActive(false);
    }

    public void OnJoinTown(int newResidentID)
    {
        connectPanel.SetActive(false);
        lobbyPanel.SetActive(false);
        UpdateNames(Resident.Instance.town.Residents);
        roomIdLabel.text = $"Room ID: {Resident.Instance.town.Id}";
        townPanel.SetActive(true);
    }

    public void OnLeaveTown()
    {
        connectPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        townPanel.SetActive(false);
    }
}
