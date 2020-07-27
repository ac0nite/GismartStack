using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMovement : MonoBehaviour
{
    [SerializeField] private Transform _moveDirection = null;
    [SerializeField] private bool _comeMove = false;
    private float _speed = 0f;
    private float _maxDistanceMovementBlock = 0f;
    private bool _axis = true; //true - x, false - z
    private Transform _transform = null;
    private Vector3 _target = Vector3.zero;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _speed = GameController.Instance.Speed;
    }

    private void Start()
    {
        if (_comeMove)
        {
            if (Math.Abs(Vector3.Dot(Forward(), _transform.forward)) > 0.9f)
            {
                _maxDistanceMovementBlock = Math.Abs(_transform.position.z);
                _axis = true;
            }
            else
            {
                _maxDistanceMovementBlock = Math.Abs(_transform.position.x);
                _axis = false;
            }
        }
    }

    private void Update()
    {
        if (_comeMove)
        {
            float distance = 0f;
            if (_axis)
                distance = _moveDirection.transform.position.z;
            else
                distance = _moveDirection.transform.position.x;
            
            if (Math.Abs(distance) > _maxDistanceMovementBlock)
            {
                _moveDirection.Rotate(Vector3.up, 180f);
            }
            // if (_target.magnitude - _transform.position.magnitude <= 0.5f)
            // {
            //     _moveDirection.Rotate(Vector3.up, 180f);
            //     _target = Forward() * _maxDistanceMovementBlock;
            // }

            //_transform.Translate(Forward() * (_speed * Time.deltaTime));
            _transform.position = Vector3.Lerp(_transform.position, Forward() + _transform.position,  _speed / 2f * Time.deltaTime);
        }
    }
    
    public void SetPoint(Transform _pointBegin, Vector3 _startPosition)
    {
        _moveDirection.transform.rotation = _pointBegin.rotation;

        if (Math.Abs(Vector3.Dot(Forward(), Vector3.forward)) < 0.001f)
            transform.position = new Vector3(_pointBegin.transform.position.x, _pointBegin.transform.position.y, _startPosition.z);
        else
            transform.position = new Vector3(_startPosition.x, _pointBegin.transform.position.y, _pointBegin.transform.position.z);
    }

    public void Move()
    {
        _comeMove = true;
    }

    public void Stop()
    {
        _comeMove = false;
    }

    public Vector3 Forward()
    {
        return _moveDirection.forward;
    }
}
