using UnityEngine;
using System;

public class DisableWatcher : MonoBehaviour
{
    void OnDisable()
    {
        Debug.LogError(
            $"[DisableWatcher] '{name}' was DISABLED\n" +
            Environment.StackTrace,
            this
        );
    }

    void OnEnable()
    {
        Debug.Log($"[DisableWatcher] '{name}' was ENABLED", this);
    }

    void OnDestroy()
    {
        Debug.LogError(
            $"[DisableWatcher] '{name}' was DESTROYED\n" +
            Environment.StackTrace,
            this
        );
    }
}
