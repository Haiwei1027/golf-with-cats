using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MultiplayerUI : MonoBehaviour
{
    public static MultiplayerUI Instance { get; private set; }

    [Header("Connect UI")]
    [SerializeField] GameObject connectPanel;
    [Header("Lobby UI")]
    [SerializeField] GameObject lobbyPanel;
    [Header("Town UI")]
    [SerializeField] GameObject townPanel;
    [SerializeField] TMP_Text usernameList;

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

    public void Start()
    {
        OnDisconnected();
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

    public void OnJoinTown()
    {
        connectPanel.SetActive(false);
        lobbyPanel.SetActive(false);
        townPanel.SetActive(true);
    }

    public void OnLeaveTown()
    {
        connectPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        townPanel.SetActive(false);
    }
}
