using System;

#region IStateMachine
/*

*/
#endregion


public interface IStateMachine
{
    public event Action OnEnter;
    public event Action OnUpdate;
    public event Action OnExit;

    public void Enter();
    public void Update();
    public void Exit();
}
