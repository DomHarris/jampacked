using System;
using System.Collections.Generic;
using UnityEngine;

public static class MonotoneCubicSplineInterpolation
{
    public static float Calc(List<Vector2> points, float x)
    {
        var length = points.Count;

        if (length == 0)
            throw new ArgumentException("Length cannot be 0!");

        if (length == 1)
            return x;

        var delta = new float[length - 1];
        var m = new float[length];

        int i;
        for (i = 0; i < length - 1; i++)
        {
            delta[i] = (points[i + 1].y - points[i].y) / (points[i + 1].x - points[i].x);
            if (i > 0)
                m[i] = (delta[i - 1] + delta[i]) / 2;
        }

        var toFix = new List<int>();
        for (i = 1; i < length - 1; i++)
            if ((delta[i] > 0 && delta[i - 1] < 0) || (delta[i] < 0 && delta[i - 1] > 0))
                toFix.Add(i);

        foreach (var val in toFix)
            m[val] = 0;

        m[0] = delta[0];
        m[length - 1] = delta[length - 2];

        toFix.Clear();
        for (i = 0; i < length - 1; i++)
            if (Mathf.Approximately(delta[i], 0))
                toFix.Add(i);

        foreach (var val in toFix)
        {
            m[val] = 0;
            m[val + 1] = 0;
        }

        var alpha = new float[length - 1];
        var beta = new float[length - 1];
        var dist = new float[length - 1];
        var tau = new float[length - 1];
        for (i = 0; i < length - 1; i++)
        {
            alpha[i] = m[i] / delta[i];
            beta[i] = m[i + 1] / delta[i];
            dist[i] = alpha[i] * alpha[i] + beta[i] * beta[i];
            tau[i] = 3 / Mathf.Sqrt(dist[i]);
        }

        toFix.Clear();
        for (i = 0; i < length - 1; i++)
        {
            if (dist[i] > 9)
            {
                toFix.Add(i);
            }
        }

        foreach (var val in toFix)
        {
            m[val] = tau[val] * alpha[val] * delta[val];
            m[val + 1] = tau[val] * beta[val] * delta[val];
        }

        for (i = points.Count - 2; i > 0; --i)
        {
            if (points[i].x <= x)
            {
                break;
            }
        }

        var h = points[i + 1].x - points[i].x;
        var t = (x - points[i].x) / h;
        var t2 = t * t;
        var t3 = t * t * t;
        var h00 = 2 * t3 - 3 * t2 + 1;
        var h10 = t3 - 2 * t2 + t;
        var h01 = -2 * t3 + 3 * t2;
        var h11 = t3 - t2;
        return h00 * points[i].y + h10 * h * m[i] + h01 * points[i + 1].y + h11 * h * m[i + 1];
    }
}