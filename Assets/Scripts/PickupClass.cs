// using UnityEngine;
// using Photon.Pun;
// using System.Collections;

// public class PickupClass : MonoBehaviourPunCallbacks
// {
//     [Header("Pickup Settings")]
//     [SerializeField] private LayerMask PickupLayer;
//     [SerializeField] private GameObject PlayerCamera;
//     [SerializeField] private float PickupRange = 3f;
//     [SerializeField] private Transform Hand;
//     [SerializeField] private float ThrowingForce = 10f;
//     [SerializeField] private float KnockbackForce = 10f;
//     [SerializeField] private float KnockbackUpwardForce = 2f;

//     [Header("Animation")]
//     [SerializeField] private PickupAnimationController animationController;

//     private Rigidbody CurrentObjectRigidBody;
//     private Collider CurrentObjectCollider;
//     private PhotonView photonView;
//     private bool isThrown = false;

//     public event System.Action OnObjectPickup;
//     public event System.Action OnObjectDrop;
//     public event System.Action OnObjectThrow;

//     void Start()
//     {
//         photonView = GetComponent<PhotonView>();

//         if (animationController == null)
//         {
//             animationController = GetComponent<PickupAnimationController>();
//         }

//         // Ensure we have all required components
//         if (PlayerCamera == null)
//         {
//             PlayerCamera = Camera.main.gameObject;
//         }
//     }

//     void Update()
//     {
//         if (!photonView.IsMine) return;

//         // Pickup/Drop Logic
//         if (Input.GetKeyDown(KeyCode.E))
//         {
//             Ray Pickupray = new Ray(PlayerCamera.transform.position, PlayerCamera.transform.forward);
//             if (Physics.Raycast(Pickupray, out RaycastHit hitInfo, PickupRange, PickupLayer))
//             {
//                 PhotonView objectView = hitInfo.collider.gameObject.GetComponent<PhotonView>();
//                 if (objectView != null)
//                 {
//                     if (CurrentObjectRigidBody)
//                     {
//                         photonView.RPC("DropAndPickup", RpcTarget.All, objectView.ViewID);
//                     }
//                     else
//                     {
//                         photonView.RPC("Pickup", RpcTarget.All, objectView.ViewID);
//                     }
//                 }
//                 return;
//             }
//             if (CurrentObjectRigidBody)
//             {
//                 photonView.RPC("Drop", RpcTarget.All);
//             }
//         }

//         // Throw Logic
//         if (Input.GetKeyDown(KeyCode.Q) && CurrentObjectRigidBody)
//         {
//             photonView.RPC("Throw", RpcTarget.All, PlayerCamera.transform.forward);
//         }

//         // Update held object position
//         if (CurrentObjectRigidBody && !isThrown)
//         {
//             UpdateObjectPosition();
//         }
//     }

//     private void UpdateObjectPosition()
//     {
//         if (photonView.IsMine)
//         {
//             CurrentObjectRigidBody.MovePosition(Hand.position);
//             CurrentObjectRigidBody.MoveRotation(Hand.rotation);
//         }
//     }

//     [PunRPC]
//     private void Pickup(int objectViewID)
//     {
//         GameObject pickupObject = PhotonView.Find(objectViewID).gameObject;
//         CurrentObjectRigidBody = pickupObject.GetComponent<Rigidbody>();
//         CurrentObjectCollider = pickupObject.GetComponent<Collider>();

//         CurrentObjectRigidBody.isKinematic = true;
//         CurrentObjectCollider.enabled = false;
//         isThrown = false;

//         if (animationController != null)
//         {
//             animationController.HandlePickupAnimation();
//         }

//         OnObjectPickup?.Invoke();
//     }

//     [PunRPC]
//     private void Drop()
//     {
//         if (CurrentObjectRigidBody != null)
//         {
//             CurrentObjectRigidBody.isKinematic = false;
//             CurrentObjectCollider.enabled = true;
//             CurrentObjectRigidBody = null;
//             CurrentObjectCollider = null;
//             isThrown = false;

//             if (animationController != null)
//             {
//                 animationController.HandleDropAnimation();
//             }

//             OnObjectDrop?.Invoke();
//         }
//     }

//     [PunRPC]
//     private void Throw(Vector3 direction)
//     {
//         if (CurrentObjectRigidBody != null)
//         {
//             CurrentObjectRigidBody.isKinematic = false;
//             CurrentObjectCollider.enabled = true;
//             CurrentObjectRigidBody.AddForce(direction * ThrowingForce, ForceMode.Impulse);
//             isThrown = true;

//             if (animationController != null)
//             {
//                 animationController.HandleThrowAnimation();
//             }

