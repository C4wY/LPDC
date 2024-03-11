using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using UnityEngine;

/// <summary>
/// A serializable dictionary that can be converted to and from JSON.
/// <para>Usage:</para>
/// <code>
///     SerializableDictionary
///        .Create(avatars.Select(a => (a.gameObject.GetInstanceID(), a.transform.position)))
///        .ToJsonFile(AvatarPositionPath, prettyPrint: true);
/// </code>
/// </summary>
public static class SerializableDictionary
{
    public static SerializableDictionary<K, V> Create<K, V>(IDictionary<K, V> dictionary)
    {
        return new SerializableDictionary<K, V>(dictionary);
    }

    public static SerializableDictionary<K, V> Create<K, V>(IEnumerable<(K, V)> entries)
    {
        return new SerializableDictionary<K, V>(entries.ToDictionary(x => x.Item1, x => x.Item2));
    }
}

[System.Serializable]
public class SerializableDictionary<K, V> : Dictionary<K, V>, ISerializationCallbackReceiver
{
    [SerializeField]
    K[] keys;

    [SerializeField]
    V[] values;

    public SerializableDictionary() : base() { }

    public SerializableDictionary(IDictionary<K, V> dictionary) : base(dictionary) { }

    // Required by ISerializationCallbackReceiver
    public void OnBeforeSerialize()
    {
        keys = new K[Count];
        values = new V[Count];

        int i = 0;
        foreach (KeyValuePair<K, V> pair in this)
        {
            keys[i] = pair.Key;
            values[i] = pair.Value;
            i++;
        }
    }

    // Required by ISerializationCallbackReceiver
    public void OnAfterDeserialize()
    {
        Clear();

        if (keys.Length != values.Length)
        {
            throw new SerializationException("Key and value count mismatch during deserialization.");
        }

        for (int i = 0; i < keys.Length; i++)
        {
            Add(keys[i], values[i]);
        }

        keys = null;
        values = null;
    }

    public string ToJsonString(bool prettyPrint = false)
    {
        return JsonUtility.ToJson(this, prettyPrint);
    }

    public void ToJsonFile(string path, bool prettyPrint = false)
    {
        System.IO.File.WriteAllText(path, ToJsonString(prettyPrint));
    }

    public static SerializableDictionary<K, V> FromJsonString(string json)
    {
        return JsonUtility.FromJson<SerializableDictionary<K, V>>(json);
    }

    public static SerializableDictionary<K, V> FromJsonFile(string path)
    {
        if (System.IO.File.Exists(path) == false)
            return new(); // Return an empty dictionary.

        return FromJsonString(System.IO.File.ReadAllText(path));
    }
}
