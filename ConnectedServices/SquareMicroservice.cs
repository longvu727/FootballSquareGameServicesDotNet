using System.Text.Json;
using System.Text.Json.Serialization;

namespace FootballSquares.ConnectedServices {
    public record GetSquareBySquareIDRequest([property:JsonPropertyName("square_id")] int squareID);

    public record GetSquareBySquareIDResponse(
        [property: JsonPropertyName("square_id")] int squareID,
        [property: JsonPropertyName("square_guid")] Guid squareGUID,
        [property: JsonPropertyName("square_size")] int squareSize,
        [property: JsonPropertyName("row_points")] string rowPoints,
        [property: JsonPropertyName("column_points")] string columnPoints,
        [property: JsonPropertyName("error_message")] string errorMessage
    );

    public class SquareMicroservice {
        public GetSquareBySquareIDResponse getSquareBySquareID(GetSquareBySquareIDRequest request) {
            Console.WriteLine(JsonSerializer.Serialize(request));
            return new GetSquareBySquareIDResponse(0, Guid.Empty, 0, "", "", "");
        }
    }
}