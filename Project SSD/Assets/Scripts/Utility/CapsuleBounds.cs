using UnityEngine;

[System.Serializable]
public struct CapsuleBounds {
    Vector3 point1;
    Vector3 point2;
    float radius;
    public Vector3 center {
        get {
            return Vector3.Lerp(point1, point2, .5f);
        }
    }
    public float height {
        get {
            return Vector3.Distance(point1, point2);
        }
    }

    public CapsuleBounds(Vector3 point1, Vector3 point2, float radius) {
        this.point1 = point1;
        this.point2 = point2;
        this.radius = radius;
    }
    public CapsuleBounds(Vector3 center, float height, float radius) {
        this.point1 = center + height*new Vector3(0, .5f, 0);
        this.point2 = center - height*new Vector3(0, .5f, 0);
        this.radius = radius;
    }
    public (Vector3, Vector3, float) RaycastComponent {
        get {
            return (point1, point2, radius);
        }
    }
}