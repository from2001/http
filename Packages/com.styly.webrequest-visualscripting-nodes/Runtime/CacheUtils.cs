using UnityEngine;
using System.Security.Cryptography;
using System.Text;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using System;

// ToDo
// ・isDownloadingファイルが存在する間は待つ処理を入れる
// ・エラーが起きた場合にもisDownloadingのクリーンアップ
// ・isDownloadingのクリーンアップ関数作成
// ・キャッシュファイルのクリーンアップ関数作成
// ・エラーハンドリング確認(ネットワークエラーなど)
// ・DoNotUseCacheのときにキャッシュを作らないように - [済]

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
                return GenerateCacheFilePath(uri);
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
                System.IO.File.WriteAllBytes(GenerateCacheFilePath(uri), data);
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
            return NetworkInterface.GetIsNetworkAvailable();
        }

        /// <summary>
        /// Generate cache file name from uri.
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
            return sb.ToString();
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

    }
}
