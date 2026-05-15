using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;


#region CAgent
/*

*/
#endregion

[RequireComponent(typeof(NavMeshAgent))]
public class CAgent : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private Transform _target;
    [SerializeField] private float _speed = 5;
    #endregion

    #region 내부 변수
    private NavMeshAgent _agent;
    private InputActions _actions;
    private InputAction moveAction;
    #endregion

    #region 유니티 이벤트
    void Awake()
    {
        if (_agent == null)
        {
            if (!TryGetComponent(out _agent))
            {
                this.gameObject.AddComponent<NavMeshAgent>();
            }
        }
        if (_agent.IsNull("_agent")) return;

        _agent.updateRotation = false;
        _agent.updateUpAxis = false;

        InitAction();
    }

    void Start()
    {
    }

    private void OnEnable()
    {
        moveAction.Enable();
        //moveAction.started += Started;
        //moveAction.performed += Performed;
        //moveAction.canceled += Canceled;
    }

    private void OnDisable()
    {
        moveAction.Disable();
        //moveAction.started -= Started;
        //moveAction.performed -= Performed;
        //moveAction.canceled -= Canceled;
    }

    void Update()
    {
        Vector2 dir = moveAction.ReadValue<Vector2>();

        if (dir != Vector2.zero || _target == null)
        {
            dir.Normalize();

            if (_agent.hasPath) _agent.ResetPath();

            _agent.Move(dir * _speed * Time.deltaTime);
            return;
        }
        SetDestination(_target);
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
        moveAction = _actions.Player.Move;
    }

    private static float agentDrift = 0.0001f; // minimal
    private void SetDestination(Transform target)
    {
        Vector3 driftPos = target.position;
        if (Mathf.Abs(this.transform.position.x - target.transform.position.x) < agentDrift)
        {
            driftPos = target.transform.position + new Vector3(agentDrift, 0f, 0f);
        }
        _agent.SetDestination(driftPos);
    }
    #endregion
}
