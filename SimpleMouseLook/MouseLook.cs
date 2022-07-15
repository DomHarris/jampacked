using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private Camera mainCam;
    
    private Vector3 _inputWorldPos;

    private void Update()
    {
        _inputWorldPos.z = transform.position.z;
        transform.up = _inputWorldPos - transform.position;
    }

    public void MouseMove(InputAction.CallbackContext ctx)
    {
        if (mainCam == null) return;
        Vector3 input = ctx.ReadValue<Vector2>();
        input.z = 1;
        _inputWorldPos = mainCam.ScreenToWorldPoint(input);
    }
}
