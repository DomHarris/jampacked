# Damage
A quick and robust setup for making things that get hit.

## Usage
Here's an example for damaging something in 2D OnCollisionEnter:

```csharp
private void OnCollisionEnter2D(Collision2D other)
{
    // get all the scripts attached to the thing we hit that want to know about the hit
    var hitReceivers = other.gameObject.GetComponentsInChildren<IHitReceiver>();

    var damagePacket = new HitData
    {
        // the collision info, used so we can get any relevant data in the receiving object 
        CollisionInfo = other, 
        // how much damage should we do?
        Damage = damage,
        // what sort of damage is being dealt?
        DamageType = DamageType.Projectile,
        // which direction is the damage coming from?
        IncomingDirection = transform.right,
        // what object caused the damage?
        IncomingObject = gameObject
    };
    
    // tell all the objects that were hit that they were hit, and send the data
    foreach (var hitReceiver in receivers)
        hitReceiver.ReceiveHit(damagePacket);
}
```

This will send a message to everything that implements IHitReceiver. There's an example implementation of this in Health.cs, but you could use it for everything from buttons that activate on hit, to shields that can only be disabled by certain types of damage, etc etc etc.

You should edit the HitData struct as needed, it's just a container for any data that you want to send along with the damage.

Here's a simple example implementation of IHitReceiver that moves a platform with DOTween when an object is hit:

```csharp
using DG.Tweening;
using UnityEngine;

public class ProjectileMovePlatform : MonoBehaviour, IHitReceiver
{
    [SerializeField] private Transform platform;
    [SerializeField] private Vector3 startPos;
    [SerializeField] private Vector3 endPos;
    [SerializeField] private float moveDuration;
    [SerializeField] private Ease ease = Ease.InOutQuint;

    private bool _moving = false;
    
    public void ReceiveHit(HitData data)
    {
        if (data.DamageType == DamageType.Projectile)
            Toggle();
    }
    
    public void MoveToStartPos()
    {
        if (_moving) return;
        _moving = true;
        platform.DOMove(startPos, moveDuration).SetEase(ease).OnComplete(() => _moving = false);
    }

    public void MoveToEndPos()
    {
        if (_moving) return;
        _moving = true;
        platform.DOMove(endPos, moveDuration).SetEase(ease).OnComplete(() => _moving = false);
    }

    public void Toggle()
    {
        if (platform.position == endPos)
            MoveToStartPos();
        else
            MoveToEndPos();
    }
}
```