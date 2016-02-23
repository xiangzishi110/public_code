//----------------------------------------------
// protoBuf 协议配置文件解析和关联类
// @author: ChenXing
// @email:  onechenxing@163.com
// @date:   2014/12/23
//----------------------------------------------
using System.Collections.Generic;
using System;
using UnityEngine;
using com.sy599.guaji3d.message.packet.protoPacket;
using Net.Handlers;
using System.Collections;

/// <summary>
/// protoBuf 协议配置文件解析和关联类
/// </summary>
public class ProtoBufConfig
{
    /// <summary>
    /// 消息类/处理类映射字典
    /// </summary>
    private Dictionary<Type, IMessageHandler> _messageClassDic = new Dictionary<Type, IMessageHandler>();

    /// <summary>
    /// 消息类型/vo映射字典
    /// </summary>
    private Dictionary<short, MessageVo> _messageVoDic = new Dictionary<short, MessageVo>();

    private static ProtoBufConfig _instance;
    /// <summary>
    /// 单例
    /// </summary>
    public static ProtoBufConfig instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ProtoBufConfig();
            }
            return _instance;
        }
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public ProtoBufConfig()
    {
        InitProtoBuf();
    }

    #region 初始化所有消息类

    /// <summary>
    /// 初始化消息类
    /// </summary>
    private void InitProtoBuf()
    {
        //消息映射
        //登录
     /*   _messageClassDic.Add(typeof(UserLoginReqMsg), null);
        //返回玩家信息
        _messageClassDic.Add(typeof(PlayerInfoResMsg), new PlayerInfoHandler());

        _messageClassDic.Add(typeof(CreateRoleReqMsg), null);

        _messageClassDic.Add(typeof(UserLoginResMsg), null);
        _messageClassDic.Add(typeof(ItemInfoResMsg), null);
        _messageClassDic.Add(typeof(ItemListResMsg), null);
        _messageClassDic.Add(typeof(Vector3Msg),null);
        _messageClassDic.Add(typeof(IntReqMsg),null);
        _messageClassDic.Add(typeof(EquipReqMsg),null);
        _messageClassDic.Add(typeof(EquipInfoResMsg),null);
        _messageClassDic.Add(typeof(EquipListResMsg), new EquipListResHandler());
        _messageClassDic.Add(typeof(BasePropertyResMsg),null);
        _messageClassDic.Add(typeof(ExtendPropertyResMsg), null);
        _messageClassDic.Add(typeof(ArtifactPropertyResMsg), null);
        _messageClassDic.Add(typeof(SpecialPropertyResMsg), null);
        _messageClassDic.Add(typeof(SpecialPropertysResMsg),null);
        _messageClassDic.Add(typeof(ConsumeListResMsg), new ConsumeListResHandler());
        _messageClassDic.Add(typeof(ConsumeResMsg), null);
        _messageClassDic.Add(typeof(ErrorResMsg), new ErrorResHandler());
        _messageClassDic.Add(typeof(EnterBattleReqMsg), null);
        _messageClassDic.Add(typeof(CheckBattleReqMsg), null);
        _messageClassDic.Add(typeof(CheckBattleResMsg), null);
        _messageClassDic.Add(typeof(BattleReqMsg), null);
        _messageClassDic.Add(typeof(BattleResultReqMsg), null);
		_messageClassDic.Add(typeof(TreasureDragReqMsg), null);
        _messageClassDic.Add(typeof(MapResMsg), new BattleMapResHandler());
        _messageClassDic.Add(typeof(MapArenaListResMsg), new BattleMapResHandler());
        _messageClassDic.Add(typeof(OfflineBattleResMsg), new OfflineBattleResHandler());
        _messageClassDic.Add(typeof(HeroListResMsg),new HeroListResMsgHandler());
        _messageClassDic.Add(typeof(HeroInfoResMsg), new HeroInfoResMsgHandler());
        _messageClassDic.Add(typeof(HeroReqMsg), null);
        _messageClassDic.Add(typeof(PubResMsg), new HeroCallResMsgHandler());
       
        //主城移动
        _messageClassDic.Add(typeof(MoveResMsg), new MoveResHandler());
        _messageClassDic.Add(typeof(MoveToPosReqMsg), null);
        _messageClassDic.Add(typeof(OtherPlayerListResMsg), new OtherPlayerListResHandler());
        _messageClassDic.Add(typeof(RobotPlayerListResMsg), new OtherPlayerListResHandler());
        _messageClassDic.Add(typeof(ShopReqMsg), null);
        _messageClassDic.Add(typeof(ShopResMsg), new ShopResHandler());
        _messageClassDic.Add(typeof(MoneyTreeResMsg),null);
        _messageClassDic.Add(typeof(ShopListResMsg), null);
        _messageClassDic.Add(typeof(ShopItemResMsg), null);
        _messageClassDic.Add(typeof(SettingReqMsg), null);
        _messageClassDic.Add(typeof(SettingResMsg), null);*/

        

        
    }

    /// <summary>
    /// 根据类名获取消息类
    /// </summary>
    /// <param name="className"></param>
    /// <returns></returns>
    private Type FindTypeByClassName(string className)
    {
        foreach (var t in _messageClassDic)
        {
            if (t.Key.Name == className)
            {
                return t.Key;
            }
        }
        return null;
    }

    #endregion

    /// <summary>
    /// 根据消息类获取消息id(没有返回-1)
    /// </summary>
    /// <param name="messageClass"></param>
    /// <returns></returns>
    public short GetMessageID(Type messageClass)
    {
        foreach(var item in _messageVoDic)
        {
            if (item.Value.messageClass == messageClass)
            {
                return item.Key;
            }
        }
        return -1;
    }

    /// <summary>
    /// 获取消息id对应的消息类
    /// </summary>
    /// <param name="messageId"></param>
    /// <returns></returns>
    public Type GetMessageClass(short messageId)
    {
        if (_messageVoDic.ContainsKey(messageId))
        {
            return _messageVoDic[messageId].messageClass;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 获取收到消息id对应的处理类
    /// </summary>
    /// <returns></returns>
    public IMessageHandler GetMessageHandler(short messageId)
    {
        IMessageHandler handler = null;
        if (_messageVoDic.ContainsKey(messageId))
        {
            handler = _messageVoDic[messageId].handler;
        }
        return handler;
    }

    /// <summary>
    /// 解析配置
    /// </summary>
    /// <param name="node"></param>
	public void PaserConfig(XMLNode node)
    {
        _messageVoDic.Clear();
        XMLNodeList list = node.GetNodeList("messageList>0>message");
        for(int i = 0 ; i < list.Count ; i++)
        {
            XMLNode nowNode = list[i] as XMLNode;
            MessageVo vo = new MessageVo();
            vo.id = System.Convert.ToInt16(nowNode.GetValue("@id"));
            vo.messageClass = FindTypeByClassName(nowNode.GetValue("@name"));
            //配置文件里面有，游戏内还没加
            if(vo.messageClass == null)
            {
                Logger.LogWarning("ProtoBufConfig未配置消息id" + vo.id);
                continue;
            }
            if (_messageClassDic.ContainsKey(vo.messageClass))
            {
                vo.handler = _messageClassDic[vo.messageClass];
            }
            _messageVoDic.Add(vo.id, vo); 
        }
    }

    /// <summary>
    /// 打印所有消息Vo
    /// </summary>
    public void PrintAllMessageVo()
    {
        string info = "";
        foreach(var item in _messageVoDic)
        {
            info += item.Value.ToString() + "\n";
        }
        Logger.Log(info);
    }
}

#region 消息VO定义

/// <summary>
/// 消息Vo
/// </summary>
class MessageVo
{
    /// <summary>
    /// 消息id
    /// </summary>
    public short id;
    /// <summary>
    /// 消息类
    /// </summary>
    public Type messageClass;
    /// <summary>
    /// 接收消息处理器对象，如果为null，表示没有单独的处理器
    /// </summary>
    public IMessageHandler handler = null;

    public override string ToString()
    {
        return String.Format("[MessageVo id={0},messageClass={1},handler={2}]", id, messageClass, handler);
    }
}

#endregion