using System;
using System.ComponentModel;
using System.Reflection;


/// <summary>
/// 反射帮助类
/// </summary>
public class ReflectionHelper
{
    /// <summary>
    /// 创建类型的实例
    /// </summary>
    /// <param name="classType">类型</param>
    /// <returns></returns>
    public static object CreateInstance(Type classType)
    {
        return System.Activator.CreateInstance(classType);
    }

    /// <summary>
    /// 通过属性名获取对象属性值
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="fieldName"></param>
    /// <returns></returns>
    public static object GetObjectValue(object obj,string fieldName)
    {
        FieldInfo info = obj.GetType().GetField(fieldName);
        if (info != null)
        {
            return info.GetValue(obj);
        }
        return null;
    }

    /// <summary>
    /// 通过属性名给某个对象的属性设置值
    /// </summary>
    /// <param name="obj">对象</param>
    /// <param name="fieldName">属性名</param>
    /// <param name="value">值</param>
    public static void SetObjectValue(object obj,string fieldName, object value)
    {
        FieldInfo info = obj.GetType().GetField(fieldName);
        if(info != null)
        {
            info.SetValue(obj, value);
        }
    }

    /// <summary>
    /// 通过属性名给某个对象的属性设置值(字符串值自动转换为对应数据类型值,Null和空格会设置为对应数值类型的初始值)
    /// </summary>
    /// <param name="obj">对象</param>
    /// <param name="fieldName">属性名</param>
    /// <param name="value">值</param>
    public static void SetObjectByStringValueCheckEmpty(object obj, string fieldName, string value)
    {
        FieldInfo info = obj.GetType().GetField(fieldName);
        if (info != null)
        {
            if ((value == null || value.Trim() == "") && ( info.FieldType == typeof(int) || info.FieldType == typeof(short) || info.FieldType == typeof(float) || info.FieldType == typeof(double)))
            {
                info.SetValue(obj, 0);
            }
            else
            {
                info.SetValue(obj, Convert.ChangeType(value.Trim(), info.FieldType));
            }
        }
    }

    /// <summary>
    /// 通过属性名给某个对象的属性设置值(字符串值自动转换为对应数据类型值)
    /// </summary>
    /// <param name="obj">对象</param>
    /// <param name="fieldName">属性名</param>
    /// <param name="value">值</param>
    public static void SetObjectByStringValue(object obj, string fieldName, string value)
    {
        FieldInfo info = obj.GetType().GetField(fieldName);
        if (info != null)
        {
            info.SetValue(obj, StringFormat(value, info.FieldType));
        }
    }

    /// <summary>
    /// 将字符串格式化成指定的数据类型
    /// </summary>
    /// <param name="str"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Object StringFormat(String str, Type type)
    {
        if (String.IsNullOrEmpty(str))
            return null;
        if (type == null)
            return str;
        if (type.IsArray)
        {
            Type elementType = type.GetElementType();
            String[] strs = str.Split(new char[] { '^' });
            Array array = Array.CreateInstance(elementType, strs.Length);
            for (int i = 0, c = strs.Length; i < c; ++i)
            {
                array.SetValue(ConvertSimpleType(strs[i], elementType), i);
            }
            return array;
        }
        return ConvertSimpleType(str,type);
    }

    /// <summary>
    /// 简单数据类型转换
    /// </summary>
    /// <param name="value"></param>
    /// <param name="destinationType"></param>
    /// <returns></returns>
    public static object ConvertSimpleType(object value, Type destinationType)
    {
        object returnValue;
        if ((value == null) || destinationType.IsInstanceOfType(value))
        {
            return value;
        }
        string str = value as string;
        if ((str != null) && (str.Length == 0))
        {
            return null;
        }
        TypeConverter converter = TypeDescriptor.GetConverter(destinationType);
        bool flag = converter.CanConvertFrom(value.GetType());
        if (!flag)
        {
            converter = TypeDescriptor.GetConverter(value.GetType());
        }
        if (!flag && !converter.CanConvertTo(destinationType))
        {
            throw new InvalidOperationException("无法转换成类型：" + value.ToString() + "==>" + destinationType);
        }
        try
        {
            returnValue = flag ? converter.ConvertFrom(null, null, value) : converter.ConvertTo(null, null, value, destinationType);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("类型转换出错：" + value.ToString() + "==>" + destinationType, e);
        }
        return returnValue;
    }
}
