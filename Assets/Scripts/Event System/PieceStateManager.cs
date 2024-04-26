public class PieceStateManager
{

    private static PieceStateManager _instance;
    public static PieceStateManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PieceStateManager();
            }
            return _instance;
        }
    }

    public PieceState CurrentPieceState { get; private set; }

    public delegate void PieceStateChangeHandler(PieceState newPieceState);
    public event PieceStateChangeHandler OnPieceStateChanged;

    private PieceStateManager()
    {

    }

    public void SetState(PieceState newPieceState)
    {
        if (newPieceState == CurrentPieceState) return;

        CurrentPieceState = newPieceState;
        OnPieceStateChanged?.Invoke(newPieceState);
    }

    public void SwitchState()
    {
        if (CurrentPieceState == PieceState.Animation)
        {
            SetState(PieceState.Default);
        }
        else
        {
            SetState(PieceState.Animation);
        }
    }
}
