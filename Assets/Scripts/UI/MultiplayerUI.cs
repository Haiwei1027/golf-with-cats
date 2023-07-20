using UnityEngine;

public class MultiplayerUI : MonoBehaviour
{
    [SerializeField] GameObject connectPanel;
    [SerializeField] GameObject lobbyPanel;
    [SerializeField] GameObject townPanel;


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
