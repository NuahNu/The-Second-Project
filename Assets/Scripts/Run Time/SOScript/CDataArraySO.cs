using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


#region CDataArraySO
/*
▶ 작성자 류연우

가능하면 매니저를 통해 사용할것.
아마도, 인스턴스를 생성하지 않고 직접 사용했을때, 메모리 문제가 생기지 않을 것이다. 아마도.

새로운 데이터가 추가되면 
NEW
를 검색. 총 6곳이 있음.

*/
#endregion

[CreateAssetMenu(menuName = "Create SO/Data/Data Array (SO)", fileName = "DataArraySO_")]
public class CDataArraySO : ScriptableObject
{
    static readonly string NAME = "DataArraySO";
    public enum EDataType
    {
        EnemyData,
        // NEW
    }

    #region 인스펙터
    [SerializeField] private List<CCharacterDataSO> _enemyDataArr;
    // NEW
    #endregion

    #region 내부 변수
    private Dictionary<int, ICSVData> _enemyDataDic;
    // NEW
    #endregion

    #region 프로퍼티
    // 있으면 반환, 없으면 만든다.
    // 얘는 public 일 필요가 있나?
    public IReadOnlyDictionary<int, ICSVData> EnemyDataDic => _enemyDataDic ??= InitDataDic(_enemyDataArr);
    // NEW

    // 읽기 전용 딕셔너리를 반환하는 인덱서.
    public IReadOnlyDictionary<int, ICSVData> this[EDataType dataType] => dataType switch
    {
        EDataType.EnemyData => EnemyDataDic,
        // NEW
        _ => null
    };
    #endregion
    public string SetData()
    {
#if UNITY_EDITOR
        Debug.Log("Set DataArraySO");

        SetSOAsset(ref _enemyDataArr);
        // NEW
#endif
        return Path.Combine(CGSSLoader.SO_PATH, NAME + ".asset");
    }
    private void SetSOAsset<T>(ref List<T> soList) where T : UnityEngine.Object, ICSVData
    {
#if UNITY_EDITOR
        // 리스트 타입의 SO 긁어오기
        string[] list = AssetDatabase.FindAssets($"t:{typeof(T).Name}");

        // 없으면 만들기
        soList ??= new List<T>();

        for (int j = 0; j < list.Length; j++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(list[j]);
            soList.Add(AssetDatabase.LoadAssetAtPath<T>(assetPath));
        }

        Debug.Log($"{typeof(T).Name} : {list.Length}");
#endif
    }

    // 리스트를 딕셔너리로 바꿔주는 함수.
    private Dictionary<int, ICSVData> InitDataDic<T>(List<T> list) where T : ICSVData
    {
        var dic = new Dictionary<int, ICSVData>();
        foreach (var item in list)
        {
            if (item == null) continue;

            if (!dic.TryAdd(item.ID, item))
            {
                Debug.LogError($"중복된 ID 발견: {item.ID} (타입: {typeof(T).Name})");
            }
        }
        return dic;
    }
}
