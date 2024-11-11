using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Animator animator;
    public Rigidbody[] ragdollRigidbodies;
    public CharacterController characterController;
    public Transform hips; // Reference to the character's hips/pelvis
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isRagdollActive = false;

    void Start()
    {
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
        if (collision.gameObject.CompareTag("hit") && !isRagdollActive)
        {
            Debug.Log($"Hit detected with: {collision.gameObject.name}");
            ToggleRagdoll(true);
            
            // Optional: Add force to ragdoll based on collision
            if (collision.rigidbody != null)
            {
                Vector3 force = collision.impulse * 10f; // Adjust multiplier as needed
                foreach (var rb in ragdollRigidbodies)
                {
                    rb.AddForce(force, ForceMode.Impulse);
                }
            }
        }
    }

    void Update()
    {
        // Debug controls
        if (Input.GetKeyDown(KeyCode.P))
        {
            ToggleRagdoll(true);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            ToggleRagdoll(false);
        }
    }
}