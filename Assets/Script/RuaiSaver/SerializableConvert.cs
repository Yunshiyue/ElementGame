using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerializableConvert
{
    public static bool CanSerializeType<T>()
    {
        if(typeof(T) == typeof(Vector3) || typeof(T) == typeof(Vector2))
        {
            return false;
        }
        return true;
    }
    public static SerializableVector3 GetSerializableVector3(Vector3 vector3)
    {
        return new SerializableVector3(vector3.x, vector3.y, vector3.z);
    }
    public static SerializableVector2 GetSerializableVector2(Vector2 vector2)
    {
        return new SerializableVector2(vector2.x, vector2.y);
    }
    public static Vector3 DeserializableVector3(SerializableVector3 serializableVector3)
    {
        return new Vector3(serializableVector3.x, serializableVector3.y, serializableVector3.z);
    }
    public static Vector2 DeserializableVector2(SerializableVector2 serializableVector2)
    {
        return new Vector2(serializableVector2.x, serializableVector2.y);
    }
}

public struct SerializableVector3
{
    public float x;
    public float y;
    public float z;
    public SerializableVector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}
public struct SerializableVector2
{
    public float x;
    public float y;
    public SerializableVector2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }
}
