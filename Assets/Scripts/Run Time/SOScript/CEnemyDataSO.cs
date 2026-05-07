using System;
using UnityEngine;


#region CEnemyDataSO
/*

*/
#endregion

public class CEnemyDataSO : ScriptableObject, ICSVData
{
    static readonly string NAME = "EnemyData";

    public enum ECharactorType
    {
        CommonSoldier
    }

    // 시트에서 읽어올 데이터
    #region 인스펙터
    [SerializeField] private int _ID;
    [SerializeField] private string _name;
    [SerializeField] private float _att;
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _maxHp;
    [SerializeField] private ECharactorType _type;
    [SerializeField] private Color _filterColor;
    #endregion

    // 실 사용을 위해 가공이 필요한 경우
    #region 내부 변수

    #endregion

    // 실제로 사용할 데이터
    #region 프로퍼티
    public int ID => _ID;


    #endregion

    #region public
    public string ParseData(string data)
    {
        string[] dataArr = data.Split(",");

        _ID = int.Parse(dataArr[0]);
        _name = dataArr[1]; 
        _att = float.Parse(dataArr[2]);
        _walkSpeed = float.Parse(dataArr[3]);
        _runSpeed = float.Parse(dataArr[4]);
        _maxHp = float.Parse(dataArr[5]);
        _type = _type.ParseData(dataArr[6]);


        return CGSSLoader.SOSavePath(NAME) + $"/{NAME}SO_{_ID}.asset";
    }
    #endregion

    #region private

    #endregion
}
