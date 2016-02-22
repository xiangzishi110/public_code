using System.Collections.Generic;


/// <summary>
/// 资源配置数组，只提供加载便利，不提供是否已加载缓存判断。
/// 以ResourceItem为准。
/// </summary>
public class ResourceGroup : IResource
{
    private string _name;

    /// <summary>
    /// ResourceItem 数组
    /// </summary>
    public List<IResource> children;
    public ResourceGroup()
    {
        children = new List<IResource>();
    }
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
    /// <summary>
    /// 过滤掉重复的
    /// </summary>
    public void Formatting()
    {
        List<IResource> old =  children;
        children = new List<IResource>();
        ResourceGroup group;
        foreach (IResource item in old)
        {
            if (item is ResourceGroup)
            {
                item.Formatting();
                group = item as ResourceGroup;
                foreach (IResource child in group.children)
                {
                    if (!children.Contains(child))
                    {
                        children.Add(child);
                    }
                }
            }
            else
            {
                if (!children.Contains(item))
                {
                    children.Add(item);
                }
            }
        }
    }
    public object GetContent()
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        ResourceGroup group;
        ResourceItem rItem;
        foreach (IResource item in children)
        {
            if (item is ResourceGroup)
            {
                group = item as ResourceGroup;
                foreach (ResourceItem child in group.children)
                {
                    dic.Add(child.Name, child.GetContent());
                }
            }
            else
            {
                rItem = item as ResourceItem;
                dic.Add(rItem.Name, rItem.GetContent());
            }
        }
        return dic;
    }

    public object GetBaseContent()
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        ResourceGroup group;
        ResourceItem rItem;
        foreach (IResource item in children)
        {
            if (item is ResourceGroup)
            {
                group = item as ResourceGroup;
                foreach (ResourceItem child in group.children)
                {
                    dic.Add(child.Name, child.GetBaseContent());
                }
            }
            else
            {
                rItem = item as ResourceItem;
                dic.Add(rItem.Name, rItem.GetBaseContent());
            }
        }
        return dic;
    }
}

