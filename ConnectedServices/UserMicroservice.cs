using System.Text.Json;
using System.Text.Json.Serialization;

namespace FootballSquares.ConnectedServices {
    public record GetUserByUserIDRequest([property:JsonPropertyName("square_id")] int squareID);

    public record GetUserByUserIDResponse(
        [property: JsonPropertyName("user_id")] int userID,
        [property: JsonPropertyName("user_guid")] string userGUID,
        [property: JsonPropertyName("ip")] string ip,
        [property: JsonPropertyName("device_name")] string deviceName,
        [property: JsonPropertyName("user_name")] string userName,
        [property: JsonPropertyName("alias")] string alias,
        [property: JsonPropertyName("error_message")] string errorMessage
    );

    public class UserMicroservice {
        public GetUserByUserIDResponse getUserByUserID(GetUserByUserIDRequest request) {
            Console.WriteLine(JsonSerializer.Serialize(request));
            return new GetUserByUserIDResponse(0, "", "", "", "long", "lvu", "");
        }
    }
}