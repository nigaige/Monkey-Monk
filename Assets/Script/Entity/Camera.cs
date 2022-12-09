using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public GameObject player;
    public Vector3 MaxOffset;
    // Start is called before the first frame update
    void Start()
    {
        //MaxOffset = new Vector3(0.5f, 0.1f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 NewPosition = transform.position;
        float x_offset = player.transform.position.x - transform.position.x;
        // D�placement � droite
        if (x_offset > 0)
        {
            NewPosition.x += x_offset;
        }
        // D�placement � gauche
        else if (x_offset < -(MaxOffset.x))
        {
            NewPosition.x += (x_offset + MaxOffset.x);
        }

        float y_offset = player.transform.position.y - transform.position.y;
        // D�placement en haut
        if (y_offset > MaxOffset.y)
        {
            NewPosition.y += (y_offset - MaxOffset.y);
        }
        // D�placement en bas
        else if ((y_offset < -(MaxOffset.y)) && NewPosition.y > -3f)
        {
            NewPosition.y += (y_offset + MaxOffset.y);
            if(NewPosition.y < -3f)
            {
                NewPosition.y = -3f;
            }
        }
        transform.position = NewPosition;
    }
}
