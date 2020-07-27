using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Camera _camera = null;
    [SerializeField] private UIManager _uiManager = null;
    private Vector3 _defaultPositionCamera = Vector3.zero;
    private Vector3 _currentViewCamera = Vector3.zero;
    private Vector3 _nextViewCamera = Vector3.zero;

    [SerializeField] private Transform BaseCube = null;
    [SerializeField] private Transform _plane = null;

    private void Awake()
    {
        _camera = Camera.main;
        _defaultPositionCamera = _camera.transform.position;
        _nextViewCamera = _defaultPositionCamera;
        _currentViewCamera = _defaultPositionCamera;
        GameController.Instance.EventChangeRecord += OnChangeCameraView;
        _uiManager.EventGoGame += OnSetDefaultCamera;
        
        Debug.Log($"_defualtTransformCamera: {_defaultPositionCamera}");
    }

    private void OnDestroy()
    {
        GameController.Instance.EventChangeRecord -= OnChangeCameraView;
        _uiManager.EventGoGame -= OnSetDefaultCamera;
    }

    private void OnSetDefaultCamera()
    {
        Debug.Log($"OnSetDefaultCamera: {_defaultPositionCamera}");
        _nextViewCamera = _defaultPositionCamera;
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
            _nextViewCamera = _defaultPositionCamera;
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

        _nextViewCamera = _nextViewCamera.normalized * (_nextViewCamera.magnitude + (max - min));
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

        //Debug.DrawLine(a, b, Color.red, 1f); //x
        //Debug.DrawLine(b, c, Color.grey, 1f);
        //Debug.DrawLine(c, a, Color.blue, 1f); //y

        var center = Vector3.Lerp(b, c, 0.5f);
        center.y += 0.5f;

        var x1 = (a - b).magnitude;
        var x2 = (c - a).magnitude;

        _plane.transform.localScale = new Vector3((b-a).magnitude / 10,  _plane.transform.localScale.y, (a - c).magnitude / 10);
        _plane.transform.SetPositionAndRotation(center, _plane.transform.rotation);
    }

    void LateUpdate()
    {
        _currentViewCamera = Vector3.Lerp(_camera.transform.position, _nextViewCamera, 5f * Time.deltaTime);
        _camera.transform.SetPositionAndRotation(_currentViewCamera, _camera.transform.rotation);
    }
}
