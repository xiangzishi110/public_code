using System.Collections.Generic;

/// <summary>
/// 单个资源配置
/// </summary>
public class ResourceItem : IResource
{
    /// <summary>
    /// 名称
    /// </summary>
    private string _name;
    /// <summary>
    /// 资源相对路径不带后缀名唯一标签
    /// </summary>
    public string path;
    /// <summary>
    /// 资源类型
    /// </summary>
    public string type;
    /// <summary>
    /// 从那里加载
    /// </summary>
    public RESManageType loadBy;

    /// <summary>
    /// 加载时是否带后缀名
    /// </summary>
    public string suffix;

    /// <summary>
    /// 名称
    /// </summary>
    public string Name
    {
        get
        {
            return _name;
        }
        set
        {
            _name = value;
        }
    }

    public void Formatting()
    {

    }
    public object GetContent()
    {
        return RES.GetRES(path, type);
    }

    public object GetBaseContent()
    {
        return RES.GetBaseRES(path, type);
    }
}

