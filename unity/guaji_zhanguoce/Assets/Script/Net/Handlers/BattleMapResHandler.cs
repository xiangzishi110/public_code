using System;
using UnityEngine;
using com.sy599.guaji3d.message.packet.protoPacket;

namespace Net.Handlers
{
    /// <summary>
    /// 战斗地图改变监听
    /// </summary>
    public class BattleMapResHandler: IMessageHandler
    {
        public void Handler(object msgBody)
        {
            //区域
            //if (msgBody is MapArenaListResMsg) {
               // MapArenaListResMsg msg = msgBody as MapArenaListResMsg;
                //Data.WorldMap.WorldMapModel.Instance.SetAreaInfo(msg);
            //}
           
       }
    }
}
