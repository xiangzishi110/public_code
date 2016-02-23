//----------------------------------------------
// 游戏客户端中使用的通信类
// 开始连接：StartConnect()
// 发送消息：SendMessage()
// @author: ChenXing
// @email:  onechenxing@163.com
// @date:   2014/12/23
//----------------------------------------------
using UnityEngine;
using System.Collections.Generic;
using CxLib.Net;
using System;
using System.Net.Sockets;


/// <summary>
/// 网络通信管理器
/// </summary>
public class NetManager 
{
    /// <summary>
    /// 连接成功
    /// </summary>
    public event Action OnConnectCallBack;
    /// <summary>
    /// 连接出错
    /// </summary>
    public event Action<string> OnErrorCallback;

    /// <summary>
    /// 消息回调定义
    /// </summary>
    /// <param name="message"></param>
    public delegate void MessageCallback(object message);   

    /// <summary>
    /// 消息回调方式1字典
    /// </summary>
    private Dictionary<short, MessageCallbackHandler> _messageTagCallbackDic = new Dictionary<short, MessageCallbackHandler>();

    /// <summary>
    /// 通信客户端
    /// </summary>
    private SocketClient_Block _client;

    /// <summary>
    /// 服务器ip地址
    /// </summary>
    private string _ip;
    /// <summary>
    /// 服务器端口
    /// </summary>
    private int _port;

    /// <summary>
    /// 执行心跳
    /// </summary>
    private bool _pingRun = false;
    /// <summary>
    /// 心跳更新计时
    /// </summary>
    private float _pingTime;
    /// <summary>
    /// 等待后台返回
    /// </summary>
    private bool _pingWaitForServer = false;

