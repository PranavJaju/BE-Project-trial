//  // // using UnityEngine;

// // // public class RagdollController : MonoBehaviour 
// // // {
// // //     public BoxCollider MainCollider;
// // //     public GameObject thisGuysRig;
// // //     public Animator thisGuysAnimator;
    
// // //     private Collider[] ragdollColliders;
// // //     private Rigidbody[] limbRigidbodies;
// // //     private CharacterController characterController;
    
// // //     void Start() 
// // //     {
// // //         GetRagdollBits();
// // //         characterController = GetComponent<CharacterController>();
// // //         RagdollModeOff();
// // //     }
    
// // //     void Update() 
// // //     {
// // //         if (Input.GetKey(KeyCode.F)) 
// // //         {
// // //             EnableRagdoll();
// // //         }
// // //     }
    
// // //     void GetRagdollBits() 
// // //     {
// // //         // Get all colliders except the character controller
// // //         ragdollColliders = thisGuysRig.GetComponentsInChildren<Collider>();
// // //         limbRigidbodies = thisGuysRig.GetComponentsInChildren<Rigidbody>();
// // //     }
    
// // //     void EnableRagdoll() 
// // //     {
// // //         foreach (Collider col in ragdollColliders) 
// // //         {
// // //             // Only enable if it's not the character controller
// // //             if (!(col is CharacterController))
// // //             {
// // //                 col.enabled = true;
// // //             }
// // //         }
        
// // //         foreach (Rigidbody rb in limbRigidbodies) 
// // //         {
// // //             // Only modify if it's not the main rigidbody
// // //             if (rb != GetComponent<Rigidbody>())
// // //             {
// // //                 rb.isKinematic = false;
// // //             }
// // //         }
        
// // //         thisGuysAnimator.enabled = false;
// // //         MainCollider.enabled = false;
        
// // //         // Disable character controller during ragdoll
// // //         if (characterController != null)
// // //         {
// // //             characterController.enabled = false;
// // //         }
// // //     }
    
// // //     void RagdollModeOff() 
// // //     {
// // //         foreach (Collider col in ragdollColliders) 
// // //         {
// // //             // Only disable if it's not the character controller
// // //             if (!(col is CharacterController))
// // //             {
// // //                 col.enabled = false;
// // //             }
// // //         }
        
// // //         foreach (Rigidbody rb in limbRigidbodies) 
// // //         {
// // //             // Only modify if it's not the main rigidbody
// // //             if (rb != GetComponent<Rigidbody>())
// // //             {
// // //                 rb.isKinematic = true;
// // //             }
// // //         }
        
// // //         thisGuysAnimator.enabled = true;
// // //         MainCollider.enabled = true;
        
// // //         // Re-enable character controller when ragdoll is off
// // //         if (characterController != null)
// // //         {
// // //             characterController.enabled = true;
// // //         }
// // //     }
// // // }

// // using System.Collections;
// // using System.Collections.Generic;
// // using UnityEngine;

// // public class NewBehaviourScript : MonoBehaviour
// // {
// //     public Animator animator;
// //     public Rigidbody[] ragdollRigidbodies;
// //     public CharacterController characterController; // Reference to character controller
// //     private Vector3 originalPosition;
// //     private Quaternion originalRotation;
// //     private bool isRagdollActive = false;

// //     void Start()
// //     {
// //         // Get all rigidbodies in children
// //         ragdollRigidbodies = GetComponentsInChildren<Rigidbody>(true);
        
// //         // Get character controller if not assigned
// //         if (characterController == null)
// //         {
// //             characterController = GetComponent<CharacterController>();
// //         }
        
// //         // Get animator if not assigned
// //         if (animator == null)
// //         {
// //             animator = GetComponent<Animator>();
// //         }
        
// //         // Initialize all rigidbodies
// //         SetupRagdollRigidbodies();
        
// //         // Start with ragdoll disabled
// //         ToggleRagdoll(false);
// //     }

// //     void SetupRagdollRigidbodies()
// //     {
// //         foreach (var rb in ragdollRigidbodies)
// //         {
// //             // Configure each rigidbody
// //             rb.isKinematic = true;
// //             rb.useGravity = false;
            
