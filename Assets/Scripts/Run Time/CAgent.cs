using System;
using UnityEngine;
using UnityEngine.AI;


#region CAgent
/*
NavMeshAgent 관리 클래스
*/
#endregion

[RequireComponent(typeof(NavMeshAgent))]
public class CAgent : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private Transform _target;
    // NavMeshAgent의 속도를 사용하도록 변경 고려. 둘 중 하나만 써야지.
    [SerializeField] private float _speed = 5;
    [SerializeField] private Grid _grid;
    #endregion

    #region 내부 변수
    private NavMeshAgent _agent;
    // 하위 클래스에서 입력한 값으로 인한 변화값
    protected Vector2 _inputDir = Vector2.zero;

    //private Vector2[] _dirArr;

    private Vector2 _lastDir = new Vector2(0, -1);

    public event Action<Vector2> OnDirChange;
    public event Action<bool> OnMoveChange;
    public event Action<bool> OnWalkChange;

    private bool _isMoving = false;
    private bool _isWalk = false;
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

        // InitData를 호출하지 않는 녀석들은 이게 필요함.
        if (_grid.IsNull("_grid")) return;

        InitGridData();
        //


        //_dirArr = new Vector2[8];

        //float x = _gridCellSize.x;
        //float y = _gridCellSize.y;

        //_dirArr[0] = new Vector2(x, 0);
        //_dirArr[1] = new Vector2(x, y).normalized;
        //_dirArr[2] = new Vector2(0, y);
        //_dirArr[3] = new Vector2(-x, y).normalized;
        //_dirArr[4] = new Vector2(-x, 0);
        //_dirArr[5] = new Vector2(-x, -y).normalized;
        //_dirArr[6] = new Vector2(0, -y);
        //_dirArr[7] = new Vector2(x, -y).normalized;
    }


    void Start()
    {

    }



    protected virtual void Update()
    {

    }
    #endregion

    #region public
    public void InitData(CCharacterDataSO data, Grid grid)
    {
        _speed = data.RunSpeed;
        //_speed = data.WalkSpeed;
        if (grid != null)
            _grid = grid;
        InitGridData();
    }
    public void MoveAgent()
    {
        UpdateMoveFlag(false);
        if (_inputDir != Vector2.zero)
        {
            Vector2 dir = _inputDir;
            dir.x = _inputDir.x * _gridCellSize.x;
            dir.y = _inputDir.y * _gridCellSize.y;

            dir.Normalize();

            if (_agent.hasPath) _agent.ResetPath();

            _agent.Move(dir * _speed * Time.deltaTime);

            UpdateMoveFlag(true);
            UpdateDir(dir);
            return;
        }
        SetDestination(_target);
    }
    public void SetInputDir(Vector2 input)
    {
        if (_inputDir != input)
            _inputDir = input;
    }
    public void Warp(Vector3 pos)
    {
        _agent.Warp(pos);
    }
    public void SetTarget(Transform target)
    {
        if(_target ==  target) return;
        _target = target;
    }
    #endregion

    #region protected
    protected void ToggleWalk()
    {
        UpdateWalk(!_isWalk);
    }
    #endregion

    #region private
    private static float agentDrift = 0.0001f; // minimal
    private void SetDestination(Transform target)
    {
        if (target == null)
        {
            if (_agent.hasPath) _agent.ResetPath();
            return;
        }

        Vector3 driftPos = target.position;
        if (Mathf.Abs(this.transform.position.x - target.transform.position.x) < agentDrift)
        {
            driftPos = target.transform.position + new Vector3(agentDrift, 0f, 0f);
        }
        _agent.SetDestination(driftPos);

        Vector2 dv = _agent.desiredVelocity;

        // 충분히 가까우면 그냥 리턴
        if (dv.SqrMagnitude() == 0)
        {
            return;
        }

        Vector2 dir = dv.GetClosestDirection();

        _agent.velocity = dir * _speed;

        //if (_agent.hasPath) _agent.ResetPath();

        UpdateMoveFlag(true);
        UpdateDir(dir);
    }

    private void UpdateDir(Vector2 newDir)
    {
        if (_lastDir == newDir) return;
        _lastDir = newDir;
        OnDirChange?.Invoke(_lastDir);
    }

    private void UpdateMoveFlag(bool flag)
    {
        if (_isMoving == flag) return;
        _isMoving = flag;
        OnMoveChange?.Invoke(_isMoving);
    }

    private void UpdateWalk(bool flag)
    {
        if (_isWalk == flag) return;
        _isWalk = flag;
        OnWalkChange?.Invoke(_isWalk);
    }

    private void InitGridData()
    {
        _gridCellSize = _grid.cellSize;
    }
    #endregion
}
