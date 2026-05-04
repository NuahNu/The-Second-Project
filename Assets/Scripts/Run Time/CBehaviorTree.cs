using System;
using System.Collections.Generic;
using UnityEngine;


#region CBehaviorTree
/*
노드를 조립하고 액션노드에 연결할 함수를 구현한다.
*/
#endregion


public interface IBTNode
{
    public enum EState
    {
        Run,
        Success,
        Failed
    }

    // 상태 리턴
    public EState Evaluate();
}

public class CActionNode : IBTNode
{
    public Func<IBTNode.EState> action;

    public CActionNode(Func<IBTNode.EState> action)
    {
        this.action = action;
    }

    public IBTNode.EState Evaluate()
    {
        return action?.Invoke() ?? IBTNode.EState.Failed;
    }
}

/// <summary>
/// OR
/// </summary>
public class CSelectiorNode : IBTNode
{
    private List<IBTNode> _children;

    public CSelectiorNode() { _children = new List<IBTNode>(); }

    public void Add(IBTNode node) { _children.Add(node); }

    public IBTNode.EState Evaluate()
    {
        for (int i = 0; i < _children.Count; i++)
        {
            IBTNode childNode = _children[i];

            IBTNode.EState state = childNode.Evaluate();

            switch (state)
            {
                case IBTNode.EState.Success:
                    return IBTNode.EState.Success;
                case IBTNode.EState.Run:
                    return IBTNode.EState.Run;
            }
        }
        //자식이 전부 Failed 인 경우
        return IBTNode.EState.Failed;
    }
}

/// <summary>
/// AND
/// </summary>
public class CSequenceNode : IBTNode
{
    private List<IBTNode> _children;

    public CSequenceNode() { _children = new List<IBTNode>(); }

    public void Add(IBTNode node) { _children.Add(node); }

    public IBTNode.EState Evaluate()
    {
        if (_children.Count <= 0) return IBTNode.EState.Failed;

        for (int i = 0; i < _children.Count; i++)
        {
            IBTNode childNode = _children[i];
            switch (childNode.Evaluate())
            {
                case IBTNode.EState.Run:
                    return IBTNode.EState.Run;
                case IBTNode.EState.Success:
                    continue;
                case IBTNode.EState.Failed:
                    return IBTNode.EState.Failed;
            }
        }
        return IBTNode.EState.Success;
    }
}


public class CBehaviorTree : MonoBehaviour
{
    #region 인스펙터

    #endregion

    #region 내부 변수

    #endregion

    #region 유니티 이벤트
    void Awake()
    {

    }

    void Start()
    {

    }

    void Update()
    {

    }
    #endregion

    #region public

    #endregion

    #region protected

    #endregion

    #region private

    #endregion
}
