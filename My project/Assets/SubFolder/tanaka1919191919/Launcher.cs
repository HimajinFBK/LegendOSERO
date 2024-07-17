using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField]TextMeshProUGUI message;
    [SerializeField] GameObject lobbyPanel,roomPanel;
    [SerializeField] TMP_InputField createRoomNameImput, joinRoomInput;
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        message.text = "Loading...";
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("connected to master");
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        lobbyPanel.SetActive(true);
        message.text = "Lobby";
    }
    public void CreteRoom()
    {
        PhotonNetwork.CreateRoom(createRoomNameImput.text);
        message.text = "Loading...";
    }
    public override void OnJoinedRoom()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        message.text = PhotonNetwork.CurrentRoom.Name;
    }
    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinRoomInput.text);
    }
    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }
}
