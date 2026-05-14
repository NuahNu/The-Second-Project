using UnityEngine;
using UnityEngine.AI;


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
    }

    void Start()
    {
    }

    void Update()
    {
        Vector2 dir = Vector2.zero;

        if (Input.GetKey(KeyCode.W)) dir.y += 1;
        if (Input.GetKey(KeyCode.S)) dir.y -= 1;
        if (Input.GetKey(KeyCode.A)) dir.x -= 1;
        if (Input.GetKey(KeyCode.D)) dir.x += 1;


        if (dir != Vector2.zero || _target == null)
        {
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
    static float agentDrift = 0.0001f; // minimal
    void SetDestination(Transform target)
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
