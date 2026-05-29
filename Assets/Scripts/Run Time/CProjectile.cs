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

public class CProjectile : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite[] _sprites;
    [SerializeField] private Vector2[] _colliderPos;
    [SerializeField] private float _lifeTime = 3;

    // 2D 화면상의 높이를 표현하기 위한 값.
    [SerializeField] private float _heightOffset;

    [SerializeField] private float _speed;
    [SerializeField] private Vector2 _dir;

    [SerializeField] private float _att;
    #endregion

    #region 내부 변수
    private Collider2D _collider;
    private float _timeElapsed;
    #endregion

    public float ATT { get { return _att; } set { _att = value; } }

    #region 유니티 이벤트
    void Awake()
    {
        if (_spriteRenderer.IsNull("_spriteRenderer")) return;

        _spriteRenderer.transform.localPosition = new Vector3(0, _heightOffset, 0);

        if (!TryGetComponent(out _collider))
        {
            Debug.LogWarning("_collider == null");
            return;
        }
        if (_collider is CircleCollider2D CC)
        {
            CC.radius = 0.01f;
        }
    }

    void Start()
    {

    }

    private void OnEnable()
    {
        _timeElapsed = 0;
    }

    void Update()
    {
        Vector3 speed = _dir.GetClosestDirection() * _speed;
        transform.position = transform.position + speed * Time.deltaTime;

        _timeElapsed += Time.deltaTime;
        if (_timeElapsed > _lifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_collider != null)
        {
            Debug.Log($"{name} with {collision.gameObject.name}");
            // 아군이 아니고
            if (tag != collision.gameObject.tag)
            {
                // 캐릭터라면
                if (collision.gameObject.TryGetComponent(out CCharacter character))
                {
                    character.GetDamage(_att);
                }
                _lifeTime = 0;
            }
        }
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

    public void SetLifeTime(float lifeTime)
    {
        _lifeTime = lifeTime;
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

        Vector2 offset = _colliderPos[index];

        _collider.offset = offset;
    }
    #endregion
}
