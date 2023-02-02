using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AsyncGameServer
{

    [Serializable]
    public class Room
    {
        public enum RoomStatus
        {
            CREATED,
            WAITING_FOR_OPPONENT,
            ACTIVE,
            PLAYER_ONE_LEFT,
            PLAYER_TWO_LEFT,
            CLOSED,
            EXPIRED
        }

        [JsonProperty("room_id")] public int RoomID;
        [JsonProperty("room_status"), JsonConverter(typeof(StringEnumConverter))] public RoomStatus Status;

        [JsonProperty("primary_user_data")] public RoomUserData PrimaryUserData;
        [JsonProperty("secondary_user_data")] public RoomUserData SecondaryUserData;

        [JsonProperty("commandInvocations")] public Command[] CommandInvocations;

        public bool HasSecondUser => SecondaryUserData != null;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }

    [Serializable]
    public class RoomUserData
    {
        [JsonProperty("user_id")] public int UserID;
        [JsonProperty("display_name")] public string DisplayName;
        [JsonProperty("public_user_state")] public string PublicUserDataJson;
    }

    [Serializable]
    public class Command
    {
        [JsonProperty("sender_user_id")] public int SenderUserID;
        [JsonProperty("command_type")] public string CommandType;
        [JsonProperty("timestamp")] public long Timestamp;
        [JsonProperty("data")] public string Data;

        public DateTime FormattedTimestamp => DateTimeOffset.FromUnixTimeMilliseconds(Timestamp).DateTime;
    }

}