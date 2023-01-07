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

    private List<InputActivable> _inputActivables = new();

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        /*
        if (_rb.velocity.x > 0) catchBoxPosition.localPosition = new Vector3(1, 0, 0);
        if (_rb.velocity.x < 0) catchBoxPosition.localPosition = new Vector3(-1, 0, 0);
        */
    }

    public void OnGrab(InputAction.CallbackContext obj)
    {
        if (!obj.started) return;

        //grabbing
        if (_grabbedObj == null)
        {
            // input activables
            if (_inputActivables.Count > 0) // TODO : write this better
            {
                float minDist = float.MaxValue;
                InputActivable closest = null;

                for (int i = _inputActivables.Count - 1; i >= 0; i--)
                {
                    InputActivable curr = _inputActivables[i];

                    if (curr == null)
                    {
                        _inputActivables.RemoveAt(i);
                        continue;
                    }

                    float currDist = Vector2.Distance(transform.position, curr.transform.position);

                    if (currDist < minDist)
                    {
                        closest = curr;
                        minDist = currDist;
                    }
                }

                if (closest != null)
                {
                    closest.Activate();
                    return;
                }

            }

            // check grab
            _overlappingColliders = Physics.OverlapBox(catchBoxPosition.position, catchBoxBoundsExtents);

            for (int i = 0; i < _overlappingColliders.Length; i++)
            {
                Collider collider = _overlappingColliders[i];

                // Grab first grabbable
                if (collider.TryGetComponent(out Grabbable grab) && grab.IsGrabbable())
                {
                    GrabObject(grab);
                    break;
                }
            }
        }
        //launching
        else
        {
            Vector2 shotDir = ((Vector3)_pointerPos - UnityEngine.Camera.main.WorldToScreenPoint(transform.position)).normalized;
            _grabbedObj.transform.SetParent(null);
            _grabbedObj.GetComponent<Grabbable>().OnUnGrabbed(this);

            if (_grabbedObj.TryGetComponent(out Projectile proj))
                proj.Initialize(shotDir, gameObject);
            else
            {
                _grabbedObj.GetComponent<Rigidbody>().velocity = shotDir * force;
                Debug.Log(_grabbedObj.GetComponent<Rigidbody>().velocity);
            } 
                

            _grabbedObj = null;
        }
        
    }

    public void GrabObject(Grabbable grab)
    {
        if (!grab.IsGrabbable()) return;

        Collider collider = grab.gameObject.GetComponent<Collider>();

        grab.OnGrabbed(this);

        if (grab.IsHeavy()) collider.transform.SetParent(heavyGrabParent, false);
        else collider.transform.SetParent(lightGrabParent, false);

        collider.transform.localPosition = Vector3.zero;

        _grabbedObj = collider.gameObject;
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


    // Input activables

    private void OnTriggerEnter(Collider other) // TODO : opti this
    {
        if (other.TryGetComponent(out InputActivable activable)) _inputActivables.Add(activable);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out InputActivable activable)) _inputActivables.Remove(activable);
    }
}
