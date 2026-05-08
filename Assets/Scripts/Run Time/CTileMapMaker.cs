using UnityEngine;
using UnityEngine.Tilemaps;
using NavMeshPlus.Components;
using System.Collections;
using System;
using System.Collections.Generic;


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
    [SerializeField] private TileBase _holeTile;

    [Header("네비메쉬")]
    [SerializeField] private NavMeshSurface _surfase2D;

    [Header("디버그 키")]
    [SerializeField] private bool _showMapLog = false;

    [SerializeField] private KeyCode _makeDataKey = KeyCode.M;
    #endregion

    #region 내부 변수
    private ETileType[,] _tileTypeArray;

    private CWorldMaker _worldMaker;

    private Grid _grid;
    private Dictionary<TilemapLayer, CTilemapData> tilemapDic;
    //private
    #endregion

    #region 유니티 이벤트
    void Awake()
    {
        _worldMaker = new CWorldMaker();
        _tileTypeArray = _worldMaker.MakeWorld(_worldData, _BSPData, _CAData);

        if (_surfase2D.IsNull("_surfase2D")) return;

        if (_floorTile.IsNull("_floorTile")) return;
        if (_structureFloorTile.IsNull("_structureFloorTile")) return;
        if (_wallTile.IsNull("_wallTile")) return;
        if (_holeTile.IsNull("_holeTile")) return;

        MakeTileMapDic();
        // 스폰 위치 지정.
    }

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(_makeDataKey))
        {
            StartCoroutine(Co_MakeMap());
        }
    }
    #endregion

    #region public
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

        if(gameObject.TryGetComponent(out tilemapData.tilemapCollider2D))
        {
            Debug.Log($"프리팹의 {name}에 tilemapCollider2D이 있음");
        }

        return tilemapData;
    }

    // 얘를 매 프레임 해준다?
    private void SetTile()
    {
        for(int i = 0; i < (int)TilemapLayer.Count; i++)
        {
            tilemapDic[(TilemapLayer)i].tilemap.ClearAllTiles();
        }

        // 얘를 반복 돌면서 읽어와 타일을 깐다.
        for (int i = 0; i < _tileTypeArray.GetLength(0); i++)
        {
            for (int j = 0; j < _tileTypeArray.GetLength(1); j++)
            {
                ETileType tt = _tileTypeArray[i, j];

                Vector3Int pos = new Vector3Int(i, j, 0);
                if (tt.HasFlag(ETileType.Floor))
                {
                    tilemapDic[TilemapLayer.Floor].tilemap.SetTile(pos, _floorTile);
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
                    tilemapDic[TilemapLayer.Structure].tilemap.SetTile(pos, _wallTile);
                }
            }
        }
    }

    private IEnumerator Co_MakeMap()
    {
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
    }
    #endregion
}