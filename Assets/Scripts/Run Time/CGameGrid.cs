using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#region CGameGrid
/*

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
