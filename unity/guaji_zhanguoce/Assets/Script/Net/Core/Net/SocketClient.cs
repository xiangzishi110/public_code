//----------------------------------------------
// Socket通信客户端类
// @author: ChenXing
// @email:  onechenxing@163.com
// @date:   2014/12/23
//----------------------------------------------
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CxLib.Net
{
    /// <summary>
    /// Socket通信客户端
    /// (异步非阻塞)
    /// </summary>
    public class SocketClient
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
        /// 发送次数，用于验证
        /// </summary>
        private int _sendNum;

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
            NetLogger.Log(message);
            lock (handler_error)
            { 
                handler_error = message;
            }
        }

        /// <summary>
        /// 客户端连接服务器
        /// </summary>
        /// <param name="ip">ip地址</param>
        /// <param name="port">端口</param>
        public void Connect(string ip, int port)
        {
            IPHostEntry ipHost = Dns.GetHostEntry(ip);
            IPEndPoint ipe = new IPEndPoint(ipHost.AddressList[0], port);
            try
            {
                _sendNum = 0;//重置次数
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.Blocking = false;//非阻塞Socket
                //开始连接
                _socket.BeginConnect(ipe, new AsyncCallback(ConnectCallback), _socket);
            }
            catch(Exception e)
            {
                DoError("连接出错:" + e.Message);
            }
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
        }

        /// <summary>
        /// 连接返回
        /// </summary>
        /// <param name="ar"></param>
        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                //与服务器取得连接
                _socket.EndConnect(ar);

                // 设置timeout
                _socket.SendTimeout = 8000;
                _socket.ReceiveTimeout = 8000;

                //通知连接成功
                handler_connect = true;
                NetLogger.Log("连接成功");
                //开始异步接收数据
                BeginReceive(_socket);
            }
            catch (Exception e)
            {
                DoError("连接出错:" + e.Message);
            }
        }

        #region 接收处理

        /// <summary>
        /// 绑定socket开始接收一个数据包
        /// 服务器程序调用此方法创建客户端连接
        /// 或者客户端内部自动调用他接收服务器数据包
        /// </summary>
        /// <param name="socket"></param>
        public void BeginReceive(Socket socket)
        {
            _socket = socket;
            //开始异步接收数据
            NetPacket packet = new NetPacket();
            _socket.BeginReceive(packet.bytes, 0, packet.headerLength, SocketFlags.None, new AsyncCallback(ReceiveHeader), packet);
        }

        /// <summary>
        /// 接收包头回调
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveHeader(IAsyncResult ar)
        {
            NetPacket packet = (NetPacket)ar.AsyncState;
            try
            {
                int read = _socket.EndReceive(ar);
                NetLogger.Log("读取了部分包头，长度为" + read.ToString(), 0);
                //断开
                if (read < 1)  
                {
                    //断开连接通知
                    DoError("断开连接");
                    return;
                }
                packet.readLength += read;
                //必须读满
                if (packet.readLength < packet.headerLength)
                {
                    _socket.BeginReceive(packet.bytes, packet.readLength, packet.headerLength - packet.readLength, SocketFlags.None, new AsyncCallback(ReceiveHeader), packet);
                }
                else
                {
                    //解析包头
                    packet.ReadHeader();
                    packet.readLength = 0;
                    NetLogger.Log("收到包头，解析包体长度为" + packet.bodyLength.ToString());
                    //没有包体，直接完毕
                    if(packet.bodyLength == 0)
                    {
                        //读取消息完毕
                        GetDataFinish(packet);
                        return;
                    }
                    //开始读取包体
                    _socket.BeginReceive(packet.bytes, packet.headerLength, packet.bodyLength, SocketFlags.None, new AsyncCallback(ReceiveBody), packet);
                }
            }
            catch (Exception e)
            {
                DoError("接收包头出错:" + e.Message);
            }
        }

        /// <summary>
        /// 接收包体回调
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveBody(IAsyncResult ar)
        {
            NetPacket packet = (NetPacket)ar.AsyncState;
            try
            {
                int read = _socket.EndReceive(ar);
                NetLogger.Log("读取了部分包体，长度为" + read.ToString(), 0);
                //断开
                if (read < 1)
                {
                    //断开连接通知
                    DoError("断开连接");
                    return;
                }
                packet.readLength += read;
                //必须读满
                if (packet.readLength < packet.bodyLength)
                {
                    _socket.BeginReceive(packet.bytes, packet.headerLength + packet.readLength, packet.bodyLength - packet.readLength, SocketFlags.None, new AsyncCallback(ReceiveBody), packet);
                }
                else
                {
                    NetLogger.Log("收到包体，长度：" + packet.bodyLength);
                    //读取消息完毕
                    GetDataFinish(packet);          
                }
            }
            catch (Exception e)
            {
                DoError("接收包体出错:" + e.Message);
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
            //开始下一个包的读取
            BeginReceive(_socket);
        }

        #endregion

        #region 发送处理(非阻塞)

        /// <summary>
        /// 发包(非阻塞)
        /// </summary>
        /// <param name="packet"></param>
        public void Send(NetPacket packet)
        {
            NetLogger.Log("准备发送");
            if (_socket.Connected)
            {
                try
                {
                    _socket.BeginSend(packet.bytes, 0, packet.length, 0, new AsyncCallback(SendCallback), packet);
                }
                catch (Exception e)
                {
                    DoError("SocketCline Send发送出错:" + e.Message);
                }
            }
            else
            {
                NetLogger.Log("Socket当前没有连接");
            }        
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                NetPacket packet = (NetPacket)ar.AsyncState;
                int bytesSent = _socket.EndSend(ar);
            }
            catch (Exception e)
            {
                DoError("SendCallback发送出错:" + e.Message);
            }
            NetLogger.Log("发送完毕");
        }
        #endregion

        #region 发送处理 (阻塞)
        /* 
        /// <summary>
        /// 发包 (阻塞)
        /// </summary>
        /// <param name="packet"></param>
        public void Send(NetPacket packet)
        {
            NetLogger.Log("准备发送");
            NetworkStream ns;
            lock(_socket)
            {
                ns = new NetworkStream(_socket);
                if(ns.CanWrite)
                {
                    try
                    {
                        _sendNum++;
                        packet.checkNum = _sendNum;
                        NetLogger.Log("发送，包长度为" + packet.length);
                        ns.BeginWrite(packet.bytes, 0, packet.length, new AsyncCallback(SendCallback), ns);
                    }
                    catch (Exception e)
                    {
                        DoError("Send发送出错:" + e.Message);
                    }
                }
                else
                {
                    NetLogger.Log("Socket当前不能写入");
                }
            }
        }

        /// <summary>
        /// 发送回调
        /// </summary>
        /// <param name="ar"></param>
        private void SendCallback(IAsyncResult ar)
        {
            NetworkStream ns = (NetworkStream)ar.AsyncState;
            try
            {
                ns.EndWrite(ar);
                ns.Flush();
                ns.Close();
            }
            catch (Exception e)
            {
                DoError("SendCallback发送出错:" + e.Message);
            }
            NetLogger.Log("发送完毕");
        }        
        */
        #endregion        
    }
}
