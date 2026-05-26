using UnityEngine;


#region CSpawner
/*
추적 관리가 필요하면 여기서 한다.
*/
#endregion

public class CSpawner : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private CDataArraySO _dataArraySO;
    #endregion

    #region 내부 변수

    #endregion

    #region 유니티 이벤트
    void Awake()
    {
        if (_dataArraySO.IsNull("dataArraySO"))
        {
            return;
        }
    }

    void Start()
    {

    }

    void Update()
    {

    }
    #endregion

    #region public

    // 선택 규칙 추가. 지금은 그냥 소환중
    public CCharacter SpawnEnemy()
    {
        int index = Random.Range(0, 1);
        CEnemyDataSO cEnemyDataSO = _dataArraySO[CDataArraySO.EDataType.EnemyData][index] as CEnemyDataSO;

        CCharacter enemy = Instantiate(cEnemyDataSO.Prefab);

        enemy.InitData(cEnemyDataSO);

        return enemy;
    }
    #endregion

    #region protected

    #endregion

    #region private

    #endregion
}