// //             // Ignore collisions between character controller and ragdoll colliders
// //             Collider ragdollCollider = rb.GetComponent<Collider>();
// //             if (ragdollCollider != null && characterController != null)
// //             {
// //                 Physics.IgnoreCollision(ragdollCollider, characterController.GetComponent<Collider>(), true);
// //             }
// //         }
// //     }

// //     void ToggleRagdoll(bool isRagdoll)
// //     {
// //         isRagdollActive = isRagdoll;
        
// //         if (animator != null)
// //         {
// //             animator.enabled = !isRagdoll;
// //         }

// //         // Enable/disable character controller
// //         if (characterController != null)
// //         {
// //             characterController.enabled = !isRagdoll;
// //         }

// //         foreach (var rb in ragdollRigidbodies)
// //         {
// //             if (rb != null)
// //             {
// //                 rb.isKinematic = !isRagdoll;
// //                 rb.useGravity = isRagdoll;
                
// //                 // Get the collider for this rigidbody
// //                 // Collider col = rb.GetComponent<Collider>();
// //                 // if (col != null)
// //                 // {
// //                 //     col.enabled = isRagdoll;
// //                 // }
// //             }
// //         }
// //     }

// //     void OnCollisionEnter(Collision collision)
// //     {
// //         if (collision.gameObject.CompareTag("hit") && !isRagdollActive)
// //         {
// //             Debug.Log($"Hit detected with: {collision.gameObject.name}");
// //             ToggleRagdoll(true);
            
// //             // Optional: Add force to ragdoll based on collision
// //             if (collision.rigidbody != null)
// //             {
// //                 Vector3 force = collision.impulse * 10f; // Adjust multiplier as needed
// //                 foreach (var rb in ragdollRigidbodies)
// //                 {
// //                     rb.AddForce(force, ForceMode.Impulse);
// //                 }
// //             }
// //         }
// //     }

// //     void Update()
// //     {
// //         // Debug controls
// //         if (Input.GetKeyDown(KeyCode.P))
// //         {
// //             ToggleRagdoll(true);
// //         }
// //         if (Input.GetKeyDown(KeyCode.O))
// //         {
// //             ToggleRagdoll(false);
// //         }
// //     }
// // }


// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class NewBehaviourScript : MonoBehaviour
// {
//     public Animator animator;
//     public Rigidbody[] ragdollRigidbodies;
//     public CharacterController characterController;
//     public Transform hips; // Reference to the character's hips/pelvis
//     private Vector3 originalPosition;
//     private Quaternion originalRotation;
//     private bool isRagdollActive = false;

//     void Start()
//     {
//         // Get all rigidbodies in children
//         ragdollRigidbodies = GetComponentsInChildren<Rigidbody>(true);
        
//         // Get character controller if not assigned
//         if (characterController == null)
//         {
//             characterController = GetComponent<CharacterController>();
//         }
        
//         // Get animator if not assigned
//         if (animator == null)
//         {
//             animator = GetComponent<Animator>();
//         }

//         // Find hips if not assigned (usually the first rigidbody is the hips)
//         if (hips == null && ragdollRigidbodies.Length > 0)
//         {
//             hips = ragdollRigidbodies[0].transform;
//         }
        
//         // Initialize all rigidbodies
//         SetupRagdollRigidbodies();
        
//         // Start with ragdoll disabled
//         ToggleRagdoll(false);
//     }

//     void SetupRagdollRigidbodies()
//     {
//         foreach (var rb in ragdollRigidbodies)
//         {
//             // Configure each rigidbody
//             rb.isKinematic = true;
//             rb.useGravity = false;
            
//             // Ignore collisions between character controller and ragdoll colliders
//             Collider ragdollCollider = rb.GetComponent<Collider>();
//             if (ragdollCollider != null && characterController != null)
//             {
//                 Physics.IgnoreCollision(ragdollCollider, characterController.GetComponent<Collider>(), true);
//             }
//         }
//     }

