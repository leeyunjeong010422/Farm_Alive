using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ModeSlectInteractable : XRGrabInteractable
{
    [Header("다음 오브젝트 상황")]
    public GameObject stageObject;
    public GameObject parentObject;

    [Header("GameMode")]
    public E_GameMode gameMode;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private bool _isSelected;
    private Coroutine resetCoroutine;

    protected override void Awake()
    {
        base.Awake();

        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    private void Update()
    {
        if (!_isSelected && Vector3.Distance(transform.position, initialPosition) > 0.01f)
        {
            if (resetCoroutine == null)
            {
                resetCoroutine = StartCoroutine(ResetToInitialPosition());
            }
        }
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        _isSelected = true;
        SoundManager.Instance.PlaySFX("SFX_Lobby_CropSelected");

        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
            resetCoroutine = null;
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        _isSelected = false;
#if UNITY_EDITOR
        Debug.Log($"{args.interactableObject.transform.name}가 선택되었습니다.");
#endif
        StartCoroutine(SelectObjectDestroy(args.interactableObject.transform.gameObject));
    }

    IEnumerator SelectObjectDestroy(GameObject targetObject)
    {
        // 3초 대기
        yield return new WaitForSeconds(3f);

        // 물체를 초기 위치와 회전으로 즉시 이동
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        // 선택된 게임 모드 넘겨주기
        PunManager.Instance.SetGameMode(gameMode);

        stageObject.SetActive(true);
        parentObject.SetActive(false);
    }

    private IEnumerator ResetToInitialPosition()
    {
        // 3초 대기
        yield return new WaitForSeconds(3f);

        if (!isSelected)
        {
            transform.position = initialPosition;
            transform.rotation = initialRotation;
        }

        resetCoroutine = null;
    }
}
