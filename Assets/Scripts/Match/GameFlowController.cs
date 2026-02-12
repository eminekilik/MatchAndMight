using UnityEngine;

public class GameFlowController : MonoBehaviour
{
    public static GameFlowController Instance;

    public enum GameState
    {
        Idle,
        Resolving,
        Locked
    }

    public GameState currentState;

    void Awake()
    {
        Instance = this;
        currentState = GameState.Idle;
    }

    void Start()
    {
        MatchDestroyer.Instance.OnResolveFinished += OnBoardResolved;
    }

    public void StartResolve()
    {
        currentState = GameState.Resolving;
        MatchDestroyer.Instance.ResolveMatches();
    }

    void OnBoardResolved()
    {
        currentState = GameState.Idle;
    }

    public bool CanInteract()
    {
        return currentState == GameState.Idle;
    }
}