//     void ToggleRagdoll(bool isRagdoll)
//     {
//         isRagdollActive = isRagdoll;
        
//         if (animator != null)
//         {
//             animator.enabled = !isRagdoll;
//         }

//         // Enable/disable character controller
//         if (characterController != null)
//         {
//             characterController.enabled = !isRagdoll;
//         }

//         foreach (var rb in ragdollRigidbodies)
//         {
//             if (rb != null)
//             {
//                 rb.isKinematic = !isRagdoll;
//                 rb.useGravity = isRagdoll;
//             }
//         }

//         // If we're turning ragdoll off, reset the character's position
//         if (!isRagdoll && hips != null)
//         {
//             // Get the position from the hips
//             Vector3 hipsPosition = hips.position;
            
//             // Adjust the character's position to match the hips
//             transform.position = new Vector3(hipsPosition.x, hipsPosition.y, hipsPosition.z);
            
//             // Reset the rotation to face upright
//             transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            
//             // Reset all ragdoll part rotations to their default poses
//             if (animator != null)
//             {
//                 animator.Rebind();
//                 animator.Update(0f);
//             }

//             // Ensure the character is grounded
//             transform.position += Vector3.up * 0.1f; // Slight lift to prevent ground clipping
//         }
//     }

//     void OnCollisionEnter(Collision collision)
//     {
//         if (collision.gameObject.CompareTag("hit") && !isRagdollActive)
//         {
//             Debug.Log($"Hit detected with: {collision.gameObject.name}");
//             ToggleRagdoll(true);
            
//             // Optional: Add force to ragdoll based on collision
//             if (collision.rigidbody != null)
//             {
//                 Vector3 force = collision.impulse * 10f; // Adjust multiplier as needed
//                 foreach (var rb in ragdollRigidbodies)
//                 {
//                     rb.AddForce(force, ForceMode.Impulse);
//                 }
//             }
//         }
//     }

//     void Update()
//     {
//         // Debug controls
//         if (Input.GetKeyDown(KeyCode.P))
//         {
//             ToggleRagdoll(true);
//         }
//         if (Input.GetKeyDown(KeyCode.O))
//         {
//             ToggleRagdoll(false);
//         }
//     }
// }
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerScript : MonoBehaviour
{
    public Animator animator;
    public Rigidbody[] ragdollRigidbodies;
    public CharacterController characterController;
    public Transform hips; // Reference to the character's hips/pelvis
    private bool isRagdollActive = false;
    public float moveSpeed = 5f;
    public float turnSpeed = 360f;

    void Start()
    {
        // Get all rigidbodies in children
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>(true);
        
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (hips == null && ragdollRigidbodies.Length > 0)
        {
            hips = ragdollRigidbodies[0].transform;
        }
        
        SetupRagdollRigidbodies();
        ToggleRagdoll(false);
    }

    void SetupRagdollRigidbodies()
    {
        foreach (var rb in ragdollRigidbodies)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            
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

        if (!isRagdoll && hips != null)
        {
            Vector3 hipsPosition = hips.position;
            transform.position = new Vector3(hipsPosition.x, hipsPosition.y, hipsPosition.z);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

            if (animator != null)
            {
                animator.Rebind();
                animator.Update(0f);
            }

            transform.position += Vector3.up * 0.1f;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("hit") && !isRagdollActive)
        {
            Debug.Log($"Hit detected with: {collision.gameObject.name}");
            ToggleRagdoll(true);

            if (collision.rigidbody != null)
            {
                Vector3 force = collision.impulse * 10f;
                foreach (var rb in ragdollRigidbodies)
                {
                    rb.AddForce(force, ForceMode.Impulse);
                }
            }
        }
    }

    void Update()
    {
        if (isRagdollActive) return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSpeed, 0.1f);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            characterController.Move(moveDirection * moveSpeed * Time.deltaTime);

            // Set animation parameters
            animator.SetFloat("Speed", Mathf.Abs(vertical));
            animator.SetFloat("Direction", horizontal);
        }
        else
        {
            animator.SetFloat("Speed", 0);
            animator.SetFloat("Direction", 0);
        }

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

