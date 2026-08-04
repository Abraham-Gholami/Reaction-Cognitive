using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperClasses 
{
}
[System.Serializable]
public enum Answer
{
    Right,Wrong
}
[System.Serializable]
public class CustomDictionary <K,V>
{
    [SerializeField]
    List<K> keys;
    [SerializeField]
    List<V> values;
    public void Add(K key,V value)
    {
        keys.Add(key);
        values.Add(value);
    }
}

