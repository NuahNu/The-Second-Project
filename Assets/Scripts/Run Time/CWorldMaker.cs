using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

#region CWorldMaker
/*

*/
#endregion


public enum EThemeType
{
    Defualt,
    Cave,
    Castle,
    Island, // ?
}
public enum ERoomType
{
    None,
    Start,
    Road,
    Boss,
}
[Flags]
public enum ETileType
{
    None = 0,
    Wall = 1 << 0,
    Floor = 1 << 1,
    Hole = 1 << 2,
    End = 1 << 3,
    PlayerSpawn = 1 << 16,
    ItemSpawn = 1 << 17,
    EnemySpawn = 1 << 18,
    // 보스 스폰이자 출구 위치.
    BossSpawn = 1 << 19,

    SpawnMask = PlayerSpawn | ItemSpawn | EnemySpawn | BossSpawn,
}

public class TreeNode
{
    public TreeNode leftNode;
    public TreeNode rightNode;
    public TreeNode parentNode;
    public TreeNode roadNode;   // 이거 노드로 해야하나?

    // 트리 노드의 Rect
    // 분리 되기 전의 영역
    public RectInt nodeRect;

    // 분리된 후 실제 생성된 방의 기준 Rect
    // 원래는 리프 노드에만 존재하지만, 연결을 위해 각자 기준 방의 역할도 한다.
    public RectInt standardRoomRect;

    // 위 Rect를 갖는 노드
    public TreeNode standardNode;

    // 다른 기준을 갖는 트리를 위한 포인터
    public List<TreeNode> sibling = new List<TreeNode>();

    // 자신을 포함한 노드의 리스트
    public List<TreeNode> nodeList = new List<TreeNode>();
    public bool isHorizontal = false;

    public ERoomType roomType;
    public TreeNode(int x, int y, int width, int height)
    {
        // 기준점은 좌하단
        nodeRect.x = x;
        nodeRect.y = y;
        nodeRect.width = width;
        nodeRect.height = height;
    }
}

[Serializable]
public class WorldData
{
    public Vector2Int mapSize = new Vector2Int(100, 100);
    public EThemeType type = EThemeType.Defualt;

    public ETileType[,] tileMap;
    public ETileType[,] bufferTileMap;
}

[Serializable]
public class BinarySpacePartitioningData
{
    // 이진 공간 분할
    public int maxLoopCount = 5;
    // 0 ~ 0.5
    // 0.5 에 가까울수록 오류가 생길 가능성이 높다. 예외처리 없음.
    public float divideRatio = 0.2f;
    public float roomMinRatio = 0.5f;

    // 다리 구역의 최소 너비 / 길이
    public int minRoadSize = 4;
}

//[Serializable]
//public class Rule
//{
//    // 자신은 ~가 ~개 이상일때 ~로 변한다.
//    public int[] rules = new int[3] { 9, 9, 9 };

//    public int this[int index]
//    {
//        get { return rules[index]; }
//        //set { }
//    }
//}

[Serializable]
public class CellularAutomataData
{
    // 세포 자동자
    public float wallRatio = 0.05f;
    public float floorRatio = 0.5f;
    public float holeRatio = 0.01f;
    public int smoothCount = 3;
    public int smoothRatio = 4;

    //public Rule[] rule = new Rule[3];
}

public class CWorldMaker
{
    #region 내부 변수
    private WorldData _worldData;

    private BinarySpacePartitioningData _BSPData;

    private CellularAutomataData _CAData;

    // _tileMap
    private ETileType[,] _tileMap;
    private ETileType[,] _bufferTileMap;
    public TreeNode RootNode { get; private set; }
    private List<TreeNode> _leafNodeList;
    public TreeNode MultiRootNode { get; private set; }
    public Dictionary<TreeNode, int> NodeDepthDic;
    public int MultiTreeDepth { get; private set; }
    #endregion

