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

        public IHttpRequest GetAudioClip(string uri, AudioType audioType = AudioType.UNKNOWN)
        {
            return new UnityHttpRequest(UnityWebRequestMultimedia.GetAudioClip(uri, audioType));
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

            HttpResponse response = new();
            UnityWebRequestAsyncOperation ret = null;
            try
            {
                ret = unityWebRequest.SendWebRequest();
            }
            catch (Exception e)
            {
                response.Error = e.Message;
            }
            yield return ret;

            try
            {
                response = CreateResponse(unityWebRequest);
                if (!response.IsSuccessful)
                {
                    onError?.Invoke(response);
                }
                else if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError)
                {
                    // Delete cache downloading flag file
                    CacheUtils.DeleteCacheDownloadingFlagFile(unityHttpRequest.URL, unityHttpRequest.ignorePatternsForCacheFilePathGeneration);
                    onNetworkError?.Invoke(response);
                }
                else if (unityWebRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    // Delete cache downloading flag file
                    CacheUtils.DeleteCacheDownloadingFlagFile(unityHttpRequest.URL, unityHttpRequest.ignorePatternsForCacheFilePathGeneration);
                    onError?.Invoke(response);
                }
                else
                {
                    // Create cache file
                    CacheUtils.CreateCacheFile(unityHttpRequest.URL, response.Bytes, unityHttpRequest.ignorePatternsForCacheFilePathGeneration);
                    onSuccess?.Invoke(response);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                response.Error = e.Message;
                onError?.Invoke(response);
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
            HttpResponse response = null;
            try
            {
                response = new HttpResponse
                {
                    Url = unityWebRequest.url,
                    Bytes = unityWebRequest.downloadHandler?.data,
                    Text = null,
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

                if ((unityWebRequest.downloadHandler as DownloadHandlerAudioClip)?.GetType()
                    == Type.GetType("UnityEngine.Networking.DownloadHandlerAudioClip, UnityEngine.UnityWebRequestAudioModule"))
                {
                    // AudioClip
                    if (response.AudioClip == null || response.AudioClip.length == 0)
                    {
                        response.IsSuccessful = false;
                        response.Error = "Failed to get AudioClip";
                        Debug.Log("Failed to get AudioClip");
                    }
                }
                else if ((unityWebRequest.downloadHandler as DownloadHandlerTexture)?.GetType()
                    == Type.GetType("UnityEngine.Networking.DownloadHandlerTexture, UnityEngine.UnityWebRequestTextureModule"))
                {
                    // Texture
                    if (response.Texture == null)
                    {
                        response.IsSuccessful = false;
                        response.Error = "Failed to get Texture.";
                        Debug.Log("Failed to get Texture.");
                    }
                }
                else
                {
                    // Text
                    response.Text = unityWebRequest.downloadHandler?.text;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                response = new HttpResponse
                {
                    Url = unityWebRequest.url,
                    IsSuccessful = false,
                    Error = e.Message
                };
            }
            return response;
        }
    }
}