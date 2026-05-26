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
    [SerializeField] private CTileMapMaker _tileMapMaker;
    [SerializeField] private CSpawner _spawner;
    [SerializeField] private CCamera _gameCamera;

    // inputManager로 변경 고려
    [SerializeField] private CPlayerInput _playerInput;
    #endregion

    #region 내부 변수
    
    #endregion

    #region 유니티 이벤트
    void Awake()
    {
        if (_tileMapMaker.IsNull("_tileMapMaker")) return;
        if (_spawner.IsNull("_spawner")) return;
        if (_gameCamera.IsNull("_gameCamera")) return;
        if (_playerInput.IsNull("_playerInput")) return;
    }

    void Start()
    {
        
    }

    void Update()
    {
        // 맵 생성 및 가시화
        // 플레이어 스폰 포인트로 이동
        // 적 스폰 

        // 시작

        // 플레이어를 제외한 모두 비활성화
        // '' 제거
    }
    #endregion

    #region public
    
    #endregion

    #region protected
    
    #endregion

    #region private
    
    #endregion
}
