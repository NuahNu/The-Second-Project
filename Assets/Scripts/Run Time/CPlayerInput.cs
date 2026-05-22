using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


#region CPlayerAgent
/*
 이런 식으로 만드는건 아닌 것 같은데 고민할 시간이 없다. 일단 해봐

캐릭터별로 agent를 만든다?
키 입력을 하는 주체를 변경하는게 좋을 것 같음. 지금 방식은 좋지 않음
*/
#endregion

public class CPlayerInput : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private GameObject _target;
    #endregion

    #region 내부 변수
    private InputActions _actions;
    private Dictionary<string, InputAction> _actionDic;

    private CAgent _agent;
    private CCharacter _character;

    private bool _aimFlag = false;
    private Vector2 _aimDir = Vector2.zero;
    #endregion

    #region 유니티 이벤트
    private void Awake()
    {
        InintTarget();
        InitAction();
    }

    private void OnEnable()
    {
        foreach (var action in _actionDic.Values)
        {
            action.Enable();
        }
        Subscribe();
    }

    private void OnDisable()
    {
        foreach (var action in _actionDic.Values)
        {
            action.Disable();
        }
        Unsubscribe();
    }

    private void OnDestroy()
    {
        _actionDic.Clear();
        _actionDic = null;
    }
    private void Update()
    {
        // 이것도 아래처럼 분리... {_character.SetAimDir(_aimDir);} 참고
        _character.SetMoveDir(_actionDic["Move"].ReadValue<Vector2>());
    }
    #endregion

    #region public
    public event Action<bool> OnAimChange;
    public event Action<Vector2> OnAimDirChange;

    public void InitTarget(GameObject target)
    {
        if (target != null)
            _target = target;
        InintTarget();
    }
    #endregion

    #region protected

    #endregion

    #region private
    private void InintTarget()
    {
        if (_target.IsNull("_target")) return;

        if (!_target.TryGetComponent(out _agent))
        {
            if (_agent.IsNull("_agent")) return;
        }
        if (!_target.TryGetComponent(out _character))
        {
            if (_character.IsNull("_agent")) return;
        }
    }
    private void InitAction()
    {
        _actions = new InputActions();

        _actionDic = new Dictionary<string, InputAction>();

        _actionDic["Move"] = _actions.Player.Move;
        _actionDic["AimDir"] = _actions.Player.AimDirectional;
        _actionDic["AimAng"] = _actions.Player.AimAngular;
        _actionDic["Aim"] = _actions.Player.Aim;
    }

    private void Subscribe()
    {

        _actionDic["Aim"].started += AimTrue;
        _actionDic["Aim"].canceled += AimFalse;

        _actionDic["AimDir"].started += AimTrue;
        _actionDic["AimDir"].performed += AimDirKey;
        _actionDic["AimDir"].canceled += AimFalse;

        _actionDic["AimAng"].performed += AimDirMouse;
    }

    private void Unsubscribe()
    {
        _actionDic["Aim"].started -= AimTrue;
        _actionDic["Aim"].canceled -= AimFalse;

        _actionDic["AimDir"].started -= AimTrue;
        _actionDic["AimDir"].performed -= AimDirKey;
        _actionDic["AimDir"].canceled -= AimFalse;

        _actionDic["AimAng"].performed -= AimDirMouse;
    }

    private void AimDirMouse(InputAction.CallbackContext obj)
    {
        Vector2 mousePos = obj.ReadValue<Vector2>();
        // 연산량이 많다. 이걸 매번 한다고?
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector2 dir = worldPos - _target.transform.position;
        SetAimDir(dir.GetClosestDirection());
    }

    private void AimDirKey(InputAction.CallbackContext obj)
    {
        Vector2 keyDir = obj.ReadValue<Vector2>();
        Vector2 dir = keyDir.GetClosestDirection();
        SetAimDir(dir);
    }

    private void AimTrue(InputAction.CallbackContext obj)
    {
        SetAim(true);
    }

    private void AimFalse(InputAction.CallbackContext obj)
    {
        SetAim(false);
    }

    private void SetAim(bool flag)
    {
        if (_aimFlag == flag) return;
        _aimFlag = flag;
        Debug.Log($"Aim {_aimFlag}");

        if (_aimFlag == true)
            _character.ChangeState("Aim");
        else
            _character.ChangeState("Idle");

        OnAimChange?.Invoke(_aimFlag);
    }

    private void SetAimDir(Vector2 dir)
    {
        if (_aimDir == dir) return;
        _aimDir = dir;
        //Debug.Log($"Aim {_aimDir}");

        _character.SetAimDir(_aimDir);

        OnAimDirChange?.Invoke(_aimDir);
    }
    #endregion
}
