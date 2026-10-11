using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Simulation을 사람이 조작할 수 있도록 하는 Controller
/// </summary>
public class SimulationController : MonoBehaviour
{

    [Header("Input")]
    [SerializeField] public InputActionReference pauseAction;
    [SerializeField] public InputActionReference speed1Action;
    [SerializeField] public InputActionReference speed2Action;
    [SerializeField] public InputActionReference speed3Action;
    [SerializeField] public InputActionReference speed6Action;


    // 바깥에서 참조할 수 있는 Simulation. SimulationController가 Simulation을 가지고 있어야 함.
    public Simulation Simulation { get; private set; }
    [Header("Map")]
    public MapRenderer MapRenderer;

    [Header("Time")]
    public GameObject RightDownUI;

    [Header("Buildings")]
    [SerializeField] private BuildingRegistry _buildingRegistry;

    [Header("UI")]
    [SerializeField] public ScreenUIManager ScreenUIManager;

    // 틱 실행 연산을 위해 흐른 시간을 계산하는 변수
    private double _timeAccumulator = 0d;
    // 1배속 기준 1초에 10틱
    private const double TickInterval = 0.1d;
    private TimeChange _timeChange;

    // 일시정지 상태와 게임속도 상태를 나타내는 변수들과 그의 enum
    public enum TimeSpeed { One = 1, Two = 2, Three = 3, Six = 6 };
    public bool IsPaused { get; private set; } = true;
    public TimeSpeed GameSpeed { get; private set; } = TimeSpeed.One;

    private void Awake()
    {

        // _buildingRegistry는 Prototype에만 있을 예정인데 나중에도 작동되게 만들어야할수도.
        Simulation = new Simulation(GameDataTransfer.GetGameData(), 
            _buildingRegistry.CreateInitialStates());

    }

    void Start()
    {

        if (MapRenderer == null)
        {
            Debug.Log("Prototype 감지 from SimulationController.Start()");

            ScreenUIManager.Initialize(Simulation.Clock, Simulation.GameState, IsPaused, GameSpeed);
            
            Simulation.FailureOccurred += ScreenUIManager.ShowFailureUI;
            Simulation.VictoryAchieved += ScreenUIManager.ShowVictoryUI;

            Simulation.BuildingConstructionCompleted += ScreenUIManager.OnBuildingConstructionCompleted;
            Simulation.BuildingConstructionCompleted += _buildingRegistry.RefreshBySiteId;

        }
        else
        {
            MapRenderer.RenderMap(Simulation.Map);
        }
    }

    private void OnDestroy()
    {
        if (Simulation == null)
        {
            return;
        }

        Simulation.VictoryAchieved -= ScreenUIManager.ShowVictoryUI;
        Simulation.FailureOccurred -= ScreenUIManager.ShowFailureUI;
        Simulation.BuildingConstructionCompleted -= ScreenUIManager.OnBuildingConstructionCompleted;
        Simulation.BuildingConstructionCompleted -= _buildingRegistry.RefreshBySiteId;
    }

    private void OnEnable()
    {
        pauseAction.action.performed += OnPause;
        speed1Action.action.performed += OnSpeed1;
        speed2Action.action.performed += OnSpeed2;
        speed3Action.action.performed += OnSpeed3;
        speed6Action.action.performed += OnSpeed6;

        pauseAction.action.Enable();
        speed1Action.action.Enable();
        speed2Action.action.Enable();
        speed3Action.action.Enable();
        speed6Action.action.Enable();
    }

    private void OnDisable()
    {
        pauseAction.action.performed -= OnPause;
        speed1Action.action.performed -= OnSpeed1;
        speed2Action.action.performed -= OnSpeed2;
        speed3Action.action.performed -= OnSpeed3;
        speed6Action.action.performed -= OnSpeed6;

        pauseAction.action.Disable();
        speed1Action.action.Disable();
        speed2Action.action.Disable();
        speed3Action.action.Disable();
        speed6Action.action.Disable();
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        TogglePause();
    }

    public void OnPauseClicked()
    {
        TogglePause();
    }

    public void DoNotPause()
    {
        IsPaused = false;

        Debug.Log($"Game Resumed at {(int)GameSpeed}x");

        ScreenUIManager.TimeSwitchChanged(IsPaused, GameSpeed);
    }

    private void TogglePause()
    {
        IsPaused = !IsPaused;

        Debug.Log(IsPaused
            ? $"Game Paused in {Simulation.Clock.ToString()} Tick"
            : $"Game Resumed at {(int)GameSpeed}x");

        ScreenUIManager.TimeSwitchChanged(IsPaused, GameSpeed);
    }

    private void OnSpeed1(InputAction.CallbackContext context)
    {
        SetGameSpeed(TimeSpeed.One);
    }

    private void OnSpeed2(InputAction.CallbackContext context)
    {
        SetGameSpeed(TimeSpeed.Two);
    }

    private void OnSpeed3(InputAction.CallbackContext context)
    {
        SetGameSpeed(TimeSpeed.Three);
    }

    private void OnSpeed6(InputAction.CallbackContext context)
    {
        SetGameSpeed(TimeSpeed.Six);
    }

    public void OnSpeed1Clicked()
    {
        SetGameSpeed(TimeSpeed.One);
    }
    public void OnSpeed2Clicked()
    {
        SetGameSpeed(TimeSpeed.Two);
    }
    public void OnSpeed3Clicked()
    {
        SetGameSpeed(TimeSpeed.Three);
    }
    public void OnSpeed6Clicked()
    {
        SetGameSpeed(TimeSpeed.Six);
    }

    private void SetGameSpeed(TimeSpeed timeSpeed)
    {
        GameSpeed = timeSpeed;
        //Debug.Log($"Game Speed Set to {GameSpeed}x");

        ScreenUIManager.TimeSwitchChanged(IsPaused, GameSpeed);
    }


    private void Update()
    {

        //Debug.Log($"GameFlow : {Simulation.GameState.GameFlow}");
        

        if (IsPaused)
        {
            return;
        }

        //Time.DeltaTime은 TimeScale에 영향을 받으니
        //만약 나중에 TimeScale을 건든다면 문제가 생기므로 선제적으로 Time.unscaledDeltaTime로 변경
        // Simulation.GameStatus는 게임이 진행중인지 끝난건지를 위한 인수.
        _timeAccumulator += (double)Time.unscaledDeltaTime * (int)GameSpeed
            * Simulation.GameState.GameFlow;
        
        while (_timeAccumulator >= TickInterval)
        {
            _timeAccumulator -= TickInterval;
            _timeChange = Simulation.Tick();
        }

        if (_timeChange.DayChanged)
        {
            ScreenUIManager.DayUIRefresh();
            ScreenUIManager.HourUIRefresh();
            _timeChange = new TimeChange();
        }
        else if (_timeChange.HourChanged)
        {
            ScreenUIManager.HourUIRefresh();
            _timeChange = new TimeChange();
        }
        else
        {

            // 필요없는 Method인듯.
            // Todo: UI 다 구현하고 이거 재고하기
            //ScreenUIManager.GeneralUIRefresh();
        }
    }
}
