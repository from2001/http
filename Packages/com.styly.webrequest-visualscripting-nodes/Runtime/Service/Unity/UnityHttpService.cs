using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityEngine;
using UnityEngine.Networking;

namespace STYLY.Http.Service.Unity
{
    public class UnityHttpService : IHttpService
    {
        public IHttpRequest Get(string uri)
        {
            return new UnityHttpRequest(UnityWebRequest.Get(uri));
        }

        public IHttpRequest GetTexture(string uri)
        {
            return new UnityHttpRequest(UnityWebRequestTexture.GetTexture(uri));
        }

        public IHttpRequest GetAudioClip(string uri)
        {
            return new UnityHttpRequest(UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.UNKNOWN));
        }

        public IHttpRequest Post(string uri, string postData)
        {
            return new UnityHttpRequest(UnityWebRequest.PostWwwForm(uri, postData));
        }

        public IHttpRequest Post(string uri, WWWForm formData)
        {
            return new UnityHttpRequest(UnityWebRequest.Post(uri, formData));
        }

        public IHttpRequest Post(string uri, Dictionary<string, string> formData)
        {
            return new UnityHttpRequest(UnityWebRequest.Post(uri, formData));
        }

        public IHttpRequest Post(string uri, List<IMultipartFormSection> multipartForm)
        {
            return new UnityHttpRequest(UnityWebRequest.Post(uri, multipartForm));
        }

        public IHttpRequest Post(string uri, byte[] bytes, string contentType)
        {
            var unityWebRequest = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbPOST)
            {
                uploadHandler = new UploadHandlerRaw(bytes)
                {
                    contentType = contentType
                },
                downloadHandler = new DownloadHandlerBuffer()
            };
            return new UnityHttpRequest(unityWebRequest);
        }

        public IHttpRequest PostJson(string uri, string json)
        {
            return Post(uri, Encoding.UTF8.GetBytes(json), "application/json");
        }

        public IHttpRequest PostJson<T>(string uri, T payload) where T : class
        {
            return PostJson(uri, JsonUtility.ToJson(payload));
        }

        public IHttpRequest Put(string uri, byte[] bodyData)
        {
            return new UnityHttpRequest(UnityWebRequest.Put(uri, bodyData));
        }

        public IHttpRequest Put(string uri, string bodyData)
        {
            return new UnityHttpRequest(UnityWebRequest.Put(uri, bodyData));
        }

        public IHttpRequest Delete(string uri)
        {
            return new UnityHttpRequest(UnityWebRequest.Delete(uri));
        }

        public IHttpRequest Head(string uri)
        {
            return new UnityHttpRequest(UnityWebRequest.Head(uri));
        }

        public IEnumerator Send(IHttpRequest request, Action<HttpResponse> onSuccess = null,
            Action<HttpResponse> onError = null, Action<HttpResponse> onNetworkError = null)
        {
            var unityHttpRequest = (UnityHttpRequest)request;
            var unityWebRequest = unityHttpRequest.UnityWebRequest;

            yield return unityWebRequest.SendWebRequest();

            var response = CreateResponse(unityWebRequest);

            if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                // Delete cache downloading flag file
                CacheUtils.DeleteCacheDownloadingFlagFile(response.Url, unityHttpRequest.ignorePatternsForCacheFilePathGeneration);

                onNetworkError?.Invoke(response);
            }
            else if (unityWebRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                // Delete cache downloading flag file
                CacheUtils.DeleteCacheDownloadingFlagFile(response.Url, unityHttpRequest.ignorePatternsForCacheFilePathGeneration);

                onError?.Invoke(response);
            }
            else
            {
                // Create cache file
                CacheUtils.CreateCacheFile(response.Url, response.Bytes, unityHttpRequest.ignorePatternsForCacheFilePathGeneration);

                onSuccess?.Invoke(response);
            }
        }

        public void Abort(IHttpRequest request)
        {
            var unityHttpRequest = request as UnityHttpRequest;
            if (unityHttpRequest?.UnityWebRequest != null && !unityHttpRequest.UnityWebRequest.isDone)
            {
                unityHttpRequest.UnityWebRequest.Abort();
            }
        }

        private static HttpResponse CreateResponse(UnityWebRequest unityWebRequest)
        {
            return new HttpResponse
            {
                Url = unityWebRequest.url,
                Bytes = unityWebRequest.downloadHandler?.data,
                Text = (unityWebRequest.downloadHandler as DownloadHandlerAudioClip)?.audioClip ? null : unityWebRequest.downloadHandler?.text,
                IsSuccessful = unityWebRequest.result != UnityWebRequest.Result.ProtocolError
                            && unityWebRequest.result != UnityWebRequest.Result.ConnectionError,
                IsHttpError = unityWebRequest.result == UnityWebRequest.Result.ProtocolError,
                IsNetworkError = unityWebRequest.result == UnityWebRequest.Result.ConnectionError,
                Error = unityWebRequest.error,
                StatusCode = unityWebRequest.responseCode,
                ResponseHeaders = unityWebRequest.GetResponseHeaders(),
                Texture = (unityWebRequest.downloadHandler as DownloadHandlerTexture)?.texture,
                AudioClip = (unityWebRequest.downloadHandler as DownloadHandlerAudioClip)?.audioClip,
            };
        }

    }
}
