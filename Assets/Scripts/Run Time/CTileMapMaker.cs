using NavMeshPlus.Components;
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Tilemaps;


#region CWorldMaker
/*

맵의 크기와 속성 등을 정해준다.
맵에 생성될 수 있는 구역의 개수를?

어뚷게 생성을 하고, 정보를 가져와서 맵을 만들까?
이진 공간 분할을 통해 구역을 나누고
연결은? 어떻게든 하고

Cellular Automata로 부드럽게 만든다. - 언제까지?
*/
#endregion


[Serializable]
public class CTilemapData
{
    public Tilemap tilemap;
    public TilemapRenderer tilemapRenderer;
    public TilemapCollider2D tilemapCollider2D;

    //public NavMeshModifier meshModifier;
}

// 보류
//[Serializable]
//public class CGridData
//{
//    
//    public Grid grid;
//
//
//}

public class CTileMapMaker : MonoBehaviour
{
    public enum TilemapLayer
    {
        // 제일 아래에 그려진다.
        // 화면에 그려지지는 않지만 네비메쉬 생성에 사용할 타일맵
        Floor,
        Hole,
        // 플레이어와 같은 Order Layer에 있어야 한다.
        Structure,
        // 얘들은 그려지면 안되지
        Collider,
        EndTrigger,
        Count
    }

    #region 인스펙터
    [SerializeField] private WorldData _worldData;
    [SerializeField] private BinarySpacePartitioningData _BSPData;
    [SerializeField] private CellularAutomataData _CAData;

    [Header("타일맵 관련")]
    [SerializeField] private GameObject _gridObeject;
    // 그냥 그리드를 생성자에 넣으면 자동으로 데이터를 만드는 클래스를 고려해본다.
    // 이 경우 게임 전용 그리드 프리팹을 만드는게 좋을 듯.

    [Header("타일맵 리소스")]
    [SerializeField] private TileBase _floorTile;
    [SerializeField] private TileBase _structureFloorTile;
    [SerializeField] private TileBase _wallTile;
    [SerializeField] private TileBase _wallColliderTile;
    [SerializeField] private TileBase _holeTile;
    [SerializeField] private TileBase _endTile;
    [SerializeField] private TileBase _endTriggerTile;

    [Header("트리거 스포너?")]

    [Header("네비메쉬")]
    [SerializeField] private NavMeshSurface _surfase2D;

    [Header("디버그 키")]
    [SerializeField] private bool _showMapLog = false;
    [SerializeField] private bool _useDebugKey = false;
    [SerializeField] private KeyCode _makeDataKey = KeyCode.M;


    public enum EGizmoMode
    {
        Default,
        New
    }

    [Header("기즈모")]
    [SerializeField] private bool _gizmoFlag = false;
    [SerializeField] private bool _stFlag = false;
    [SerializeField] private bool _wireFlag = false;
    [SerializeField] private EGizmoMode _gizmoMode = EGizmoMode.Default;
    [SerializeField] private float _gizmoColorRatio = 1f;
    [SerializeField] private Color _gizmoDefualtColor = Color.red;
    #endregion

    #region 내부 변수
    private ETileType[,] _tileTypeArray;

    private CWorldMaker _worldMaker;

    private Grid _grid;
    private Dictionary<TilemapLayer, CTilemapData> tilemapDic;

    private Rect _gridRect;

    private Vector2Int _playerSpawnPos;
    private Vector3 _playerWorldSpawnPos;
    private Vector2Int _bossSpawnPos;
    private Vector3 _bossWorldSpawnPos;
    private List<Vector2Int> _enemySpawnPos;
    private List<Vector3> _enemyWorldSpawnPos;
    #endregion

    public Vector3 PlayerSpawnPos
    {
        get
        {
            return _playerWorldSpawnPos;
        }
    }

    public List<Vector3> EnemySpawnPos
    {
        get
        {
            return _enemyWorldSpawnPos;
        }
    }

    public Vector3 BossSpawnPos
    {
        get
        {
            return _bossWorldSpawnPos;
        }
    }

    public event Action<Rect> OnMakeMap;

    #region 유니티 이벤트
    void Awake()
    {
        _worldMaker = new CWorldMaker();

        if (_surfase2D.IsNull("_surfase2D")) return;

        if (_floorTile.IsNull("_floorTile")) return;
        if (_structureFloorTile.IsNull("_structureFloorTile")) return;
        if (_wallTile.IsNull("_wallTile")) return;
        if (_wallColliderTile.IsNull("_wallColliderTile")) return;
        if (_holeTile.IsNull("_holeTile")) return;
        if (_endTile.IsNull("_endTile")) return;
        if (_endTriggerTile.IsNull("_endTriggerTile")) return;


        MakeTileMapDic();
        // 스폰 위치 지정.
    }

    void Start()
    {

    }

    void Update()
    {
        if (_useDebugKey && Input.GetKeyDown(_makeDataKey))
        {
            MakeNewMap();
        }
    }

