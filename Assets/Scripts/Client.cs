
using NetMQ;
using NetMQ.Sockets;
using System;
using System.IO;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Client : MonoBehaviour
{
    private readonly ConcurrentQueue<Action> runOnMainThread = new ConcurrentQueue<Action>();
    private Receiver receiver;
    private Texture2D tex;
    public RawImage image;
    private bool first = true;

    public void Start()
    {
        tex = new Texture2D(2, 2, TextureFormat.RGB24, mipChain: false);
        image.texture = tex;

        AsyncIO.ForceDotNet.Force();
        // - You might remove it, but if you have more than one socket
        //   in the following threads, leave it.
        receiver = new Receiver();
        receiver.Start((Data d) => runOnMainThread.Enqueue(() =>
        {
            Debug.Log(d.str);
            if (first)
            {
                tex.LoadImage(d.image);
                //Save image

                var bytes = tex.EncodeToPNG();
                var file = File.Open(Application.dataPath + "/transferpear.png", FileMode.Create);
                var binary = new BinaryWriter(file);
                binary.Write(bytes);
                file.Close();
                first = false;
            }
        }
        ));
    }

    public void Update()
    {
        if (!runOnMainThread.IsEmpty)
        {
            Action action;
            while (runOnMainThread.TryDequeue(out action))
            {
                action.Invoke();
            }
        }
    }

    private void OnDestroy()
    {
        receiver.Stop();
        NetMQConfig.Cleanup();  // Must be here to work more than once
    }
}