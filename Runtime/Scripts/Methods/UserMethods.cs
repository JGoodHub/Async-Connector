namespace AsyncGameServer
{

    public static class UserMethods
    {
        public static Promise<DeviceUser> CreateGuestUser()
        {
            Promise<DeviceUser> promise = new Promise<DeviceUser>();

            AsyncConnector.HttpClient.Get<DeviceUser>("users/create_user_guest", null, newUser =>
            {
                promise.ResolveHandler(newUser);
            }, (message) =>
            {
                promise.ErrorHandler(message);
            });

            return promise;
        }

        public static Promise<DeviceUser> GetUserByID(int userID)
        {
            Promise<DeviceUser> promise = new Promise<DeviceUser>();

            AsyncConnector.HttpClient.Get<DeviceUser>("users/get_user_by_id",
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

        public static Promise<DeviceUser[]> GetAllUsers(int userID)
        {
            Promise<DeviceUser[]> promise = new Promise<DeviceUser[]>();

            AsyncConnector.HttpClient.Get<DeviceUser[]>("users/get_all_users", null,
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