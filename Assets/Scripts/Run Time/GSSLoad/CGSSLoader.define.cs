

#region CGSSLoader.define
/*
▶ 작성자 류연우

파싱과 불러오기에 사용할 각종 파일 경로들을 Define 하기 위한 파일.
*/
#endregion

using System;
using System.Collections.Generic;
using UnityEngine;

public partial class CGSSLoader : MonoBehaviour
{
    #region 스프레드시트 URL
    //static readonly string URL = "https://docs.google.com/spreadsheets/d/1wx7tsBCYFjxJkCGeNdoklLhEJamttKUGMLcJNghr1wc";
    static readonly string URL = "https://docs.google.com/spreadsheets/d/1aQ184xwpJq4ReThdphO5I6lR6eU5Als7PWTeq6SWZAk";
    static readonly string EXTRA_URL = "/export?format=";
    static readonly string LOAD_TYPE = "csv";
    // sheet 페이지별 gid의 배열.
    static readonly string[] SHEET_NUMBER = new string[]
    {
        ""
    };
    #endregion

    #region 파일 경로
    public static readonly string SO_PATH = "Assets/Resources/SO";
    public static readonly string Texture2D_PATH = "Texture2D";
    public static readonly string Sprite_PATH = "Sprite";
    public static readonly string Mesh_PATH = "Mesh";
    public static readonly string Prefab_PATH = "Prefab";
    public static readonly Dictionary<Type, string> Path_Map = new Dictionary<Type, string>()
    {
        { typeof(Texture2D), Texture2D_PATH },
        { typeof(Sprite),    Sprite_PATH },
        { typeof(Mesh),      Mesh_PATH },
        { typeof(CCharacter), Prefab_PATH }
    };
    #endregion
}
