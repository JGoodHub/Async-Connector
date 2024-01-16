using System;
using System.Threading.Tasks;
using Async.Connector.Models;
using UnityEngine;
using UnityEngine.Networking;

namespace Async.Connector
{

    public static class AsyncConnector
    {

        private static ServerConfig _serverConfig;
        private static AsyncHttpClient _httpClient;
        private static CorePlayerData _user;

        private static bool _connectionVerified;
        private static bool _offlineAllowed;

        public static bool ConnectionVerified => _connectionVerified;

        public static AsyncHttpClient HttpClient => _httpClient;

        public static bool LoggedIn => _user != null;

        public static void InitialiseAndLogin(bool offlineAllowed)
        {
            _serverConfig = ServerConfig.Singleton;

            _offlineAllowed = offlineAllowed;

            VerifyServerConnection(verified =>
            {
                _connectionVerified = verified;

                if (verified == false && _offlineAllowed == false)
                    return;

                _httpClient = new AsyncHttpClient(_serverConfig);

                Login();
            });
        }

        private static void Login()
        {
            if (_connectionVerified == false && _offlineAllowed)
            {
                _user = CorePlayerData.GetLocallyStoredDeviceUser();
                return;
            }

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
