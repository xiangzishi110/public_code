//----------------------------------------------
// Socket通信客户端类
// @author: ChenXing
// @email:  onechenxing@163.com
// @date:   2016/6/6
//----------------------------------------------
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CxLib.Net
{
    /// <summary>
    /// Socket通信客户端(同步阻塞方式)
    /// </summary>
    public class SocketClient_Block
    {
        /// <summary>
        /// 收到消息回调
        /// </summary>
        public event Action<Socket, NetPacket> OnData;
        /// <summary>
        /// 连接成功回调
        /// </summary>
        public event Action<Socket> OnConnect;
        /// <summary>
        /// 网络出错回调
        /// </summary>
        public event Action<Socket, string> OnError;

        /// <summary>
        /// 接收包待处理队列
        /// </summary>
        private Queue<NetPacket> handler_packetQueue = new Queue<NetPacket>();
        /// <summary>
        /// 连接回调处理标记
        /// </summary>
        private bool handler_connect = false;
        /// <summary>
        /// 错误回调处理标记
        /// </summary>
        private string handler_error = "";

        /// <summary>
        /// 实际socket对象
        /// </summary>
        private Socket _socket;

        /// <summary>
        /// 接收线程
        /// </summary>
        private Thread _receiveThread;

        /// <summary>
        /// 是否销毁
        /// </summary>
        private bool _isDestory = false;

        /// <summary>
        /// ip
        /// </summary>
        private string _ip;
        /// <summary>
        /// 端口
        /// </summary>
        private int _port;

        /// <summary>
        /// 获得关联socket对象
        /// </summary>
        public Socket socket
        {
            get
            {
                return _socket;
            }
        }

        /// <summary>
        /// 主循环 ，更新消息
        /// </summary>
        public void Update()
        {
            //数据队列处理
            if (handler_packetQueue.Count > 0)
            {
                lock(handler_packetQueue)
                {
                    while(handler_packetQueue.Count > 0)
                    {
                        OnData(_socket, handler_packetQueue.Dequeue());
                    }
                }
            }
            //连接成功
            if(handler_connect)
            {
                OnConnect(_socket);
                handler_connect = false;
            }
            //错误消息处理
            if(handler_error != "")
            {
                OnError(_socket, handler_error);
                lock (handler_error)
                {                    
                    handler_error = "";
                }
            }
        }
                

        /// <summary>
        /// 错误处理
        /// </summary>
        public void DoError(string message)
        {
            Logger.Log("SocketClient_Block  错误处理:"+message);
            lock (handler_error)
            { 
                handler_error = message;
            }
        }

        /// <summary>
        /// 客户端连接服务器
        /// (注意不要重复连接，收到连接成功或错误后再执行断线重连)
        /// </summary>
        /// <param name="ip">ip地址</param>
        /// <param name="port">端口</param>
        public void Connect(string ip, int port)
        {
            Destory();
            _ip = ip;
            _port = port;
            _receiveThread = new Thread(DoThread);
            _receiveThread.IsBackground = true;
            _receiveThread.Start();
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void Destory()
        {
            if (_socket != null)
            {
                _socket.Close();
                _socket = null;
            }
            if (_receiveThread != null)
            {
                _receiveThread.Abort();
                _receiveThread = null;
            }
        }

        /// <summary>
        /// Socket线程
        /// </summary>
        public void DoThread()
        {
            try
            {
                DoConnect();
                DoReceive();
            }
            catch(Exception e)
            {
                DoError(e.Message);
            }
        }

        /// <summary>
        /// 建立连接
        /// </summary>
        public void DoConnect()
        {
            IPHostEntry ipHost = Dns.GetHostEntry(_ip);
            IPEndPoint ipe = new IPEndPoint(ipHost.AddressList[0], _port);

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Blocking = true;
            _socket.Connect(ipe);
            //通知连接成功
            handler_connect = true;
            NetLogger.Log("连接成功");
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="socket"></param>
        public void DoReceive()
        {
            //死循环
            while (true)
            {
                NetPacket packet = new NetPacket();

                //读取包头
                //必须读满，防止粘包
                while (packet.readLength < packet.headerLength)
                {
                    int r = _socket.Receive(packet.bytes, packet.readLength, packet.headerLength - packet.readLength, SocketFlags.None);
                    packet.readLength += r;
                    NetLogger.Log("收到包头长度:" + r);
                }

                //解析包头
                packet.ReadHeader();
                packet.readLength = 0;
                NetLogger.Log("解析包体长度为:" + packet.bodyLength.ToString());
                //没有包体，直接完毕
                if (packet.bodyLength > 0)
                {
                    //读取包体
                    //必须读满，防止粘包
                    while (packet.readLength < packet.bodyLength)
                    {
                        int r = _socket.Receive(packet.bytes, packet.headerLength + packet.readLength, packet.bodyLength - packet.readLength, SocketFlags.None);
                        packet.readLength += r;
                        NetLogger.Log("收到包体长度:" + r);
                    }
                }
                //消息完毕
                GetDataFinish(packet);
            }
        }

        /// <summary>
        /// 收到数据完成处理
        /// </summary>
        private void GetDataFinish(NetPacket packet)
        {
            //加入队列等待处理
            lock(handler_packetQueue)
            {
                handler_packetQueue.Enqueue(packet);
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="packet"></param>
        public void Send(NetPacket packet)
        {
            try
            {
                Logger.Log("发送消息包总长度:" + packet.length);
                if (packet.bytes == null || packet.bytes.Length == 0)
                {
                    Logger.Log("发送消息包bytes null:");
                }
                int r = _socket.Send(packet.bytes, 0, packet.length, SocketFlags.None);
                NetLogger.Log("发送消息长度:" + r);
            }
            catch (Exception e)
            {
                DoError("SocketClient_Block Send发送出错:" + e.Message);
            }
        }
    }
}
