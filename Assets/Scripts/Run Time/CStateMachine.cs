using System;


#region CStateMachine
/*
그냥 얘를 상속하는 무언가로 만들어버리기?
*/
#endregion

public class CStateMachine : IStateMachine
{
    public event Action OnEnter;
    public event Action OnUpdate;
    public event Action OnExit;

    public string Name { get; set; }

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
