// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// [RequireComponent(typeof(CharacterController))]
// public class PlayerMovement : MonoBehaviour
// {
//     public Camera playerCamera;
//     public float walkSpeed = 6f;
//     public float runSpeed = 12f;
//     public float jumpPower = 10f;
//     public float gravity = 10f;
//     public float lookSpeed = 2f;
//     public float lookXLimit = 45f;
//     public float defaultHeight = 2f;
//     public float crouchHeight = 1f;
//     public float crouchSpeed = 3f;
//     public float rotationSpeed = 1f;

//     private Vector3 moveDirection = Vector3.zero;
//     private float rotationX = 0;
//     private CharacterController characterController;
//     private Animator animator;

//     private bool canMove = true;

//     private float inputx, inputz;

//     void Start()
//     {
//         characterController = GetComponent<CharacterController>();
//         animator = GetComponent<Animator>();
//         Cursor.lockState = CursorLockMode.Locked;
//         Cursor.visible = false;
//     }

//     void Update()
//     {
//         inputx = Input.GetAxis("Horizontal");
//         inputz = Input.GetAxis("Vertical");
//         Vector3 forward = transform.TransformDirection(Vector3.forward);
//         Vector3 right = transform.TransformDirection(Vector3.right);
//         animator.SetFloat("vertical", Input.GetAxis("Vertical"));
//         animator.SetFloat("horizontal", 0);
//         bool isRunning = Input.GetKey(KeyCode.LeftShift);
//         float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
//         float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
//         float movementDirectionY = moveDirection.y;
//         moveDirection = (forward * curSpeedX) + (right * curSpeedY);

//         if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
//         {
//             moveDirection.y = jumpPower;
//             // animator.SetBool("isJump", true);
//         }
//         else
//         {
//             moveDirection.y = movementDirectionY;
//         }

//         if (!characterController.isGrounded)
//         {
//             moveDirection.y -= gravity * Time.deltaTime;
//             // animator.SetBool("isJump", true);
//         }
//         if (characterController.isGrounded)
//         {
//             animator.SetBool("isJump", false);
//         }

//         if (Input.GetKey(KeyCode.R) && canMove)
//         {
//             characterController.height = crouchHeight;
//             walkSpeed = crouchSpeed;
//             runSpeed = crouchSpeed;

//         }
//         else
//         {
//             characterController.height = defaultHeight;
//             walkSpeed = 6f;
//             runSpeed = 12f;
//         }

//         characterController.Move(moveDirection * Time.deltaTime);

//         if (canMove)
//         {
//             rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
//             rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
//             playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
//             //  if (Input.GetKey(KeyCode.LeftArrow))
//             // {
//             //     transform.Rotate(0, -rotationSpeed, 0);
//             // }
//             // else if (Input.GetKey(KeyCode.RightArrow))
//             // {
//             //     transform.Rotate(0, rotationSpeed, 0);
//             // }
//             transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
//         }
//     }

//     void FixedUpdate()
//     {
//         characterController.transform.Rotate(Vector3.up * inputx * (100f * Time.deltaTime));
//     }
// }

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PhotonAnimatorView))]
public class PlayerMovement : MonoBehaviourPunCallbacks, IPunObservable
{
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 10f;
    public float gravity = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;
    public float rotationSpeed = 1f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;
    private Animator animator;
    private PhotonAnimatorView photonAnimatorView;

    private bool canMove = true;
    private float inputx, inputz;

    // Networking variables
    private Vector3 networkPosition;
    private Quaternion networkRotation;
    private float lagDistance = 10f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        photonAnimatorView = GetComponent<PhotonAnimatorView>();

        // Only enable camera and cursor lock for the local player
        if (photonView.IsMine)
        {
            playerCamera.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            if (playerCamera != null)
                playerCamera.gameObject.SetActive(false);
        }

        // Initialize network variables
        networkPosition = transform.position;
        networkRotation = transform.rotation;
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            HandleMovementInput();
            HandleAnimations();
        }
        else
        {
            // Smooth out movement for remote players
            UpdateRemotePlayer();
        }
    }

    void HandleMovementInput()
    {
        inputx = Input.GetAxis("Horizontal");
        inputz = Input.GetAxis("Vertical");

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * inputz : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * inputx : 0;

        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Handle crouching
        if (Input.GetKey(KeyCode.R) && canMove)
        {
            characterController.height = crouchHeight;
            walkSpeed = crouchSpeed;
            runSpeed = crouchSpeed;
        }
        else
        {
            characterController.height = defaultHeight;
            walkSpeed = 6f;
            runSpeed = 12f;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    void HandleAnimations()
    {
        // Set animation parameters
        animator.SetFloat("vertical", inputz);
        animator.SetFloat("horizontal", inputx);
        animator.SetBool("isJump", !characterController.isGrounded);
    }

    void UpdateRemotePlayer()
    {
        // Smooth position
        float distance = Vector3.Distance(transform.position, networkPosition);
        if (distance > lagDistance)
        {
            transform.position = networkPosition;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10f);
        }

        // Smooth rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * 10f);
    }

    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            characterController.transform.Rotate(Vector3.up * inputx * (100f * Time.deltaTime));
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // Receive data
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}