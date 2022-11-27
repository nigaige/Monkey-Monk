using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGrab : MonoBehaviour
{
    [SerializeField] private float force = 5f;
    [SerializeField] private Transform heavyGrabParent;
    [SerializeField] private Transform lightGrabParent;
    [SerializeField] private Transform catchBoxPosition;
    [SerializeField] private Vector3 catchBoxBoundsExtents;

    private Rigidbody _rb;

    private Collider[] _overlappingColliders = new Collider[6];
    private GameObject _grabbedObj;

    private Vector2 _pointerPos;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (_rb.velocity.x > 0) catchBoxPosition.localPosition = new Vector3(1, 0, 0);
        if (_rb.velocity.x < 0) catchBoxPosition.localPosition = new Vector3(-1, 0, 0);
    }

    public void OnGrab(InputAction.CallbackContext obj)
    {
        if (!obj.started) return;

        if (_grabbedObj == null)
        {
            _overlappingColliders = Physics.OverlapBox(catchBoxPosition.position, catchBoxBoundsExtents);

            for (int i = 0; i < _overlappingColliders.Length; i++)
            {
                Collider collider = _overlappingColliders[i];

                // Grab first grabbable
                if (collider.TryGetComponent(out Grabbable grab) && grab.IsGrabbable())
                {
                    grab.OnGrabbed(this);

                    if (grab.IsHeavy()) collider.transform.SetParent(heavyGrabParent, false);
                    else collider.transform.SetParent(lightGrabParent, false);

                    collider.transform.localPosition = Vector3.zero;

                    _grabbedObj = collider.gameObject;

                    break;
                }
            }
        }
        else
        {
            Vector2 shotDir = ((Vector3)_pointerPos - Camera.main.WorldToScreenPoint(transform.position)).normalized;
            _grabbedObj.transform.SetParent(null);
            _grabbedObj.GetComponent<Grabbable>().OnUnGrabbed(this);

            if (_grabbedObj.TryGetComponent(out Projectile proj))
                proj.Initialize(shotDir, gameObject);
            else 
                _grabbedObj.GetComponent<Rigidbody>().velocity = shotDir * force;

            _grabbedObj = null;
        }
        
    }

    private void OnDrawGizmosSelected()
    {
        if (catchBoxPosition == null) return;

        Gizmos.DrawWireCube(catchBoxPosition.transform.position, catchBoxBoundsExtents);
    }

    public void OnPointer(InputAction.CallbackContext callback)
    {
        _pointerPos = callback.ReadValue<Vector2>();
    }
}
