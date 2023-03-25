using System.Net.Http.Headers;

namespace eStoreWeb.Extension
{
     public static class HttpRequestSupport
    {
        public static string GetQueryPath(Dictionary<string,string> param)
        {
            string path = "?";
            foreach (var item in param)
            {
                path += item.Key;
                path += "=" + item.Value;
                path += "&";                
            }
            path = path.Remove(path.Length - 1);
            return path;
        }
        public static void AddTokenHeader(this HttpClient client, string? token)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
