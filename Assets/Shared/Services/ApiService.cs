using System.Collections.Generic;
using System.Net.Http;
using Cysharp.Threading.Tasks;
using HyperCasual.Runner;
using UnityEngine;

namespace Shared.Services
{
    public static class ApiService
    {
        public static async UniTask<bool> MintCoins(string to, string quantity)
        {
            var success = false;
            var url = $"{Config.SERVER_URL}/mint/token";
            Debug.Log($"MintCoins url: {url}");
#if UNITY_WEBGL
            var form = new WWWForm();
            form.AddField("to", to);
            form.AddField("quantity", quantity);

            UnityWebRequest request = UnityWebRequest.Post(url, form);

            await request.SendWebRequest().ToUniTask();
            success = request.result == UnityWebRequest.Result.Success &&
                                request.responseCode >= 200 && request.responseCode < 300;
#else
            var nvc = new List<KeyValuePair<string, string>>
            {
                // Set 'to' to the player's wallet address
                new("to", to),
                // Set 'quanity' to the number of coins collected
                new("quantity", quantity)
            };
            using var client = new HttpClient();
            using var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(nvc) };
            using var res = await client.SendAsync(req);
            success = res.IsSuccessStatusCode;
#endif
            return success;
        }

        public static async UniTask<bool> MintFox(string to)
        {
            var url = $"{Config.SERVER_URL}/mint/fox";
            Debug.Log($"MintFox url: {url}");
#if UNITY_WEBGL
            var form = new WWWForm();
            form.AddField("to", to);

            UnityWebRequest request = UnityWebRequest.Post(url, form);

            await request.SendWebRequest().ToUniTask();
            return request.result == UnityWebRequest.Result.Success &&
                                request.responseCode >= 200 && request.responseCode < 300;
#else
            var nvc = new List<KeyValuePair<string, string>>
            {
                // Set 'to' to the player's wallet address
                new("to", to)
            };
            using var client = new HttpClient();
            using var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(nvc) };
            using var res = await client.SendAsync(req);
            return res.IsSuccessStatusCode;
#endif
        }
    }
}