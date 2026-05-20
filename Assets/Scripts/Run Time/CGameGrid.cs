using UnityEngine;


#region CGameGrid
/*
현재 Grid에만 적용을 하고 있다. Grid의 TileMap들에도 적용을 해야 한다.
TileMap의 Tile과 그 크기에 따라서도 달라져야한다.
*/
#endregion

public class CGameGrid : MonoBehaviour
{
    #region 인스펙터
    [SerializeField ]private Grid _grid;
    #endregion

    #region 내부 변수
    #endregion

    #region 유니티 이벤트
    void Awake()
    {
        if (_grid.IsNull("_grid")) return;
    }

    void Start()
    {
        _grid.cellSize = Define.GRID_CELL_SIZE;
    }

    void Update()
    {
        
    }
    #endregion

    #region public
    
    #endregion

    #region protected
    
    #endregion

    #region private
    
    #endregion
}
