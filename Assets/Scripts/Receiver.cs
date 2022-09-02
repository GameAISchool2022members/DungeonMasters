
using NetMQ;
using NetMQ.Sockets;
using AsyncIO;
using System.Collections.Concurrent;
using System.Threading;
using System;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Receiver
{
    private readonly Thread receiveThread;
    private bool running;

    public Receiver()
    {
        receiveThread = new Thread((object callback) =>
        {
            using (var socket = new RequestSocket())
            {
                socket.Connect("tcp://localhost:5555");

                while (running)
                {
                    socket.SendFrameEmpty();
                    string message = socket.ReceiveFrameString();
                    Data data = JsonUtility.FromJson<Data>(message);
                    ((Action<Data>)callback)(data);
                }
            }
        });
    }

    public void Start(Action<Data> callback)
    {
        running = true;
        receiveThread.Start(callback);
    }

    public void Stop()
    {
        running = false;
        receiveThread.Join();
    }
}