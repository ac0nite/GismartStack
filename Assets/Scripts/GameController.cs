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
    [SerializeField] public float Speed = 10f;
    [SerializeField] private float _incrementSpeed = 0.1f;
    private bool _game = false;

    //[SerializeField] private ClickDetect _tapDetect = null;
    [SerializeField] private UIManager _uiManager = null;

    public event Action EventTapDown;
    public event Action EventReStart;
    public event Action EventChangeScore;
    public event Action EventChangeRecord;

    protected override void Awake()
    {
        base.Awake();
        _uiManager.EventGoGame += OnGoGame;
    }

    private void Start()
    {
        //CreateBlock(BlockPrefab.transform);

        //DEBUG
        // Saving.Instance.Read();
        // foreach (var block in Saving.Instance.Data.list)
        // { 
        //     CreateBlock(block.Position, block.Scale);
        // }
        //end DEBUG
    }

    private void Update()
    {
        if(_game && Input.GetMouseButtonDown(0))
            EventTapDown?.Invoke();
        //if(Input.GetKeyDown(KeyCode.Space))
    }

    private void OnDestroyBlock(BlockCollision _nextBlock, BlockCollision _oldBlock)
    {
//        Debug.Log($"_nextBlock!  pos:{_nextBlock.transform.position} scale:{_nextBlock.transform.localScale}");

        _oldBlock.EventDestroyBlock -= OnDestroyBlock;
        _oldBlock.EventExitRaund -= OnExitRaund;
        
        Destroy(_oldBlock.gameObject);
        Base.transform.Translate(-Vector3.up * (_nextBlock.transform.localScale.y));
        //Camera.main.transform.Translate(Vector3.up * (_nextBlock.transform.localScale.y));
        CreateBlock(_nextBlock.transform);
        EventChangeScore?.Invoke();
    }

    private void CreateBlock(Transform _newBlock)
    {
        var block = Instantiate(BlockPrefab);
        block.transform.localScale = _newBlock.localScale;

        //var nextPoint = PointBegin[UnityEngine.Random.Range(0, PointBegin.Count)];
        block.SetPoint(PointBegin[UnityEngine.Random.Range(0, PointBegin.Count)], _newBlock.position);
        block.SetEventTap();
        block.EventDestroyBlock += OnDestroyBlock;
        block.EventExitRaund += OnExitRaund;

        Speed += _incrementSpeed;
    }

    private void CreateBlock(Vector3 position, Vector3 localScale)
    {
        var block = Instantiate(BlockPrefab);
        block.transform.localScale = localScale;
        block.transform.SetPositionAndRotation(position, Quaternion.identity);
        block.Movement.Stop();
    }
    
    private void OnExitRaund(BlockCollision _oldBlock)
    {
        _game = false;
        _oldBlock.EventDestroyBlock -= OnDestroyBlock;
        _oldBlock.EventExitRaund -= OnExitRaund;
        Destroy(_oldBlock.gameObject);
        Debug.Log("Exit Rount");

        var stack = Base.gameObject.GetComponentsInChildren<BlockCollision>();
        foreach (var block in stack)
        {
            Saving.Instance.Append(block.transform);
        }

        Saving.Instance.Append(123);
        Saving.Instance.Write();
        EventChangeRecord?.Invoke();
    }

    private void OnDestroy()
    {
        //_tapDetect.EventStartTapClick -= OnStartTapClick;
        _uiManager.EventGoGame -= OnGoGame;
    }

    private void OnStartTapClick()
    {
        EventTapDown?.Invoke();
    }

    private void OnGoGame()
    {
        Debug.Log($"OnGoGame");

        Init();
        
        CreateBlock(BlockPrefab.transform);
        _game = true;
        //DEBUG
        // Saving.Instance.Read();
        // foreach (var block in Saving.Instance.Data.list)
        // { 
        //     CreateBlock(block.Position, block.Scale);
        // }
        //end DEBUG
    }

    private void Init()
    {
        var blocks = Base.GetComponentsInChildren<BlockCollision>().ToList();
        if (blocks.Count > 1)
        {
            for (int i = 1; i < blocks.Count; i++)
            {
                Destroy(blocks[i].gameObject);
            }

            blocks[0].CollisionDetect = true;
            Base.transform.Translate(Vector3.up);
        }
    }
}