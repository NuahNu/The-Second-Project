using System;
using System.Collections.Generic;
using UnityEngine;


#region CCharacterStateM
/*

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

public class CCharacterStateMachine : MonoBehaviour
{
    public enum ECharacterStateMachine
    {
        Idle,
        Attack,
        Block,
        Boost,
        Death,
        GetHit,
        Run,
        Stunned,
        Walk,
        Count
    }

    #region 인스펙터
    [Header("애니메이션")]
    [SerializeField] private Animator _animator;
    [SerializeField] private string[] _paramIDs;
    [Header("애니메이션 (확인용)")]
    [SerializeField] private CAnimationParamData[] _animationParams;

    #endregion

    #region 내부 변수
    // 애니메이션
    private Dictionary<string, CAnimationParamData> _paramDic;

    // FSM
    private Dictionary<ECharacterStateMachine, CStateMachine> _FSMDic;
    private CStateMachine _currentState = null;
    #endregion

    #region 유니티 이벤트
    void Awake()
    {
        // 애니메이션
        InitAnimator();
        // FSM
        InitFSM();
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

    private void InitFSM()
    {
        // 초기화
        if (_FSMDic == null)
        {
            _FSMDic = new Dictionary<ECharacterStateMachine, CStateMachine>();
        }
        else
        {
            _FSMDic.Clear();
        }
        for (int i = 0; i < (int)ECharacterStateMachine.Count; i++)
        {
            _FSMDic[(ECharacterStateMachine)i] = new CStateMachine();
        }
        // 내용 채우기.
        _FSMDic[ECharacterStateMachine.Idle].OnEnter += () =>
        {

        };
    }

    private void ChangeState(CStateMachine state)
    {
        if (state == _currentState)
            return;

        _currentState.Exit();
        _currentState = state;
        _currentState.Enter();
    }

    #endregion
}
