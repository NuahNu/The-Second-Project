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

public class CWorldMakerTester : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private WorldData _worldData;
    [SerializeField] private BinarySpacePartitioningData _BSPData;
    [SerializeField] private CellularAutomataData _CAData;

    [Header("디버그 키")]
    [SerializeField] private KeyCode _makeData = KeyCode.M;
    #endregion

    #region 내부 변수
    private ETileType[,] _tileMap;

    private CWorldMaker _worldMaker;
    #endregion

    #region 유니티 이벤트
    void Awake()
    {
        _worldMaker = new CWorldMaker();
        _tileMap = _worldMaker.MakeWorld(_worldData, _BSPData, _CAData);
        // 스폰 위치 지정.
    }

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(_makeData))
        {
            _tileMap = _worldMaker.MakeWorld(_worldData, _BSPData, _CAData);

            int rows = _tileMap.GetLength(0);
            int cols = _tileMap.GetLength(1);
            string mapData = $"TileMap 데이터: \n";

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    ETileType tile = _tileMap[i, j];
                    mapData += tile.HasFlag(ETileType.Wall) ? "■" : tile.HasFlag(ETileType.Floor) ? "□" : "※";
                }
                mapData += "\n"; // 한 줄 끝날 때마다 줄바꿈
            }

            Debug.Log(mapData);
        }
    }
    #endregion

    #region public
    public void CreateMap(Vector2Int mapSize)
    {

    }
    #endregion

    #region private

    #endregion
}