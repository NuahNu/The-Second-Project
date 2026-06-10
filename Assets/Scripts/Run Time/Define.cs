using System;
using UnityEngine;


#region Define
/*

*/
#endregion

public static class Define
{
    // Sorting Layer
    // 이걸로 구분하는걸 고려.

    // Order in Layer
    // 보이면 안됨. - 레이어로 구분하는게 좋을 지도?
    public static int ORDER_COLLIDER = -20;
    // 보이기 시작하는 마지노선
    public static int ORDER_FLOOR = -10;
    public static int ORDER_HOLE = -10;
    public static int ORDER_SHADOW = -1;
    public static int ORDER_CHARACTER = 0;
    public static int ORDER_STRUCTURE = 0;
    public static int ORDER_STRUCTURE_A = 10;

    // Tilemap name
    public static string NAME_FLOOR = "Floor Tilemap";
    public static string NAME_HOLE = "Hole Tilemap";
    public static string NAME_STRUCTURE = "Structure Tilemap";
    public static string NAME_STRUCTURE_A = "StructureA Tilemap";
    public static string NAME_COLLIDER = "Collider Tilemap";
    public static string NAME_END = "EndTrigger Tilemap";


    // Camera
    public static int CAMERA_Z = -20;

    // Grid
    public static Vector2 GRID_CELL_SIZE = new Vector2(1f, 0.56f);

    public static Vector2[] DIR_VECTOR2 = new Vector2[8] {
        new Vector2(GRID_CELL_SIZE.x, 0),
        new Vector2(GRID_CELL_SIZE.x, GRID_CELL_SIZE.y).normalized,
        new Vector2(0, GRID_CELL_SIZE.y),
        new Vector2(-GRID_CELL_SIZE.x, GRID_CELL_SIZE.y).normalized,
        new Vector2(-GRID_CELL_SIZE.x, 0),
        new Vector2(-GRID_CELL_SIZE.x, -GRID_CELL_SIZE.y).normalized,
        new Vector2(0, -GRID_CELL_SIZE.y).normalized,
        new Vector2(GRID_CELL_SIZE.x, -GRID_CELL_SIZE.y).normalized
    };

    public enum EDIR_NAME
    {
        Right,
        RightTop,
        Top,
        LeftTop,
        Left,
        LeftBot,
        Bot,
        RightBot,
    }
}
