using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkDirectInteractor : XRDirectInteractor
{
    [SerializeField] private PhotonView photonView;

    /// <summary>
    /// 물체를 잡았을 때 동작하는 메서드.
    /// </summary>
    /// <param name="args"></param>
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        // TODO : 잡은 사실을 네트워크를 통해서 전달.
        // 1. 잡은 플레이어가 잡은 물체의 소유권을 가져오기.
        PhotonView interactablePV = args.interactableObject.transform.GetComponent<PhotonView>();
        interactablePV.RequestOwnership();
    }

    /// <summary>
    /// 물체를 놓았을 때 동작하는 메서드.
    /// </summary>
    /// <param name="args"></param>
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        // TODO : 놓은 사실을 네트워크를 통해서 전달.
        // 1. 놓은 플레이어가 잡은 물체의 소유권을 방장에게 다시 돌려주기.
        PhotonView interactablePV = args.interactableObject.transform.GetComponent<PhotonView>();
        interactablePV.TransferOwnership(PhotonNetwork.MasterClient);
    }
}
