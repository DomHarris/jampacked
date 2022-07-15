# Aim Assist
A funky aim assist implementation using the theory from t3ssel8r's YouTube video: https://youtu.be/yGci-Lb87zs

NOTE: This isn't really _complete_ yet, but it should be functional enough for a jam!

## Usage
1. Add circle colliders to each of the objects you want to affect the aim assist
2. Add the AimAssistInput script to the object you want to be affected by aim assist
3. Call either TransformAngle or TransformUpDirection with an original angle, for example:
```csharp
void Update ()
{
    Vector3 aimDirection = cam.ScreenToWorldPoint(input.mousePosition) - transform.position;
    transform.up = aimAssist.TransformUpDirection(aimDirection.normalized);
}
```

Tweak the values as you please. The graph in the editor should update to reflect how the original input (x-axis) is affeced by each object. Make sure there's an object within the radius while you're editing this, otherwise you won't see anything!