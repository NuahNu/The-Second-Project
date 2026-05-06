using UnityEngine;
using UnityEngine.Tilemaps;
using NavMeshPlus.Components;
using System.Collections;


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

public class CWorldMakerTester : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private WorldData _worldData;
    [SerializeField] private BinarySpacePartitioningData _BSPData;
    [SerializeField] private CellularAutomataData _CAData;

    [Header("타일맵 관련")]
    [SerializeField] private Grid _grid;
    // 바닥 타일맵
    // 제일 나중에 그려진다. 
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private TilemapRenderer _tilemapRenderer;

    // 구조물 타일맵
    // 플레이어와 같은 Order Layer에 있어야 한다.

    // 콜라이더 타일맵
    // 얘들은 그려지면 안되지

    // 네비메쉬 타일맵
    // 화면에 그려지지는 않지만 네비메쉬 생성에 사용할 타일맵

    [Header("타일맵 리소스")]
    [SerializeField] private TileBase _tileBase;

    [Header("네비메쉬")]
    [SerializeField] private NavMeshSurface _surfase2D;

    [Header("디버그 키")]
    [SerializeField] private bool _showMapLog = false;

    [SerializeField] private KeyCode _makeDataKey = KeyCode.M;
    [SerializeField] private KeyCode _bakeKey = KeyCode.B;
    #endregion

    #region 내부 변수
    private ETileType[,] _tileTypeArray;

    private CWorldMaker _worldMaker;

    //private
    #endregion

    #region 유니티 이벤트
    void Awake()
    {
        _worldMaker = new CWorldMaker();
        _tileTypeArray = _worldMaker.MakeWorld(_worldData, _BSPData, _CAData);

        if (_surfase2D.IsNull("_surfase2D")) return;

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
    private void SetTile()
    {
        _tilemap.ClearAllTiles();
        // 얘를 반복 돌면서 읽어와 타일을 깐다.
        for (int i = 0; i < _tileTypeArray.GetLength(0); i++)
        {
            for (int j = 0; j < _tileTypeArray.GetLength(1); j++)
            {
                ETileType tt = _tileTypeArray[i, j];

                Vector3Int pos = new Vector3Int(i, j, 0);
                if (tt.HasFlag(ETileType.Floor))
                {

                    _tilemap.SetTile(pos, _tileBase);
                }
                else if (tt.HasFlag(ETileType.Hole))
                {
                    // 트리거 타일 넣기.
                    ;
                }
                else if (tt.HasFlag(ETileType.Wall))
                {
                    // 벽 바닥 그리기
                    // 벽 타일맵에 벽 그리기
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
        _surfase2D.BuildNavMesh();
        //_surfase2D.UpdateNavMesh(_surfase2D.navMeshData);
        //NavMeshBuilder.UpdateNavMeshDataAsync(); // ???
    }
    #endregion
}