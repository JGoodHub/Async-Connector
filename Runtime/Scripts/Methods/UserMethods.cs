using Async.Connector.Models;

namespace Async.Connector.Methods
{

    public static class UserMethods
    {
        public static Promise<CorePlayerData> CreateGuestUser()
        {
            Promise<CorePlayerData> promise = new Promise<CorePlayerData>();

            AsyncConnector.HttpClient.Get<CorePlayerData>("users/create_user_guest", null, newUser =>
            {
                promise.ResolveHandler(newUser);
            }, (message) =>
            {
                promise.ErrorHandler(message);
            });

            return promise;
        }

        public static Promise<CorePlayerData> GetUserByID(int userID)
        {
            Promise<CorePlayerData> promise = new Promise<CorePlayerData>();

            AsyncConnector.HttpClient.Get<CorePlayerData>("users/get_user_by_id",
                new[]
                {
                    new AsyncHttpClient.QueryParam("user_id", userID)
                }, newUser =>
                {
                    promise.ResolveHandler(newUser);
                }, (message) =>
                {
                    promise.ErrorHandler(message);
                });

            return promise;
        }

        public static Promise<CorePlayerData[]> GetAllUsers(int userID)
        {
            Promise<CorePlayerData[]> promise = new Promise<CorePlayerData[]>();

            AsyncConnector.HttpClient.Get<CorePlayerData[]>("users/get_all_users", null,
                users =>
                {
                    promise.ResolveHandler(users);
                }, (message) =>
                {
                    promise.ErrorHandler(message);
                });

            return promise;
        }
    }

}