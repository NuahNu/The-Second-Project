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

    // Tilemap name
    public static string NAME_FLOOR = "Floor Tilemap";
    public static string NAME_HOLE = "Hole Tilemap";
    public static string NAME_STRUCTURE = "Structure Tilemap";
    public static string NAME_COLLIDER = "Collider Tilemap";


    // Camera
    public static int CAMERA_Z = -20;
}
