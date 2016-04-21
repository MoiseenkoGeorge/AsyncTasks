using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Task
{
    public static class Tasks
    {
        /// <summary>
        /// Returns the content of required uri's.
        /// Method has to use the synchronous way and can be used to compare the
        ///  performace of sync/async approaches. 
        /// </summary>
        /// <param name="uris">Sequence of required uri</param>
        /// <returns>The sequence of downloaded url content</returns>
        public static IEnumerable<string> GetUrlContent(this IEnumerable<Uri> uris)
        {
            using (var client = new WebClient())
            {
                return uris.Select(uri => client.DownloadString(uri));
            }
        }

        /// <summary>
        /// Returns the content of required uris.
        /// Method has to use the asynchronous way and can be used to compare the performace 
        /// of sync \ async approaches. 
        /// maxConcurrentStreams parameter should control the maximum of concurrent streams 
        /// that are running at the same time (throttling). 
        /// </summary>
        /// <param name="uris">Sequence of required uri</param>
        /// <param name="maxConcurrentStreams">Max count of concurrent request streams</param>
        /// <returns>The sequence of downloaded url content</returns>
        public static IEnumerable<string> GetUrlContentAsync(this IEnumerable<Uri> uris, int maxConcurrentStreams)
        {
            int counter = 0;
            using (var client = new WebClient())
            {
                foreach (var a in uris.Select(uri => client.DownloadStringTaskAsync(uri)))
                {
                    counter++;
                    if (counter >= maxConcurrentStreams)
                    {
                        yield return a.Result;
                        counter--;
                    }
                }

            }
        }

        /// <summary>
        /// Calculates MD5 hash of required resource.
        /// 
        /// Method has to run asynchronous. 
        /// Resource can be any of type: http page, ftp file or local file.
        /// </summary>
        /// <param name="resource">Uri of resource</param>
        /// <returns>MD5 hash</returns>
        public static Task<string> GetMD5Async(this Uri resource)
        {
            Func<string> action = () =>
            {
                var md5 = MD5.Create();
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(resource.OriginalString));
                return hash.Aggregate("", (current, byt) => current + byt.ToString("x2"));
            };
            return System.Threading.Tasks.Task.Run(action);
        }
    }
}
