using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InstagramApiSharp.Helpers
{
    public static class HttpExtensions
    {
        public static Uri AddQueryParameter(this Uri uri, string name, string value)
        {
            if (value == null || value == "" || value == "[]") return uri;
            var httpValueCollection = HttpUtility.ParseQueryString(uri);

            httpValueCollection.Remove(name);
            httpValueCollection.Add(name, value);

            var ub = new UriBuilder(uri);
            var q = "";
            foreach (var item in httpValueCollection)
            {
                if (q == "") q += $"{item.Key}={item.Value}";
                else q += $"&{item.Key}={item.Value}";
            }
            ub.Query = q;
            return ub.Uri;
        }

        public static Uri AddQueryParameterIfNotEmpty(this Uri uri, string name, string value)
        {
            if (value == null || value == "" || value == "[]") return uri;

            var httpValueCollection = HttpUtility.ParseQueryString(uri);
            httpValueCollection.Remove(name);
            httpValueCollection.Add(name, value);
            var ub = new UriBuilder(uri);
            var q = "";
            foreach (var item in httpValueCollection)
            {
                if (q == "") q += $"{item.Key}={item.Value}";
                else q += $"&{item.Key}={item.Value}";
            }
            ub.Query = q;
            return ub.Uri;
        }

        public static async Task<string> ReadAsStringImprovedAsync(this HttpContent content, Encoding encoding)
        {
            var responseStream = await content.ReadAsStreamAsync().ConfigureAwait(false);
            var responseContent = string.Empty;

            if (encoding == null) encoding = content.GetEncoding(Encoding.UTF8);

            using (var sr = new StreamReader(responseStream, encoding))
            {
                responseContent = await sr.ReadToEndAsync().ConfigureAwait(false);
            }

            return responseContent;
        }

        public static Encoding GetEncoding(this HttpContent content, Encoding defaultEncoding)
        {
            var encoding = defaultEncoding;
            try
            {
                var charset = content?.Headers?.ContentType?.CharSet;
                if (!string.IsNullOrEmpty(charset))
                {
                    encoding = Encoding.GetEncoding(charset);
                }
            }
            catch
            {
                encoding = defaultEncoding;
            }

            return encoding;
        }
    }
}