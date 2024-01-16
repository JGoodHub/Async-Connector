using System;
using Async.Connector.Methods;
using Newtonsoft.Json;
using UnityEngine;

namespace Async.Connector.Models
{

    [Serializable]
    [CreateAssetMenu(fileName = "CorePlayerData", menuName = "Async Game Server/New Core Player Data", order = 0)]
    public class CorePlayerData : ScriptableSingleton<CorePlayerData>
    {

        [JsonProperty("user_id")] public int UserID;
        [JsonProperty("user_name")] public string UserName;
        [JsonProperty("user_tag")] public int UserTag;
        [JsonProperty("private_user_state")] public string PrivateUserDataJson;
        [JsonProperty("public_user_state")] public string PublicUserDataJson;

        public string DisplayName => $"{UserName}#{UserTag}";

        public static Promise<CorePlayerData> GetOrRegisterDeviceUser()
        {
            Promise<CorePlayerData> userPromise = new Promise<CorePlayerData>();

            CorePlayerData user = GetLocallyStoredDeviceUser();

            if (user == null)
            {
                RegisterNewDeviceUser()
                    .Then(newUser =>
                    {
                        userPromise.ResolveHandler(newUser);
                    });
            }
            else
            {
                UserMethods.GetUserByID(user.UserID)
                    .Then(dbUser =>
                    {
                        userPromise.ResolveHandler(dbUser);
                    })
                    .Catch(error =>
                    {
                        if (error.StartsWith("404") == false)
                            return;

                        Debug.LogError("Local user data found but no matching user was not found on server. Registering as new user.");

                        RegisterNewDeviceUser()
                            .Then(newUser =>
                            {
                                userPromise.ResolveHandler(newUser);
                            });
                    });
            }

            return userPromise;
        }

        public void CopyTo(CorePlayerData otherUser)
        {
            otherUser.UserID = UserID;
            otherUser.UserName = UserName;
            otherUser.UserTag = UserTag;
            otherUser.PrivateUserDataJson = PrivateUserDataJson;
            otherUser.PublicUserDataJson = PublicUserDataJson;
        }

        public static CorePlayerData GetLocallyStoredDeviceUser()
        {
            return Singleton;
            
            if (PlayerPrefs.HasKey("device_user") == false)
                return null;

            string existingUserJson = PlayerPrefs.GetString("device_user");

            try
            {
                JsonConvert.PopulateObject(existingUserJson, Singleton);

                Debug.Log($"Loaded user for this device {Singleton.DisplayName}");

                return Singleton;
            }
            catch (JsonException exception)
            {
                Debug.LogError(exception);
                Debug.LogError($"[DeviceUser]: Error while deserializing user json from player prefs {existingUserJson}");
                return null;
            }
        }

        private static Promise<CorePlayerData> RegisterNewDeviceUser()
        {
            Promise<CorePlayerData> promise = new Promise<CorePlayerData>();

            UserMethods.CreateGuestUser()
                .Then(newUser =>
                {
                    string newUserJson = JsonConvert.SerializeObject(newUser);

                    PlayerPrefs.SetString("device_user", newUserJson);
                    PlayerPrefs.Save();

                    JsonConvert.PopulateObject(newUserJson, Singleton);

                    Debug.Log($"Registered new user for this device {newUser.DisplayName}");

                    promise.ResolveHandler(newUser);
                })
                .Catch(message =>
                {
                    promise.ErrorHandler(message);
                });

            return promise;
        }

        [ContextMenu("Save To Player Prefs")]
        public void SaveToPlayerPrefs()
        {
            string newUserJson = JsonConvert.SerializeObject(this);

            PlayerPrefs.SetString("device_user", newUserJson);
            PlayerPrefs.Save();
        }

    }

}
