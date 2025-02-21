using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Net
{
    public class SocketMsg
    {
        public int sessionID;
        public List<PackageInfo> packageList;
        public string msg;
        //这条消息的最新发送时间
        public DateTime lastTime;
    }

    /// <summary>
    /// 客户端发送的消息的数据结构
    /// </summary>
    public class PackageInfo
    {
        //包的id
        public int sessionID;
        //包的序号（如果这条信息被拆成多个包,则需要使用此字段将所有的包进行排序）
        public int orderNumber;
        //如果这条信息被拆成多个包，一共有几个包
        public int maxOrderNumber;
        //发送消息的内容
        public string content;
        //发送的时间
        public DateTime time;
        //消息类型
        public MsgType type;
    }

    /// <summary>
    /// 消息类型
    /// </summary>
    public enum MsgType
    {
        /// <summary>
        /// 常规消息
        /// </summary>
        normal,
        /// <summary>
        /// 心跳检测
        /// </summary>
        HeartBeatCheck,
    }

    class MyThreadInfo
    {
        public int sessionID;
        public bool canRunning = true;
    }
}
