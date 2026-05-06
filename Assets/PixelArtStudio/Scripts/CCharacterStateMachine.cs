using System;
using System.Collections.Generic;
using UnityEngine;


#region CCharacterStateM
/*

*/
#endregion

[Serializable]
public class CAnimationParam
{
    public enum EType
    {
        Int,
        Float,
        Bool,
        Trigger
    }
    public EType type;
    public string paramName;
    public string dicID;
    public int hash;

    private Animator _animator;

    public void Init(Animator animator)
    {
        if(paramName == "" || paramName.Length == 0)
        {
            Debug.LogWarning("paramName == \"\" || paramName.Length == 0");
            return;
        }

        if (dicID == "" || dicID.Length == 0)
        {
            Debug.LogWarning("dicID == \"\" || dicID.Length == 0");
            return;
        }

        hash = Animator.StringToHash(paramName);

        _animator = animator;
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
        if (type != EType.Int)
        {
            Debug.LogWarning("type != EType.Int");
            return;
        }
        _animator.SetInteger(hash, input);
    }
    public void SetParam(float input)
    {
        if(type != EType.Float)
        {
            Debug.LogWarning("type != EType.Float");
            return;
        }
        _animator.SetFloat(hash, input);
    }
    public void SetParam(bool input)
    {
        if (type != EType.Bool)
        {
            Debug.LogWarning("type != EType.Bool");
            return;
        }
        _animator.SetBool(hash, input);
    }
    public void SetParam()
    {
        if (type != EType.Trigger)
        {
            Debug.LogWarning("type != EType.Trigger");
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
        Walk
    }

    #region 인스펙터
    [Header("애니메이션")]
    [SerializeField] private Animator _animator;
    [SerializeField] private CAnimationParam[] _animationParams;

    #endregion

    #region 내부 변수
    // 딕셔너리 고려.
    private Dictionary<string, CAnimationParam> _paramDic;
    #endregion

    #region 유니티 이벤트
    void Awake()
    {
        if(_animator == null )
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

        if (_animationParams.Length > 0)
        {
            Debug.Log($"_animationParams.Length == {_animationParams.Length}");

            for (int i = 0; i < _animationParams.Length; i++)
            {
                _animationParams[i].Init(_animator);
                _paramDic[_animationParams[i].dicID] = _animationParams[i];
            }
        }
        else
        {
            Debug.Log("_animationParams.Length == 0");
        }
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
