using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGrab : MonoBehaviour
{
    [SerializeField] private float force = 5f;
    [SerializeField] private Transform grabParent;
    [SerializeField] private Transform catchBoxPosition;
    [SerializeField] private Vector3 catchBoxBoundsExtents;

    private Rigidbody _rb;

    private Collider[] _overlappingColliders = new Collider[6];
    private GameObject _grabbedObj;

    private void OnEnable()
    {
        PlayerInput.GetPlayerByIndex(0).actions["catch"].performed += Catch_performed;
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (_rb.velocity.x > 0) catchBoxPosition.localPosition = new Vector3(1, 0, 0);
        if (_rb.velocity.x < 0) catchBoxPosition.localPosition = new Vector3(-1, 0, 0);
    }

    private void Catch_performed(InputAction.CallbackContext obj)
    {
        if (_grabbedObj == null)
        {
            _overlappingColliders = Physics.OverlapBox(catchBoxPosition.position, catchBoxBoundsExtents);

            for (int i = 0; i < _overlappingColliders.Length; i++)
            {
                Collider collider = _overlappingColliders[i];

                if (collider.TryGetComponent(out IGrabbable grab))
                {
                    grab.OnGrabbed(this);
                    collider.transform.SetParent(grabParent, false);
                    collider.transform.localPosition = Vector3.zero;

                    _grabbedObj = collider.gameObject;

                    break;
                }
            }
        }
        else
        {
            Vector2 shotDir = ((Vector3)PlayerInput.GetPlayerByIndex(0).actions["Pointer"].ReadValue<Vector2>() - Camera.main.WorldToScreenPoint(transform.position)).normalized;
            
            _grabbedObj.transform.SetParent(null);
            _grabbedObj.GetComponent<IGrabbable>().OnUnGrabbed(this);
            _grabbedObj.GetComponent<Rigidbody>().velocity = shotDir * force;

            _grabbedObj = null;
        }
        
    }

    private void OnDrawGizmosSelected()
    {
        if (catchBoxPosition == null) return;

        Gizmos.DrawWireCube(catchBoxPosition.transform.position, catchBoxBoundsExtents);
    }
}
