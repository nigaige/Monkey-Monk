using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGrab : MonoBehaviour
{
    [SerializeField] private float force = 5f;
    [SerializeField] private Transform grabParent;
    [SerializeField] private BoxCollider2D catchBox;

    private Rigidbody2D _rb2d;

    private Collider2D[] _overlappingColliders = new Collider2D[6];
    private GameObject _grabbedObj;
    private ContactFilter2D _grabFilter = new ContactFilter2D();

    private void OnEnable()
    {
        PlayerInput.GetPlayerByIndex(0).actions["catch"].performed += Catch_performed;
    }

    private void Awake()
    {
        _rb2d = catchBox.attachedRigidbody;
    }

    private void Update()
    {
        if (_rb2d.velocity.x > 0) catchBox.transform.localPosition = new Vector3(1, 0, 0);
        if (_rb2d.velocity.x < 0) catchBox.transform.localPosition = new Vector3(-1, 0, 0);
    }

    private void Catch_performed(InputAction.CallbackContext obj)
    {
        if (_grabbedObj == null)
        {
            int collidersCount = catchBox.OverlapCollider(_grabFilter, _overlappingColliders);

            for (int i = 0; i < collidersCount; i++)
            {
                Collider2D collider2D = _overlappingColliders[i];

                if (collider2D.TryGetComponent(out IGrabbable grab))
                {
                    grab.OnGrabbed();
                    collider2D.transform.SetParent(grabParent, false);
                    collider2D.transform.localPosition = Vector3.zero;

                    _grabbedObj = collider2D.gameObject;

                    break;
                }
            }
        }
        else
        {
            Vector2 shotDir = ((Vector3)PlayerInput.GetPlayerByIndex(0).actions["Pointer"].ReadValue<Vector2>() - Camera.main.WorldToScreenPoint(transform.position)).normalized;
            
            _grabbedObj.transform.SetParent(null);
            _grabbedObj.GetComponent<IGrabbable>().OnUnGrabbed();
            _grabbedObj.GetComponent<Rigidbody2D>().velocity = shotDir * force;

            _grabbedObj = null;
        }
        
    }
}