//             OnObjectThrow?.Invoke();
//             StartCoroutine(ClearObjectAfterDelay());
//         }
//     }

//     private IEnumerator ClearObjectAfterDelay()
//     {
//         yield return new WaitForSeconds(0.1f);
//         CurrentObjectRigidBody = null;
//         CurrentObjectCollider = null;
//         isThrown = false;
//     }
// }


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PickupClass : MonoBehaviourPunCallbacks
{
    [Header("Pickup Settings")]
    [SerializeField] private LayerMask PickupLayer;
    [SerializeField] private GameObject PlayerCamera;
    [SerializeField] private float PickupRange;
    [SerializeField] private Transform Hand;
    [SerializeField] private float ThrowingForce;

    [Header("Knockback Settings")]
    [SerializeField] private float KnockbackForce = 10f;
    [SerializeField] private float KnockbackUpwardForce = 2f;

    [Header("Animation Settings")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private float pickupAnimationDuration = 1f;
    [SerializeField] private float throwAnimationDuration = 0.5f;
    [SerializeField] private float transitionDuration = 0.1f;

    [Header("Animation State Names")]
    [SerializeField] private string pickupStateName = "pickup";
    [SerializeField] private string throwStateName = "throw";
    [SerializeField] private string fallStateName = "fall";
    [SerializeField] private string idleStateName = "idle";

    private int pickupStateHash;
    private int throwStateHash;
    private int fallStateHash;
    private int idleStateHash;

    private Rigidbody CurrentObjectRigidBody;
    private Collider CurrentObjectCollider;
    private PhotonView photonView;
    private bool isThrown = false;
    private bool isAnimating = false;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (!playerAnimator)
        {
            playerAnimator = GetComponent<Animator>();
        }

        // Cache animation state hashes
        pickupStateHash = Animator.StringToHash(pickupStateName);
        throwStateHash = Animator.StringToHash(throwStateName);
        fallStateHash = Animator.StringToHash(fallStateName);
        idleStateHash = Animator.StringToHash(idleStateName);
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        if (Input.GetKeyDown(KeyCode.E) && !isAnimating)
        {
            Ray Pickupray = new Ray(PlayerCamera.transform.position, PlayerCamera.transform.forward);
            if (Physics.Raycast(Pickupray, out RaycastHit hitInfo, PickupRange, PickupLayer))
            {
                if (CurrentObjectRigidBody)
                {
                    StartCoroutine(PickupAnimationSequence(hitInfo.collider.gameObject.GetComponent<PhotonView>().ViewID, true));
                }
                else
                {
                    StartCoroutine(PickupAnimationSequence(hitInfo.collider.gameObject.GetComponent<PhotonView>().ViewID, false));
                }
                return;
            }
            if (CurrentObjectRigidBody)
            {
                photonView.RPC("Drop", RpcTarget.All);
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) && !isAnimating)
        {
            if (CurrentObjectRigidBody)
            {
                StartCoroutine(ThrowAnimationSequence());
            }
        }

        if (CurrentObjectRigidBody && !isThrown)
        {
            photonView.RPC("UpdatePosition", RpcTarget.All);
        }
    }

    private IEnumerator PickupAnimationSequence(int objectViewID, bool isDropAndPickup)
    {
        isAnimating = true;

        // Play pickup animation
        photonView.RPC("PlayPickupAnimation", RpcTarget.All);

        // Wait for animation
        yield return new WaitForSeconds(pickupAnimationDuration);

        // Return to idle
        photonView.RPC("PlayIdleAnimation", RpcTarget.All);

        // Perform actual pickup
        if (isDropAndPickup)
        {
            photonView.RPC("DropAndPickup", RpcTarget.All, objectViewID);
        }
        else
        {
            photonView.RPC("Pickup", RpcTarget.All, objectViewID);
        }

        isAnimating = false;
    }

    private IEnumerator ThrowAnimationSequence()
    {
        isAnimating = true;

        // Play throw animation
        photonView.RPC("PlayThrowAnimation", RpcTarget.All);

        // Wait for animation
        yield return new WaitForSeconds(throwAnimationDuration);

        // Return to idle
        photonView.RPC("PlayIdleAnimation", RpcTarget.All);

        // Perform actual throw
        photonView.RPC("Throw", RpcTarget.All, PlayerCamera.transform.forward);

        isAnimating = false;
    }

    [PunRPC]
    private void PlayPickupAnimation()
    {
        if (playerAnimator)
        {

            playerAnimator.CrossFade(pickupStateHash, transitionDuration);
            playerAnimator.SetTrigger("PickupTrigger");
        }
    }

    [PunRPC]
    private void PlayThrowAnimation()
    {
        if (playerAnimator)
        {

            playerAnimator.CrossFade(throwStateHash, transitionDuration);
            playerAnimator.SetTrigger("ThrowTrigger");
        }
    }

    [PunRPC]
    private void PlayFallAnimation()
    {
        if (playerAnimator)
        {
            playerAnimator.CrossFade(fallStateHash, transitionDuration);
            StartCoroutine(ReturnToIdleAfterDelay(1f));
            playerAnimator.SetTrigger("fall"); // Adjust delay as needed
        }
    }

    [PunRPC]
    private void PlayIdleAnimation()
    {
        if (playerAnimator)
        {
            playerAnimator.CrossFade(idleStateHash, transitionDuration);
        }
    }

    private IEnumerator ReturnToIdleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayIdleAnimation();
    }

    [PunRPC]
    private void Pickup(int objectViewID)
    {
        GameObject pickupObject = PhotonView.Find(objectViewID).gameObject;
        CurrentObjectRigidBody = pickupObject.GetComponent<Rigidbody>();
        CurrentObjectCollider = pickupObject.GetComponent<Collider>();

        if (!CurrentObjectRigidBody.gameObject.GetComponent<ThrowableCollisionHandler>())
        {
            CurrentObjectRigidBody.gameObject.AddComponent<ThrowableCollisionHandler>().Initialize(this);
        }

        CurrentObjectRigidBody.isKinematic = true;
        CurrentObjectCollider.enabled = false;
        isThrown = false;
    }

    [PunRPC]
    private void Drop()
    {
        CurrentObjectRigidBody.isKinematic = false;
        CurrentObjectCollider.enabled = true;
        CurrentObjectRigidBody = null;
        CurrentObjectCollider = null;
        isThrown = false;
    }

    [PunRPC]
    private void DropAndPickup(int newObjectViewID)
    {
        CurrentObjectRigidBody.isKinematic = false;
        CurrentObjectCollider.enabled = true;

        GameObject pickupObject = PhotonView.Find(newObjectViewID).gameObject;
        CurrentObjectRigidBody = pickupObject.GetComponent<Rigidbody>();
        CurrentObjectCollider = pickupObject.GetComponent<Collider>();

        if (!CurrentObjectRigidBody.gameObject.GetComponent<ThrowableCollisionHandler>())
        {
            CurrentObjectRigidBody.gameObject.AddComponent<ThrowableCollisionHandler>().Initialize(this);
        }

        CurrentObjectRigidBody.isKinematic = true;
        CurrentObjectCollider.enabled = false;
        isThrown = false;
    }

    [PunRPC]
    private void Throw(Vector3 direction)
    {
        CurrentObjectRigidBody.isKinematic = false;
        CurrentObjectCollider.enabled = true;
        CurrentObjectRigidBody.AddForce(direction * ThrowingForce, ForceMode.Impulse);
        isThrown = true;

        StartCoroutine(ClearObjectAfterDelay());
    }

    private IEnumerator ClearObjectAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);
        CurrentObjectRigidBody = null;
        CurrentObjectCollider = null;
        isThrown = false;
    }

    [PunRPC]
    private void UpdatePosition()
    {
        CurrentObjectRigidBody.position = Hand.position;
        CurrentObjectRigidBody.rotation = Hand.rotation;
    }

    [PunRPC]
    public void ApplyKnockback(int hitPlayerViewID, Vector3 hitDirection)
    {
        PhotonView hitPlayerView = PhotonView.Find(hitPlayerViewID);
        if (hitPlayerView != null)
        {
            // Play fall animation on the hit player
            Animator hitPlayerAnimator = hitPlayerView.gameObject.GetComponent<Animator>();
            if (hitPlayerAnimator)
            {
                hitPlayerView.RPC("PlayFallAnimation", RpcTarget.All);
            }

            Rigidbody playerRb = hitPlayerView.gameObject.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                Vector3 knockbackDirection = (hitDirection + Vector3.up * KnockbackUpwardForce).normalized;
                playerRb.AddForce(knockbackDirection * KnockbackForce, ForceMode.Impulse);
            }
        }
    }
}

public class ThrowableCollisionHandler : MonoBehaviour
{
    private PickupClass pickupClass;
    private bool hasCollided = false;

    public void Initialize(PickupClass pickup)
    {
        pickupClass = pickup;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasCollided) return;

        PhotonView hitPlayerView = collision.gameObject.GetComponent<PhotonView>();
        if (hitPlayerView != null && pickupClass != null)
        {
            Vector3 hitDirection = (collision.contacts[0].point - transform.position).normalized;
            pickupClass.photonView.RPC("ApplyKnockback", RpcTarget.All, hitPlayerView.ViewID, hitDirection);
            hasCollided = true;
        }
    }
}