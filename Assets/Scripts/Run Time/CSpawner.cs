using System.Collections.Generic;
using UnityEngine;


#region CSpawner
/*
추적 관리가 필요하면 여기서 한다.
*/
#endregion

public enum EProjectileType
{
    Arrow,
    Count
}

public class CSpawner : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private CDataArraySO _dataArraySO;
    [SerializeField] private CCharacter _player;
    [SerializeField] private Grid _grid;

    [SerializeField] private CProjectile[] _projectilePrefabs;
    #endregion

    #region 내부 변수
    private List<CCharacter> _enemeyList = new List<CCharacter>();
    private List<CProjectile> _projectileList = new List<CProjectile>();
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

        if(_projectilePrefabs.Length != (int)EProjectileType.Count)
        {
            Debug.LogWarning($"_ProjectilePrefabs.Length != (int)EProjectileType.Count {(int)EProjectileType.Count}");
            return;
        }

        Debug.Log("SpawnerAwakeDone");
    }

    void Start()
    {
        _player.InitData(_dataArraySO[CDataArraySO.EDataType.CharacterData][0] as CCharacterDataSO, _grid);
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

        CCharacter character = SpawnCharacter(id, pos);

        character.tag = "Enemy";

        return character;
    }
    //

    public void Clear()
    {
        //
        foreach (var c in _enemeyList)
        {
            c.Kill();
        }
        _enemeyList.Clear();

        foreach(var c in _projectileList)
        {
            c.SetLifeTime(0);
        }
        _projectileList.Clear();
        //// 아니면 그냥 root을 삭제하고 다시 만들자.
        //InitRoot();
    }

    public CProjectile SpawnProjectile(EProjectileType type)
    {
        CProjectile pro = Instantiate(_projectilePrefabs[(int)type], _root);

        _projectileList.Add(pro);

        return pro;
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
        CCharacterDataSO data = _dataArraySO[CDataArraySO.EDataType.CharacterData][id] as CCharacterDataSO;

        CCharacter character = Instantiate(data.Prefab, _root);

        character.InitData(data, _grid);

        _enemeyList.Add(character);

        return character;
    }

    private CCharacter SpawnCharacter(int id, Vector3 pos)
    {
        CCharacterDataSO data = _dataArraySO[CDataArraySO.EDataType.CharacterData][id] as CCharacterDataSO;

        CCharacter character = Instantiate(data.Prefab, pos, Quaternion.identity, _root);

        character.InitData(data, _grid);

        _enemeyList.Add(character);

        return character;
    }
    #endregion
}
