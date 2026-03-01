using UnityEngine;
using UnityEngine.InputSystem;

public class NotebookInputHandler : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputAction openMapAction;
    [SerializeField] private InputAction openMissionAction;
    [SerializeField] private InputAction closeNotebookAction;

    private NotebookManager stateManager;

    private void OnEnable()
    {
        stateManager = NotebookManager.Instance;
        if (stateManager == null) return;

        // 이벤트 연결
        openMapAction.performed += OnOpenMap;
        openMissionAction.performed += OnOpenMission;
        closeNotebookAction.performed += OnCloseNotebook;

        // 액션 활성화
        openMapAction.Enable();
        openMissionAction.Enable();
        closeNotebookAction.Enable();
    }

    private void OnDisable()
    {
        // 이벤트 해제
        openMapAction.performed -= OnOpenMap;
        openMissionAction.performed -= OnOpenMission;
        closeNotebookAction.performed -= OnCloseNotebook;

        // 액션 비활성화
        openMapAction.Disable();
        openMissionAction.Disable();
        closeNotebookAction.Disable();
    }

    private void OnOpenMap(InputAction.CallbackContext ctx) => HandleOpenOrSwitch("Map");
    private void OnOpenMission(InputAction.CallbackContext ctx) => HandleOpenOrSwitch("Mission");
    private void OnCloseNotebook(InputAction.CallbackContext ctx) => HandleClose();

    private void HandleOpenOrSwitch(string tab)
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0 ||
           (UI_PauseMenuPopup.Instance != null && !UI_PauseMenuPopup.Instance.CanOpen ||
            UI_PauseMenuPopup.Instance.IsOpen))
            return;

        switch (stateManager.CurrentState)
        {
            case NotebookState.Closed:
                stateManager.RequestOpen(tab);
                break;
            case NotebookState.Active:
                if (stateManager.NotebookUI.CurrentTabName == tab)
                {
                    stateManager.RequestClose();
                }
                else
                {
                    stateManager.RequestTabSwitch(tab);
                }
                break;
        }
    }

    private void HandleClose()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0 ||
           (UI_PauseMenuPopup.Instance != null && !UI_PauseMenuPopup.Instance.CanOpen ||
            UI_PauseMenuPopup.Instance.IsOpen))
            return;

        if (stateManager.CurrentState == NotebookState.Active)
            stateManager.RequestClose();
    }
}
