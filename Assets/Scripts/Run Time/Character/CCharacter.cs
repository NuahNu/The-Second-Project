using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


#region CCharacterStateM
/*
캐릭터 애니메이션과 상태머신
*/
#endregion

[Serializable]
public class CAnimationParamData
{
    public AnimatorControllerParameterType type;
    public string paramName;
    public string dicID;
    public int hash;

    private Animator _animator;

    public void Init(Animator animator, AnimatorControllerParameter param, string dicID = null)
    {
        if (animator.IsNull("animator")) return;

        _animator = animator;

        if (param.IsNull("param")) return;

        type = param.type;
        paramName = param.name;
        this.dicID = string.IsNullOrEmpty(dicID) ? paramName : dicID;
        hash = Animator.StringToHash(paramName);
    }

    #region SetParam
    //public void SetParam<T>(T input = default)
    //{
    //    switch (input)
    //    {
    //        case int i when type == EType.Int:
    //            _animator.SetInteger(hash, i);
    //            break;
    //
    //        case float f when type == EType.Float:
    //            _animator.SetFloat(hash, f);
    //            break;
    //
    //        case bool b when type == EType.Bool:
    //            _animator.SetBool(hash, b);
    //            break;
    //
    //        // Trigger의 경우 input이 없어도 되므로 null이나 기본값일 때 처리
    //        case null when type == EType.Trigger:
    //            _animator.SetTrigger(hash);
    //            break;
    //
    //        default:
    //            Debug.LogWarning($"타입 불일치: 현재 설정된 타입은 {type}이지만, 입력된 데이터는 {typeof(T)}입니다.");
    //            break;
    //    }
    //}
    public void SetParam(int input)
    {
        if (type != AnimatorControllerParameterType.Int)
        {
            Debug.LogWarning("type != Int");
            return;
        }
        _animator.SetInteger(hash, input);
    }
    public void SetParam(float input)
    {
        if (type != AnimatorControllerParameterType.Float)
        {
            Debug.LogWarning("type != Float");
            return;
        }
        _animator.SetFloat(hash, input);
    }
    public void SetParam(bool input)
    {
        if (type != AnimatorControllerParameterType.Bool)
        {
            Debug.LogWarning("type != Bool");
            return;
        }
        _animator.SetBool(hash, input);
    }
    public void SetParam()
    {
        if (type != AnimatorControllerParameterType.Trigger)
        {
            Debug.LogWarning("type != Trigger");
            return;
        }
        _animator.SetTrigger(hash);
    }
    #endregion
}

[RequireComponent(typeof(CAgent))]
[RequireComponent(typeof(CircleCollider2D))]
public abstract class CCharacter : MonoBehaviour
{
    #region 인스펙터
    [Header("애니메이션")]
    [SerializeField] protected Animator _animator;
    [SerializeField] protected string[] _paramIDs;
    [Header("애니메이션 (확인용)")]
    [SerializeField] protected CAnimationParamData[] _animationParams;

    #endregion

    #region 내부 변수
    // 애니메이션
    protected Dictionary<string, CAnimationParamData> _paramDic;

    // FSM
    protected abstract string[] States { get; }
    protected Dictionary<string, CStateMachine> _FSMDic;
    protected CStateMachine _currentState = null;

    protected Vector2 _moveDir;
    protected Vector2 _aimDir;

    // Agent
    protected CAgent _cAgent;

    // Mask
    private SpriteMask _spriteMask;
    private SpriteRenderer _spriteRenderer;

    //
    protected float _att;
    protected float _maxHP;
    protected float _currentHP;
    #endregion

    #region 유니티 이벤트
    void Awake()
    {
        // 애니메이션
        InitAnimator();
        InitParams();
        // FSM
        InitFSM();
        SetStates();
        // Agent
        InitAgent();
        // Mask
        if(TryGetComponent(out _spriteMask))
        {
            Debug.Log("이 캐릭터는 _spriteMask가 없구만?");
        }
        if (!TryGetComponent(out _spriteRenderer))
        {
            Debug.LogWarning("_spriteRenderer 가 없다고?");
            return;
        }
        ChangeState(_FSMDic["Idle"]);
    }

    protected virtual void Update()
    {
        _currentState.Update();
        if(_spriteMask != null)
            _spriteMask.sprite = _spriteRenderer.sprite;
    }

    private void OnDestroy()
    {
        UnsetStates();
        _paramDic.Clear();
        _paramDic = null;
        _FSMDic.Clear();
        _FSMDic = null;

        UnsetAgentEvent();

    }

    #endregion

    public void InitData(CCharacterDataSO data, Grid grid)
    {
        _att = data.Att;
        _maxHP = data.MaxHp;
        _currentHP = data.MaxHp;

        _cAgent.InitData(data, grid);
    }

    public void ChangeState(string stateName)
    {
        // 있는지 검사
        for (int i = 0; i < States.Length; i++)
        {
            if (States[i] == stateName)
            {
                ChangeState(_FSMDic[stateName]);
                return;
            }
        }
        Debug.LogWarning($"{stateName}라는 상태는 없다.");
    }

