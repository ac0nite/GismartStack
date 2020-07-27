using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using Object = System.Object;
using Vector3 = UnityEngine.Vector3;

public class BlockCollision : MonoBehaviour
{
    [SerializeField] private float _removeAfterCollision = 1.5f;
    private ContactPoint[] _contactPoints = new ContactPoint[4];
    public event Action<BlockCollision, BlockCollision> EventDestroyBlock;
    public event Action<BlockCollision> EventExitRaund;
    public bool CollisionDetect = false;
    public BlockMovement Movement { get; private set; }
    private Rigidbody _rigidbody = null;
    private Collider _collider = null;
    [SerializeField] public BlockColor BlockColor = null;
    private void Awake()
    {
        Movement = GetComponent<BlockMovement>();
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    private void Start()
    {
        
    }

    public void SetEventTap()
    {
        GameController.Instance.EventTapDown += OnTapDown;
    }

    private void OnCollisionEnter(Collision other)
    {
    }

    private void OnCollisionStay(Collision other)
    {
        if(!_rigidbody.useGravity)
            return;

        if (!other.transform.GetComponent<BlockCollision>().CollisionDetect)
        {
            return;   
        }

        other.GetContacts(_contactPoints);
        foreach (ContactPoint contact in _contactPoints)
        {
            Debug.DrawRay(contact.point, contact.normal * 2, Color.red, 1f);
        }

        var arr_point = _contactPoints.Select(a => a.point).ToList();

        Debug.DrawLine(arr_point[0], arr_point[1], Color.green, 1f);
        Debug.DrawLine(arr_point[0], arr_point[2], Color.green, 1f);
        Debug.DrawLine(arr_point[0], arr_point[3], Color.green, 1f);
        List<Vector3> point = new List<Vector3>();

        point.Add(arr_point[0] - arr_point[1]);
        point.Add(arr_point[0] - arr_point[2]);
        point.Add(arr_point[0] - arr_point[3]);
        
        point.Sort((a, b) => a.magnitude.CompareTo(b.magnitude));

        Vector3 av = point[0];
        Vector3 bv = point[1];
        
        Debug.Log($"  old scale:{transform.localScale}  position:{transform.position}");
        Debug.Log($"side: {point[0].magnitude}  {point[1].magnitude} {point[2].magnitude}");
        
        Vector3 center = Vector3.Lerp(arr_point[0] - point[0], arr_point[0] - point[1], 0.5f);
        Debug.DrawLine(arr_point[0], center, Color.blue, 1f);

        var forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);

        Vector3 scale = Vector3.zero;
        Vector3 position = Vector3.zero;
        
        position = new Vector3(center.x, transform.position.y, center.z);
        
        var side = Math.Abs(Vector3.Dot(av.normalized, Vector3.forward));
        Debug.Log($"dot: {side}");
        if (side >= 0.9f)
        {
            scale = new Vector3(bv.magnitude, transform.localScale.y, av.magnitude);   
        }
        else
        {
            scale = new Vector3(av.magnitude, transform.localScale.y, bv.magnitude);
        }

        Debug.Log($"scale: {scale}");

        var block = Instantiate(GameController.Instance.BlockPrefab);

        block.transform.localScale = scale;
        block.transform.position = position;
        block.BlockColor.Color = BlockColor.Color;
        block.BlockColor.applyColor();

        int direction = 1;
        if (Math.Abs(Vector3.Dot(Movement.Forward(), Vector3.forward)) < 0.001f)
        {
            Debug.Log("left-right");
            direction = (block.transform.position.x > transform.position.x) ? -1 : 1;
            var delta_x = (Math.Abs(Vector3.Dot(av.normalized, Vector3.left)) >= 0.9f) ? av.magnitude : bv.magnitude;
            Debug.Log($"delta_x: {delta_x}");
            transform.position = new Vector3(transform.position.x + delta_x/2f * direction, transform.position.y, transform.position.z);
            transform.localScale = new Vector3(transform.localScale.x - delta_x, transform.localScale.y, transform.localScale.z);
            
            _rigidbody.AddForce(Vector3.right * direction * 2f, ForceMode.Impulse);
        }
        else
        {
            Debug.Log("top-botton");
            direction = (block.transform.position.z > transform.position.z) ? -1 : 1;
            var delta_z = (Math.Abs(Vector3.Dot(av.normalized, Vector3.forward)) >= 0.9f) ? av.magnitude : bv.magnitude;
            Debug.Log($"delta_z: {delta_z}");
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + delta_z/2f * direction);
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z - delta_z);
            
            _rigidbody.AddForce(Vector3.forward * direction * 2f, ForceMode.Impulse);
        }

        Debug.Log($"1 new scale:{scale}  position:{position}");
        Debug.Log($"2 new scale:{transform.localScale}  position:{transform.position}");

        StartCoroutine(NextBlock(block));
        block.transform.SetParent(GameController.Instance.Base.transform);

        block._rigidbody.useGravity = true;
        block.Movement.Stop();
        block.CollisionDetect = false;

        other.transform.GetComponent<BlockCollision>().CollisionDetect = false;
        CollisionDetect = false;
    }

    private void OnTapDown()
    {
        _rigidbody.useGravity = true;    
        _rigidbody.isKinematic = false;
        Movement.Stop();
        StartCoroutine(ObjectExist());
        GameController.Instance.EventTapDown -= OnTapDown;
    }

    public void SetPoint(Transform _pointBegin, Vector3 _newPosition)
    {
        Movement.SetPoint(_pointBegin, _newPosition);
    }

    private void FixedUpdate()
    {
    }

    IEnumerator NextBlock(BlockCollision _nextBlock)
    {
        yield return new WaitForSeconds(_removeAfterCollision);
        EventDestroyBlock?.Invoke(_nextBlock, this);
        _nextBlock.CollisionDetect = true;
    }

    IEnumerator ObjectExist()
    {
        yield return new WaitForSeconds(_removeAfterCollision + 0.5f);
        Debug.Log("Раунд завершен");
        EventExitRaund?.Invoke(this);
    }
}
