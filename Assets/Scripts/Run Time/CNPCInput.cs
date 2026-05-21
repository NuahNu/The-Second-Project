using UnityEngine;


#region CNPCInput
/*

*/
#endregion

public class CNPCInput : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private GameObject _target;
    #endregion

    #region 내부 변수
    private CAgent _agent;
    private CCharacter _character;
    #endregion

    #region 유니티 이벤트
    void Awake()
    {
        InintTarget();
    }

    void Start()
    {

    }

    void Update()
    {

    }
    #endregion

    #region public
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
    #endregion
}
