using Assets.DataLayer;
using Assets.DataLayer.Interfaces;
using Assets.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DataManager : MonoBehaviour
{
   /* public static DataManager Instance { get; private set; }
    public GenericRequester Requester { get; private set; }

    [HideInInspector]
    public object ProcessedResponse;

    [HideInInspector]
    public object IntermediateResponse;

    private List<Collectables> currentCollectables;

    private List<Texture2D> textures;

    public bool canStartGenerating;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        Requester = new GenericRequester("http://127.0.0.1:5000");
    }

    // Start is called before the first frame update
    void Start()
    {
        //Populate all web api related state variables
        StartCoroutine(InitCoroutine());
    }

    IEnumerator InitCoroutine()
    {
        yield return ImageBatchGET_Coroutine(6);
        textures = (List<Texture2D>)DataManager.Instance.ProcessedResponse;
        canStartGenerating = true;
}

    void ParseImages(ImageBatchResponse response)
    {
        IntermediateResponse = response;
        var imageBatchResponse = (ImageBatchResponse)response;
        var imgs = imageBatchResponse.Imgs;
        var texList = new List<Texture2D>();


        foreach (var img in imgs)
        {
            byte[] byteArray = Convert.FromBase64String(img);
            Texture2D tex = new Texture2D(2, 2);
            //Load data into the texture and upload it to the GPU.
            tex.LoadImage(byteArray);
            texList.Add(tex);


        }

        ProcessedResponse = texList;
    }

    public List<Collectables> GetCurrentLevelCollectables()
    {
        if (currentCollectables == null)
            currentCollectables = new List<Collectables>();
        currentCollectables.Clear();

        foreach(var tex in textures)
        {
            Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
            currentCollectables.Add( new Collectables {
                image = sprite 
            });
        }

        return currentCollectables;
    }

    public List<Texture2D> GetCurrentLevelTextures()
    {
        if (textures == null)
            textures = new List<Texture2D>();

        return textures;
    }

    public IEnumerator UploadSelectedCollectables(List<Collectables> collectables)
    {
        var byteImages = new List<byte[]>();
        for (int i = 0; i < collectables.Count; i++)
        {
            byte[] bytes = collectables[i].image.texture.EncodeToPNG();
            byteImages.Add(bytes);
        }

        yield return ImageBatchPOST_Coroutine(byteImages, 6);
        textures = (List<Texture2D>)DataManager.Instance.ProcessedResponse;

    }


    public IEnumerator ImageBatchPOST_Coroutine(List<byte[]> byteImages, int batchSize)
    {
        yield return Requester.PostFileSections<ImageBatchResponse>(byteImages, batchSize, "ImageBatch", ParseImages);
    }

    public IEnumerator ImageBatchGET_Coroutine(int batchSize)
    {
        yield return Requester.GetObject<ImageBatchResponse>($"ImageBatch/{batchSize}", ParseImages);
    }



    */
}
