using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace AsyncGameServer
{

    public class AsyncHttpClient
    {
        [Serializable]
        public class QueryParam
        {
            public readonly string Key;
            public readonly string Value;

            public QueryParam(string key, object value)
            {
                Key = key;
                Value = value.ToString();
            }
        }

        private ServerConfig _serverConfig;

        public AsyncHttpClient(ServerConfig serverConfig)
        {
            _serverConfig = serverConfig;
        }

        public static string ConstructURL(ServerConfig serverConfig, string endpoint, IReadOnlyList<QueryParam> queryParams = null)
        {
            StringBuilder endpointBuilder = new StringBuilder($"{serverConfig.ServerURL}/{endpoint}");

            if (queryParams == null || queryParams.Count == 0)
                return endpointBuilder.ToString();

            endpointBuilder.Append("?");

            for (int i = 0; i < queryParams.Count; i++)
            {
                endpointBuilder.Append($"{queryParams[i].Key}={queryParams[i].Value}");

                if (i < queryParams.Count - 1)
                    endpointBuilder.Append("&");
            }

            return endpointBuilder.ToString();
        }

        private static void HandleJsonResponse<T>(UnityWebRequest request, Action<T> successCallback, Action<string> errorCallback)
        {
            if (request.responseCode != 200)
            {
                string errorMessage = $"{request.responseCode} Error code received from server for request to {request.url}";
                Debug.LogError(errorMessage);
                errorCallback?.Invoke(errorMessage);
                return;
            }

            string responseJson = request.downloadHandler.text;

            try
            {
                T responseObject = JsonConvert.DeserializeObject<T>(responseJson);
                successCallback.Invoke(responseObject);
            }
            catch (JsonException e)
            {
                string errorMessage = $"Error when attempting to deserialize the response json {responseJson} into the type {typeof(T)}.\n{e}";
                Debug.LogError(errorMessage);
                errorCallback?.Invoke(errorMessage);
            }
        }

        public async void Get<T>(string endpoint, QueryParam[] queryParams, Action<T> successCallback, Action<string> errorCallback = null) where T : class
        {
            string endpointUrl = ConstructURL(_serverConfig, endpoint, queryParams);

            using UnityWebRequest request = UnityWebRequest.Get(endpointUrl);

            AsyncOperation operation = request.SendWebRequest();

            while (operation.isDone == false)
                await Task.Delay(ServerConfig.REQUEST_YIELD_TIME);

            HandleJsonResponse(request, successCallback, errorCallback);
        }

        public async void Put<T>(string endpoint, QueryParam[] queryParams, string bodyJSON, Action<T> successCallback, Action<string> errorCallback = null) where T : class
        {
            string endpointUrl = ConstructURL(_serverConfig, endpoint, queryParams);

            using UnityWebRequest request = UnityWebRequest.Put(endpointUrl, bodyJSON);

            request.SetRequestHeader("Content-Type", "application/json");

            AsyncOperation operation = request.SendWebRequest();

            while (operation.isDone == false)
                await Task.Delay(ServerConfig.REQUEST_YIELD_TIME);

            HandleJsonResponse(request, successCallback, errorCallback);
        }
    }

}