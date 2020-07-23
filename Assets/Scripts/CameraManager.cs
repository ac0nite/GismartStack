using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Camera _camera = null;
    [SerializeField] private UIManager _uiManager = null;
    private Transform _defualtTransformCamera = null;
    private Vector3 _currentViewCamera = Vector3.zero;
    private Vector3 _nextViewCamera = Vector3.zero;

    [SerializeField] private Transform BaseCube = null;
    [SerializeField] private Transform _plane = null;
    private List<Vector3> points = new List<Vector3>();

    private void Awake()
    {
        _camera = Camera.main;
        _defualtTransformCamera = _camera.transform;
//        Debug.Log($"_defualtTransformCamera: {_defualtTransformCamera.transform.position}");
        _nextViewCamera = _defualtTransformCamera.transform.position;
        _currentViewCamera = _defualtTransformCamera.transform.position;
        GameController.Instance.EventChangeRecord += OnChangeCameraView;
        _uiManager.EventGoGame += OnSetDefaultCamera;

        points.Add(new Vector3(_plane.transform.position.x + _plane.localScale.x / 2, _plane.transform.position.y + _plane.localScale.y/2, _plane.localScale.z ));
        points.Add(new Vector3(_plane.transform.position.x - _plane.localScale.x / 2, _plane.transform.position.y + _plane.localScale.y / 2, _plane.localScale.z));
        points.Add(new Vector3(_plane.transform.position.x - _plane.localScale.x / 2, _plane.transform.position.y - _plane.localScale.y / 2, _plane.localScale.z));
        points.Add(new Vector3(_plane.transform.position.x + _plane.localScale.x / 2, _plane.transform.position.y - _plane.localScale.y / 2, _plane.localScale.z));
    }

    private void OnDestroy()
    {
        GameController.Instance.EventChangeRecord -= OnChangeCameraView;
        _uiManager.EventGoGame -= OnSetDefaultCamera;
    }

    private void OnSetDefaultCamera()
    {
        //lerp?
        //transform.SetPositionAndRotation(_defualtTransformCamera.position, _defualtTransformCamera.rotation);
        _nextViewCamera = _defualtTransformCamera.transform.position;
    }

    private void OnChangeCameraView()
    {
        ChangeViewCamera();
    }

    public void ChangeViewCamera()
    {
        var blocks = BaseCube.GetComponentsInChildren<BlockCollision>();
        if (blocks.Length == 1)
        {
            _nextViewCamera = _defualtTransformCamera.position;
            return;
        }
        
        Debug.Log($"blocks length = {blocks.Length}");
        foreach (var block in blocks)
        {
            Debug.Log($"{_camera.WorldToViewportPoint(block.transform.position)} {block.transform.position}", block.gameObject);
        }
        var blocks2 = blocks.ToList();

        float max = blocks2.Max(b => _camera.WorldToViewportPoint(b.transform.position).magnitude);
        float min = blocks2.Min(b => _camera.WorldToViewportPoint(b.transform.position).magnitude);

        Debug.Log($"max: {max}  min: {min}");

        //_nextViewCamera = _nextViewCamera.normalized * (_nextViewCamera.magnitude + (max - min));

        // foreach (var block in blocks)
        // {
        //     Debug.Log($"ViePortBlock: {_camera.WorldToViewportPoint(block.transform.position)}  position: {block.transform.position}", block);
        // }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var a = _camera.ViewportToWorldPoint(new Vector3(0, 0, _camera.farClipPlane));
        var b = _camera.ViewportToWorldPoint(new Vector3(1, 0, _camera.farClipPlane));
        var c = _camera.ViewportToWorldPoint(new Vector3(0, 1, _camera.farClipPlane));

        //var a = _camera.ViewportToScreenPoint(new Vector3(0, 0, _camera.farClipPlane));
        //var b = _camera.ViewportToScreenPoint(new Vector3(1, 0, _camera.farClipPlane));
        //var c = _camera.ViewportToScreenPoint(new Vector3(0, 1, _camera.farClipPlane));

        Debug.DrawLine(a, b, Color.red, 1f); //x
        Debug.DrawLine(b, c, Color.grey, 1f);
        Debug.DrawLine(c, a, Color.blue, 1f); //y

        var center = Vector3.Lerp(b, c, 0.5f);
        //Debug.DrawLine(c, center, Color.red, 1f);

        var x1 = (a - b).magnitude;
        var x2 = (c - a).magnitude;
        Debug.Log($"{x1}  {x2}");

        //var a1 = _plane.transform.TransformPoint(a);
        //var b1 = _plane.transform.TransformPoint(b);
        //var c1 = _plane.transform.TransformPoint(c);

        //Debug.DrawLine(a1, b1, Color.red, 1f); //x
        //Debug.DrawLine(b1, c1, Color.grey, 1f);
        //Debug.DrawLine(c1, a1, Color.blue, 1f); //

        _plane.transform.localScale = new Vector3((b-a).magnitude / 10,  _plane.transform.localScale.y, (a - c).magnitude / 10);
        //_plane.transform.SetPositionAndRotation(center, _plane.transform.rotation);
    }

    void LateUpdate()
    {
        _currentViewCamera = Vector3.Lerp(_camera.transform.position, _nextViewCamera, 0.5f * Time.deltaTime);
//        Debug.Log($"c:{_camera.transform.position}  n:{_currentViewCamera}");
        _camera.transform.SetPositionAndRotation(_currentViewCamera, _camera.transform.rotation);
        //_camera.transform.Translate(_currentViewCamera);
    }
}
