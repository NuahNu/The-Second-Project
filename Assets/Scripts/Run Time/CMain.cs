using UnityEngine;


#region CMain
/*

*/
#endregion

public class CMain : MonoBehaviour
{
    private enum EPhase
    {
        MakeMap,
        PlayerSpawn,
        Spawn,
        Play,
        Clear,
        End
    }

    #region 인스펙터
    [SerializeField] private CCharacter _player;

    [SerializeField] private CTileMapMaker _tileMapMaker;
    [SerializeField] private CSpawner _spawner;
    [SerializeField] private CCamera _gameCamera;

    // inputManager로 변경 고려
    [SerializeField] private CPlayerInput _playerInput;

    [Header("디버그 키")]
    [SerializeField] private bool _useDebugKey = false;
    [SerializeField] private KeyCode _startKey = KeyCode.F1;
    [SerializeField] private KeyCode _endKey = KeyCode.F2;
    #endregion

    #region 내부 변수
    public static CMain Instance;
    #endregion

    #region 유니티 이벤트
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("Instance != null && Instance != this");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (_player.IsNull("_player")) return;
        if (_tileMapMaker.IsNull("_tileMapMaker")) return;
        if (_spawner.IsNull("_spawner")) return;
        if (_gameCamera.IsNull("_gameCamera")) return;
        if (_playerInput.IsNull("_playerInput")) return;
    }

    void Start()
    {
        FuncStart(false);
    }

    void Update()
    {
        if (_useDebugKey)
        {
            if (Input.GetKeyDown(_startKey))
            {
                FuncStart();
            }
            if (Input.GetKeyDown(_endKey))
            {
                FuncEnd();
            }
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
    #endregion

    #region public
    public void FuncStart(bool flag = true)
    {
        // 맵 생성
        // ㄴ 완료 이벤트 있음.
        _tileMapMaker.MakeNewMap(flag);
        // 플레이어 스폰 포인트로 이동
        // 이건 순간이동 시켜도 알 수 없을것이다. 배경이 없으니까?
        _player.Warp(_tileMapMaker.PlayerSpawnPos);
        // 적 스폰 및 가시화 
        // 알파값을 천천히 변경하는 기능.
        foreach (var pos in _tileMapMaker.EnemySpawnPos)
        {
            _spawner.SpawnEnemy(pos);
        }

        //_spawner.SpawnEnemy(_tileMapMaker.BossSpawnPos);

        // 시작

        // 플레이어를 제외한 모두 비활성화 / 비 가시화?
        // 초기화 하거나 정리해주는 Clear() 작성.
        // ㄴ 기존 생성 함수에 앞에서 정리해주는 녀석들도 있음.
    }
    public void FuncEnd()
    {
        _spawner.Clear();
    }

    public CProjectile SpawnProjectile(EProjectileType type)
    {
        return _spawner.SpawnProjectile(type);
    }
    #endregion

    #region protected

    #endregion

    #region private

    #endregion
}
