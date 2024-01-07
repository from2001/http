using UnityEngine;
using System.Security.Cryptography;
using System.Text;
using System.Net.NetworkInformation;

namespace STYLY.Http
{
    public class CacheUtils
    {
        private static readonly string cacheDir = Application.persistentDataPath + "/cache/";
        private static readonly string cacheDownloadingExtension = ".downloading";

        public static string GetWebRequestUri(string uri, bool useCacheFlag, bool useCacheOnlyWhenOfflineFlag)
        {
            bool shouldUseCache = useCacheFlag && CacheFileExists(uri);
            bool shouldUseCacheWhenOffline = useCacheOnlyWhenOfflineFlag && CacheFileExists(uri) && !IsOnline();

            if (shouldUseCache || shouldUseCacheWhenOffline)
            {
                return GenerateCacheFilePath(uri);
            }
            return uri;
        }

        private static string GenerateCacheFilePath(string uri)
        {
            return cacheDir + GenerateChacheFileName(uri);
        }

        private static string GenerateCacheDownloadingFlagFilePath(string uri)
        {
            return cacheDir + GenerateChacheFileName(uri) + cacheDownloadingExtension;
        }

        public static bool CacheFileExists(string uri)
        {
            return System.IO.File.Exists(GenerateCacheFilePath(uri));
        }

        public static bool IsNowDownloading(string uri)
        {
            return System.IO.File.Exists(GenerateCacheDownloadingFlagFilePath(uri));
        }

        public static void CreateCacheFile(string uri, byte[] data)
        {
            if (IsNowDownloading(uri))
            {
                System.IO.File.WriteAllBytes(GenerateCacheFilePath(uri), data);
                DeleteCacheDownloadingFlagFile(uri);
            }
        }

        public static byte[] ReadCacheFile(string uri)
        {
            return System.IO.File.ReadAllBytes(GenerateCacheFilePath(uri));
        }

        public static void DeleteCacheFile(string uri)
        {
            System.IO.File.Delete(GenerateCacheFilePath(uri));
        }

        public static void CreateCacheDownloadingFlagFile(string uri)
        {
            if (!System.IO.Directory.Exists(cacheDir)) System.IO.Directory.CreateDirectory(cacheDir);
            System.IO.File.Create(GenerateCacheDownloadingFlagFilePath(uri)).Dispose();
        }

        public static void DeleteCacheDownloadingFlagFile(string uri)
        {
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
        private static string GenerateChacheFileName(string uri)
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

    }
}
