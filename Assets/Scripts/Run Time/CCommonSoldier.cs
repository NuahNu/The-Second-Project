using UnityEngine;


#region CCommonSoldier
/*

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
