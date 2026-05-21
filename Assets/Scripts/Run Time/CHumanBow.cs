using UnityEngine;


#region CHumanBow1
/*

*/
#endregion

public class CHumanBow : CCharacter
{
    #region 인스펙터

    #endregion

    #region 내부 변수
    protected override string[] States => new string[] { "Idle", "Move", "Aim", "Action", "Attack", "GetHit", "Death" };

    #endregion

    #region 유니티 이벤트
    #endregion

    #region public

    #endregion

    #region protected
    protected override void SetStates()
    {
        base.SetStates();
        _FSMDic["Aim"].OnEnter += AimEnter;

    }

    protected override void UnsetStates()
    {
        base.UnsetStates();
        _FSMDic["Aim"].OnEnter -= AimEnter;

    }
    #endregion

    #region private
    private void AimEnter()
    {
        _paramDic["Aim"].SetParam();
    }
    #endregion
}
