//----------------------------------------------
// ProtoBuf编码解析辅助类
// @author: ChenXing
// @email:  onechenxing@163.com
// @date:   2014/12/23
//----------------------------------------------
using UnityEngine;
using System.Collections;

/// <summary>
/// ProtoBuf编码解析辅助类
/// </summary>
public class ProtoBufSerializerHelper
{
    /// <summary>
    /// 序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <returns></returns>
    public static byte[] ProtoRuntimeSerialize<T>(T message) where T:ProtoBuf.IExtensible
    {
        byte[] bs;
        using(System.IO.MemoryStream stream = new System.IO.MemoryStream())
        {
            ProtoBuf.Serializer.Serialize<T>(stream, message);
            bs = stream.ToArray();
        }
        return bs;
    }

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <returns></returns>
    public static T ProtoRuntimeDeserialize<T>(byte[] bs) where T:ProtoBuf.IExtensible
    {
        T t = default(T);
        using(System.IO.MemoryStream stream = new System.IO.MemoryStream(bs))
        {
            t = ProtoBuf.Serializer.Deserialize<T>(stream);
        }
        return t;
    }

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="bs"></param>
    /// <returns></returns>
    public static object ProtoRuntimeDeserialize(System.Type type, byte[] bs)
    {
        object t;
        using (System.IO.MemoryStream stream = new System.IO.MemoryStream(bs))
        {
            ProtoBuf.Meta.RuntimeTypeModel model = ProtoBuf.Meta.RuntimeTypeModel.Create();
            t = model.Deserialize(stream, null, type);
        }
        return t;
    }
}
