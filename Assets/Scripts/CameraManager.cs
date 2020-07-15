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

    private void Awake()
    {
        _camera = Camera.main;
        _defualtTransformCamera = _camera.transform;
//        Debug.Log($"_defualtTransformCamera: {_defualtTransformCamera.transform.position}");
        _nextViewCamera = _defualtTransformCamera.transform.position;
        _currentViewCamera = _defualtTransformCamera.transform.position;
        GameController.Instance.EventChangeRecord += OnChangeCameraView;
        _uiManager.EventGoGame += OnSetDefaultCamera;
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
        var blocks2 = blocks.ToList();

        float max = blocks2.Max(b => _camera.WorldToViewportPoint(b.transform.position).magnitude);
        float min = blocks2.Min(b => _camera.WorldToViewportPoint(b.transform.position).magnitude);

        Debug.Log($"max: {max}  min: {min}");

        _nextViewCamera = _nextViewCamera.normalized * (_nextViewCamera.magnitude + (max - min) * 1.2f);

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
        
    }

    void LateUpdate()
    {
        _currentViewCamera = Vector3.Lerp(_camera.transform.position, _nextViewCamera, 0.5f * Time.deltaTime);
//        Debug.Log($"c:{_camera.transform.position}  n:{_currentViewCamera}");
        _camera.transform.SetPositionAndRotation(_currentViewCamera, _camera.transform.rotation);
        //_camera.transform.Translate(_currentViewCamera);
    }
}
