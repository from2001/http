using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cysharp.Threading.Tasks;
using Adrenak.Http.Service;

namespace Adrenak.Http {
    /// <summary>
    /// A utility class for http.
    /// <see cref="https://developer.mozilla.org/en-US/docs/Web/HTTP/Status"/>
    /// </summary>
    public static class HttpUtils {
        /// <summary>
        /// Format and append parameters to a uri
        /// </summary>
        /// <param name="uri">The uri to append the properties to.</param>
        /// <param name="parameters">A dictionary of parameters to append to the uri.</param>
        /// <returns>The uri with the appended parameters.</returns>
        public static string ConstructUriWithParameters(string uri, Dictionary<string, string> parameters) {
            if (parameters == null || parameters.Count == 0) {
                return uri;
            }

            var stringBuilder = new StringBuilder(uri);

            for (var i = 0; i < parameters.Count; i++) {
                var element = parameters.ElementAt(i);
                stringBuilder.Append(i == 0 ? "?" : "&");
                stringBuilder.Append(element.Key);
                stringBuilder.Append("=");
                stringBuilder.Append(element.Value);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Sends the request using UniTask. NOTE: request.OnSuccess, OnError, OnNetworkError events get overriden by this.
        /// If using this method, use try/await/catch to handle success and errors.
        /// </summary>
        /// <returns>
        /// The <see cref="HttpResponse"/> object in a UniTask
        /// </returns>
        public static UniTask<HttpResponse> SendAsync(this IHttpRequest request) {
            var source = new UniTaskCompletionSource<HttpResponse>();
            request.OnSuccess(x => source.TrySetResult(x))
            .OnError(x => source.TrySetException(new Exception(x.Error)))
            .OnNetworkError(x => source.TrySetException(new Exception(x.Error)))
            .Send();
            return source.Task;
        }
    }
}
