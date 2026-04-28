using System.Collections.Generic;
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

public enum EThemeType
{
    Defualt,
    Cave,
    Castle,
    Island, // ?
}
public enum EAreaType
{
    None,
    Start,
    Shortcut,
    Boss,
}
public enum ETileType
{
    Wall,
    Floor,
    Hole
}

public class CWorldMaker : MonoBehaviour
{
    public class TreeNode
    {
        public TreeNode _leftTree;
        public TreeNode _rightTree;
        public TreeNode _parentTree;
        // 트리 노드의 Rect
        public RectInt _nodeRect;
        // 실제 생성된 방의 기준 Rect
        public RectInt _standardRoomRect;
        // 자신을 포함한 노드의 리스트
        public List<TreeNode> _nodeList = new List<TreeNode>();
        public bool isHorizontal = false;
        public TreeNode(int x, int y, int width, int height)
        {
            // 기준점은 좌하단
            _nodeRect.x = x;
            _nodeRect.y = y;
            _nodeRect.width = width;
            _nodeRect.height = height;
        }
    }

    #region 인스펙터
    [SerializeField] private EThemeType _type = EThemeType.Defualt;

    [Header("이진 공간 분할")]
    [SerializeField] private Vector2Int _mapSize = new Vector2Int(100, 100);
    [SerializeField] private int _maxLoopCount = 5;
    // 0 ~ 0.5
    // 0.5 에 가까울수록 오류가 생길 가능성이 높다. 예외처리 없음.
    [SerializeField] private float _divideRatio = 0.2f;
    [SerializeField] private float _roomMinRatio = 0.5f;

    [Header("Cellular Automata")]
    [SerializeField] private float _wallRatio = 0.05f;
    [SerializeField] private float _floorRatio = 0.5f;
    [SerializeField] private float _holeRatio = 0.01f;
    [SerializeField] private int _smoothRatio = 4;
    [SerializeField] private int _smoothCount = 3;

    [Header("디버그 키")]
    [SerializeField] private KeyCode _makeData = KeyCode.M;
    #endregion

    #region 내부 변수
    private ETileType[,] _tileMap;
    private ETileType[,] _bufferTileMap;
    #endregion

