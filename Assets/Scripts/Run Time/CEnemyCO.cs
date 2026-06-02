using UnityEngine;


#region CBehaviorTree
/*
적 객체 소환 후 이 클래스를 컴포넌트로 추가.
*/
#endregion

public class CEnemyCO : MonoBehaviour
{
    #region 인스펙터
    [Header("")]

    // 추적 할 플레이어
    [SerializeField] private Transform _player;
    [SerializeField] private int _detectiveRange = 5;
    [SerializeField] private int _attackRange = 2;
    #endregion

    #region 내부 변수
    private CCharacter _character;
    private Transform _origin;

    private CSelectorNode _rootNode;
    private CSequenceNode _attackSequence;
    private CSequenceNode _detectiveSequence;
    private CActionNode _idleAction;
    private CActionNode _returnAction;
    #endregion

    #region 유니티 이벤트
    void Awake()
    {

    }

    void Start()
    {
        SetOriginPos(transform);

        _character = GetComponent<CCharacter>();

        _attackSequence = new CSequenceNode();
        _attackSequence.Add(new CActionNode(CheckInAttackRange));
        _attackSequence.Add(new CActionNode(Attack));

        _detectiveSequence = new CSequenceNode();
        _detectiveSequence.Add(new CActionNode(CheckInDetectiveRange));
        _detectiveSequence.Add(new CActionNode(TraceTarget));

        _returnAction = new CActionNode(ReturnAction);
        _idleAction = new CActionNode(IdleAction);

        // 루트 노드 생성
        // 우선순위 주의
        _rootNode = new CSelectorNode();
        _rootNode.Add(_attackSequence);
        _rootNode.Add(_detectiveSequence);
        _rootNode.Add(_returnAction);
        _rootNode.Add(_idleAction);
    }

    void Update()
    {
        _rootNode.Evaluate();
    }
    #endregion

    #region public
    public void SetOriginPos(Transform origin)
    {
        _origin = origin;
    }

    public void SetPlayer(Transform player)
    {
        _player = player;
    }
    #endregion

    #region protected

    #endregion

    #region private
    IBTNode.EState Attack()
    {
        Debug.Log("공격중");
        return IBTNode.EState.Run;
    }

    IBTNode.EState CheckInAttackRange()
    {
        if (_player == null)
            return IBTNode.EState.Failed;
        if (Vector3.Distance(transform.position, _player.position) < _attackRange)
        {
            //Debug.Log("공격 범위에 대상 감지 됨");
            _character.SetTarget(null);
            _character.ChangeState("Attack");
            return IBTNode.EState.Success;
        }
        _character.ChangeState("Idle");
        return IBTNode.EState.Failed;
    }

    IBTNode.EState TraceTarget()
    {
        if (Vector3.Distance(transform.position, _player.position) >= 0.1f)
        {
            //Debug.Log("Trace!!");
            return IBTNode.EState.Run;
        }
        else
            return IBTNode.EState.Failed;
    }

    IBTNode.EState IdleAction()
    {
        Debug.Log("Idle..");
        return IBTNode.EState.Run;
    }

    IBTNode.EState ReturnAction()
    {
        if (Vector3.Distance(transform.position, _origin.position) >= 0.1f)
        {
            //Debug.Log("Return..");
            _character.SetTarget(_origin);
            return IBTNode.EState.Run;
        }
        _character.SetTarget(null);
        return IBTNode.EState.Failed;
    }

    IBTNode.EState CheckInDetectiveRange()
    {
        if (Vector3.Distance(transform.position, _player.position) < _detectiveRange)
        {
            //Debug.Log("Detective..");
            _character.SetTarget(_player);
            return IBTNode.EState.Success;
        }
        _character.SetTarget(null);

        return IBTNode.EState.Failed;
    }
    #endregion
}
