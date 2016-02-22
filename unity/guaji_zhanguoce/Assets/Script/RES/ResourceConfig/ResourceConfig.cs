using System;
using System.Collections.Generic;
using UnityEngine;


public class ResourceConfig
{
    private Dictionary<string, ResourceItem> _itemDic;

    private Dictionary<string, ResourceGroup> _groupDic;
    public ResourceConfig()
    {
        _itemDic = new Dictionary<string, ResourceItem>();
        _groupDic = new Dictionary<string, ResourceGroup>();
    }
    public void init(XMLNode xml)
    {
        XMLNodeList itemList = xml.GetNodeList("root>0>item");
        if (itemList != null)
        {
            foreach (XMLNode item in itemList)
            {
                AddResourceItem(item.GetValue("@name"), item.GetValue("@path"), item.GetValue("@type"), item.GetValue("@loadBy"), item.GetValue(@"suffix"));
            }
        }

        XMLNodeList groupList = xml.GetNodeList("root>0>group");
        if (groupList != null)
        {
            string temp;
            string[] children;
            foreach (XMLNode group in groupList)
            {
                temp = group.GetValue("@children");
                children = temp.Split(new char[] { ',' });
                AddResourceGroup(group.GetValue("@name"), children);
            }
        }

    }
    public void AddResourceItem(string name, string url, string type, string loadBy,string suffix)
    {
        int loadByInt = int.Parse(loadBy);
        if (!_itemDic.ContainsKey(name))
        {
            ResourceItem item = new ResourceItem();
            item.Name = name;
            item.path = url;
            item.type = type;
            item.suffix = suffix;
            if (loadByInt == 0)
            {
                item.loadBy = RES.resManageType;
            }
            else if (loadByInt == 1)
            {
                item.loadBy = RESManageType.ASSETBUNDLE;
            }
            else
            {
                item.loadBy = RESManageType.RESOURCES;
            }
            _itemDic.Add(item.Name, item);
        }
        else
        {
            throw new Exception("检测到ResourceConfig.xml中的item有重复！");
        }
    }
    /// <summary>
    /// 添加一个资源组
    /// </summary>
    /// <param name="name">资源组的名称</param>
    /// <param name="children">资源组中的项列表</param>
    public void AddResourceGroup(string name, string[] children)
    {
        if (!_groupDic.ContainsKey(name))
        {
            ResourceGroup group = new ResourceGroup();
            group.Name = name;
            IResource child;
            foreach (string childName in children)
            {
                child = GetItemOrGroup(childName) as IResource;
                if (child == null)
                {
                    throw new Exception("资源配置表分析出错！" + name + "资源组中的" + childName + "不存在");
                }
                else
                {
                    group.children.Add(child);
                }
            }
            _groupDic.Add(group.Name, group);
            group.Formatting();
        }
    }
    /// <summary>
    /// 通过名称获取资源项或组
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IResource GetItemOrGroup(string name)
    {
        if (_itemDic.ContainsKey(name))
        {
            return _itemDic[name];
        }
        else if (_groupDic.ContainsKey(name))
        {
            return _groupDic[name];
        }
        return null;
    }

    public bool ResourceExits(string name)
    {
        if (_itemDic.ContainsKey(name))
        {
            return true;
        }
        else if (_groupDic.ContainsKey(name))
        {
            return true;
        }
        return false;
    }
}

