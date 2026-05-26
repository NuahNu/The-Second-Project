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
    [SerializeField] private KeyCode _DebugKey = KeyCode.M;
    #endregion

    #region 내부 변수

    #endregion

    #region 유니티 이벤트
    void Awake()
    {
        if (_player.IsNull("_player")) return;
        if (_tileMapMaker.IsNull("_tileMapMaker")) return;
        if (_spawner.IsNull("_spawner")) return;
        if (_gameCamera.IsNull("_gameCamera")) return;
        if (_playerInput.IsNull("_playerInput")) return;
    }

    void Start()
    {
        Func();
    }

    void Update()
    {
        if (_useDebugKey && Input.GetKeyDown(_DebugKey))
        {
            Func();
        }
    }
    #endregion

    #region public

    #endregion

    #region protected

    #endregion

    #region private
    private void Func()
    {
        // 맵 생성
        _tileMapMaker.MakeMap();
        // 플레이어 스폰 포인트로 이동
        _player.Warp(_tileMapMaker.PlayerSpawnPos);
        // 적 스폰 및 가시화 
        _spawner.SpawnEnemy(_tileMapMaker.BossSpawnPos);
        // 시작

        // 플레이어를 제외한 모두 비활성화
        // '' 제거
    }
    #endregion
}
