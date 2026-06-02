using UnityEngine;


#region CCommonSoldier
/*
이 스크립트를 컴포넌트로 추가하면, CAgent가 추가될 때 Agent 컴포넌트의 값이 이상해 버그가 생길 수 있다.
*/
#endregion

public class CCommonSoldier : CCharacter
{
    #region 인스펙터
    // CEnemyCO에도 같은 값이 있다.
    [SerializeField] private int _attackRange = 2;
    #endregion

    #region 내부 변수
    protected override string[] States => new string[] { "Idle", "Move", "Attack", "GetHit", "Death" };
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
        // 왜 Hit 이야?
        if (eventName == "Hit")
        {
            // 공격을 위한 무언가 생성.
            CProjectile projectile = CMain.Instance.SpawnProjectile(EProjectileType.Common);
            projectile.transform.position = transform.position;
            projectile.SetDir(_aimDir);
            projectile.tag = this.tag;
            projectile.ATT = _att;
        }
    }
    #endregion

    #region protected
    protected override void SetStates()
    {
        base.SetStates();
        _FSMDic["Attack"].OnEnter += AttackEnter;
        _FSMDic["Attack"].OnUpdate += AttackUpdate;
        _FSMDic["Attack"].OnExit += AttackExit;
    }

    protected override void UnsetStates()
    {
        base.UnsetStates();
        _FSMDic["Attack"].OnEnter -= AttackEnter;
        _FSMDic["Attack"].OnUpdate -= AttackUpdate;
        _FSMDic["Attack"].OnExit -= AttackExit;
    }

    protected override void SetAgentEvent()
    {
        base.SetAgentEvent();
        _cAgent.OnDirChange += OnDirChangeCommonSoldier;
    }

    protected override void UnsetAgentEvent()
    {
        base.UnsetAgentEvent();
        _cAgent.OnDirChange -= OnDirChangeCommonSoldier;
    }
    #endregion

    #region private
    private void AttackEnter()
    {
        _paramDic["Attack"].SetParam();
    }

    private void AttackUpdate()
    {
    }

    private void AttackExit()
    {
    }
    private void OnDirChangeCommonSoldier(Vector2 obj)
    {
        SetAimDir(obj);
    }
    #endregion
}
