using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


#region CPlayerAgent
/*

*/
#endregion

public class CPlayerAgent : CAgent
{
    #region 인스펙터

    #endregion

    #region 내부 변수
    private InputActions _actions;
    private Dictionary<string, InputAction> _actionDic;
    #endregion

    #region 유니티 이벤트
    protected override void Awake()
    {
        base.Awake();

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
    protected override void Update()
    {
        _inputDir = _actionDic["Move"].ReadValue<Vector2>();
        _inputDir.x *= _gridCellSize.x;
        _inputDir.y *= _gridCellSize.y;
        base.Update();
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
        _actionDic["Aim"].started += Aimtrue;
        _actionDic["AimDir"].started += Aimtrue;

        _actionDic["Aim"].canceled += Aimfalse;
        _actionDic["AimDir"].canceled += Aimfalse;
    }

    private void Unsubscribe()
    {
        _actionDic["Aim"].started -= Aimtrue;
        _actionDic["AimDir"].started -= Aimtrue;

        _actionDic["Aim"].canceled -= Aimfalse;
        _actionDic["AimDir"].canceled -= Aimfalse;
    }

    private void Aimtrue(InputAction.CallbackContext obj)
    {
        SetAim(true);
    }

    private void Aimfalse(InputAction.CallbackContext obj)
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
