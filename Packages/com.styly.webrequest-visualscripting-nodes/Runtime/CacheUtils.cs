using UnityEngine;
using System.Security.Cryptography;
using System.Text;

namespace STYLY.Http
{
    public enum CacheType
    {
        DoNotUseCache = 0,
        UseCacheAlways = 1,
        UseCacheOnlyWhenOffline = 2
    }

    public class CacheUtils
    {
        private static readonly string cacheDir = Application.persistentDataPath + "/cache/";
        private static readonly string cacheDownloadingExtension = ".downloading";

        public static string GetWebRequestUri(string uri, bool useCacheFlag, bool useCacheOnlyWhenOfflineFlag, string[] ignorePatternsForCacheFilePathGeneration)
        {
            bool shouldUseCache = useCacheFlag && CacheFileExists(uri, ignorePatternsForCacheFilePathGeneration);
            bool shouldUseCacheWhenOffline = useCacheOnlyWhenOfflineFlag && CacheFileExists(uri, ignorePatternsForCacheFilePathGeneration) && !IsOnline();

            if (shouldUseCache || shouldUseCacheWhenOffline)
            {
                string CacheFilePath = GenerateCacheFilePath(uri);
                Debug.Log("Using cache: " + CacheFilePath);
                return CacheFilePath;
            }
            return uri;
        }

        private static string GenerateCacheFilePath(string uri)
        {
            return cacheDir + GenerateCacheFileName(uri);
        }

        private static string GenerateCacheDownloadingFlagFilePath(string uri)
        {
            return cacheDir + GenerateCacheFileName(uri) + cacheDownloadingExtension;
        }

        public static string RemoveIgnorePatternsFromUri(string uri, string[] ignorePatternsForCacheFilePathGeneration)
        {
            if (ignorePatternsForCacheFilePathGeneration != null)
            {
                foreach (string ignorePattern in ignorePatternsForCacheFilePathGeneration)
                {
                    // Remove ignore pattern from uri with regex
                    uri = System.Text.RegularExpressions.Regex.Replace(uri, ignorePattern, "");
                }
            }
            return uri;
        }

        public static bool CacheFileExists(string uri, string[] ignorePatternsForCacheFilePathGeneration)
        {
            uri = RemoveIgnorePatternsFromUri(uri, ignorePatternsForCacheFilePathGeneration);
            return System.IO.File.Exists(GenerateCacheFilePath(uri));
        }

        public static bool IsNowDownloading(string uri, string[] ignorePatternsForCacheFilePathGeneration)
        {
            uri = RemoveIgnorePatternsFromUri(uri, ignorePatternsForCacheFilePathGeneration);
            return System.IO.File.Exists(GenerateCacheDownloadingFlagFilePath(uri));
        }

        public static void CreateCacheFile(string uri, byte[] data, string[] ignorePatternsForCacheFilePathGeneration)
        {
            uri = RemoveIgnorePatternsFromUri(uri, ignorePatternsForCacheFilePathGeneration);
            if (IsNowDownloading(uri, ignorePatternsForCacheFilePathGeneration))
            {
                string CacheFilePath = GenerateCacheFilePath(uri);
                System.IO.File.WriteAllBytes(CacheFilePath, data);
                Debug.Log("Cache file is created: " + CacheFilePath);
                DeleteCacheDownloadingFlagFile(uri, ignorePatternsForCacheFilePathGeneration);
            }
        }

        public static byte[] ReadCacheFile(string uri, string[] ignorePatternsForCacheFilePathGeneration)
        {
            uri = RemoveIgnorePatternsFromUri(uri, ignorePatternsForCacheFilePathGeneration);
            return System.IO.File.ReadAllBytes(GenerateCacheFilePath(uri));
        }

        public static void DeleteCacheFile(string uri, string[] ignorePatternsForCacheFilePathGeneration)
        {
            uri = RemoveIgnorePatternsFromUri(uri, ignorePatternsForCacheFilePathGeneration);
            System.IO.File.Delete(GenerateCacheFilePath(uri));
        }

        public static void CreateCacheDownloadingFlagFile(string uri, string[] ignorePatternsForCacheFilePathGeneration)
        {
            uri = RemoveIgnorePatternsFromUri(uri, ignorePatternsForCacheFilePathGeneration);
            if (!System.IO.Directory.Exists(cacheDir)) System.IO.Directory.CreateDirectory(cacheDir);
            System.IO.File.Create(GenerateCacheDownloadingFlagFilePath(uri)).Dispose();
        }

        public static void DeleteCacheDownloadingFlagFile(string uri, string[] ignorePatternsForCacheFilePathGeneration)
        {
            uri = RemoveIgnorePatternsFromUri(uri, ignorePatternsForCacheFilePathGeneration);
            System.IO.File.Delete(GenerateCacheDownloadingFlagFilePath(uri));
        }

        public static bool IsOnline()
        {
            return !(Application.internetReachability == NetworkReachability.NotReachable);
        }

        /// <summary>
        /// Generate cache file name (with extension) from uri.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private static string GenerateCacheFileName(string uri)
        {
            // SSHA256 hash will be used as cache file name.
            using SHA256 sha256 = SHA256.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(uri);
            byte[] hashBytes = sha256.ComputeHash(inputBytes);
            var sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }
            string fileName = sb.ToString();
            string extension = GetExtensionFromURL(uri);
            if (extension != null)
            {
                fileName += "." + extension;
            }
            return fileName;
        }

        public static string[] GetSignedUrlIgnorePatters()
        {
            string[] patterns = new string[] {
                "(Expires=[^&]+&?|GoogleAccessId=[^&]+&?|Signature=[^&]+&?)", // for Google Cloud Storage
                "(Expires=[^&]*&?|Signature=[^&]*&?|Key-Pair-Id=[^&]*&?)",  // for Amazon CloudFront
                "(se=[^&]*&?|sp=[^&]*&?|sv=[^&]*&?|sr=[^&]*&?|sig=[^&]*&?)"  // for Azure Blob Storage
            };
            return patterns;
        }

        /// <summary>
        /// Returns the extension of the URL or null if the extension cannot be determined.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static string GetExtensionFromURL(string url)
        {
            string extension = null;

            // Remove query string
            int queryIndex = url.IndexOf("?");
            if (queryIndex != -1) { url = url[..queryIndex]; }

            // Get the string after the last dot
            int lastDotIndex = url.LastIndexOf(".");
            if (lastDotIndex != -1) { extension = url[(lastDotIndex + 1)..]; }

            // If the extension is too long, it is not an extension
            if (extension.Length > 5) { return null; }

            // If the extension contains a slash, it is not an extension
            if (extension.Contains("/")) { return null; }

            // If the extension is empty, it is not an extension
            if (extension == "") { return null; }

            return extension;
        }


    }
}
