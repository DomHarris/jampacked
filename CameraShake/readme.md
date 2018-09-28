# 2D Camera Shake

Add a bit of juice to your game with this easy camera shake function. Import it, add it to your camera, and use it like this:
```cs
// trauma value sets the "intensity" of the camera shake
// everything else is worked out from that
CameraShake.Trauma = 0.4f;

// move your camera with the camera shake's target position.
// the camera will always be at this position
CameraShake.TargetPos = transform.position;
```

NB: only works for 2D, and is currently a Singleton for ease of use. It doesn't have to be, though - you're welcome to change it for now!
