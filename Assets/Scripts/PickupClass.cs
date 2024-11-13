// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class PickupClass : MonoBehaviour
// {
//     [SerializeField] private LayerMask PickupLayer;
//     [SerializeField] private GameObject PlayerCamera;
//     [SerializeField] private float PickupRange;
//     [SerializeField] private Transform Hand;
//     [SerializeField] private float ThrowingForce;

//     private Rigidbody CurrentObjectRigidBody;
//     private Collider CurrentObjectCollider;

//     void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.E))
//         {
//             Ray Pickupray = new Ray(PlayerCamera.transform.position, PlayerCamera.transform.forward);
//             // if (Physics.Raycast(Pickupray, out RaycastHit hitInfo, PickupRange, PickupLayer))
//             // {
//             //     if (CurrentObjectRigidBody)
//             //     {
//             //         // Code to handle picking up and dropping objects
//             //     }
//             //     else
//             //     {
//             //         CurrentObjectRigidBody = hitInfo.rigidbody;
//             //         CurrentObjectCollider = hitInfo.collider;

//             //         CurrentObjectRigidBody.isKinematic = true;
//             //         CurrentObjectCollider.enabled = false;
//             //     }
//             // }
//             if (Physics.Raycast(Pickupray, out RaycastHit hitInfo, PickupRange, PickupLayer))
//             {
//                 if (CurrentObjectRigidBody)
//                 {
//                     CurrentObjectRigidBody.isKinematic = false;
//                     CurrentObjectCollider.enabled = true;
                    
//                     CurrentObjectRigidBody = hitInfo.rigidbody;
//                     CurrentObjectCollider = hitInfo.collider;
                    
//                     CurrentObjectRigidBody.isKinematic = true;
//                     CurrentObjectCollider.enabled = false;
//                 }
//                 else
//                 {
//                     CurrentObjectRigidBody = hitInfo.rigidbody;
//                     CurrentObjectCollider = hitInfo.collider;
                    
//                     CurrentObjectRigidBody.isKinematic = true;
//                     CurrentObjectCollider.enabled = false;
//                 }
//                 return;
//             }
//             if(CurrentObjectRigidBody){
//                 CurrentObjectRigidBody.isKinematic = false;
//                 CurrentObjectCollider.enabled = true;
//                 CurrentObjectRigidBody = null;
//                 CurrentObjectCollider = null;
//             }
//         }
//         if(Input.GetKeyDown(KeyCode.Q)){
//             if(CurrentObjectRigidBody){
//                 CurrentObjectRigidBody.isKinematic = false;
//                 CurrentObjectCollider.enabled = true;
//                 CurrentObjectRigidBody.AddForce(PlayerCamera.transform.forward*ThrowingForce,ForceMode.Impulse);
//                 CurrentObjectRigidBody = null;
//                 CurrentObjectCollider = null;

//             }
//         }
//         if(CurrentObjectRigidBody){
//             CurrentObjectRigidBody.position = Hand.position;
//             CurrentObjectRigidBody.rotation = Hand.rotation;

//         }
//     }
// }


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // Add Photon namespace

public class PickupClass : MonoBehaviourPunCallbacks // Inherit from PunCallbacks instead of MonoBehaviour
{
    [SerializeField] private LayerMask PickupLayer;
    [SerializeField] private GameObject PlayerCamera;
    [SerializeField] private float PickupRange;
    [SerializeField] private Transform Hand;
    [SerializeField] private float ThrowingForce;
    private Rigidbody CurrentObjectRigidBody;
    private Collider CurrentObjectCollider;
    private PhotonView photonView; // Add PhotonView reference

    void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    void Update()
    {
        // Only process input for the local player
        if (!photonView.IsMine) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray Pickupray = new Ray(PlayerCamera.transform.position, PlayerCamera.transform.forward);
            if (Physics.Raycast(Pickupray, out RaycastHit hitInfo, PickupRange, PickupLayer))
            {
                if (CurrentObjectRigidBody)
                {
                    photonView.RPC("DropAndPickup", RpcTarget.All, hitInfo.collider.gameObject.GetComponent<PhotonView>().ViewID);
                }
                else
                {
                    photonView.RPC("Pickup", RpcTarget.All, hitInfo.collider.gameObject.GetComponent<PhotonView>().ViewID);
                }
                return;
            }
            if(CurrentObjectRigidBody)
            {
                photonView.RPC("Drop", RpcTarget.All);
            }
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            if(CurrentObjectRigidBody)
            {
                photonView.RPC("Throw", RpcTarget.All, PlayerCamera.transform.forward);
            }
        }

        if(CurrentObjectRigidBody)
        {
            photonView.RPC("UpdatePosition", RpcTarget.All);
        }
    }

    [PunRPC]
    private void Pickup(int objectViewID)
    {
        GameObject pickupObject = PhotonView.Find(objectViewID).gameObject;
        CurrentObjectRigidBody = pickupObject.GetComponent<Rigidbody>();
        CurrentObjectCollider = pickupObject.GetComponent<Collider>();
        
        CurrentObjectRigidBody.isKinematic = true;
        CurrentObjectCollider.enabled = false;
    }

    [PunRPC]
    private void Drop()
    {
        CurrentObjectRigidBody.isKinematic = false;
        CurrentObjectCollider.enabled = true;
        CurrentObjectRigidBody = null;
        CurrentObjectCollider = null;
    }

    [PunRPC]
    private void DropAndPickup(int newObjectViewID)
    {
        CurrentObjectRigidBody.isKinematic = false;
        CurrentObjectCollider.enabled = true;
        
        GameObject pickupObject = PhotonView.Find(newObjectViewID).gameObject;
        CurrentObjectRigidBody = pickupObject.GetComponent<Rigidbody>();
        CurrentObjectCollider = pickupObject.GetComponent<Collider>();
        
        CurrentObjectRigidBody.isKinematic = true;
        CurrentObjectCollider.enabled = false;
    }

    [PunRPC]
    private void Throw(Vector3 direction)
    {
        CurrentObjectRigidBody.isKinematic = false;
        CurrentObjectCollider.enabled = true;
        CurrentObjectRigidBody.AddForce(direction * ThrowingForce, ForceMode.Impulse);
        CurrentObjectRigidBody = null;
        CurrentObjectCollider = null;
    }

    [PunRPC]
    private void UpdatePosition()
    {
        CurrentObjectRigidBody.position = Hand.position;
        CurrentObjectRigidBody.rotation = Hand.rotation;
    }
}