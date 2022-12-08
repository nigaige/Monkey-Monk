using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

namespace MonkeyMonk.Map
{
    public class NodePath : MonoBehaviour
    {
        [SerializeField] private SplineContainer splineContainer;
        public SplineContainer SplineContainer { get => splineContainer; }

        [SerializeField] private Node startNode;
        [SerializeField] private Node endNode;

        public Node GetTargetNode(Node node)
        {
            if (node == startNode) return endNode;
            if (node == endNode) return startNode;
            return null;
        }

        public bool IsTarget(Node node)
        {
            if (node == endNode) return true;
            return false;
        }

        public BezierKnot GetTargetKnot(Node node)
        {
            if (node != startNode && node != endNode) return new BezierKnot();

            int index;
            if (node == startNode) index = 0;
            else index = SplineContainer.Spline.Count - 1;

            return SplineContainer.Spline[index];
        }

        public void SetTargetKnot(Node node, BezierKnot knot)
        {
            if (node != startNode && node != endNode) return;

            int index;
            if (node == startNode) index = 0;
            else index = SplineContainer.Spline.Count - 1;

            SplineContainer.Spline[index] = knot;
        }
    }
}