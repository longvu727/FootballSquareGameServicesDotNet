using System.Text.Json;
using FootballSquares.ConnectedServices;
using FootballSquares.ConnectedServices.FootballSquareGameMicroservice;
using FootballSquares.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FootballSquares.Controllers{
    
    [ApiController]
    [Route("")]
    public class GameController: ControllerBase {
        public static readonly string sport = "football";
        public static readonly string jsonResponse =
            """
            {
                "game_guid": "b5502c1f-86d6-4dcf-92ea-1ad974f882aa",
                "sport": "football",
                "team_a": "Long",
                "team_b": "Vu",
                "square_size": 10,
                "row_points": "",
                "column_points": "",
                "football_squares": [
                    {
                        "column_index": 1,
                        "row_index": 1,
                        "winner_quarter_number": 0,
                        "winner": false,
                        "user_guid": "9f2956f1-9705-47a0-ae31-ea7b21d08b55",
                        "user_alias": "avu",
                        "user_name": "AnhVu"
                    },
                    {
                        "column_index": 2,
                        "row_index": 1,
                        "winner_quarter_number": 0,
                        "winner": false,
                        "user_guid": "9f2956f1-9705-47a0-ae31-ea7b21d08b55",
                        "user_alias": "avu",
                        "user_name": "AnhVu"
                    },
                    {
                        "column_index": 3,
                        "row_index": 1,
                        "winner_quarter_number": 0,
                        "winner": false,
                        "user_guid": "9f2956f1-9705-47a0-ae31-ea7b21d08b55",
                        "user_alias": "avu",
                        "user_name": "AnhVu"
                    }
                ],
                "error_message": ""
            }
            """;
        public static readonly GameResponse? gameDto = JsonSerializer.Deserialize<GameResponse>(jsonResponse);

        private readonly IConfiguration _appConfig;

        public GameController(IConfiguration appConfig) {
            _appConfig = appConfig;
        }

        [HttpGet("/GetGame/{guid}")]
        public async Task<GameResponse> GetGameByGUID(Guid guid) {
            GameMicroservice gameMicroservice = new GameMicroservice(this._appConfig);
            GetGameByGUIDResponse getGameByIDResponse =  await gameMicroservice.getGameByGUIDAsync(new GetGameByGUIDRequest(guid));

            FootballSquareGameMicroservice footballSquareGameMicroservice = new FootballSquareGameMicroservice();
            GetFootballSquareGameByGameIDResponse getFootballSquareGameByGameIDResponse =
                footballSquareGameMicroservice.getFootballSquareGameByGameID(
                    new GetFootballSquareGameByGameIDRequest(getGameByIDResponse.gameId)
                );
            
            SquareMicroservice squareMicroservice = new SquareMicroservice();
            GetSquareBySquareIDResponse getSquareBySquareIDResponse = squareMicroservice.getSquareBySquareID(new GetSquareBySquareIDRequest(
                getFootballSquareGameByGameIDResponse.footballSquares[0].squareID
            ));

            UserMicroservice userMicroservice = new UserMicroservice();
            List<SquareRow> squareRows = new List<SquareRow>();

            foreach (FootballSquare footballSquare in getFootballSquareGameByGameIDResponse.footballSquares){
                GetUserByUserIDResponse getUserByUserIDResponse = userMicroservice.getUserByUserID(
                    new GetUserByUserIDRequest(footballSquare.squareID)
                );
                squareRows.Add(new SquareRow(
                    footballSquare.columnIndex,
                    footballSquare.rowIndex,
                    footballSquare.winnerQuarterNumber,
                    footballSquare.winner,
                    getUserByUserIDResponse.userGUID,
                    getUserByUserIDResponse.alias,
                    getUserByUserIDResponse.userName
                ));
            }

            return new GameResponse(
                getGameByIDResponse.gameGUID,
                getGameByIDResponse.sport,
                getGameByIDResponse.teamA,
                getGameByIDResponse.teamB,

                getSquareBySquareIDResponse.squareSize,
                getSquareBySquareIDResponse.rowPoints,
                getSquareBySquareIDResponse.columnPoints,

                squareRows,

                ""
            );
        }

        [HttpPost("/CreateGame")]
        public CreateGameResponse CreateGame(CreateGameRequest createGameRequest) {
            Console.WriteLine(JsonSerializer.Serialize(createGameRequest));
            return new CreateGameResponse(Guid.NewGuid(), sport);
        }

        [HttpPost("/Reserve")]
        public ReserveGameResponse ReserveGame(ReserveGameRequest reserveGameRequest){
            Console.WriteLine(JsonSerializer.Serialize(reserveGameRequest));
            return new ReserveGameResponse(true, "");
        }
    }
}