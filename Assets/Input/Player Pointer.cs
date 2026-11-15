using UnityEngine;
using UnityEngine.InputSystem;

public static class PlayerPointer
{
    public static Vector2 Position
    {
        get
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (Touchscreen.current != null)
                return Touchscreen.current.primaryTouch.position.ReadValue();
#endif
            return Pointer.current != null ? Pointer.current.position.ReadValue() : Vector2.zero;
        }
    }
}
