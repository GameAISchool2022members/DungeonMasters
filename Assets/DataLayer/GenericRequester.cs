using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Assets.DataLayer.Interfaces;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.DataLayer
{
    public class GenericRequester : IGenericRequester
    {
        static readonly HttpClient client = new HttpClient();
        public GenericRequester(string host)
        {
            Endpoint = host;
        }

        public string Endpoint { get; }


        public IEnumerator GetObject<Tout>(string path, Action<Tout> responseHandler = null)
        {

            // parse response
            using (UnityWebRequest www = UnityWebRequest.Get($"{Endpoint}/{path}"))
            {
                www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");

                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                    Debug.Log(www.error);
                else
                    responseHandler?.Invoke(JsonConvert.DeserializeObject<Tout>(www.downloadHandler.text));
            }

        }

        public IEnumerator PostObject<Tin, Tout>(Tin bodyObj, string path, Action<Tout> responseHandler = null)
        {
            var json = JsonConvert.SerializeObject(bodyObj);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            // parse response
            using (UnityWebRequest www = UnityWebRequest.Post($"{Endpoint}/{path}", json))
            {
                www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json; charset=utf-8");
                //www.SetRequestHeader("Content-Length", bodyRaw.Length.ToString());
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    responseHandler?.Invoke(JsonConvert.DeserializeObject<Tout>(www.downloadHandler.text));

                }
            }

        }

        public IEnumerator PostObjectSections<Tout>(Dictionary<string, (dynamic, string)> formData, string path, Action<Tout> responseHandler = null)
        {
            var sections = new List<IMultipartFormSection>();

            // According to your servers needs
            foreach(var key in formData.Keys)
            {
                sections.Add(new MultipartFormDataSection(key, formData[key].Item1, formData[key].Item2));
            }
            

            // parse response
            using (UnityWebRequest www = UnityWebRequest.Post($"{Endpoint}/{path}", sections))
            {
                www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json; charset=utf-8");
                //www.SetRequestHeader("Content-Length", bodyRaw.Length.ToString());
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    responseHandler?.Invoke(JsonConvert.DeserializeObject<Tout>(www.downloadHandler.text));

                }
            }

        }

        public IEnumerator PostFileSections<Tout>(List<byte[]> byteImages, int batchSize, string path, Action<Tout> responseHandler)
        {
            //var sections = new List<IMultipartFormSection>();

            //foreach (var byteImage in byteImages)
            //    sections.Add(new MultipartFormDataSection("selected_images", byteImage, "image/png"));
            //sections.Add(new MultipartFormDataSection("batch_size", BitConverter.GetBytes(batchSize), "text"));


            WWWForm form = new WWWForm();
            form.AddField("batch_size", batchSize.ToString());
            //form.AddBinaryData("selected_images", byteImages[0]);
            foreach (var byteImage in byteImages)
                form.AddBinaryData("selected_images", byteImage);

            // parse response
            using (UnityWebRequest www = UnityWebRequest.Post($"{Endpoint}/{path}", form))
            {

                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    responseHandler?.Invoke(JsonConvert.DeserializeObject<Tout>(www.downloadHandler.text));

                }
            }

        }

    }
}
