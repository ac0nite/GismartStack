using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Camera _camera = null;
    [SerializeField] private UIManager _uiManager = null;
    private Transform _defualtTransformCamera = null;

    private void Awake()
    {
        _camera = Camera.main;
        _defualtTransformCamera = _camera.transform;
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
        transform.SetPositionAndRotation(_defualtTransformCamera.position, _defualtTransformCamera.rotation);
    }

    private void OnChangeCameraView()
    {
        //lerp
        _camera.pixelHeight
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
