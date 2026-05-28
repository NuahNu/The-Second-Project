using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#region CProjectile
/*

*/
#endregion

[RequireComponent(typeof(SpriteRenderer))]
public class CProjectile : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private Sprite[] _sprites;
    [SerializeField] private float _longLength;
    [SerializeField] private float _shortLength;
    [SerializeField] private float _angle;
    #endregion

    #region 내부 변수
    private SpriteRenderer _spriteRenderer;
    #endregion

    #region 유니티 이벤트
    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
    #endregion

    #region public
    public void SetDir(Vector2 dir)
    {
        SetCollierPos(dir);
    }
    #endregion

    #region protected

    #endregion

    #region private
    private void SetCollierPos(Vector2 dir)
    {

    }
    #endregion
}
