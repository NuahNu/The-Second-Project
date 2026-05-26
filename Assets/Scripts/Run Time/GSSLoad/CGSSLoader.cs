using System;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Networking;


#region CGSSLoader
/*
▶ 작성자 류연우

런타임에 작동할 녀석이 아닙니다.
미리 SO를 만들어줄 녀석입니다.

프로젝트 빌드 전 별도의 씬에서 SO 생성을 위해 사용할 클래스이므로 
아마 느려도 상관 없을것...

이 클래스를 통해 만들어진 SO를 사용할것.

SO의 값을 변경하지 말고 스프레드시트의 값을 수정할것.
    배열을 저장할 경우, 공백이 없는;로 구분한다.

인스펙터에서 새로 생성할 시트를 골라 씬을 실행시키면 된다.

    참고
https://data-pandora.tistory.com/entry/Unity-%EA%B5%AC%EA%B8%80-%EC%8A%A4%ED%94%84%EB%A0%88%EB%93%9C%EC%8B%9C%ED%8A%B8-%EC%97%B0%EB%8F%99
*/
#endregion

public partial class CGSSLoader : MonoBehaviour
{
    private enum ESheetType
    {
        EnemyData,
        Count
    }

    [Flags]
    public enum ECreateFlag
    {
        EnemyData = 1 << 0,
    }

    #region 인스펙터
    public bool PrintData = true;
    public ECreateFlag CreateFlag = 0;

    public bool UseMakeDataArraySO = true;

    public KeyCode RunKey = KeyCode.R;

    public bool RunFlag = false;
    #endregion


    private void Update()
    {
        if (Input.GetKeyDown(RunKey))
        {
            if (!RunFlag)
                StartCoroutine(MakeDataSO());
        }
    }

    private IEnumerator LoadFromURL(ESheetType type)
    {
        UnityWebRequest www = UnityWebRequest.Get(URL + EXTRA_URL + LOAD_TYPE + SHEET_NUMBER[(int)type]);
        yield return www.SendWebRequest();

        string data = www.downloadHandler.text;
        string[] dataArr = data.Split("\n");

        // 0,1은 사용자 편의를 위한 수치가 들어감. 2번부터 데이터임.
        for (int i = 2; i < dataArr.Length; i++)
        {
            switch (type)
            {
                case ESheetType.EnemyData:
                    ParseData<CCharacterDataSO>(dataArr[i]);
                    break;
            }
        }

#if UNITY_EDITOR
        //AssetDatabase.SaveAssets();
#endif
        if (PrintData)
        {
            print(data);
        }
    }

    private IEnumerator MakeDataSO()
    {
        RunFlag = true;
        Debug.Log("생성 시작");
        for (int i = 0; i < (int)ESheetType.Count; i++)
        {
            if ((CreateFlag & (ECreateFlag)(1 << i)) != 0)
            {
                yield return StartCoroutine(LoadFromURL((ESheetType)i));
            }
        }

        if (UseMakeDataArraySO)
            MakeDataArraySO();
        Debug.Log("생성 끝");
        RunFlag = false;
    }

    private void MakeDataArraySO()
    {
#if UNITY_EDITOR
        CDataArraySO so = ScriptableObject.CreateInstance<CDataArraySO>();

        string path = so.SetData();

        EditorUtility.SetDirty(so);

        AssetDatabase.CreateAsset(so, path);
#endif
    }

    private void ParseData<T>(string data) where T : ScriptableObject, ICSVData
    {
        T ed = ScriptableObject.CreateInstance<T>();
        string path = ed.ParseData(data);

#if UNITY_EDITOR
        AssetDatabase.CreateAsset(ed, path);
#endif
    }

    public static string SOSavePath(string name)
    {
        // 폴더가 있는지 검사하고 없으면 만드는 기능 필요.
        return SO_PATH + "/" + name;
    }

}
