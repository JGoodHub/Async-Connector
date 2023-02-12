using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace AsyncGameServer
{

    public static class AsyncConnector
    {
        private static ServerConfig _serverConfig;
        private static bool _connectionVerified;
        private static AsyncHttpClient _httpClient;
        private static CorePlayerData _user;

        public static bool ConnectionVerified => _connectionVerified;

        public static AsyncHttpClient HttpClient => _httpClient;

        public static bool LoggedIn => _user != null;

        public static void InitialiseAndLogin()
        {
            _serverConfig = ServerConfig.Instance;

            VerifyServerConnection(verified =>
            {
                _connectionVerified = verified;

                if (verified)
                {
                    _httpClient = new AsyncHttpClient(_serverConfig);

                    Login();
                }
            });
        }

        private static void Login()
        {
            CorePlayerData.GetOrRegisterDeviceUser()
                .Then(user =>
                {
                    _user = user;
                })
                .Catch(error =>
                {
                    Debug.LogError($"Login failed: {error}");
                });
        }

        private static async void VerifyServerConnection(Action<bool> callback = null)
        {
            string statusURL = AsyncHttpClient.ConstructURL(_serverConfig, "server_status");

            using UnityWebRequest request = UnityWebRequest.Get(statusURL);

            AsyncOperation operation = request.SendWebRequest();

            while (operation.isDone == false)
                await Task.Delay(ServerConfig.REQUEST_YIELD_TIME);

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Success: Connection to server verified");
                callback?.Invoke(true);
            }
            else
            {
                Debug.LogError("Failed: Connection to server could not be verified");
                callback?.Invoke(false);
            }
        }
    }

}