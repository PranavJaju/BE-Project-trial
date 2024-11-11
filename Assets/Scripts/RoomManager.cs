using UnityEngine;
using Photon.Pun;
using Photon.Realtime; // Add this for RoomOptions

public class RoomManager : MonoBehaviourPunCallbacks
{
    public GameObject player;
    
    [Space]
    public Transform spawnPoint;

    void Start()
    {
        Debug.Log("Connecting...");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Connected to Server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        // Create room options if needed
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4; // Set your desired max players
        PhotonNetwork.JoinOrCreateRoom("test", roomOptions, TypedLobby.Default);
        Debug.Log("Attempting to join or create room...");
    }

    // Add this callback
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Joined Room Successfully");
        GameObject _player = PhotonNetwork.Instantiate(player.name, spawnPoint.position, Quaternion.identity);
    }

    // Optionally add error handling
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Room creation failed: {message}");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Room joining failed: {message}");
    }
}