    private void OnDrawGizmos()
    {
        if (!_gizmoFlag) return;
        if (_worldMaker == null) return;
        if (_worldMaker.BSPRootNode == null) return;

        TreeNode rootNode = _worldMaker.BSPRootNode;
        if (rootNode == null) return;

        List<TreeNode> leafnodeList = _worldMaker.leafNodeList;

        switch (_gizmoMode)
        {
            case EGizmoMode.Default:
                if (_stFlag)
                    rootNode.DrawAllStRectInt(_gizmoDefualtColor, _gizmoColorRatio, _wireFlag);
                else
                    rootNode.DrawAllRectInt(_gizmoDefualtColor, _gizmoColorRatio, _wireFlag);
                break;
            default:
                rootNode.DrawAllRead(Color.yellow, _gizmoColorRatio, _wireFlag);
                for (int i = 0; i < leafnodeList.Count; i++)
                {
                    TreeNode node = leafnodeList[i];
                    switch (node.roomType)
                    {
                        case ERoomType.Start:
                            Gizmos.color = Color.red; break;
                        case ERoomType.Road:
                            Gizmos.color = Color.green; break;
                        case ERoomType.Boss:
                            Gizmos.color = Color.blue; break;
                        default:
                            Gizmos.color = Color.black; break;
                    }
                    if (_stFlag)
                        node.DrawStRectInt(_wireFlag);
                    else
                        node.DrawRectInt(_wireFlag);
#if UNITY_EDITOR
                    if (_worldMaker.NodeDepthDic.ContainsKey(node))
                        Handles.Label(node.standardRoomRect.center, _worldMaker.NodeDepthDic[node].ToString());
#endif
                }
                break;
        }
    }
    #endregion

    #region public
    public void MakeNewMap(bool flag = true)
    {
        StartCoroutine(Co_MakeMap(flag));
        Camera.main.transform.position = new Vector3(PlayerSpawnPos.x, PlayerSpawnPos.y, Define.CAMERA_Z);
    }
    #endregion

    #region private
    private void MakeTileMapDic()
    {
        if (_gridObeject.IsNull("_gridObeject")) return;

        if (!_gridObeject.TryGetComponent(out _grid))
        {
            Debug.LogWarning("인스펙터 확인");
            return;
        }

        tilemapDic = new Dictionary<TilemapLayer, CTilemapData>();

        tilemapDic[TilemapLayer.Floor] = FindTilemap(Define.NAME_FLOOR);
        tilemapDic[TilemapLayer.Floor].tilemapRenderer.sortingOrder = Define.ORDER_FLOOR;

        tilemapDic[TilemapLayer.Structure] = FindTilemap(Define.NAME_STRUCTURE);
        tilemapDic[TilemapLayer.Structure].tilemapRenderer.sortingOrder = Define.ORDER_STRUCTURE;

        tilemapDic[TilemapLayer.Collider] = FindTilemap(Define.NAME_COLLIDER);
        tilemapDic[TilemapLayer.Collider].tilemapRenderer.sortingOrder = Define.ORDER_COLLIDER;

        tilemapDic[TilemapLayer.Hole] = FindTilemap(Define.NAME_HOLE);
        tilemapDic[TilemapLayer.Hole].tilemapRenderer.sortingOrder = Define.ORDER_HOLE;

        tilemapDic[TilemapLayer.EndTrigger] = FindTilemap(Define.NAME_END);
        tilemapDic[TilemapLayer.EndTrigger].tilemapRenderer.sortingOrder = Define.ORDER_FLOOR;
    }

    private CTilemapData FindTilemap(string name)
    {
        Transform transform = _gridObeject.transform.Find(name);

        if (transform.IsNull(name)) return null;

        GameObject gameObject = transform.gameObject;

        CTilemapData tilemapData = new CTilemapData();

        if (!gameObject.TryGetComponent(out tilemapData.tilemap))
        {
            Debug.LogWarning($"프리팹의 {name}에 Tilemap이 없음");
            return null;
        }

        if (!gameObject.TryGetComponent(out tilemapData.tilemapRenderer))
        {
            Debug.LogWarning($"프리팹의 {name}에 TilemapRenderer이 없음");
            return null;
        }

        if (gameObject.TryGetComponent(out tilemapData.tilemapCollider2D))
        {
            Debug.Log($"프리팹의 {name}에 tilemapCollider2D이 있음");
        }

        return tilemapData;
    }

