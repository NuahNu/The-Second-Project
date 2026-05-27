using System.Collections.Generic;
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
    [SerializeField] private Grid _grid;
    #endregion

    #region 내부 변수
    private List<CCharacter> _eEnemeyList = new List<CCharacter>();
    private Transform _root;
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
        if (_grid.IsNull("_grid"))
        {
            return;
        }
        InitRoot();

        Debug.Log("SpawnerAwakeDone");
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

    public CCharacter SpawnEnemy(Vector3 pos)
    {
        int id = Random.Range(1, 3);
        return SpawnCharacter(id, pos);
    }
    //

    public void Clear()
    {
        //
        foreach (var c in _eEnemeyList)
        {
            c.Kill();
        }
        _eEnemeyList.Clear();
        //// 아니면 그냥 root을 삭제하고 다시 만들자.
        //InitRoot();
    }

    #endregion

    #region protected

    #endregion

    #region private
    private void InitRoot()
    {
        if( _root != null)
            Destroy(_root.gameObject);
        _root = new GameObject("root").transform;
        _root.parent = this.transform;
    }
    private CCharacter SpawnCharacter(int id)
    {
        CCharacterDataSO data = _dataArraySO[CDataArraySO.EDataType.EnemyData][id] as CCharacterDataSO;

        CCharacter character = Instantiate(data.Prefab, _root);

        character.InitData(data, _grid);

        _eEnemeyList.Add(character);

        return character;
    }

    private CCharacter SpawnCharacter(int id, Vector3 pos)
    {
        CCharacterDataSO data = _dataArraySO[CDataArraySO.EDataType.EnemyData][id] as CCharacterDataSO;

        CCharacter character = Instantiate(data.Prefab, pos, Quaternion.identity, _root);

        character.InitData(data, _grid);

        _eEnemeyList.Add(character);

        return character;
    }
    #endregion
}
