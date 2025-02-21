using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Assets.Scripts.Net;
using UnityEngine;
using System.Linq;

public class Client
{
    private static UdpClient localSocket = null;
    private static IPEndPoint serverPonint;
    private static string serverIP = MyTools.GetLocalIP();
    private static int serverPort = 9999;
    private static int locatePort = 9001;
    private static readonly int msgMaxLength = 10;
    public static ConcurrentQueue<SocketMsg> serverMsgsQueue = new ConcurrentQueue<SocketMsg>();
    public static List<SocketMsg> serverMsgsList = new List<SocketMsg>();
    //为serverMsgsList集合提供锁
    private static object _lockList = new object();
    //游戏正在运行
    public static bool isRunning = true;
    private static ConcurrentQueue<MyThreadInfo> myThreadInfos = new ConcurrentQueue<MyThreadInfo>();
    #region 可以被调用
    /// <summary>
    /// 重启服务
    /// </summary>
    public static void Restart()
    {
        Close();//关闭现有连接
        MyThreadInfo t0 = new MyThreadInfo { sessionID = MyTools.GenerateIntID(8), canRunning = true };
        MyThreadInfo t1 = new MyThreadInfo { sessionID = MyTools.GenerateIntID(8), canRunning = true };
        MyThreadInfo t2 = new MyThreadInfo { sessionID = MyTools.GenerateIntID(8), canRunning = true };
        myThreadInfos.Enqueue(t0);
        myThreadInfos.Enqueue(t1);
        myThreadInfos.Enqueue(t2);
        CreateSocket();//创建本地连接
        ReceiveMsgAsync();//接收服务器消息
        LoopCheckTimeOutAsync(t1.sessionID);//执行心跳检测
        LoopCheckListMsgAsync(t2.sessionID);//清理集合内超时消息
    }

