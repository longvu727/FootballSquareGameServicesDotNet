using System.Text.Json;
using System.Text.Json.Serialization;

namespace FootballSquares.ConnectedServices.FootballSquareGameMicroservice {


    public record FootballSquare(
        [property:JsonPropertyName("football_square_game_id")] int footballSquareGameID,
        [property:JsonPropertyName("column_index")] int columnIndex,
        [property:JsonPropertyName("row_index")] int rowIndex,
        [property:JsonPropertyName("winner_quarter_number")] int winnerQuarterNumber,
        [property:JsonPropertyName("winner")] bool winner,
        [property:JsonPropertyName("user_id")] int userID,
        [property:JsonPropertyName("square_id")] int squareID,
        [property:JsonPropertyName("game_id")] int gameID,
        [property:JsonPropertyName("user_name")] string userName,
        [property:JsonPropertyName("user_alias")] string userAlias
    );

    public record GetFootballSquareGameByGameIDRequest([property: JsonPropertyName("game_id")] int gameID);

    public record GetFootballSquareGameByGameIDResponse(
        [property: JsonPropertyName("football_squares")] List<FootballSquare> footballSquares,
        [property: JsonPropertyName("error_message")] string errorMessage
    );

    public class FootballSquareGameMicroservice {
        public GetFootballSquareGameByGameIDResponse getFootballSquareGameByGameID(GetFootballSquareGameByGameIDRequest request){
            Console.WriteLine(JsonSerializer.Serialize(request));
            return new GetFootballSquareGameByGameIDResponse(
                new List<FootballSquare>(){
                    new FootballSquare(0,0,0,0,false,0,0,0,"lvu1",""),
                    new FootballSquare(0,0,0,0,false,0,0,0,"lvu2",""),
                    new FootballSquare(0,0,0,0,false,0,0,0,"lvu3",""),
                },
                ""
            );
        }
    }
}