    #region 유니티 이벤트
    void Awake()
    {
        // 타일맵 초기화
        _tileMap = new ETileType[_mapSize.x, _mapSize.y];
        _bufferTileMap = new ETileType[_mapSize.x, _mapSize.y];

        // 타일맵 데이터 생성.
        TreeNode rootNode = new TreeNode(0, 0, _mapSize.x, _mapSize.y);
        DivideTree(rootNode, 0);
        GenerateRoom(rootNode, 0);
        GenerateRoad(rootNode, 0);
        // Cellular Automata - SmoothMap

    }

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(_makeData))
        {
            // 타일맵 초기화
            _tileMap = new ETileType[_mapSize.x, _mapSize.y];
            _bufferTileMap = new ETileType[_mapSize.x, _mapSize.y];

            // 타일맵 데이터 생성.
            TreeNode rootNode = new TreeNode(0, 0, _mapSize.x, _mapSize.y); //루트가 될 트리 생성
            DivideTree(rootNode, 0); //트리 분할
            GenerateRoom(rootNode, 0); //방 생성
            GenerateRoad(rootNode, 0); //길 연결

            // Cellular Automata - SmoothMap

            int rows = _tileMap.GetLength(0);
            int cols = _tileMap.GetLength(1);
            string mapData = $"TileMap 데이터: \n_mapSize: {_mapSize} _maxLoopCount: {_maxLoopCount} _divideRatio: {_divideRatio} _roomMinRatio: {_roomMinRatio}\n";

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    ETileType tile = _tileMap[i, j];
                    mapData += tile == ETileType.Wall ? "■" : tile == ETileType.Floor ? "□" : "※";
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
    private void DivideTree(TreeNode node, int loopCount)
    {
        // 캐싱
        RectInt size = node._nodeRect;

        if (loopCount < _maxLoopCount)
        {
            // 반으로 나누는 기준은?
            int longLength = size.width >= size.height ? size.width : size.height;
            int splitLength = Mathf.RoundToInt(Random.Range(longLength * (0.5f - _divideRatio), longLength * (0.5f + _divideRatio)));

            // 노드를 반으로 자른다.
            node.isHorizontal = size.width >= size.height;
            if (node.isHorizontal)
            {
                node._leftTree = new TreeNode(size.x, size.y, splitLength, size.height);
                node._rightTree = new TreeNode(size.x + splitLength, size.y, size.width - splitLength, size.height);
            }
            else
            {
                node._leftTree = new TreeNode(size.x, size.y, size.width, splitLength);
                node._rightTree = new TreeNode(size.x, size.y + splitLength, size.width, size.height - splitLength);
            }

            // 자식 노드를 만들어 연결한다.
            node._leftTree._parentTree = node;
            node._rightTree._parentTree = node;

            // 재귀
            loopCount++;
            DivideTree(node._leftTree, loopCount);
            DivideTree(node._rightTree, loopCount);
        }
    }

    private void GenerateRoom(TreeNode node, int loopCount)
    {
        // 자신 추가
        node._nodeList.Add(node);

        if (loopCount == _maxLoopCount)
        {
            RectInt size = node._nodeRect;

            int width = Random.Range(Mathf.RoundToInt(size.width * _roomMinRatio), size.width);
            int height = Random.Range(Mathf.RoundToInt(size.height * _roomMinRatio), size.height);

            int x = node._nodeRect.x + Random.Range(0, size.width - width);
            int y = node._nodeRect.y + Random.Range(0, size.height - height);

            // 타일맵 반영
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    float total = _wallRatio + _floorRatio + _holeRatio;
                    float flag = Random.Range(0f, total);
                    //_tileMap[x + i, y + j] = flag < _floorRatio ? ETileType.Floor : ETileType.Wall;
                    _tileMap[x + i, y + j] = flag < _floorRatio ? ETileType.Floor : flag < _floorRatio + _holeRatio ? ETileType.Hole : ETileType.Wall;
                    _bufferTileMap[x + i, y + j] = _tileMap[x + i, y + j];
                }
            }
            // smooth
            // 버퍼에 작업
            for (int k = 0; k < _smoothCount; k++)
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        _bufferTileMap[x + i, y + j] = CellularAutomata(x + i, y + j);
                    }
                }
                // 적용
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        _tileMap[x + i, y + j] = _bufferTileMap[x + i, y + j];
                    }
                }
            }

            node._standardRoomRect = new RectInt(x, y, width, height);
        }
        else
        {
            loopCount++;
            GenerateRoom(node._leftTree, loopCount);
            GenerateRoom(node._rightTree, loopCount);

            // 자식 추가.
            List<TreeNode> lList = node._leftTree._nodeList;
            List<TreeNode> rList = node._rightTree._nodeList;

            node._nodeList.AddRange(lList);
            node._nodeList.AddRange(rList);

            // 양쪽 자식들 중 제일 가까운 한 쌍이 기준이다.
            int n = node._leftTree._nodeList.Count;
            int m = node._rightTree._nodeList.Count;

            float minSqrDist = float.MaxValue;

            RectInt lRoom;
            RectInt rRoom;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    lRoom = lList[i]._standardRoomRect;
                    rRoom = rList[j]._standardRoomRect;

                    float sqrDist = Vector2.SqrMagnitude(lRoom.center - rRoom.center);

                    if (sqrDist < minSqrDist)
                    {
                        minSqrDist = sqrDist;
                        node._leftTree._standardRoomRect = lRoom;
                        node._rightTree._standardRoomRect = rRoom;
                    }
                }
            }
        }
    }

    private void GenerateRoad(TreeNode node, int loopCount)
    {
        if (loopCount == _maxLoopCount) return;

        // 자식 노드 2개의 _roomRect.center를 이어준다.
        RectInt lRect = node._leftTree._standardRoomRect;
        RectInt rRect = node._rightTree._standardRoomRect;

        // 기준?
        Vector2 lRectCenter = lRect.center;
        Vector2 rRectCenter = rRect.center;
        Vector2 center = (lRectCenter + rRectCenter) / 2;

        if (node.isHorizontal)
        {
            // 연결하기.
            if (rRectCenter.x < lRectCenter.x)
            {
                //for (int i = rRectCenter.x) ;
            }
            else
            {

            }

        }
        else
        {
            // 연결하기.

        }
    }

    private void CreateArea()
    {

    }

    private void CreateTile()
    {

    }

    private ETileType CellularAutomata(int x, int y)
    {
        int wallCount = 0;
        int floorCount = 0;
        int holeCount = 0;
        int endCount = 0;
        // 현재 좌표를 기준으로 주변 8칸 검사
        for (int nX = x - 1; nX <= x + 1; nX++)
        {
            for (int nY = y - 1; nY <= y + 1; nY++)
            {
                // 맵 범위 검사
                if (nX >= 0 && nX < _mapSize.x && nY >= 0 && nY < _mapSize.y)
                {
                    if (nX == x && nY == y) continue;

                    switch (_tileMap[nX, nY])
                    {
                        case ETileType.Wall: wallCount++; break;
                        case ETileType.Floor: floorCount++; break;
                        case ETileType.Hole: holeCount++; break;
                    }
                }
                else
                {
                    // 맵 밖은 벽이다. 벽인가? 아닐 경우 여기를 수정.
                    endCount++;
                }
            }
        }
        switch (_tileMap[x, y])
        {
            case ETileType.Wall:
                // 벽은 바닥이 될 수 있다.
                if (wallCount + endCount < _smoothRatio)
                    return ETileType.Floor;

                return ETileType.Wall;
            case ETileType.Floor:
                // 바닥은 구멍이 될 수 있다.
                if (holeCount + wallCount > _smoothRatio)
                    return ETileType.Hole;

                // 바닥은 벽이 될 수 있다.
                if (wallCount + endCount > _smoothRatio)
                    return ETileType.Wall;

                return ETileType.Floor;
            case ETileType.Hole:

                return ETileType.Hole;
        }

        return ETileType.Floor;
    }
    #endregion
}