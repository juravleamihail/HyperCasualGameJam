using UnityEngine;

public abstract class SimpleSingletoneGeneric<T> : MonoBehaviour where T : SimpleSingletoneGeneric<T>
{
    public static T Instance;

    protected virtual void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this as T;
    }
}
