using System;
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
    [SerializeField] private Grid _grid;
    #endregion

    #region 내부 변수
    private NavMeshAgent _agent;
    protected Vector2 _inputDir = Vector2.zero;

    private Vector2[] _dirArr;

    private Vector2 _lastDir = new Vector2(0, -1);

    public event Action<Vector2> OnDirChange;

    // 그리드 관련 값
    protected Vector2 _gridCellSize;
    #endregion

    #region 유니티 이벤트
    protected virtual void Awake()
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

        if (_grid.IsNull("_grid")) return;

        InitGridData();

        _dirArr = new Vector2[8];

        float x = _gridCellSize.x;
        float y = _gridCellSize.y;

        _dirArr[0] = new Vector2(x, 0);
        _dirArr[1] = new Vector2(x, y).normalized;
        _dirArr[2] = new Vector2(0, y);
        _dirArr[3] = new Vector2(-x, y).normalized;
        _dirArr[4] = new Vector2(-x, 0);
        _dirArr[5] = new Vector2(-x, -y).normalized;
        _dirArr[6] = new Vector2(0, -y);
        _dirArr[7] = new Vector2(x, -y).normalized;
    }


    void Start()
    {

    }



    protected virtual void Update()
    {
        if (_inputDir != Vector2.zero || _target == null)
        {
            _inputDir.Normalize();

            if (_agent.hasPath) _agent.ResetPath();

            _agent.Move(_inputDir * _speed * Time.deltaTime);
            UpdateDir(_inputDir);
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
    private static float agentDrift = 0.0001f; // minimal
    private void SetDestination(Transform target)
    {
        if (target == null) return;

        Vector3 driftPos = target.position;
        if (Mathf.Abs(this.transform.position.x - target.transform.position.x) < agentDrift)
        {
            driftPos = target.transform.position + new Vector3(agentDrift, 0f, 0f);
        }
        _agent.SetDestination(driftPos);

        Vector2 dv = _agent.desiredVelocity;

        float max = 0;
        int index = -1;
        for (int i = 0; i < 8; i++)
        {
            float dot = Vector2.Dot(_dirArr[i], dv);
            if (dot > max)
            {
                max = dot;
                index = i;
            }
        }
        _agent.velocity = _dirArr[index] * _speed;
        UpdateDir(_dirArr[index]);
    }

    private void UpdateDir(Vector2 newDir)
    {
        if (_lastDir == newDir) return;
        _lastDir = newDir;
        OnDirChange?.Invoke(_lastDir);
    }

    private void InitGridData()
    {
        _gridCellSize = _grid.cellSize;
    }
    #endregion
}