    private static NetManager _instance;
    /// <summary>
    /// 单例
    /// </summary>
    public static NetManager instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new NetManager();
            }
            return _instance;
        }
    }

    public void Print(string txt)
    {
        Logger.LogError("[Socket] info:" + txt + ",    hash:" + _client.socket.GetHashCode() + ",   time:" + Time.realtimeSinceStartup);
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    private NetManager()
    {

    }

    /// <summary>
    /// 需要每帧调用，更新消息收取队列
    /// </summary>
    public void Update()
    {
        if (_client == null) return;
        _client.Update();
        
        //心跳（在后台返回后才开始执行）
        if (_pingRun )//&& GameInitialize.IsHasPlayerInfo)
        {
            Logger.LogError("GameInitialize.IsHasPlayerInfo  需要实现");
            //等待服务器ping返回
            if (_pingWaitForServer == true)
            {
                if(Time.realtimeSinceStartup > _pingTime + 25)//等待25秒后台返回
                {
                    OnError(_client.socket, "服务器没有返回ping消息");
                }
            }
            else
            //等待发送Ping
            if (Time.realtimeSinceStartup > _pingTime + 10)//等待10秒发送
            {
                _pingTime = Time.realtimeSinceStartup;
                Logger.LogError("这里要实现心跳包功能");
               // var pingMsg = new com.sy599.guaji3d.message.packet.protoPacket.PingReqMsg();
               // _pingWaitForServer = true;
                //SendMessage<com.sy599.guaji3d.message.packet.protoPacket.PingReqMsg>(pingMsg, OnPingCallBack, new List<Type>() { typeof(com.sy599.guaji3d.message.packet.protoPacket.IntResMsg) });
            }
        }
    }
    /// <summary>
    /// 加速次数验证
    /// </summary>
    private int checkAddSpeedTick = 0;
    /// <summary>
    /// 服务器Ping返回
    /// </summary>
    /// <param name="message"></param>
    private void OnPingCallBack(object message)
    {
        Logger.LogError("加速检测需要实现");
       /* var intRes = message as com.sy599.guaji3d.message.packet.protoPacket.IntResMsg;
        //有参数表示加速了
        if(intRes.@params != null && intRes.@params.Count > 0)
        {
            checkAddSpeedTick++;
            Logger.LogError("加速警告：连续加速次数:" + checkAddSpeedTick);
            //连续3此加速才判断为加速
            if(checkAddSpeedTick >= 3)
            {                
                _pingRun = false;
                PopupTishiUI.Instance.Show("反作弊系统提示", "你的操作已经记录，请不要继续使用加速器！", "去关闭加速器", delegate(int i, object o)
                {    
                    Application.Quit();
                });
            }
        }
        else
        {
            checkAddSpeedTick = 0;
        }*/
        _pingTime = Time.realtimeSinceStartup;
        _pingWaitForServer = false;
    } 

    /// <summary>
    /// 开始连接服务器
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    public void StartConnect(string ip, int port)
    {
        //如果前面还有连接，清理
        if(_client != null)
        {
            _client.OnConnect -= OnConnect;
            _client.OnData -= OnData;
            _client.OnError -= OnError;
            _client.Destory();
        }
        _pingTime = Time.realtimeSinceStartup;
        _ip = ip;
        _port = port;
        _client = new SocketClient_Block();
        Logger.Log("WWW -> 开始连接服务器");
        _client.OnConnect += OnConnect;
        _client.OnData += OnData;
        _client.OnError += OnError;
        _client.Connect(_ip, _port);
    }

    /// <summary>
    /// 重新链接
    /// </summary>
    public void ReConnect()
    {
        //清空上次连接的回调字典
        _messageTagCallbackDic = new Dictionary<short, MessageCallbackHandler>();
        _client.Connect(_ip, _port);
    }

    /// <summary>
    /// 连接成功
    /// </summary>
    private void OnConnect(Socket socket)
    {
        Logger.Log("WWW -> 服务器连接成功!");
        OnConnectCallBack();
        //开始心跳
        _pingRun = true;
        _pingWaitForServer = false;
        _pingTime = Time.realtimeSinceStartup;
    }

    /// <summary>
    /// 发送ProtoBuf消息
    /// </summary>
    /// <typeparam name="T">消息类型</typeparam>
    /// <param name="message">消息体</param>
    /// <param name="callback">回调（只回调第一个匹配的消息一次）</param>
    /// <param name="filters">收到消息类型过滤器（如不过滤，由后台第一个回调标识决定）</param>
    public void SendMessage<T>(T message, MessageCallback callback = null, List<Type> filters = null) where T : ProtoBuf.IExtensible
    {
        //消息类型
        short messageId = ProtoBufConfig.instance.GetMessageID(message.GetType());
        if(messageId == -1)
        {
            Debug.LogError("WWW -> 没有对应的消息类型:" + message.GetType());
            return;
        }
        Logger.Log("WWW -> 发送消息类型:" + messageId);
        //消息体
        byte[] data = ProtoBufSerializerHelper.ProtoRuntimeSerialize<T>(message);
        short messageTag = 0;
        //如果有回调，加入回调数组
        if (callback != null)
        {
            MessageCallbackHandler handler = new MessageCallbackHandler();
            handler.callback = callback;
            handler.filters = filters;
            handler.time = Time.realtimeSinceStartup;
            //生成一个标识号
            messageTag = BuildMessageTag();
            if (_messageTagCallbackDic.ContainsKey(messageTag) == false)
            {
                _messageTagCallbackDic.Add(messageTag, handler);
            }
            else
            {
                Debug.LogError("生成NetManager 回调 messagaTag 已经存在了");
            }
        }
        //创建消息
        NetPacket packet = NetPacket.Create(messageId, data, messageTag);
        //发送       
        _client.Send(packet);
    }

    private short messageTag = 1;
    private short BuildMessageTag()
    {
        if(messageTag >= short.MaxValue)
        {
            messageTag = 1;
        }
        else
        {
            messageTag++;
        }
        return messageTag;
    }    

    /// <summary>
    /// 收到服务端数据
    /// </summary>
    /// <param name="packet"></param>
    private void OnData(Socket socket, NetPacket packet)
    {
        Logger.Log("WWW -> 收到消息类型:" + packet.messageId);
        
        //根据id获取消息类
        Type messageClass = ProtoBufConfig.instance.GetMessageClass(packet.messageId);
        if(messageClass != null)
        {
            //获得协议包数据体
            byte[] msgBody = packet.ReadBody();
            //特定协议用gzip解压
            if(packet.messageId == 10033)
            {
                msgBody = NetGZip.UnGzip(msgBody); 
            }            
            object message = ProtoBufSerializerHelper.ProtoRuntimeDeserialize(messageClass, msgBody);
            
            //是否有处理器对象
            IMessageHandler handler = ProtoBufConfig.instance.GetMessageHandler(packet.messageId);
                     

            if (handler != null)
            {               
                handler.Handler(message);
            }
            else
            {
                Logger.Log("WWW -> 收到消息类型 没有Handdler:" + handler);
            }
      
            //是否有回调函数
            if (_messageTagCallbackDic.ContainsKey(packet.messageTag))
            {
            
                bool ifDoCallBack = true;
                MessageCallbackHandler callbackHandler = _messageTagCallbackDic[packet.messageTag];
                if(callbackHandler.filters != null)
                {
                    if(callbackHandler.filters.Contains(message.GetType()) == false)
                    {
                        ifDoCallBack = false;
                    }
                }
                if(ifDoCallBack)
                {
                    //只回调第一个匹配的消息一次
                    callbackHandler.callback(message);
                    _messageTagCallbackDic.Remove(packet.messageTag);
                }
            }
            else
            {
                //Logger.Log("WWW -> 收到消息类型 没有回调函数 :" + _messageTagCallbackDic.ContainsKey(packet.messageTag) + ";" + packet.messageId);
            }
        }
        else
        {
            Debug.LogError("NetManager:收到未配置消息：" + packet.messageId);
        }
    }

    /// <summary>
    /// 连接出错
    /// </summary>
    /// <param name="obj"></param>
    private void OnError(Socket socket, string obj)
    {
        Debug.LogWarning("WWW -> 网络连接出错 OnError:" + obj);   
        //出错后断开链接
        _client.Destory();
        //出错后停止心跳
        _pingRun = false;
        _pingWaitForServer = false;
        if (OnErrorCallback != null) OnErrorCallback(obj);
    }

    /// <summary>
    /// 主动触发错误，执行断线重连
    /// </summary>
    public void SendError()
    {
        OnError(_client.socket, "主动调用SendError()");
    }

    /// <summary>
    /// 释放
    /// </summary>
    public void Destroy()
    {
        _messageTagCallbackDic = new Dictionary<short, MessageCallbackHandler>();
        if (_client != null)
        {
            _client.Destory();
            _client = null;
        }
        _instance = null;
    }
}

/// <summary>
/// 回调监听结构体
/// </summary>
class MessageCallbackHandler
{
    public NetManager.MessageCallback callback;
    public List<Type> filters;
    public float time;
}