    #region public
    public void Init(WorldData worldData, BinarySpacePartitioningData BSPData, CellularAutomataData CAData)
    {
        _worldData = worldData;
        _BSPData = BSPData;
        _CAData = CAData;
    }
    public ETileType[,] MakeWorld(WorldData worldData, BinarySpacePartitioningData BSPData, CellularAutomataData CAData)
    {
        Init(worldData, BSPData, CAData);

        return MakeWorld();
    }
    public ETileType[,] MakeWorld()
    {
        // 타일맵 초기화
        _tileMap = new ETileType[_worldData.mapSize.x, _worldData.mapSize.y];
        for (int x = 0; x < _worldData.mapSize.x; x++)
            for (int y = 0; y < _worldData.mapSize.y; y++)
                _tileMap[x, y] = ETileType.Hole;
        _bufferTileMap = new ETileType[_worldData.mapSize.x, _worldData.mapSize.y];

        // 타일맵 데이터 생성.
        TreeNode rootNode = new TreeNode(0, 0, _worldData.mapSize.x, _worldData.mapSize.y);


        if (_leafNodeList == null)
            _leafNodeList = new List<TreeNode>();
        _leafNodeList.Clear();

        if (NodeDepthDic == null)
            NodeDepthDic = new Dictionary<TreeNode, int>();
        NodeDepthDic.Clear();

        MultiTreeDepth = int.MinValue;

        DivideTree(rootNode, 0);
        GenerateRoom(rootNode, 0);
        GenerateRoad(rootNode, 0);

        GenerateMultiTree(rootNode, 0);
        GenerateMultiTreeData();
        SetRoomType();
        SetSpawnPos();

        RootNode = rootNode;
        return _tileMap;
    }



    #endregion

    //BSP
    #region private
    private void DivideTree(TreeNode node, int loopCount)
    {
        // 캐싱
        RectInt size = node.nodeRect;

        if (loopCount < _BSPData.maxLoopCount)
        {
            // 반으로 나누는 기준은?
            int longLength = size.width >= size.height ? size.width : size.height;
            int splitLength = Mathf.RoundToInt(UnityEngine.Random.Range(longLength * (0.5f - _BSPData.divideRatio), longLength * (0.5f + _BSPData.divideRatio)));

            // 노드를 반으로 자른다.
            node.isHorizontal = size.width >= size.height;
            if (node.isHorizontal)
            {
                node.leftNode = new TreeNode(size.x, size.y, splitLength, size.height);
                node.rightNode = new TreeNode(size.x + splitLength, size.y, size.width - splitLength, size.height);
            }
            else
            {
                node.leftNode = new TreeNode(size.x, size.y, size.width, splitLength);
                node.rightNode = new TreeNode(size.x, size.y + splitLength, size.width, size.height - splitLength);
            }

            // 자식 노드를 만들어 연결한다.
            node.leftNode.parentNode = node;
            node.rightNode.parentNode = node;

            // 재귀
            loopCount++;
            DivideTree(node.leftNode, loopCount);
            DivideTree(node.rightNode, loopCount);
        }
    }

