using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiggingManager : MonoBehaviourPun
{
    public Transform leftHandIK;            // 왼손 IK 
    public Transform righttHandIK;          // 오른손 IK
    public Transform headIK;                // HMD IK

    public Transform leftHandController;    // 왼손 컨트롤러
    public Transform rightHandController;   // 오른손 컨트롤러
    public Transform hmd;                   // HMD

    public Vector3[] leftOffset;            // 왼손 Offset
    public Vector3[] rightOffset;           // 오른손 Offset
    public Vector3[] headOffset;            // hmd Offset

    public float smoothValue = 0.1f;        // 부드럽게 움직일 값
    public float modelHeight = 1.1176f;     // 캐릭터 높이 값


    #region XR Origin과 캐릭터 분리시
    //private void Start()
    //{
    //// PhotonView.IsMine인 경우에만 실행
    //if (photonView.IsMine)
    //{
    //    FindControllers();
    //}
    //}
    #endregion

    /// <summary>
    /// 컨트롤러가 움직인 후 IK의 Transform을 맞추려고.
    /// </summary>
    private void LateUpdate()
    {
        if (photonView.IsMine)
        {
            // 로컬 플레이어의 동작 처리
            MappingHandTranform(leftHandIK, leftHandController, true);
            MappingHandTranform(righttHandIK, rightHandController, false);
            MappingBodyTransform(headIK, hmd);
            MappingHeadTransform(headIK, hmd);

            // 동기화된 위치 및 회전을 RPC로 전송
            photonView.RPC("SyncIKRPC", RpcTarget.Others, 
                leftHandIK.position, leftHandIK.rotation,
                righttHandIK.position, righttHandIK.rotation, 
                headIK.position, headIK.rotation);
        }
    }

    #region XR Origin과 캐릭터 분리시
    //private void FindControllers()
    //{
    //    // XR Origin을 찾아서 컨트롤러와 HMD 연결
    //    GameObject xrOrigin = GameObject.Find("Player(XR Origin)(Clone)");

    //    if (xrOrigin != null)
    //    {
    //        leftHandController = xrOrigin.transform.Find("Camera Offset/Left Controller");
    //        rightHandController = xrOrigin.transform.Find("Camera Offset/Right Controller");
    //        hmd = xrOrigin.transform.Find("Camera Offset/Main Camera");

    //        Debug.Log("RiggingManager에 컨트롤러 연결완료");
    //    }
    //    else
    //    {
    //        Debug.LogError("Player(XR Origin) 찾지못함! XR Origin이름 틀림.");
    //    }
    //}
    #endregion

    /// <summary>
    /// 컨트롤러의 싱크를 맞추기 위한 Offset.
    /// </summary>
    /// <param name="ik"></param>
    /// <param name="controller"></param>
    /// <param name="isLeft"></param>
    private void MappingHandTranform(Transform ik, Transform controller, bool isLeft)
    {
        // ik의 Transform = Controller의 Transform
        var offset = isLeft ? leftOffset : rightOffset;

        // 컨트롤러 위치 값. [0]
        ik.position = controller.TransformPoint(offset[0]);
        // 컨트롤러 회전 값. [1]
        ik.rotation = controller.rotation * Quaternion.Euler(offset[1]);
    }

    /// <summary>
    /// HMD와 캐릭터의 몸이 같이 돌아가도록 설정한 메서드.
    /// </summary>
    /// <param name="ik"></param>
    /// <param name="hmd"></param>
    private void MappingBodyTransform(Transform ik, Transform hmd)
    {
        this.transform.position = new Vector3(hmd.position.x, modelHeight, hmd.position.z);
        float yaw = hmd.eulerAngles.y;
        var targetRotation = new Vector3(this.transform.eulerAngles.x, yaw, this.transform.eulerAngles.z);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(targetRotation), smoothValue);
    }

    /// <summary>
    /// HMD의 IK Offset.
    /// </summary>
    /// <param name="ik"></param>
    /// <param name="hmd"></param>
    private void MappingHeadTransform(Transform ik, Transform hmd)
    {
        ik.position = hmd.TransformPoint(headOffset[0]);
        ik.rotation = hmd.rotation * Quaternion.Euler(headOffset[1]);
    }

    /// <summary>
    /// IK의 동기화 값 RPC.
    /// </summary>
    /// <param name="leftHandPos"></param>
    /// <param name="leftHandRot"></param>
    /// <param name="rightHandPos"></param>
    /// <param name="rightHandRot"></param>
    /// <param name="headPos"></param>
    /// <param name="headRot"></param>
    [PunRPC]
    private void SyncIKRPC(Vector3 leftHandPos, Quaternion leftHandRot,
        Vector3 rightHandPos, Quaternion rightHandRot, Vector3 headPos, Quaternion headRot)
    {
        leftHandIK.position = leftHandPos;
        leftHandIK.rotation = leftHandRot;

        righttHandIK.position = rightHandPos;
        righttHandIK.rotation = rightHandRot;

        headIK.position = headPos;
        headIK.rotation = headRot;
    }
}