    public void SetMoveDir(Vector2 input)
    {
        if (_moveDir != input)
            _moveDir = input;
    }

    public void SetAimDir(Vector2 input)
    {
        if( _aimDir != input)
            _aimDir = input;
    }

    public void Warp(Vector3 pos)
    {
        _cAgent.Warp(pos);
    }

    public void SetTarget(Transform target)
    {
        _cAgent.SetTarget(target);
    }

    public void Kill()
    {
        gameObject.SetActive(false);
        //if(this.IsDestroyed()) return;
        //Destroy(gameObject);
    }

    public void Respawn()
    {
        _currentHP = _maxHP;
    }

    public Collider2D[] CheckTrigger()
    {
        return Physics2D.OverlapPointAll(this.transform.position);
    }

    public abstract void AnimationEventHandler(string eventName);

    public void GetDamage(float damage)
    {
        Debug.Log($"{gameObject.name}: {_currentHP} -> {_currentHP - damage}");
        _currentHP -= damage;

        if (_currentHP <= float.Epsilon)
            Kill();
    }

    public void ChangeAnimDir(Vector2 input)
    {
        OnDirChange(input);
    }

    #region protected
    protected virtual void SetStates()
    {
        _FSMDic["Idle"].OnEnter += IdleEnter;
        _FSMDic["Idle"].OnUpdate += IdleUpdate;
    }
    protected virtual void UnsetStates()
    {
        _FSMDic["Idle"].OnEnter -= IdleEnter;
        _FSMDic["Idle"].OnUpdate -= IdleUpdate;
    }

    protected void ChangeState(CStateMachine state)
    {
        if (state == null)
        {
            Debug.Log("state == null");
            return;
        }
        if (state == _currentState)
        {
            //Debug.Log($"state == {_currentState.Name}");
            return;
        }
        Debug.Log($"{_currentState?.Name ?? "None"} -> {state.Name} ");

        // 기본값은 null 이다.
        _currentState?.Exit();
        _currentState = state;
        _currentState.Enter();
    }

    protected virtual void SetAgentEvent()
    {
        _cAgent.OnDirChange += OnDirChange;
        _cAgent.OnMoveChange += OnMoveChange;
        _cAgent.OnWalkChange += OnWalkChange;
    }

    protected virtual void UnsetAgentEvent()
    {
        _cAgent.OnDirChange -= OnDirChange;
        _cAgent.OnMoveChange -= OnMoveChange;
        _cAgent.OnWalkChange -= OnWalkChange;
    }
    #endregion

    #region private
    private void InitAnimator()
    {
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();

            if (_animator == null)
            {
                _animator = this.gameObject.AddComponent<Animator>();
            }
        }

        if (_animator == null)
        {
            Debug.LogError("_animator == null");
            return;
        }
    }

    private void InitParams()
    {
        if (_paramDic == null)
        {
            _paramDic = new Dictionary<string, CAnimationParamData>();
        }

        var parameters = _animator.parameters;

        if (parameters != null || parameters.Length > 0)
        {
            Debug.Log($"parameters.Length == {parameters.Length}");

            _animationParams = new CAnimationParamData[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                _animationParams[i] = new CAnimationParamData();
                _animationParams[i].Init(_animator, parameters[i], i < _paramIDs.Length ? _paramIDs[i] : null);
                _paramDic[_animationParams[i].dicID] = _animationParams[i];
            }
        }
        else
        {
            Debug.LogWarning("parameters != null || parameters.Length > 0");
        }
    }

    // fsm
    private void InitFSM()
    {
        // 초기화
        if (_FSMDic == null)
        {
            _FSMDic = new Dictionary<string, CStateMachine>();
        }
        else
        {
            _FSMDic.Clear();
        }
        foreach (string state in States)
        {
            _FSMDic[state] = new CStateMachine();
            _FSMDic[state].Name = state;
        }
    }

    private void IdleEnter()
    {
        _paramDic["Idle"].SetParam();
    }
    private void IdleUpdate()
    {
        // 속도가 달라지면 이동하는코드.
        // 달리는 상태냐 걷는 상태냐
        _cAgent.SetInputDir(_moveDir);
        _cAgent.MoveAgent();
    }

    private void InitAgent()
    {
        if (!TryGetComponent(out _cAgent))
        {
            Debug.LogWarning("이게 왜 없죠?");
            return;
        }
        SetAgentEvent();
    }

    private void OnDirChange(Vector2 obj)
    {
        _paramDic["fHorizontal"].SetParam(obj.x);
        _paramDic["fVertical"].SetParam(obj.y);
    }

    private void OnMoveChange(bool flag)
    {
        _paramDic["IsMove"].SetParam(flag);
    }

    private void OnWalkChange(bool flag)
    {
        _paramDic["IsWalk"].SetParam(flag);
    }
    //
    #endregion
}
