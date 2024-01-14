﻿using System;
using System.Collections.Generic;

namespace STYLY.Http.Service
{
    public enum CacheType
    {
        DoNotUseCache = 0,
        UseCacheAlways = 1,
        UseCacheOnlyWhenOffline = 2
    }

    public interface IHttpRequest
    {
        IHttpRequest RemoveSuperHeaders();
        IHttpRequest SetHeader(string key, string value);
        IHttpRequest SetHeaders(IEnumerable<KeyValuePair<string, string>> headers);
        IHttpRequest OnUploadProgress(Action<float> onProgress);
        IHttpRequest OnDownloadProgress(Action<float> onProgress);
        IHttpRequest OnSuccess(Action<HttpResponse> onSuccess);
        IHttpRequest OnError(Action<HttpResponse> onError);
        IHttpRequest OnNetworkError(Action<HttpResponse> onNetworkError);
        bool RemoveHeader(string key);
        IHttpRequest SetTimeout(int duration);
        IHttpRequest Send();
        IHttpRequest SetRedirectLimit(int redirectLimit);
        void Abort();
        IHttpRequest UseCache(CacheType cacheType = CacheType.UseCacheAlways, string[] ignorePatternsForCacheFilePathGeneration = null);
    }
}
