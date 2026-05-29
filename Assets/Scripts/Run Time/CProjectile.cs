using UnityEngine;


#region CProjectile
/*
원하는 스프라이트를 넣고
스프라이트에 맞는 콜라이더 위치도 넣고
기본 속도값도 넣고

생성 후 방향을 설정하고
속도를 바꾸고 싶다면 속도도 바꿔서
*/
#endregion

[RequireComponent(typeof(SpriteRenderer))]
public class CProjectile : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private Sprite[] _sprites;
    [SerializeField] private Vector2[] _colliderPos;
    [SerializeField] private float _speed;
    [SerializeField] private Vector2 _dir;

    #endregion

    #region 내부 변수
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    #endregion

    #region 유니티 이벤트
    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if(!TryGetComponent(out _collider))
        {
            Debug.LogWarning("_collider == null");
            return;
        }
        if(_collider is CircleCollider2D CC)
        {
            CC.radius = 0.01f;
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        Vector3 speed = _dir.GetClosestDirection() * _speed;
        transform.position = transform.position + speed * Time.deltaTime;
    }
    #endregion

    #region public
    public void SetDir(Vector2 dir)
    {
        _dir = dir;
        SetDir();
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }
    #endregion

    #region protected

    #endregion

    #region private
    [ContextMenu("CallSetDir")]
    private void SetDir()
    {
        int index = _dir.GetClosestIndex();
        _spriteRenderer.sprite = _sprites[index];
        _collider.offset = _colliderPos[index];
    }
    #endregion
}
