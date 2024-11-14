using UnityEngine;
using Photon.Pun;
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(Animator))]
public class PickupAnimationController : MonoBehaviourPunCallbacks
{
    [Header("Animation Parameters")]
    [SerializeField] private string pickupTriggerName = "PickupTrigger";
    [SerializeField] private string holdingBoolName = "IsHolding";
    [SerializeField] private string throwTriggerName = "ThrowTrigger";

    private Animator playerAnimator;
    private PhotonView photonView;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        photonView = GetComponent<PhotonView>();
    }

    public void HandlePickupAnimation()
    {
        photonView.RPC("PlayPickupAnimation", RpcTarget.All);
    }

    public void HandleDropAnimation()
    {
        photonView.RPC("PlayDropAnimation", RpcTarget.All);
    }

    public void HandleThrowAnimation()
    {
        photonView.RPC("PlayThrowAnimation", RpcTarget.All);
    }

    [PunRPC]
    private void PlayPickupAnimation()
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger(pickupTriggerName);
            playerAnimator.SetBool(holdingBoolName, true);
        }
    }

    [PunRPC]
    private void PlayDropAnimation()
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetBool(holdingBoolName, false);
        }
    }

    [PunRPC]
    private void PlayThrowAnimation()
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger(throwTriggerName);
            playerAnimator.SetBool(holdingBoolName, false);
        }
    }
}