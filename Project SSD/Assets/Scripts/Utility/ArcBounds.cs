using System.Collections.Generic;
using UnityEngine;

public struct ArcBounds {
    public Vector3 center;
    public Vector3 direction;
    public float radius;
    public float angle;
    public float startOffsetRatio;
    
    Vector3 StartPoint { get { return center - (direction*radius*startOffsetRatio); } }
    
    public ArcBounds(Vector3 center, Vector3 direction, float radius, float angle, float startOffsetRatio=0) {
        this.center = center;
        this.direction = direction;
        this.radius = radius;
        this.angle = angle;
        this.startOffsetRatio = startOffsetRatio;
    }
    public void DrawShape() {
        Gizmos.DrawWireSphere(StartPoint, radius);
        Vector3 left = Quaternion.AngleAxis(-angle*.5f, Vector3.up) * direction.normalized;
        Vector3 right = Quaternion.AngleAxis(angle*.5f, Vector3.up) * direction.normalized;
        Gizmos.DrawLine(StartPoint, StartPoint + left*radius);
        Gizmos.DrawLine(StartPoint, StartPoint + right*radius);
    }
    public Collider[] OverlapShape(int layerMask=1<<8, bool checkBack=false) {
        List<Collider> results = new List<Collider>();
        foreach(Collider inner in Physics.OverlapSphere(StartPoint, radius, layerMask)) {
            Debug.Log(Vector3.Angle(direction, inner.transform.position-StartPoint));
            if(Vector3.Angle(StartPoint+direction, StartPoint+inner.transform.position) < angle*.5f) {
                if(!checkBack ||
                Vector3.Distance(center, inner.transform.position)>radius)
                    results.Add(inner);
            }
        }
        return results.ToArray();
    }
}