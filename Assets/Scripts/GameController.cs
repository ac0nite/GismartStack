using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameController : Singletone<GameController>
{
    [SerializeField] public BlockCollision BlockPrefab = null;
    [SerializeField] public List<Transform> PointBegin = null;
    [SerializeField] public Transform Base = null;
    [SerializeField] public float Speed = 6f;
    [SerializeField] private float _incrementSpeed = 0.1f;
    [SerializeField] private CameraManager _cameraManager = null;
    [SerializeField] private ClickDetect _clickDetect = null;
    private float _startSpeed = 0f;
    private bool _game = false;
    private Vector3 _targetBase = Vector3.zero;
    private Vector3 _Base = Vector3.zero;

    //[SerializeField] private ClickDetect _tapDetect = null;
    [SerializeField] private UIManager _uiManager = null;

    public event Action EventTapDown;
    public event Action EventReStart;
    public event Action EventChangeScore;
    public event Action EventChangeRecord;

    protected override void Awake()
    {
        if (TryInstance != null)
        {
            //if(_uiManager != null)
            _uiManager.EventGoGame += OnGoGame;
            // if(_clickDetect != null)
            _clickDetect.EventEndLoadScreen += OnLoadPreviousRound;
        }

        base.Awake();
        _targetBase = Base.transform.position;
        _startSpeed = Speed;
    }

    private void Start()
    {
    }

    private void Update()
    {
        if(_game && Input.GetMouseButtonDown(0))
            EventTapDown?.Invoke();
        
        Base.transform.position = Vector3.Lerp(Base.transform.position, _targetBase, Speed * Time.deltaTime);
    }

    private void OnDestroyBlock(BlockCollision _nextBlock, BlockCollision _oldBlock)
    {
//        Debug.Log($"_nextBlock!  pos:{_nextBlock.transform.position} scale:{_nextBlock.transform.localScale}");

        _oldBlock.EventDestroyBlock -= OnDestroyBlock;
        _oldBlock.EventExitRaund -= OnExitRaund;
        
        Destroy(_oldBlock.gameObject);
        
        _targetBase = _targetBase + (Vector3.down * (_nextBlock.transform.localScale.y));
        //Base.transform.Translate(Vector3.down * (_nextBlock.transform.localScale.y));
        Vector3 b = Base.transform.position;

        CreateBlock(_nextBlock.transform);
        EventChangeScore?.Invoke();
    }

    private void CreateBlock(Transform _newBlock)
    {
        var block = Instantiate(BlockPrefab);
        block.transform.localScale = _newBlock.localScale;
        block.BlockColor.setColor();
        block.BlockColor.applyColor();
        
        //var nextPoint = PointBegin[UnityEngine.Random.Range(0, PointBegin.Count)];
        block.SetPoint(PointBegin[UnityEngine.Random.Range(0, PointBegin.Count)], _newBlock.position);
        block.SetEventTap();
        block.EventDestroyBlock += OnDestroyBlock;
        block.EventExitRaund += OnExitRaund;

        Speed += _incrementSpeed;
    }

    private BlockCollision CreateBlock(Vector3 position, Vector3 localScale, Color color)
    {
        //Debug.Log($"CreateBlock Color: {color}");
        var block = Instantiate(BlockPrefab);
        block.transform.localScale = localScale;
        block.transform.SetPositionAndRotation(position, Quaternion.identity);
        block.Movement.Stop();
        block.BlockColor.applyColor(color);
        return block;
    }
    
    private void OnExitRaund(BlockCollision _oldBlock)
    {
        _game = false;
        _oldBlock.EventDestroyBlock -= OnDestroyBlock;
        _oldBlock.EventExitRaund -= OnExitRaund;
        Destroy(_oldBlock.gameObject);
        Debug.Log("Exit Rount");

        var stack = Base.gameObject.GetComponentsInChildren<BlockCollision>();
        Saving.Instance.Clear();
        foreach (var block in stack)
        {
            Saving.Instance.Append(block.transform, block.BlockColor.Color);
        }
        
        Saving.Instance.Write();
        EventChangeRecord?.Invoke();
    }

    private void OnApplicationQuit()
    {
        if (TryInstance != null)
        {
            _uiManager.EventGoGame -= OnGoGame;
            _clickDetect.EventEndLoadScreen -= OnLoadPreviousRound;   
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    private void OnStartTapClick()
    {
        EventTapDown?.Invoke();
    }

    private void OnLoadPreviousRound()
    {
        Debug.Log("OnLoadPreviousRound");
        Init(true);
        if (Saving.Instance.Read())
        {
            Debug.Log("LoadPreviousRound");
            foreach (var block in Saving.Instance.Data.list)
            { 
                CreateBlock(block.Position, block.Scale, block.Color).transform.SetParent(Base.transform);
            }   
        }
        else
        {
            Debug.Log($"create one block");
            //var block = CreateBlock(new Vector3(-1f, 0f, -1f), BlockPrefab.transform.localScale);
            var block = CreateBlock(Vector3.zero, BlockPrefab.transform.localScale, GradientManager.Instance.GetColor());
            block.transform.SetParent(Base.transform);
            block.CollisionDetect = true;
        }
    }
    private void OnGoGame()
    {
        Debug.Log($"OnGoGame");

        Init(false);
        
        CreateBlock(BlockPrefab.transform);
        _game = true;
        Speed = _startSpeed;
        //DEBUG
        // Saving.Instance.Read();
        // foreach (var block in Saving.Instance.Data.list)
        // { 
        //     CreateBlock(block.Position, block.Scale);
        // }
        //end DEBUG
    }

    private void Init(bool clean)
    {
        var blocks = Base.GetComponentsInChildren<BlockCollision>().ToList();
        
        //если чистая сцена
        // if (blocks.Count == 0)
        // {
        //     CreateBlock(BlockPrefab.transform);
        // }
        
        //удаление всех блоков
        if (clean)
        {
            foreach (var block in blocks)
            {
                Destroy(block.gameObject);
            }
        }
        // удалление блоков кроме нижнего + сдвигаем вверх
        else
        {
            if (blocks.Count > 1)
            {
                for (int i = 1; i < blocks.Count; i++)
                {
                    Destroy(blocks[i].gameObject);
                }

                blocks[0].CollisionDetect = true;
                blocks[0].transform.SetPositionAndRotation(Vector3.zero, Base.transform.rotation);
                blocks[0].BlockColor.applyColor(GradientManager.Instance.GetColor());
                //Base.transform.SetPositionAndRotation(new Vector3(1f, 0f, 1f), Base.transform.rotation);
                Debug.Log("@@@@@");
                //Base.transform.Translate(new Vector3(1f,0f,1f));
            }   
        }
    }
}