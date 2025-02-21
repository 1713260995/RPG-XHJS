using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class ServiceManager : MonoBehaviour
{
    public static ServiceManager instance;


    public bool isRunService = true;
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(instance);
        Client.Restart();
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Client.LoopReadQueueAsync();//读取队列的消息
    }

    private void OnApplicationQuit()
    {
        Client.isRunning = false;
        Client.Close();
    }

}
