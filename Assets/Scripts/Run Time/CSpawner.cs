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
    [SerializeField] private CCharacter _player;
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
        if (_player.IsNull("_player"))
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
    public CCharacter SpawnPlayer(int i = 0)
    {
        return SpawnCharacter(i);
    }

    public CCharacter SpawnEnemy()
    {
        int id = Random.Range(1, 3);
        return SpawnCharacter(id);
    }
    //
    private CCharacter SpawnCharacter(int id)
    {
        CCharacterDataSO data = _dataArraySO[CDataArraySO.EDataType.EnemyData][id] as CCharacterDataSO;

        CCharacter character = Instantiate(data.Prefab);

        character.InitData(data);

        return character;
    }
    #endregion

    #region protected

    #endregion

    #region private

    #endregion
}
