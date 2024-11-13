using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RagDollController: MonoBehaviourPunCallbacks  // Changed class name from NewBehaviourScript
{
    public Animator animator;
    public Rigidbody[] ragdollRigidbodies;
    public CharacterController characterController;
    public Transform hips;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isRagdollActive = false;
    private PhotonView photonView;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        
        // Get all rigidbodies in children
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>(true);
        
        // Get character controller if not assigned
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }
        
        // Get animator if not assigned
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // Find hips if not assigned (usually the first rigidbody is the hips)
        if (hips == null && ragdollRigidbodies.Length > 0)
        {
            hips = ragdollRigidbodies[0].transform;
        }
        
        // Initialize all rigidbodies
        SetupRagdollRigidbodies();
        // Start with ragdoll disabled
        ToggleRagdoll(false);
    }

    void SetupRagdollRigidbodies()
    {
        foreach (var rb in ragdollRigidbodies)
        {
            // Configure each rigidbody
            rb.isKinematic = true;
            rb.useGravity = false;
            
            // Set ownership for network synchronization
            if (rb.gameObject.GetComponent<PhotonView>() == null)
            {
                rb.gameObject.AddComponent<PhotonView>();
            }
            
            // Ignore collisions between character controller and ragdoll colliders
            Collider ragdollCollider = rb.GetComponent<Collider>();
            if (ragdollCollider != null && characterController != null)
            {
                Physics.IgnoreCollision(ragdollCollider, characterController.GetComponent<Collider>(), true);
            }
        }
    }

    void ToggleRagdoll(bool isRagdoll)
    {
        if (!photonView.IsMine) return; // Only the owner can toggle ragdoll state
        
        photonView.RPC("RPC_ToggleRagdoll", RpcTarget.All, isRagdoll);
    }

    [PunRPC]
    private void RPC_ToggleRagdoll(bool isRagdoll)
    {
        isRagdollActive = isRagdoll;
        
        if (animator != null)
        {
            animator.enabled = !isRagdoll;
        }

        // Enable/disable character controller
        if (characterController != null)
        {
            characterController.enabled = !isRagdoll;
        }

        foreach (var rb in ragdollRigidbodies)
        {
            if (rb != null)
            {
                rb.isKinematic = !isRagdoll;
                rb.useGravity = isRagdoll;
                
                // Transfer ownership of rigidbody to the local player when ragdolling
                PhotonView rbView = rb.gameObject.GetComponent<PhotonView>();
                if (rbView != null && isRagdoll && photonView.IsMine)
                {
                    rbView.RequestOwnership();
                }
            }
        }

        // If we're turning ragdoll off, reset the character's position
        if (!isRagdoll && hips != null)
        {
            // Get the position from the hips
            Vector3 hipsPosition = hips.position;
            
            // Adjust the character's position to match the hips
            transform.position = new Vector3(hipsPosition.x, hipsPosition.y, hipsPosition.z);
            
            // Reset the rotation to face upright
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            
            // Reset all ragdoll part rotations to their default poses
            if (animator != null)
            {
                animator.Rebind();
                animator.Update(0f);
            }

            // Ensure the character is grounded
            transform.position += Vector3.up * 0.1f; // Slight lift to prevent ground clipping
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!photonView.IsMine) return; // Only process collisions for the local player
        
        if (collision.gameObject.CompareTag("hit") && !isRagdollActive)
        {
            Debug.Log($"Hit detected with: {collision.gameObject.name}");
            ToggleRagdoll(true);
            
            // Optional: Add force to ragdoll based on collision
            if (collision.rigidbody != null)
            {
                Vector3 force = collision.impulse * 10f; // Adjust multiplier as needed
                photonView.RPC("RPC_ApplyRagdollForce", RpcTarget.All, force);
            }
        }
    }

    [PunRPC]
    private void RPC_ApplyRagdollForce(Vector3 force)
    {
        if (!isRagdollActive) return;
        
        foreach (var rb in ragdollRigidbodies)
        {
            if (rb != null)
            {
                rb.AddForce(force, ForceMode.Impulse);
            }
        }
    }

    void Update()
    {
        // Only process input for the local player
        if (!photonView.IsMine) return;
        
        // Debug controls for ragdoll toggling
        if (Input.GetKeyDown(KeyCode.P))
        {
            ToggleRagdoll(true);
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            ToggleRagdoll(false);
        }
    }
}