using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TEST_PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 2.0f;

    public bool IsGrounded { get; private set; }

    private Vector2 _inputMovement;
    private Rigidbody _rb;

    private void OnEnable()
    {
        PlayerInput.GetPlayerByIndex(0).actions["Movement"].started += Movement_performed;
        PlayerInput.GetPlayerByIndex(0).actions["Movement"].performed += Movement_performed;
        PlayerInput.GetPlayerByIndex(0).actions["Movement"].canceled += Movement_performed;

        PlayerInput.GetPlayerByIndex(0).actions["Jump"].performed += Jump_performed;
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Movement_performed(InputAction.CallbackContext obj)
    {
        _inputMovement = obj.ReadValue<Vector2>();
    }

    private void Jump_performed(InputAction.CallbackContext obj)
    {
        _rb.AddForce(Vector2.up * 7, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        _rb.velocity = Vector2.up * _rb.velocity.y + Vector2.right * _inputMovement.x * speed;
    }

}
