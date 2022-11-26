using MonkeyMonk.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class PlayerMapMovement : MonoBehaviour
{
    [SerializeField] private Node startingNode;
    [SerializeField] private float speed = 0.2f;

    private Node _currentNode;
    private bool _isMoving = false;

    private Vector2 _inputMovement;

    private void Awake()
    {
        _currentNode = startingNode;
        transform.position = _currentNode.transform.position + Vector3.up * transform.lossyScale.y / 2f;
    }

    private void Update()
    {
        if(!_isMoving)
        {
            if(_inputMovement != Vector2.zero)
            {
                NodePath path = _currentNode.GetPath(_inputMovement);

                if (path == null) return;

                _isMoving = true;
                StartCoroutine(MoveToNextNode(path, path.IsTarget(_currentNode)));
                _currentNode = path.GetTargetNode(_currentNode);
            }
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

    public void OnMovement(InputAction.CallbackContext obj)
    {
        _inputMovement = obj.ReadValue<Vector2>();
    }

    public void OnClick(InputAction.CallbackContext obj)
    {
        if (obj.started && _currentNode != null) _currentNode.OnClick();
    }
}