    private void GenerateRoom(TreeNode node, int loopCount)
    {
        if (loopCount == _BSPData.maxLoopCount)
        {
            // 자신 추가
            node.nodeList.Add(node);

            // 리프노드 리스트에 추가.
            _leafNodeList.Add(node);

            RectInt size = node.nodeRect;

            // 방의 크기
            int width = UnityEngine.Random.Range(Mathf.RoundToInt(size.width * _BSPData.roomMinRatio), size.width);
            int height = UnityEngine.Random.Range(Mathf.RoundToInt(size.height * _BSPData.roomMinRatio), size.height);

            // 방의 위치
            int x = node.nodeRect.x + UnityEngine.Random.Range(0, size.width - width);
            int y = node.nodeRect.y + UnityEngine.Random.Range(0, size.height - height);

            // 타일맵 반영
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    float total = _CAData.wallRatio + _CAData.floorRatio + _CAData.holeRatio;
                    float flag = UnityEngine.Random.Range(0f, total);
                    _tileMap[x + i, y + j] = flag < _CAData.floorRatio ? ETileType.Floor : flag < _CAData.floorRatio + _CAData.holeRatio ? ETileType.Hole : ETileType.Wall;
                    _bufferTileMap[x + i, y + j] = _tileMap[x + i, y + j];
                }
            }
            // smooth
            for (int k = 0; k < _CAData.smoothCount; k++)
            {
                // 버퍼에 작업
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        _bufferTileMap[x + i, y + j] = CellularAutomata(x + i, y + j, ERuleFlag.ALL);
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

            // 여기는 리프노드
            node.standardRoomRect = new RectInt(x, y, width, height);
        }
        else
        {
            loopCount++;
            GenerateRoom(node.leftNode, loopCount);
            GenerateRoom(node.rightNode, loopCount);

            // 자식 추가.
            List<TreeNode> lList = node.leftNode.nodeList;
            List<TreeNode> rList = node.rightNode.nodeList;

            node.nodeList.AddRange(lList);
            node.nodeList.AddRange(rList);

            // 여기는 기준 노드
            // 양쪽 자식들 중 제일 가까운 한 쌍이 기준이다.
            int n = node.leftNode.nodeList.Count;
            int m = node.rightNode.nodeList.Count;

            float minSqrDist = float.MaxValue;

            RectInt lRoom;
            RectInt rRoom;

            for (int lIndex = 0; lIndex < n; lIndex++)
            {
                for (int rIndex = 0; rIndex < m; rIndex++)
                {
                    lRoom = lList[lIndex].standardRoomRect;
                    rRoom = rList[rIndex].standardRoomRect;

                    float sqrDist = Vector2.SqrMagnitude(lRoom.center - rRoom.center);

                    if (sqrDist < minSqrDist)
                    {
                        minSqrDist = sqrDist;
                        node.leftNode.standardRoomRect = lRoom;
                        node.leftNode.standardNode = lList[lIndex];
                        node.rightNode.standardRoomRect = rRoom;
                        node.rightNode.standardNode = rList[rIndex];
                    }
                }
            }
            // 자신 추가
            node.nodeList.Add(node);
        }
    }

    private void GenerateRoad(TreeNode node, int loopCount)
    {
        if (loopCount == _BSPData.maxLoopCount) return;

        loopCount++;
        GenerateRoad(node.rightNode, loopCount);
        GenerateRoad(node.leftNode, loopCount);

        // 자식 노드 2개의 _roomRect.center를 이어준다.
        RectInt lRect = node.leftNode.standardRoomRect;
        RectInt rRect = node.rightNode.standardRoomRect;

        // 기준?
        Vector2 lRectCenter = lRect.center;
        Vector2 rRectCenter = rRect.center;
        Vector2 center = (lRectCenter + rRectCenter) / 2;

        int minX = (int)Mathf.Min(lRectCenter.x, rRectCenter.x);
        int minY = (int)Mathf.Min(lRectCenter.y, rRectCenter.y);
        int maxX = (int)Mathf.Max(lRectCenter.x, rRectCenter.x);
        int maxY = (int)Mathf.Max(lRectCenter.y, rRectCenter.y);

        // 영역 만들기
        RectInt newRectInt = new RectInt(minX, minY, maxX - minX, maxY - minY);

        int width = Math.Max(newRectInt.width, _BSPData.minRoadSize);
        int height = Math.Max(newRectInt.height, _BSPData.minRoadSize);
        int x = newRectInt.x;
        int y = newRectInt.y;

        // 위에서 바뀌었다면...newRectInt
        newRectInt.width = width;
        newRectInt.height = height;

        node.roadNode = new TreeNode(x, y, width, height);

        //Debug.Log($"loopCount : {loopCount} RectInt : {newRectInt}");

        // 길 만들기
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                float total = _CAData.wallRatio + _CAData.floorRatio;
                float flag = UnityEngine.Random.Range(0f, total);
                _tileMap[x + i, y + j] = flag < _CAData.floorRatio ? ETileType.Floor : ETileType.Wall;
                _bufferTileMap[x + i, y + j] = _tileMap[x + i, y + j];
            }
        }
        // 세포 자동자 
        for (int k = 0; k < _CAData.smoothCount; k++)
        {
            // 버퍼
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    _bufferTileMap[x + i, y + j] = CellularAutomata(x + i, y + j, ERuleFlag.WTF);
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
    }

    private void GenerateMultiTreeData()
    {
        int minNodeIndex = -1;
        float minDist = float.MaxValue;

        for (int i = 0; i < _leafNodeList.Count; i++)
        {
            Vector2 rectCenter = _leafNodeList[i].standardRoomRect.center;

            var mapCenter = _worldData.mapSize / 2;

            float dist = (mapCenter - rectCenter).sqrMagnitude;

            if (dist < minDist)
            {
                minNodeIndex = i;
                minDist = dist;
            }
        }

        MultiRootNode = _leafNodeList[minNodeIndex];

        SetDepth(MultiRootNode, 0);
    }

    private void SetDepth(TreeNode node, int depth)
    {
        if (!NodeDepthDic.ContainsKey(node))
        {
            NodeDepthDic.Add(node, depth);

            if (MultiTreeDepth < depth)
                MultiTreeDepth = depth;

            for (int i = 0; i < node.sibling.Count; i++)
            {
                SetDepth(node.sibling[i], depth + 1);
            }
        }
    }

    // 기존 트리로 멀티 트리 만들기
    private void GenerateMultiTree(TreeNode node, int loopCount)
    {
        if (loopCount == _BSPData.maxLoopCount) return;

        TreeNode rNode = node.rightNode.standardNode;
        TreeNode lNode = node.leftNode.standardNode;

        rNode.sibling.Add(lNode);
        lNode.sibling.Add(rNode);

        loopCount++;
        GenerateMultiTree(node.rightNode, loopCount);
        GenerateMultiTree(node.leftNode, loopCount);
    }

    private void SetRoomType()
    {
        for (int i = 0; i < _leafNodeList.Count; i++)
        {
            if (!NodeDepthDic.ContainsKey(_leafNodeList[i]))
            {
                Debug.LogWarning("이럴리가 없다.");
                return;
            }
            int depth = NodeDepthDic[_leafNodeList[i]];
            //if (depth == 0 || depth == MultiTreeDepth)
            //    Debug.Log($"depth = {depth}");

            // 루트 노드가 시작 방
            if (depth == 0)
            {
                _leafNodeList[i].roomType = ERoomType.Start;
            }
            // 가장 깊은 방이 보스방
            else if (depth == MultiTreeDepth)
            {
                _leafNodeList[i].roomType = ERoomType.Boss;
            }
            // Road로 초기화.
            else
            {
                _leafNodeList[i].roomType = ERoomType.Road;
            }
            // 새로운 타입에 따른 조건 추가.
        }
    }

    private void SetSpawnPos()
    {
    }

    [Flags]
    private enum ERuleFlag
    {
        WTF = 1 << 0,
        FTH = 1 << 1,
        FTW = 1 << 2,

        ALL = WTF | FTH | FTW
    }
    private ETileType CellularAutomata(int x, int y, ERuleFlag flag = ERuleFlag.ALL)
    {
        // 카운팅
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
                if (nX >= 0 && nX < _worldData.mapSize.x && nY >= 0 && nY < _worldData.mapSize.y)
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

        // 규칙
        switch (_tileMap[x, y])
        {
            case ETileType.Wall:
                // 벽은 바닥이 될 수 있다.
                if (flag.HasFlag(ERuleFlag.WTF) && (wallCount + endCount < _CAData.smoothRatio))
                    return ETileType.Floor;

                return ETileType.Wall;
            case ETileType.Floor:
                // 바닥은 구멍이 될 수 있다.
                if (flag.HasFlag(ERuleFlag.FTH) && holeCount + wallCount > _CAData.smoothRatio)
                    return ETileType.Hole;

                // 바닥은 벽이 될 수 있다.
                if (flag.HasFlag(ERuleFlag.FTW) && wallCount + endCount > _CAData.smoothRatio)
                    return ETileType.Wall;

                return ETileType.Floor;
            case ETileType.Hole:

                return ETileType.Hole;
        }

        return ETileType.Floor;
    }
    #endregion
}
