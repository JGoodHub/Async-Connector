using Async.Connector.Models;
using Newtonsoft.Json;

namespace Async.Connector.Methods
{

    public static class RoomMethods
    {

        public static Promise<Room> CreateRoom(int userID)
        {
            Promise<Room> promise = new Promise<Room>();

            AsyncConnector.HttpClient.Get<Room>("rooms/create_room",
                new[]
                {
                    new AsyncHttpClient.QueryParam("user_id", userID)
                }, room =>
                {
                    promise.ResolveHandler(room);
                }, message =>
                {
                    promise.ErrorHandler(message);
                });

            return promise;
        }

        public static Promise<Room> JoinRoom(int userID, int roomID)
        {
            Promise<Room> promise = new Promise<Room>();

            AsyncConnector.HttpClient.Get<Room>("rooms/join_room",
                new[]
                {
                    new AsyncHttpClient.QueryParam("user_id", userID),
                    new AsyncHttpClient.QueryParam("room_id", roomID)
                }, room =>
                {
                    promise.ResolveHandler(room);
                }, message =>
                {
                    promise.ErrorHandler(message);
                });

            return promise;
        }

        public static Promise<Room> CreateOrJoinRoom(int userID)
        {
            Promise<Room> promise = new Promise<Room>();

            AsyncConnector.HttpClient.Get<Room>("rooms/create_or_join_room",
                new[]
                {
                    new AsyncHttpClient.QueryParam("user_id", userID)
                }, room =>
                {
                    promise.ResolveHandler(room);
                }, message =>
                {
                    promise.ErrorHandler(message);
                });

            return promise;
        }

        public static Promise<Room> LeaveRoom(int userID, int roomID)
        {
            Promise<Room> promise = new Promise<Room>();

            AsyncConnector.HttpClient.Get<Room>("rooms/leave_room",
                new[]
                {
                    new AsyncHttpClient.QueryParam("user_id", userID),
                    new AsyncHttpClient.QueryParam("room_id", roomID)
                }, room =>
                {
                    promise.ResolveHandler(room);
                }, message =>
                {
                    promise.ErrorHandler(message);
                });

            return promise;
        }

        public static Promise<Room> GetRoomByID(int roomID)
        {
            Promise<Room> promise = new Promise<Room>();

            AsyncConnector.HttpClient.Get<Room>("rooms/get_room_by_id",
                new[]
                {
                    new AsyncHttpClient.QueryParam("room_id", roomID)
                }, room =>
                {
                    promise.ResolveHandler(room);
                }, message =>
                {
                    promise.ErrorHandler(message);
                });

            return promise;
        }

        public static Promise<Room[]> GetAllRooms()
        {
            Promise<Room[]> promise = new Promise<Room[]>();

            AsyncConnector.HttpClient.Get<Room[]>("rooms/get_all_rooms", null,
                allRooms =>
                {
                    promise.ResolveHandler(allRooms);
                }, message =>
                {
                    promise.ErrorHandler(message);
                });

            return promise;
        }

        public static Promise<Room[]> GetRoomsForUserWithID(int userID)
        {
            Promise<Room[]> promise = new Promise<Room[]>();

            AsyncConnector.HttpClient.Get<Room[]>("rooms/get_rooms_for_user_with_id",
                new[]
                {
                    new AsyncHttpClient.QueryParam("user_id", userID)
                }, usersRooms =>
                {
                    promise.ResolveHandler(usersRooms);
                }, message =>
                {
                    promise.ErrorHandler(message);
                });

            return promise;
        }

        public static Promise<Room[]> GetRoomsWithStatus(Room.RoomStatus status)
        {
            Promise<Room[]> promise = new Promise<Room[]>();

            AsyncConnector.HttpClient.Get<Room[]>("rooms/get_rooms_with_status",
                new[]
                {
                    new AsyncHttpClient.QueryParam("room_status", status.ToString())
                }, usersRooms =>
                {
                    promise.ResolveHandler(usersRooms);
                }, message =>
                {
                    promise.ErrorHandler(message);
                });

            return promise;
        }

        public static Promise<Room> SendCommandToRoom(int roomID, Command command)
        {
            Promise<Room> promise = new Promise<Room>();

            string commandJson = JsonConvert.SerializeObject(command, Formatting.Indented);

            AsyncConnector.HttpClient.Put<Room>("rooms/send_command_to_room",
                new[]
                {
                    new AsyncHttpClient.QueryParam("room_id", roomID)
                }, commandJson,
                usersRooms =>
                {
                    promise.ResolveHandler(usersRooms);
                }, message =>
                {
                    promise.ErrorHandler(message);
                });

            return promise;
        }

    }

}
