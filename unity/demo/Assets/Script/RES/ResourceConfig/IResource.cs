using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public interface IResource
{
    /// <summary>
    /// 名称
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// 数据格式化
    /// </summary>
    void Formatting();
    /// <summary>
    /// 获取内容
    /// </summary>
    /// <returns></returns>
    object GetContent();

    /// <summary>
    /// 获取原始内容
    /// </summary>
    object GetBaseContent();
}

