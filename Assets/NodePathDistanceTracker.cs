using System.Collections.Generic;
using UnityEngine;

public class NodePathDistanceTracker : MonoBehaviour
{
    [SerializeField] public Transform PointContainer;

    /// <summary>
    /// Get the distance of the closest point on the center line of the section to point parameter
    /// </summary>
    public float GetT(Vector3 point)
    {
        List<Vector3> nodes = _GetNodePositions();
        if (nodes.Count < 2) return 0;

        // Find closest adjacent nodes from point container
        // Get closest node
        int closestNodeIndex = -1;
        float closestNodeSquaredDistance = Mathf.Infinity;
        for (int i = 0; i < PointContainer.childCount; i++)
        {
            float squaredDistance = (nodes[i] - point).sqrMagnitude;
            if (squaredDistance < closestNodeSquaredDistance)
            {
                closestNodeIndex = i;
                closestNodeSquaredDistance = squaredDistance;
            }
        }
        // Get second closest node
        int secondClosestNodeIndex = -1;
        if (closestNodeIndex == 0)
        {
            // Closest is first point. Second closest must be second point
            secondClosestNodeIndex = 1;
        }
        else if (closestNodeIndex == PointContainer.childCount - 1)
        {
            // Closest is last point. Second closest must be second to last point
            secondClosestNodeIndex = closestNodeIndex - 1;
        }
        else
        {
            // Closest is one of the middle points
            Vector3 previousNode = nodes[closestNodeIndex - 1];
            Vector3 currentNode = nodes[closestNodeIndex];
            Vector3 nextNode = nodes[closestNodeIndex + 1];

            Vector3 v1 = (currentNode - previousNode).normalized;
            Vector3 v2 = (nextNode - currentNode).normalized;

            Vector3 cornerTangent = (v1 + v2).normalized;
            bool inPreviousSection = Vector3.Dot(point - currentNode, cornerTangent) < 0;
            secondClosestNodeIndex = inPreviousSection ? closestNodeIndex - 1 : closestNodeIndex + 1;
        }
        // Determine order of closest nodes
        int node1Index = Mathf.Min(closestNodeIndex, secondClosestNodeIndex);
        int node2Index = node1Index + 1;
        Vector3 node1 = nodes[node1Index];
        Vector3 node2 = nodes[node2Index];
        Vector3 sectionDirection = (node2 - node1).normalized;

        // Get the normals defining the section boundary planes
        Vector3 boundaryPlane1Normal = Vector3.zero;    // Zero = not set. The normals should never be zero
        Vector3 boundaryPlane2Normal = Vector3.zero;
        if (node1Index == 0)
        {
            boundaryPlane1Normal = sectionDirection;
        }
        if (node2Index == nodes.Count - 1)
        {
            boundaryPlane2Normal = sectionDirection;
        }
        if (boundaryPlane1Normal.sqrMagnitude == 0)
        {
            Vector3 previousSectionDirection = (nodes[node1Index] - nodes[node1Index - 1]).normalized;
            boundaryPlane1Normal = (previousSectionDirection + sectionDirection).normalized;
        }
        if (boundaryPlane2Normal.sqrMagnitude == 0)
        {
            Vector3 nextSectionDirection = (nodes[node2Index + 1] - nodes[node2Index]).normalized;
            boundaryPlane2Normal = (nextSectionDirection + sectionDirection).normalized;
        }

        // Get the intersections of point (in direction of section) and boundary planes
        Vector3 boundary1Intersection = _GetLinePlaneIntersection(point, sectionDirection, node1, boundaryPlane1Normal);
        Vector3 boundary2Intersection = _GetLinePlaneIntersection(point, sectionDirection, node2, boundaryPlane2Normal);

        // Get the distance of the point between the two intersections
        float intersection1Distance = (point - boundary1Intersection).magnitude;
        float intersection2Distance = (point - boundary2Intersection).magnitude;
        float sectionT = intersection1Distance / (intersection1Distance + intersection2Distance);
        // Clamp sectionT to 0 or 1 if point is out of bounds
        float maxDistance = (boundary1Intersection - boundary2Intersection).magnitude;
        if (intersection1Distance > maxDistance) sectionT = 1;
        if (intersection2Distance > maxDistance) sectionT = 0;

        return Mathf.Clamp01((node1Index + sectionT) / (nodes.Count - 1));
    }

    /// <summary>
    /// Adapted from https://stackoverflow.com/questions/5666222/3d-line-plane-intersection
    /// </summary>
    private Vector3 _GetLinePlaneIntersection(Vector3 linePoint, Vector3 lineDir, Vector3 planePoint, Vector3 planeNormal)
    {
        float t = (Vector3.Dot(planeNormal, planePoint) - Vector3.Dot(planeNormal, linePoint)) / Vector3.Dot(planeNormal, lineDir.normalized);
        return linePoint + lineDir.normalized * t;
    }

    private List<Vector3> _GetNodePositions()
    {
        List<Vector3> nodePositions = new List<Vector3>();
        foreach (Transform node in PointContainer)
        {
            Vector3 p = node.position;
            // p.y = 0;
            nodePositions.Add(p);
        }
        return nodePositions;
    }
}