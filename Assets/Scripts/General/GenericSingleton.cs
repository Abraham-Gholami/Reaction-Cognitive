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
            if (instance == null) {
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
    public virtual void Awake()
    {
        if (!instance)
        {
            instance = this as E;
            if(dontDestroy)
            DontDestroyOnLoad(gameObject);
        }
        
    }
    public virtual  void OnDestroy() 
    {
        instance = null;
    }
}




