using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class BuildingSelectionControllerScript : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera _userCamera;
    [Header("Input")]
    [SerializeField] public InputActionReference SelectAction;
    [Header("UI")]
    [SerializeField] private BuildingInfoUIPanelScript BuildingInfoUIPanel;
    [SerializeField] private AudioSource BuildingSelectSound;

    private RaycastHit _hit;
    private Vector2 _mousePosition;
    private Ray _ray;
    private ISelectableBuilding _nowHoveredObject;
    private ISelectableBuilding _prevHoveredObject;
    private ISelectableBuilding _nowSelectedObject;
    private ISelectableBuilding _prevSelectedObject;
    private bool _selectionRequested;
    private bool mustOffUI;

    private void OnEnable()
    {
        SelectAction.action.performed += OnSelect;

        SelectAction.action.Enable();
    }

    private void OnDisable()
    {
        SelectAction.action.performed -= OnSelect;

        SelectAction.action.Disable();
        _selectionRequested = false;
    }

    private void OnSelect(InputAction.CallbackContext context)
    {
        _selectionRequested = true;
    }

    private void LateUpdate()
    {
        if (!_selectionRequested)
        {
            return;
        }

        _selectionRequested = false;

        // UI 버튼을 누른 클릭이 월드까지 넘치는 것을 막기
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // LateUpdate 시점에는 이번 프레임의 Hover 판정이 이미 끝나 있다.
        _nowSelectedObject = _nowHoveredObject;

        BuildingSelectSound.Play();

        //Debug.Log("Object is" + _nowSelectedObject);
        //Debug.Log("BuildingState is " + _nowSelectedObject?.SelectableBuildingState);
        mustOffUI = BuildingInfoUIPanel.ShowUIPanel(_nowSelectedObject?.SelectableBuildingState);

        if (mustOffUI)
        {
            BuildingInfoUIPanel.OffUIPanel();
        }

        _prevSelectedObject = _nowSelectedObject;
    }

    void Update()
    {
        if (_userCamera == null || Pointer.current == null)
        {
            return;
        }

        _mousePosition = Pointer.current.position.ReadValue();
        _ray = _userCamera.ScreenPointToRay(_mousePosition);

        //Debug.Log(_mousePosition.ToString());

        // 1. 마우스 포인터의 Ray를 쬐여 맨 먼저 만나는 대상을 받음
        if (Physics.Raycast(_ray, out _hit, Mathf.Infinity))

        {
            //Debug.DrawRay(_ray.origin, _ray.direction * _hit.distance, Color.yellow);
            //Debug.Log("Did Hit");
            _nowHoveredObject = _hit.collider.gameObject.GetComponent<ISelectableBuilding>();

            // 2. 해당 Object가 Selectable이면 다음 활동들이 활성화됨
            _nowHoveredObject?.HighlightOn();

            if (_nowHoveredObject != _prevHoveredObject)
            {
                _prevHoveredObject?.HighlightOff();
                _prevHoveredObject = _nowHoveredObject;
            }
            
        }
        else
        {
            //Debug.DrawRay(_ray.origin, _ray.direction * 1000, Color.white);
            //Debug.Log("Did not Hit");
            _prevHoveredObject = _nowHoveredObject;
            _nowHoveredObject = null;
        }

    }
}
