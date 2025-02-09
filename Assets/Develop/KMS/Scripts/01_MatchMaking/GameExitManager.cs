using TMPro;
using UnityEngine;
using UnityEngine.XR;

public class GameExitManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject exitConfirmationPanel;
    public TMP_Text exitConfirmationText;

    [Header("Panel Settings")]
    public float panelDistance = 2f;

    private bool _isExitRequest = false;
    private float _buttonPressDuration = 0f;
    private const float _requiredHoldTime = 1f;

    private void Start()
    {
        if (exitConfirmationPanel)
            exitConfirmationPanel.SetActive(false);
        exitConfirmationText.text = "게임을 종료하시겠습니까?\n(게임 종료하기 - A / 게임 속으로 - B)";
    }

    private void Update()
    {
        HandleControllerInput();

        if (exitConfirmationPanel && exitConfirmationPanel.activeSelf)
        {
            UpdatePanelPosition();
        }
    }

    private void HandleControllerInput()
    {
        InputDevice leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        InputDevice rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        // 컨트롤러 Y 버튼 (알림창 표시)
        if (leftController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool isYPressed))
        {
            HandleExitRequest(isYPressed);
        }

        // 컨트롤러 A 버튼 (게임 종료)
        if (rightController.TryGetFeatureValue(CommonUsages.primaryButton, out bool isAPressed))
        {
            if (isAPressed && exitConfirmationPanel.activeSelf)
            {
                ConfirmExit();
            }
        }

        // 컨트롤러 B 버튼 (알림창 닫기)
        if (rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool isBPressed))
        {
            if (isBPressed && exitConfirmationPanel.activeSelf)
            {
                CancelExit();
            }
        }
    }

    private void HandleExitRequest(bool isPressed)
    {
        if (isPressed)
        {
            _buttonPressDuration += Time.deltaTime;

            if (_buttonPressDuration >= _requiredHoldTime && !_isExitRequest)
            {
                _isExitRequest = true;
                ShowExitConfirmation();
            }
        }
        else
        {
            // 버튼이 떼어졌을 때 초기화
            _buttonPressDuration = 0f;
            _isExitRequest = false;
        }
    }

    private void ShowExitConfirmation()
    {
        if (exitConfirmationPanel)
            exitConfirmationPanel.SetActive(true);
    }

    private void HideExitConfirmation()
    {
        if (exitConfirmationPanel)
            exitConfirmationPanel.SetActive(false);
    }

    private void ConfirmExit()
    {
        Debug.Log("게임 종료!");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void CancelExit()
    {
        Debug.Log("게임 종료 취소!");
        HideExitConfirmation();
    }

    private void UpdatePanelPosition()
    {
        Camera mainCamera = Camera.main;

        if (exitConfirmationPanel != null)
        {
            Vector3 targetPosition = mainCamera.transform.position + mainCamera.transform.forward * panelDistance;
            exitConfirmationPanel.transform.position = targetPosition;

            exitConfirmationPanel.transform.rotation = Quaternion.LookRotation(exitConfirmationPanel.transform.position - mainCamera.transform.position);
        }
    }
}
