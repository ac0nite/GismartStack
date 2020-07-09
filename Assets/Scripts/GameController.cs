using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameController : Singletone<GameController>
{
    [SerializeField] public BlockCollision BlockPrefab = null;
    [SerializeField] public List<Transform> PointBegin = null;
    [SerializeField] public Transform Base = null;
    [SerializeField] public float Speed = 10f;
    [SerializeField] private float _incrementSpeed = 0.1f;

    [SerializeField] private ClickDetect _tapDetect = null;
    [SerializeField] private UIManager _uiManager = null;

    public event Action EventTapDown;
    public event Action EventReStart;

    protected override void Awake()
    {
        base.Awake();
        _tapDetect.EventStartTapClick += OnStartTapClick;
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
        //if(Input.GetKeyDown(KeyCode.Space))
        //    EventTapDown?.Invoke();
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
        _oldBlock.EventDestroyBlock -= OnDestroyBlock;
        _oldBlock.EventExitRaund -= OnExitRaund;
        Destroy(_oldBlock.gameObject);
        Debug.Log("Exit Rount");
        
        // List<GameObject> testList = new List<GameObject>();
        // string filename = "MyListTest.JSON";
        // string path = Application.persistentDataPath + "/" +filename;
        // Debug.Log(path);
        // //string contents = JsonUtility.ToJson(data);
        // //File.WriteAllText (path, contents);
        //
        //
        //
        // SaveDataTest s = new SaveDataTest();
        // //s.list = new List<SaveDataTransform>();
        // s.list.Add(new SaveDataTransform(transform));
        //
        // string c = JsonUtility.ToJson(s);
        // File.WriteAllText (path, c);
        var stack = Base.gameObject.GetComponentsInChildren<BlockCollision>();
        foreach (var block in stack)
        {
            //data.add(block.transform);
            //Saving.Instance.Data.Add(block.transform);
            Saving.Instance.Append(block.transform);
        }

        Saving.Instance.Append(123);
        //Saving.Instance.Data.Score = 123;
        //data.Score = 123f;
        //data.UpdateDate();
        //string filename = "MyListTest.JSON";
        //string path = Application.persistentDataPath + "/" + filename;
        //string content = JsonUtility.ToJson(data);
       // string content2 = JsonConvert.SerializeObject(data);
        //File.WriteAllText (path, content);
        Saving.Instance.Write();
        
        //SaveData dataLoad = new SaveData();
        //string contentLoad = File.ReadAllText(path);
        //dataLoad = JsonUtility.FromJson<SaveData>(contentLoad);
        
    }

    private void OnDestroy()
    {
        _tapDetect.EventStartTapClick -= OnStartTapClick;
        _uiManager.EventGoGame -= OnGoGame;
    }

    private void OnStartTapClick()
    {
        EventTapDown?.Invoke();
    }

    private void OnGoGame()
    {
        Debug.Log($"OnGoGame");
        CreateBlock(BlockPrefab.transform);

        //DEBUG
        // Saving.Instance.Read();
        // foreach (var block in Saving.Instance.Data.list)
        // { 
        //     CreateBlock(block.Position, block.Scale);
        // }
        //end DEBUG
    }
}