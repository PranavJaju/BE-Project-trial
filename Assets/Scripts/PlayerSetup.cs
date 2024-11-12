// using UnityEngine;
// using Photon.Pun;

// public class PlayerSetup : MonoBehaviourPunCallbacks
// {
//     public PlayerMovement movement;
//     public GameObject playerCamera;
//     public NetworkedRagdoll ragdollController; // Add reference to ragdoll controller
//     private PhotonView photonView;

//     void Awake()
//     {
//         photonView = GetComponent<PhotonView>();
        
//         // Disable everything by default
//         movement.enabled = false;
//         if (playerCamera != null)
//             playerCamera.SetActive(false);
            
//         // If this is our local player, enable controls
//         if (photonView.IsMine)
//         {
//             IsLocalPlayer();
//         }
//         else
//         {
//             // For non-local players, make sure their ragdoll can still be activated
//             if (ragdollController != null)
//             {
//                 ragdollController.enabled = true;
//             }
//         }
//     }

//     public void IsLocalPlayer()
//     {
//         movement.enabled = true;
//         if (playerCamera != null)
//             playerCamera.SetActive(true);
//         if (ragdollController != null)
//             ragdollController.enabled = true;
//     }
// }


using UnityEngine;
using Photon.Pun;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public PlayerMovement movement;
    public GameObject playerCamera;
    private PhotonView photonView;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        
        // Disable movement and camera by default
        movement.enabled = false;
        if (playerCamera != null)
            playerCamera.SetActive(false);

        // If this is our local player, enable controls
        if (photonView.IsMine)
        {
            IsLocalPlayer();
        }
    }

    public void IsLocalPlayer()
    {
        movement.enabled = true;
        if (playerCamera != null)
            playerCamera.SetActive(true);
    }
}