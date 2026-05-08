using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;


#region CCamera
/*

*/
#endregion

public class CCamera : MonoBehaviour
{
    #region 인스펙터
    // 이것도 비율로?
    [SerializeField]private CTileMapMaker _mapMaker;
    [Header("카메라 이동")]
    [SerializeField] private int _edgeSize = 10;
    [SerializeField] private float _cameraSpeedRatio = 1f;
    [SerializeField] private bool _useClamp = true;

    [Header("스크롤")]
    [SerializeField] private float _scrollRatio = 0.01f;
    [SerializeField] private Vector2 _scrollClamp = new Vector2(1, 10);
    [SerializeField] private bool _scrollFlip = true;
    #endregion

    #region 내부 변수
    private Camera _camera;


    private Rect _gridRect;
    #endregion

    #region 유니티 이벤트
    void Awake()
    {
        _camera = Camera.main;
        if (_mapMaker.IsNull("_mapMaker")) return;
    }

    void Start()
    {

    }

    private void OnEnable()
    {
        _mapMaker.OnMakeMap += ReSizeRect;
    }

    private void OnDisable()
    {
        _mapMaker.OnMakeMap -= ReSizeRect;
    }

    void Update()
    {

        Vector2 mousePos = Mouse.current.position.ReadValue();

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float cameraSpeed = _camera.orthographicSize * _cameraSpeedRatio;
        //Debug.Log($"{cameraSpeed} = {_camera.orthographicSize} * {_cameraSpeedRatio}");

        Vector3 delta = new Vector3();

        if (mousePos.x < screenWidth - _edgeSize)
            delta.x -= cameraSpeed * Time.deltaTime;

        if (mousePos.x > _edgeSize)
            delta.x += cameraSpeed * Time.deltaTime;

        if (mousePos.y < screenHeight - _edgeSize)
            delta.y -= cameraSpeed * Time.deltaTime;

        if (mousePos.y > _edgeSize)
            delta.y += cameraSpeed * Time.deltaTime;

        _camera.transform.position += delta;
        // clamp
        if (_useClamp)
        {
            float x = Mathf.Clamp(_camera.transform.position.x, -_gridRect.width / 2, _gridRect.width / 2);
            float y = Mathf.Clamp(_camera.transform.position.y, -_gridRect.height / 2, _gridRect.height / 2);

            _camera.transform.position = new Vector3(x, y, Define.CAMERA_Z);
        }
        // scroll
        Vector2 scrollDelta = Mouse.current.scroll.ReadValue();

        if (scrollDelta.y != 0)
        {
            float scroll = scrollDelta.y * _scrollRatio;
            _camera.orthographicSize += _scrollFlip ? scroll : -scroll;
            // clamp
            _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize, _scrollClamp.x, _scrollClamp.y);
        }

    }
    #endregion

    #region public

    #endregion

    #region protected

    #endregion

    #region private
    private void ReSizeRect(Rect rect)
    {
        _gridRect = rect;
    }
    #endregion
}
