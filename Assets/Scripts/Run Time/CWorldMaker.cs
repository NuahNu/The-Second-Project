using System;
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

public class CWorldMaker : MonoBehaviour
{
    public class TreeNode
    {
        public TreeNode _leftTree;
        public TreeNode _rightTree;
        public TreeNode _parentTree;
        // 트리 노드의 Rect
        public RectInt _nodeRect;
        // 실제 생성된 방의 Rect
        public RectInt _roomRect;

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
    [SerializeField] private int _maxLoofCount = 5;
    // 0 ~ 0.5
    // 0.5 에 가까울수록 오류가 생길 가능성이 높다. 예외처리 없음.
    [SerializeField] private float _divideRatio = 0.2f;
    [SerializeField] private float _roomMinRatio = 0.5f;

    [Header("Cellular Automata")]
    #endregion

    #region 내부 변수
    private int[,] _tileMap;
    #endregion

    #region 유니티 이벤트
    void Awake()
    {
        // 타일맵 초기화
        _tileMap = new int[_mapSize.x, _mapSize.y];

        // 타일맵 데이터 생성.
        TreeNode rootNode = new TreeNode(0, 0, _mapSize.x, _mapSize.y); //루트가 될 트리 생성
        DivideTree(rootNode, 0); //트리 분할
        GenerateRoom(rootNode, 0); //방 생성
        GenerateRoad(rootNode, 0); //길 연결

        // Cellular Automata - SmoothMap
        int rows = _tileMap.GetLength(0); // 첫 번째 차원 크기
        int cols = _tileMap.GetLength(1); // 두 번째 차원 크기
        string mapData = $"TileMap 데이터: \n_mapSize: {_mapSize} _maxLoofCount: {_maxLoofCount} _divideRatio: {_divideRatio} _roomMinRatio: {_roomMinRatio}\n";

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                mapData += _tileMap[i, j] == 0 ? "■" : "□";
            }
            mapData += "\n"; // 한 줄 끝날 때마다 줄바꿈
        }

        Debug.Log(mapData);
    }

    void Start()
    {

    }

    void Update()
    {
    }
    #endregion

    #region public
    public void CreateMap(Vector2Int mapSize)
    {

    }
    #endregion

    #region private
    private void DivideTree(TreeNode node, int loofCount)
    {
        // 캐싱
        RectInt size = node._nodeRect;

        if (loofCount < _maxLoofCount)
        {
            // 반으로 나누는 기준은?
            int longLength = size.width >= size.height ? size.width : size.height;
            int splitLength = Mathf.RoundToInt(UnityEngine.Random.Range(longLength * (0.5f - _divideRatio), longLength * (0.5f + _divideRatio)));

            // 노드를 반으로 자른다.
            if (size.width >= size.height)
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
            loofCount++;
            DivideTree(node._leftTree, loofCount);
            DivideTree(node._rightTree, loofCount);
        }
    }

    private RectInt GenerateRoom(TreeNode node, int loofCount)
    {
        if (loofCount == _maxLoofCount)
        {
            RectInt size = node._nodeRect;

            int width = UnityEngine.Random.Range(Mathf.RoundToInt(size.width * _roomMinRatio), size.width);
            int height = UnityEngine.Random.Range(Mathf.RoundToInt(size.height * _roomMinRatio), size.height);

            int x = node._nodeRect.x + UnityEngine.Random.Range(0, size.width - width);
            int y = node._nodeRect.y + UnityEngine.Random.Range(0, size.height - height);

            //
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    _tileMap[x + i, y + j] = 1;
                }
            }

            return new RectInt(x, y, width, height);
        }
        loofCount++;
        node._leftTree._roomRect = GenerateRoom(node._leftTree, loofCount);
        node._rightTree._roomRect = GenerateRoom(node._rightTree, loofCount);

        // 자식들 중 중심에 더 가까운 방이 기준이다.
        float lDist = Vector2.SqrMagnitude(node._leftTree._roomRect.center - node._nodeRect.center);
        float rDist = Vector2.SqrMagnitude(node._rightTree._roomRect.center - node._nodeRect.center);

        return lDist > rDist ? node._rightTree._roomRect : node._leftTree._roomRect;
    }

    private void GenerateRoad(TreeNode node, int loofCount)
    {
        if (loofCount == _maxLoofCount) return;

        // 자식 노드 2개의 _roomRect.center를 이어준다.
        RectInt lRect = node._leftTree._roomRect;
        RectInt rRect = node._rightTree._roomRect;

        if (MathF.Abs(lRect.center.x - rRect.center.x) > MathF.Abs(lRect.center.y - rRect.center.y))
        {
            // 가로
        }
        else
        {
            // 세로
        }
    }

    private void CreateArea()
    {

    }

    private void CreateTile()
    {

    }
    #endregion
}