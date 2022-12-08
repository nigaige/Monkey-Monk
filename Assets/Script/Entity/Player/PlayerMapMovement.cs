using MonkeyMonk.Map;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class PlayerMapMovement : MonoBehaviour
{
    [SerializeField] public UnityAction t;

    [SerializeField] private Node startingNode;
    [SerializeField] private float speed = 15;

    private bool _isLocked = false;

    private Node _currentNode;
    private bool _isMoving = false;

    private Vector2 _inputMovement;

    public event Action<Node> OnNodeChangeEvent;

    private void Awake()
    {
        _currentNode = startingNode;
        OnNodeChangeEvent.Invoke(_currentNode);
        transform.position = _currentNode.transform.position + Vector3.up * transform.lossyScale.y / 2f;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (_isLocked) return;
        if (_isMoving) return;

        if(_inputMovement != Vector2.zero)
        {
            NodePath path = _currentNode.GetPath(_inputMovement);

            if (path == null) return;

            _isMoving = true;
            StartCoroutine(MoveToNextNode(path, path.IsTarget(_currentNode)));
            _currentNode = path.GetTargetNode(_currentNode);

            OnNodeChangeEvent.Invoke(_currentNode);
        }
    }

    private IEnumerator MoveToNextNode(NodePath path, bool side)
    {
        float lerpVal = 0.0f;
        float lerpSpeed = speed / path.SplineContainer.Spline.GetLength();
        
        while (lerpVal < 1.0f)
        {
            transform.position = path.transform.position + Vector3.up * transform.lossyScale.y / 2f + (Vector3)path.SplineContainer.Spline.EvaluatePosition((side) ? 1 - lerpVal : lerpVal);

            lerpVal += lerpSpeed * Time.deltaTime;
            yield return null;
        }

        transform.position = path.transform.position + Vector3.up * transform.lossyScale.y / 2f + (Vector3)path.SplineContainer.Spline.EvaluatePosition((side) ? 0.0f : 1.0f);

        _isMoving = false;
    }

    public void Lock()
    {
        _isLocked = true;
    }

    public void OnMovement(InputAction.CallbackContext obj)
    {
        _inputMovement = obj.ReadValue<Vector2>();
    }

    public void OnClick(InputAction.CallbackContext obj)
    {
        if (_isLocked) return;

        if (obj.started && _currentNode != null && !_isMoving) _currentNode.OnClick(this);
    }
}
