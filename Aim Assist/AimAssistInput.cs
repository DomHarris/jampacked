using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AimAssistInput : MonoBehaviour
{
    public float Radius => radius;
    [SerializeField, Tooltip("At what distance should the aim assist feature stop being effective?")] private float radius = 10f;
    [SerializeField, Tooltip("How many targets should the aim assist feature take into account at once?")] private int maxTargets = 32;
    [SerializeField, Range(0, 1), Tooltip("How much should the aim assist snap to the center of the target?")] private float snapping = 0;
    [SerializeField, Tooltip("Use the target's circle collider to adjust the amount of aim assist for that object?")] private bool useCircleColliderSize = true;
    [SerializeField, Tooltip("How much should the aim assist affect each target object?")] private float targetSizeBase = 5f;

    [SerializeField, Range(0, 1)] private float aimAssistAmount = 1f; 
    
    private Vector3 _inputWorldPos;
    private Collider2D[] _targets;
    private int _numTargets = 0;

    private void Awake()
    {
        _targets = new Collider2D[maxTargets];
    }

    private void FixedUpdate()
    {
        GetTargets();
    }

    public void GetTargets()
    {
        if (_targets == null)
            _targets = new Collider2D[maxTargets];
        _numTargets = Physics2D.OverlapCircleNonAlloc(transform.position, radius, _targets);
    }

    public float TransformAngle(float inputDegrees)
    {
        return GetPointOnGraph(Mathf.InverseLerp(0, 360, inputDegrees), true, _targets, _numTargets);
    }

    public Vector3 TransformUpDirection(Vector3 up)
    {
        float angle = (360 - 90f + Mathf.Atan2(up.y, up.x) * Mathf.Rad2Deg) % 360;
        var transformed = TransformAngle(angle);
        return Quaternion.Euler(new Vector3(0, 0, transformed * 360)) * Vector3.up;
    }
    
    public float GetPointOnGraph(float x, bool useScale, Collider2D[] targets, int numTargets)
    {
        var points = new List<Vector2> {new (0,0)};
        for (var i = 0; i < numTargets; i++)
        {
            var t = targets[i];
            var colliderSize = 1f;

            if (useCircleColliderSize && t is CircleCollider2D circle)
                colliderSize = circle.radius;
            
            var direction = t.transform.position - transform.position;
            float angle = (360 - 90 + Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) % 360;

            float angleNormalised = Mathf.InverseLerp(0, 360, angle);
            float angleMinNormalised = Mathf.InverseLerp(0, 360, angle - targetSizeBase * colliderSize);
            float angleMaxNormalised = Mathf.InverseLerp(0, 360, angle + targetSizeBase * colliderSize);

            var slope = Mathf.Lerp(5, 0, snapping);
            
            points.Add(new Vector2(angleMinNormalised, angleNormalised - Mathf.InverseLerp(0, 360, slope)));
            points.Add(new Vector2(angleNormalised, angleNormalised));
            points.Add(new Vector2(angleMaxNormalised, angleNormalised + Mathf.InverseLerp(0, 360, slope)));
        }

        points.Add(new Vector2(1,1));
        points = points.OrderBy(p => p.y).ToList();
        return Mathf.Lerp(x, MonotoneCubicSplineInterpolation.Calc(points, x),  useScale ? aimAssistAmount : 1);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}