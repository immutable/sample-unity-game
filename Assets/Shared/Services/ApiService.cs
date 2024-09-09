using System.Collections.Generic;
using System.Net.Http;
using Cysharp.Threading.Tasks;
using HyperCasual.Runner;
using UnityEngine;
using UnityEngine.Networking;

namespace Shared.Services
{
    public static class ApiService
    {
        // static readonly string serverHost = "https://sample-passport-unity-game-api.dev.immutable.com";
        static readonly string serverHost = "http://localhost:6060";
        public static async UniTask<bool> MintCoins(string to, string quantity)
        {
            bool success = false;
            string url = $"{serverHost}/fox/mint/token";
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
                new KeyValuePair<string, string>("to", to),
                // Set 'quanity' to the number of coins collected
                new KeyValuePair<string, string>("quantity", quantity)
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
            string url = $"{serverHost}/mint/fox";
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
                new KeyValuePair<string, string>("to", to)
            };
            using var client = new HttpClient();
            using var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(nvc) };
            using var res = await client.SendAsync(req);
            return res.IsSuccessStatusCode;
#endif
        }
    }
}