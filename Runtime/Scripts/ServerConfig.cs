using UnityEngine;

namespace AsyncGameServer
{

    [CreateAssetMenu(fileName = "ServerConfig", menuName = "Async Game Server/New Server Config", order = 0)]
    public class ServerConfig : ScriptableSingleton<ServerConfig>
    {
        public const int REQUEST_YIELD_TIME = 75;

        public string ServerAddress = "http://localhost";
        public string ServerPort = "5000";

        public string ServerURL => $"{ServerAddress}:{ServerPort}";
    }

}