using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using static TEST_PlayerMovement;

public class TEST_PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 2.0f;

    public bool IsGrounded { get; private set; }

    private Vector2 _inputMovement;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void OnMovement(InputAction.CallbackContext obj)
    {
        _inputMovement = obj.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext obj)
    {
        if(obj.started) _rb.AddForce(Vector2.up * 7, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        _rb.velocity = Vector2.up * _rb.velocity.y + Vector2.right * _inputMovement.x * speed;
    }
}