    // 얘를 매 프레임 해준다?
    private void SetTile()
    {
        for (int i = 0; i < (int)TilemapLayer.Count; i++)
        {
            tilemapDic[(TilemapLayer)i].tilemap.ClearAllTiles();
        }

        _playerSpawnPos = Vector2Int.zero;
        _playerWorldSpawnPos = Vector3.zero;
        if (_enemySpawnPos == null)
            _enemySpawnPos = new List<Vector2Int>();
        if (_enemyWorldSpawnPos == null)
            _enemyWorldSpawnPos = new List<Vector3>();
        _enemySpawnPos.Clear();
        _enemyWorldSpawnPos.Clear();
        _bossSpawnPos = Vector2Int.zero;
        _bossWorldSpawnPos = Vector3.zero;

        // 얘를 반복 돌면서 읽어와 타일을 깐다.
        for (int x = 0; x < _tileTypeArray.GetLength(0); x++)
        {
            for (int y = 0; y < _tileTypeArray.GetLength(1); y++)
            {
                ETileType tt = _tileTypeArray[x, y];

                Vector3Int pos = new Vector3Int(x, y, 0);
                if (tt.HasFlag(ETileType.Floor))
                {
                    tilemapDic[TilemapLayer.Floor].tilemap.SetTile(pos, _floorTile);

                    if (tt.HasFlag(ETileType.PlayerSpawn))
                    {
                        _playerSpawnPos = new Vector2Int(x, y);
                        _playerWorldSpawnPos = _grid.GetCellCenterWorld(pos);
                    }
                    else if (tt.HasFlag(ETileType.EnemySpawn))
                    {
                        _enemySpawnPos.Add(new Vector2Int(x, y));
                        _enemyWorldSpawnPos.Add(_grid.GetCellCenterWorld(pos));
                    }
                    else if (tt.HasFlag(ETileType.BossSpawn))
                    {
                        tilemapDic[TilemapLayer.Floor].tilemap.SetTile(pos, _endTile);
                        tilemapDic[TilemapLayer.EndTrigger].tilemap.SetTile(pos, _endTriggerTile);
                        _bossSpawnPos = new Vector2Int(x, y);
                        _bossWorldSpawnPos = _grid.GetCellCenterWorld(pos);
                    }
                }
                else if (tt.HasFlag(ETileType.Hole))
                {
                    // 트리거 타일 넣기.
                    tilemapDic[TilemapLayer.Hole].tilemap.SetTile(pos, _holeTile);
                }
                else if (tt.HasFlag(ETileType.Wall))
                {
                    // 벽 바닥 그리기
                    // 벽 타일맵에 벽 그리기
                    tilemapDic[TilemapLayer.Floor].tilemap.SetTile(pos, _structureFloorTile);
                    tilemapDic[TilemapLayer.Collider].tilemap.SetTile(pos, _wallColliderTile);
                    tilemapDic[TilemapLayer.Structure].tilemap.SetTile(pos, _wallTile);
                }
            }
        }
    }

    private IEnumerator Co_MakeMap(bool flag = true)
    {
        if (flag)
        {

            Vector3 gridPos = Vector3.zero;
            gridPos.y = -_grid.cellSize.y * _worldData.mapSize.y / 2;
            _grid.transform.position = gridPos;

            _gridRect.x = 0;
            _gridRect.y = -_grid.cellSize.y * _worldData.mapSize.y / 2;
            _gridRect.width = _grid.cellSize.x * _worldData.mapSize.x;
            _gridRect.height = _grid.cellSize.y * _worldData.mapSize.y;

            _tileTypeArray = _worldMaker.MakeWorld(_worldData, _BSPData, _CAData);

            if (_showMapLog)
            {
                int rows = _tileTypeArray.GetLength(0);
                int cols = _tileTypeArray.GetLength(1);
                string mapData = $"TileMap 데이터: \n";

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        ETileType tile = _tileTypeArray[i, j];
                        mapData += tile.HasFlag(ETileType.Wall) ? "■" : tile.HasFlag(ETileType.Floor) ? "□" : "※";
                    }
                    mapData += "\n"; // 한 줄 끝날 때마다 줄바꿈
                }

                Debug.Log(mapData);
            }

            //Physics2D.SyncTransforms();

            SetTile();
            yield return null;
        }
        //_surfase2D.BuildNavMesh();

        // Floor와 Hole을 기준으로 만든다.
        tilemapDic[TilemapLayer.Floor].tilemapCollider2D.enabled = true;
        tilemapDic[TilemapLayer.Hole].tilemapCollider2D.enabled = true;

        yield return _surfase2D.BuildNavMeshAsync();
        //_surfase2D.UpdateNavMesh(_surfase2D.navMeshData);
        //NavMeshBuilder.UpdateNavMeshDataAsync(); // ???

        // 비활성화 해야 하는 컴포넌트
        // 내비메쉬 생성을 위한 콜라이더이다.
        tilemapDic[TilemapLayer.Floor].tilemapCollider2D.enabled = false;
        tilemapDic[TilemapLayer.Hole].tilemapCollider2D.enabled = false;

        OnMakeMap?.Invoke(_gridRect);
        Debug.Log($"ReSizeRect {_gridRect}");
        //Debug.Log($"최대 깊이{_worldMaker.MultiTreeDepth}");
    }
    #endregion
}