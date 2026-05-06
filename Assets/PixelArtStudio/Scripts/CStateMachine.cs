using System;


#region CStateMachine
/*

*/
#endregion

public class CStateMachine : IStateMachine
{
    public event Action OnEnter;
    public event Action OnUpdate;
    public event Action OnExit;

    public void Enter()
    {
        OnEnter?.Invoke();
    }

    public void Exit()
    {
        OnExit?.Invoke();
    }

    public void Update()
    {
        OnUpdate?.Invoke();
    }
}