    /// <summary>
    /// 发送消息到对方
    /// </summary>
    /// <param name="o"></param>
    public static void SendMsg(string msg, MsgType type = MsgType.normal)
    {
        try
        {
            if (msg == null || msg.Length == 0)
            {
                return;
            }
            if (msg == "*")
            {
                Close();
                return;
            }
            PackageInfo packageInfo = new PackageInfo
            {
                //包的初始长度为127
                sessionID = MyTools.GenerateIntID(8),
                content = null,
                type = type,
            };
            int order = msg.Length / msgMaxLength;
            if (msg.Length % msgMaxLength > 0)
            {
                order++;
            }
            for (int i = 0; i < order; i++)
            {
                string content;
                if (i == order - 1)
                {
                    content = msg.Substring(i * msgMaxLength, msg.Length - i * msgMaxLength);
                }
                else
                {
                    content = msg.Substring(i * msgMaxLength, msgMaxLength);//0-9,10-19......以此类推
                }
                packageInfo.content = content;
                packageInfo.orderNumber = i + 1;
                packageInfo.maxOrderNumber = order;
                packageInfo.time = DateTime.Now;
                string result="";
                //string result = JsonConvert.SerializeObject(packageInfo);
                if (packageInfo.type != MsgType.HeartBeatCheck)
                {
                    Print("发送的消息是：" + result);
                }
                byte[] buffer = Encoding.UTF8.GetBytes(result);
                if (localSocket != null)
                {
                    localSocket.Send(buffer, buffer.Length, serverPonint);
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// 持续处理队列中的消息
    /// </summary>
    public static void LoopReadQueueAsync()
    {
        string msg = TryDequeue();
        if (msg != null)
        {
            Print("这条完整的消息是:" + msg);
        }
    }
    #endregion



    #region 不可以被调用

    #endregion
    /// <summary>
    /// 等待接收消息
    /// </summary>
    private async static void ReceiveMsgAsync()
    {
        try
        {
            while (isRunning)
            {
                if (localSocket != null)
                {
                    var received = await localSocket.ReceiveAsync();
                    IPEndPoint remotePoint = received.RemoteEndPoint;
                    string msg = Encoding.UTF8.GetString(received.Buffer);
                    string from = $"{remotePoint.Address}:{remotePoint.Port}";
                    //Print($"接收到{from}的消息：{msg}");
                    //PackageInfo package = JsonConvert.DeserializeObject<PackageInfo>(msg);
                    //serverMsgsQueue.Enqueue(new SocketMsg
                    //{
                    //    sessionID = package.sessionID,
                    //    lastTime = DateTime.Now,
                    //    packageList = new List<PackageInfo>() { package }
                    //});
                }
            }
            Print("跳出ReceiveMsgAsync方法");
        }
        catch (Exception ex)
        {
            Print(ex.Message);
            Restart();
        }
    }

    /// <summary>
    /// 关闭socket
    /// </summary>
    public static void Close()
    {
        CloseAllThread();
        if (localSocket != null)
        {
            localSocket.Close();
            localSocket = null;
        }
    }

    /// <summary>
    /// 创建本地socket
    /// </summary>
    private static void CreateSocket()
    {
        IPEndPoint locatePoint = new IPEndPoint(IPAddress.Any, locatePort);
        //生成本机socket
        localSocket = new UdpClient(locatePoint);
        //生成服务器终端
        serverPonint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
        Print("客户端已就绪。。。");
    }

    /// <summary>
    /// 心跳检测, 告诉服务器,这个服务还在运行
    /// </summary>
    private async static void LoopCheckTimeOutAsync(int sessionID)
    {
        while (isRunning)
        {
            var m = myThreadInfos.Where(o => o.sessionID == sessionID).FirstOrDefault();
            if (m.canRunning == false)
            {
                break;
            }
            await Task.Delay(3000);
            SendMsg("发送心跳检测信号", MsgType.HeartBeatCheck);
            Print("我发送了一次心跳检测信号---" + DateTime.Now.ToString());
        }
        Print("已跳出LoopCheckTimeOutAsync方法");
    }

    /// <summary>
    /// 读取队列消息，从队列中读取一条消息。
    /// 如果这条消息是完整的就读取出来。
    /// 如果是不完整的，就将其放到队列的末尾。
    /// </summary>
    /// <returns></returns>
    private static string TryDequeue()
    {
        SocketMsg model;
        if (serverMsgsQueue.TryDequeue(out model))
        {
            if (model.packageList[0].maxOrderNumber == model.packageList.Count)
            {
                var list = model.packageList.OrderBy(o => o.orderNumber);
                string msg = "";
                foreach (var item in list)
                {
                    msg += item.content;
                }
                model.msg = msg;
                return model.msg;
            }
            else
            {
                //如果这条消息属于不完整消息，就将其放到集合中，等待接收完整后再重新放到队列
                SocketMsg msg = serverMsgsList.Where(o => o.sessionID == model.sessionID).FirstOrDefault();//查询此数据的关联数据是否已经被加到集合中
                if (msg != null)
                {
                    msg.lastTime = model.lastTime;
                    //如果之前已经被添加，就追加这条数据
                    msg.packageList.AddRange(model.packageList);
                    //在进行列表去除，防止有重复数据
                    msg.packageList = msg.packageList.GroupBy(x => x.orderNumber).Select(y => y.First()).ToList();
                }
                else
                {
                    //如果未被添加，就添加此数据
                    serverMsgsList.Add(model);
                }
                var list = serverMsgsList.Where(o => o.packageList[0].maxOrderNumber == o.packageList.Count).ToList();//查看集合中是否有被补充完整的数据
                if (list != null & list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        serverMsgsQueue.Enqueue(list[i]);
                        serverMsgsList.Remove(list[i]);
                    }
                }
            }
        }
        return null;
    }

    /// <summary>
    /// 清理集合内的超时消息
    /// </summary>
    private static async void LoopCheckListMsgAsync(int sessionID)
    {
        while (isRunning)
        {
            var m = myThreadInfos.Where(o => o.sessionID == sessionID).FirstOrDefault();
            if (m.canRunning == false)
            {
                break;
            }
            await Task.Delay(10000);
            lock (_lockList)
            {
                var r = serverMsgsList.RemoveAll(o => o == null || (DateTime.Now - o.lastTime).TotalSeconds > 5);
                if (r > 0)
                {
                    Print($"已删除{r}条过期消息");
                }
            }

        }
        Print("已跳出LoopCheckListMsgAsync方法");
    }

    private static void Print(object o)
    {
        Debug.Log(o);
    }

    /// <summary>
    /// 关闭所有线程
    /// </summary>
    public static void CloseAllThread()
    {
        foreach (var item in myThreadInfos)
        {
            var m = myThreadInfos.Where(o => o.sessionID == item.sessionID).FirstOrDefault();
            m.canRunning = false;
        }
    }
}


