using UnityEngine;
using System.Collections;

public class GenericSingleton<E> : MonoBehaviour
    where E : Component
{
    [SerializeField]
    bool dontDestroy;
    private static E instance;
    public static E Instance {
        get {
            if (instance == null && !Quitting) {
                var objects = FindObjectsOfType (typeof(E)) as E[];
                if (objects.Length > 0)
                    instance = objects[0];
                
                if (instance == null) {
                    GameObject newObject = new GameObject ();
                    newObject.hideFlags = HideFlags.HideAndDontSave;
                    instance = newObject.AddComponent<E>();
                }
            }
            return instance;
        }
    }
    // True while the application is tearing down, so OnDestroy handlers can avoid
    // resurrecting a singleton (the Instance getter would otherwise build a phantom
    // HideAndDontSave object during shutdown, which then shadows the real one).
    public static bool Quitting { get; private set; }

    public virtual void Awake()
    {
        if (!instance)
            instance = this as E;

        // DontDestroyOnLoad must not sit inside the !instance branch: if anything
        // touched .Instance before this Awake ran, the getter already assigned the
        // field and this object silently lost its persistence.
        if (dontDestroy)
            DontDestroyOnLoad(gameObject);
    }
    public virtual  void OnDestroy()
    {
        // Only the live singleton may clear the static. Clearing unconditionally let a
        // duplicate's teardown null out the real instance.
        if ((object)instance == this)
            instance = null;
    }
    protected virtual void OnApplicationQuit()
    {
        Quitting = true;
    }
}




