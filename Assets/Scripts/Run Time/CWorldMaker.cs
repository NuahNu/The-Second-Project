using System;
using System.Collections.Generic;
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
public enum EAreaType
{
    None,
    Start,
    Shortcut,
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
    BossSpawn = 1 << 19,

    SpawnMask = PlayerSpawn | ItemSpawn | EnemySpawn | BossSpawn,
}

public class TreeNode
{
    public TreeNode leftTree;
    public TreeNode rightTree;
    public TreeNode parentTree;
    // 트리 노드의 Rect
    public RectInt nodeRect;
    // 실제 생성된 방의 기준 Rect
    public RectInt standardRoomRect;
    // 자신을 포함한 노드의 리스트
    public List<TreeNode> nodeList = new List<TreeNode>();
    public bool isHorizontal = false;
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
                _tileMap[x, y] = ETileType.Wall;
        _bufferTileMap = new ETileType[_worldData.mapSize.x, _worldData.mapSize.y];

        // 타일맵 데이터 생성.
        TreeNode rootNode = new TreeNode(0, 0, _worldData.mapSize.x, _worldData.mapSize.y);
        DivideTree(rootNode, 0);
        GenerateRoom(rootNode, 0);
        GenerateRoad(rootNode, 0);

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
                node.leftTree = new TreeNode(size.x, size.y, splitLength, size.height);
                node.rightTree = new TreeNode(size.x + splitLength, size.y, size.width - splitLength, size.height);
            }
            else
            {
                node.leftTree = new TreeNode(size.x, size.y, size.width, splitLength);
                node.rightTree = new TreeNode(size.x, size.y + splitLength, size.width, size.height - splitLength);
            }

            // 자식 노드를 만들어 연결한다.
            node.leftTree.parentTree = node;
            node.rightTree.parentTree = node;

            // 재귀
            loopCount++;
            DivideTree(node.leftTree, loopCount);
            DivideTree(node.rightTree, loopCount);
        }
    }

    private void GenerateRoom(TreeNode node, int loopCount)
    {
        // 자신 추가
        node.nodeList.Add(node);

        if (loopCount == _BSPData.maxLoopCount)
        {
            RectInt size = node.nodeRect;

            int width = UnityEngine.Random.Range(Mathf.RoundToInt(size.width * _BSPData.roomMinRatio), size.width);
            int height = UnityEngine.Random.Range(Mathf.RoundToInt(size.height * _BSPData.roomMinRatio), size.height);

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
            // 버퍼에 작업
            for (int k = 0; k < _CAData.smoothCount; k++)
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

            node.standardRoomRect = new RectInt(x, y, width, height);
        }
        else
        {
            loopCount++;
            GenerateRoom(node.leftTree, loopCount);
            GenerateRoom(node.rightTree, loopCount);

            // 자식 추가.
            List<TreeNode> lList = node.leftTree.nodeList;
            List<TreeNode> rList = node.rightTree.nodeList;

            node.nodeList.AddRange(lList);
            node.nodeList.AddRange(rList);

            // 양쪽 자식들 중 제일 가까운 한 쌍이 기준이다.
            int n = node.leftTree.nodeList.Count;
            int m = node.rightTree.nodeList.Count;

            float minSqrDist = float.MaxValue;

            RectInt lRoom;
            RectInt rRoom;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    lRoom = lList[i].standardRoomRect;
                    rRoom = rList[j].standardRoomRect;

                    float sqrDist = Vector2.SqrMagnitude(lRoom.center - rRoom.center);

                    if (sqrDist < minSqrDist)
                    {
                        minSqrDist = sqrDist;
                        node.leftTree.standardRoomRect = lRoom;
                        node.rightTree.standardRoomRect = rRoom;
                    }
                }
            }
        }
    }

    private void GenerateRoad(TreeNode node, int loopCount)
    {
        if (loopCount == _BSPData.maxLoopCount) return;

        // 자식 노드 2개의 _roomRect.center를 이어준다.
        RectInt lRect = node.leftTree.standardRoomRect;
        RectInt rRect = node.rightTree.standardRoomRect;

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

    private ETileType CellularAutomata(int x, int y)
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
                if (wallCount + endCount < _CAData.smoothRatio)
                    return ETileType.Floor;

                return ETileType.Wall;
            case ETileType.Floor:
                // 바닥은 구멍이 될 수 있다.
                if (holeCount + wallCount > _CAData.smoothRatio)
                    return ETileType.Hole;

                // 바닥은 벽이 될 수 있다.
                if (wallCount + endCount > _CAData.smoothRatio)
                    return ETileType.Wall;

                return ETileType.Floor;
            case ETileType.Hole:

                return ETileType.Hole;
        }

        return ETileType.Floor;
    }
    #endregion
}
