using Clipper2Lib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LianeAttach : LianeAttachable
{
    [SerializeField] protected Vector2 halfExtends;

    public Vector2[] Points { get => _points; }
    protected Vector2[] _points;

    public PathD ClipperPath { get => _clipperPath; }
    protected PathD _clipperPath;

    private void OnDrawGizmosSelected()
    {
        DebugUtils.DrawBox(transform.position, halfExtends * transform.lossyScale, transform.rotation, Color.green, 0);
    }
}
