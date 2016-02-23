//----------------------------------------------
// socket通信包结构类
// @author: ChenXing
// @email:  onechenxing@163.com
// @date:   2014/12/23
//----------------------------------------------
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CxLib.Net
{
    /// <summary>
    /// 网络数据包结构
    /// 包结构: [包的总长度（2个字节）, 消息校验码（4个字节）, 消息类型ID（2个字节）, 消息标识号（2个字节）, 内容（protobuf实现）]
    /// 
    /// 包的总长度（2个字节）：从包头第一位起一直到包尾包含的字节数。
    /// 消息校验码（4个字节）：用于验证消息，防止被篡改，计算方法：累计数发送消息数+包数据的偶数位累加值。
    /// 消息类型ID（2个字节）：配置文件中定义的消息类型，对应protobuf的解析类。
    /// 消息标识号（2个字节）：用于处理前台请求回调，对于需要后台回复的请求，前台发消息给后台一个消息标识，后台回应此请求时需携带同样标识。
    /// 内容（protobuf实现）：protobuf的二进制流。
    /// 
	/// 区域划分：
    ///    包头（10个字节）：长度（2个字节）, 消息校验码（4个字节），消息类型ID（2个字节），消息标识号（2个字节）
	///    包体（长度不固定）：内容（protoc实现）
    /// </summary>
    public class NetPacket
    {
        /// <summary>
        /// 数据包最大长度
        /// </summary>
        public const int MAX_PACKET_LENGTH = 32 * 1024;

        /// <summary>
        /// 验证数(通过后台发过来的数值，再累加，为0标识不加验证数)
        /// </summary>
        public static int checkNum = 0;

        /// <summary>
        /// 包头长度
        /// </summary>
        public int headerLength
        {
            get { return 10; } 
        }

        /// <summary>
        /// 包体长度
        /// </summary>
        public int bodyLength 
        {
            get { return length - headerLength; } 
        }

        /// <summary>
        /// 包的总长度
        /// </summary>
        public int length { get; set; }

        /// <summary>
        /// 消息类型ID
        /// </summary>
        public short messageId { get; set; }

        /// <summary>
        /// 消息标识号
        /// </summary>
        public short messageTag { get; set; }

        /// <summary>
        /// 接收数据的byte数组
        /// </summary>
        public byte[] bytes { get; set; }

        /// <summary>
        /// 当前读取数据的长度标记
        /// </summary>
        public int readLength { get; set; }

        
        /// <summary>
        /// 构造函数
        /// </summary>
        public NetPacket()
        {
            Reset();
            bytes = new byte[MAX_PACKET_LENGTH];
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            readLength = 0;
            length = 0;
        }

        #region 读数据

        /// <summary>
        /// 解析包头
        /// </summary>
        public void ReadHeader()
        {
            //1.读取包长度
            short myPacketLength = System.BitConverter.ToInt16(bytes, 0);
            //转换成本地字节顺序格式
            myPacketLength = System.Net.IPAddress.NetworkToHostOrder(myPacketLength);
            length = myPacketLength;
            //2.读取校验码(修改为后台登录赋值，前台累加)
            //checkNum = System.BitConverter.ToInt32(bytes, 2);
            //checkNum = System.Net.IPAddress.NetworkToHostOrder(checkNum);
            //3.读取消息类型
            messageId = System.BitConverter.ToInt16(bytes, 6);
            messageId = System.Net.IPAddress.NetworkToHostOrder(messageId);
            //4.读取消息标识号
            messageTag = System.BitConverter.ToInt16(bytes, 8);
            messageTag = System.Net.IPAddress.NetworkToHostOrder(messageTag);
            NetLogger.Log("收到messageTag：" + messageTag, 2);
        }

        /// <summary>
        /// 获得消息体的数据
        /// </summary>
        public byte[] ReadBody()
        {
            byte[] bs = new byte[bodyLength];
            Array.Copy(bytes, headerLength, bs, 0, bodyLength);
            return bs;
        }

        #endregion

        #region 写数据

        /// <summary>
        /// 根据消息类型和数据流构建一个数据包
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static NetPacket Create(short type, byte[] data, short tag = 0)
        {
            NetPacket packet = new NetPacket();
            packet.messageId = type;
            packet.messageTag = tag;
            packet.BeginWrite();
            packet.WriteBytes(data);
            packet.EndWrite();
            NetLogger.Log("发送messageTag：" + tag, 2);
            return packet;
        }

        /// <summary>
        /// 开始写入数据
        /// </summary>
        public void BeginWrite()
        {
            length = headerLength;//从包头结束位置开始写入包体数据
        }

        /// <summary>
        /// 结束写入数据并生成包头
        /// </summary>
        public void EndWrite()
        {
            //更新验证数(为0标识不加验证数)
            if (checkNum != 0)
            {
                checkNum++;
                //超过1w归位到1
                if (checkNum > 10000)
                {
                    checkNum = 1;
                }
            }

            //构建校验码(验证数+包体数据的偶数位的累加值)
            int myCheckSum = checkNum;
            //从包头结束到包体结束
            for (int i = headerLength; i < length; i += 2)
			{
                myCheckSum += bytes[i];
			}
            //1.写入包大小
            short myPacketLength = System.Convert.ToInt16(length);
            //转换成网络大端字节顺序格式
            myPacketLength = System.Net.IPAddress.HostToNetworkOrder(myPacketLength);
            byte[] bs = System.BitConverter.GetBytes(myPacketLength);
            bs.CopyTo(bytes, 0);
            //2.写入验证数
            myCheckSum = System.Net.IPAddress.HostToNetworkOrder(myCheckSum);
            byte[] myCheckSumBytes = System.BitConverter.GetBytes(myCheckSum);
            myCheckSumBytes.CopyTo(bytes, 2);
            //3.写入消息类型
            short myType = System.Net.IPAddress.HostToNetworkOrder(messageId);
            byte[] myTypeBytes = System.BitConverter.GetBytes(myType);
            myTypeBytes.CopyTo(bytes, 6);
            //4.写入消息标识号
            short myTag = System.Net.IPAddress.HostToNetworkOrder(messageTag);
            byte[] myTagBytes = System.BitConverter.GetBytes(myTag);
            myTagBytes.CopyTo(bytes, 8);
        }


        /// <summary>
        /// 写入二进制数据
        /// </summary>
        /// <param name="bs"></param>
        public void WriteBytes(byte[] bs)
        {
            if (length + bs.Length > MAX_PACKET_LENGTH)
            {
                NetLogger.Log("写入数据超过包长度限制:" + MAX_PACKET_LENGTH, 9);
                return;
            }
            //压入数据流
            bs.CopyTo(bytes, length);
            length += bs.Length;
        }

        #endregion
    }
}
