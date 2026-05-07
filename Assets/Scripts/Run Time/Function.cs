using System;
using System.Collections.Generic;
using UnityEngine;

#region Function
/*

*/
#endregion

public static partial class Function
{
    public static bool IsNull<T>(this T obj, string varName, Action body = null) where T : class
    {
        if (obj is UnityEngine.Object unityObj)
        {
            if (unityObj == null)
            {
                Debug.LogWarning($"[Unity Object] {varName} is null or destroyed.");
                body?.Invoke();
                return true;
            }
        }
        else
        {
            if (obj == null)
            {
                Debug.LogWarning($"{varName} == null");
                body?.Invoke();
                return true;
            }
        }

        return false;
    }
    public static bool IsNull<T>(this ICollection<T> objArr, string varName, Action body = null) where T : class
    {
        if (objArr == null || objArr.Count == 0)
        {
            Debug.LogWarning($"{varName} == null || obj.Length == 0");
            body?.Invoke();
            return true;
        }
        return false;
    }

    public static T ParseData<T>(this T obj, string data)
    {
        object result;

        string[] strings;

        string path;

        switch (obj)
        {
            case int _:
                result = int.Parse(data);
                break;

            case int[] _:
                strings = data.Split(';');
                int[] iArr = new int[strings.Length];
                for (int i = 0; i < iArr.Length; i++)
                {
                    iArr[i] = int.Parse(strings[i]);
                }
                result = iArr;
                break;

            case float _:
                result = float.Parse(data);
                break;

            case float[] _:
                strings = data.Split(';');
                float[] fArr = new float[strings.Length];
                for (int i = 0; i < fArr.Length; i++)
                {
                    fArr[i] = float.Parse(strings[i]);
                }
                result = fArr;
                break;

            case double _:
                result = double.Parse(data);
                break;

            case double[] _:
                strings = data.Split(';');
                double[] dArr = new double[strings.Length];
                for (int i = 0; i < dArr.Length; i++)
                {
                    dArr[i] = double.Parse(strings[i]);
                }
                result = dArr;
                break;

            case string _:
                result = data;
                break;

            case string[] _:
                result = data.Split(';');
                break;

            case Enum e:
                result = Enum.Parse(e.GetType(), data, true);
                break;

            default:
                // 이 분기로 들어오는 대부분은 파일 이름의 배열일것.
                Type type = typeof(T);

                if (type == typeof(Texture2D))
                {
                    path = CGSSLoader.Texture2D_PATH + "/" + data.Trim().Replace("\r", "");
                    Texture2D _texture2D = Resources.Load<Texture2D>(path);
                    if (_texture2D == null)
                        Debug.Log($"_texture2D == null. {path}");
                    result = _texture2D;
                }
                else if (type == typeof(Texture2D[]))
                {
                    strings = data.Split(';');

                    Texture2D[] _texture2DArr = new Texture2D[strings.Length];

                    for (int i = 0; i < strings.Length; i++)
                    {
                        path = CGSSLoader.Texture2D_PATH + "/" + strings[i].Trim().Replace("\r", "");

                        _texture2DArr[i] = Resources.Load<Texture2D>(path);
                        if (_texture2DArr[i] == null)
                            Debug.Log($"_texture2DArr[{i}] == null. {path}");
                    }

                    result = _texture2DArr;
                }
                else if (type == typeof(Sprite))
                {
                    path = CGSSLoader.Sprite_PATH + "/" + data.Trim().Replace("\r", "");
                    Sprite _sprite = Resources.Load<Sprite>(path);
                    if (_sprite == null)
                        Debug.Log($"Sprite == null. {path}");
                    result = _sprite;
                }
                else if (type == typeof(Sprite[]))
                {
                    strings = data.Split(';');

                    Sprite[] _spriteArr = new Sprite[strings.Length];

                    for (int i = 0; i < strings.Length; i++)
                    {
                        path = CGSSLoader.Sprite_PATH + "/" + strings[i].Trim().Replace("\r", "");

                        _spriteArr[i] = Resources.Load<Sprite>(path);
                        if (_spriteArr[i] == null)
                            Debug.Log($"_texture2DArr[{i}] == null. {path}");
                    }

                    result = _spriteArr;
                }
                else if (type == typeof(Mesh))
                {
                    path = CGSSLoader.Mesh_PATH + "/" + data.Trim().Replace("\r", "");
                    Mesh _mesh = Resources.Load<Mesh>(path);
                    if (_mesh == null)
                        Debug.Log($"Mesh == null. {path}");
                    result = _mesh;
                }
                else
                    result = null;
                break;
        }

        return (T)result;
    }

}
