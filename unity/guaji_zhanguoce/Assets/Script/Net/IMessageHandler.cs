using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 消息处理接口
/// </summary>
public interface IMessageHandler
{
    void Handler(object msgBody);
}

