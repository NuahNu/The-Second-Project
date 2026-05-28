using UnityEngine;


#region CCommonSoldier
/*
이 스크립트를 컴포넌트로 추가하면, CAgent가 추가될 때 Agent 컴포넌트의 값이 이상해 버그가 생길 수 있다.
*/
#endregion

public class CCommonSoldier : CCharacter
{
    #region 인스펙터

    #endregion

    #region 내부 변수
    protected override string[] States => new string[] { "Idle", "Move", "Action", "Attack", "GetHit", "Death" };
    #endregion

    #region 유니티 이벤트
    //void Start()
    //{
    //    
    //}

    //protected override void Update()
    //{
    //    
    //}
    #endregion

    #region public
    public override void AnimationEventHandler(string eventName)
    {
        ;
    }
    #endregion

    #region protected
    protected override void SetStates()
    {
        base.SetStates();
    }
    protected override void UnsetStates()
    {
        base.UnsetStates();

    }
    #endregion

    #region private

    #endregion
}
