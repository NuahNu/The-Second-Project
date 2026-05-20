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

public class CPlayerAgent : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private CAgent _agent;
    #endregion

    #region 내부 변수
    private InputActions _actions;
    private Dictionary<string, InputAction> _actionDic;
    #endregion

    #region 유니티 이벤트
    private void Awake()
    {
        if (_agent.IsNull("_agent")) return;
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
        _agent.SetInputDir(_actionDic["Move"].ReadValue<Vector2>());
    }
    #endregion

    #region public

    #endregion

    #region protected

    #endregion

    #region private
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
        _actionDic["AimDir"].started += AimTrue;

        _actionDic["Aim"].canceled += AimFalse;
        _actionDic["AimDir"].canceled += AimFalse;
    }

    private void Unsubscribe()
    {
        _actionDic["Aim"].started -= AimTrue;
        _actionDic["AimDir"].started -= AimTrue;

        _actionDic["Aim"].canceled -= AimFalse;
        _actionDic["AimDir"].canceled -= AimFalse;
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
        // 같으면 리턴하는 코드
        Debug.Log($"Aim {flag}");
    }
    #endregion
}
