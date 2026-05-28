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


    protected bool readyToFire = false; 
    #endregion

    #region 유니티 이벤트
    #endregion

    #region public
    public override void AnimationEventHandler(string eventName)
    {
        if(eventName == "ReadyToFire")
        {
            //Debug.Log(eventName);
            ReadyToFire();
        }
    }
    #endregion

    #region protected
    protected override void SetStates()
    {
        base.SetStates();
        _FSMDic["Aim"].OnEnter += AimEnter;
        _FSMDic["Aim"].OnUpdate += AimUpdate;

    }

    protected override void UnsetStates()
    {
        base.UnsetStates();
        _FSMDic["Aim"].OnEnter -= AimEnter;

    }

    protected void ReadyToFire()
    {
        if (readyToFire) return;
        readyToFire = true;
        Debug.Log("ReadyToFire");
    }
    #endregion

    #region private
    private void AimEnter()
    {
        _paramDic["Aim"].SetParam();
    }
    private void AimUpdate()
    {
        ChangeDir(_aimDir);
    }
    #endregion
}
