using System.Text.Json;
using FootballSquares.ConnectedServices;
using FootballSquares.ConnectedServices.FootballSquareGameMicroservice;
using FootballSquares.ConnectedServices.SquareMicroservice;
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
        public async Task<GameResponse> GetGameByGUIDAsync(Guid guid) {
            GameResponse gameResponse = new GameResponse();

            GetGameByGUIDResponse getGameByIDResponse =  await new GameMicroservice(this._appConfig)
                .getGameByGUIDAsync(new GetGameByGUIDRequest(guid));

            if (getGameByIDResponse.errorMessage != "") {
                gameResponse.errorMessage = getGameByIDResponse.errorMessage;
                return gameResponse;
            }

            gameResponse.gameGUID = getGameByIDResponse.gameGUID;
            gameResponse.sport = getGameByIDResponse.sport;
            gameResponse.teamA = getGameByIDResponse.teamA;
            gameResponse.teamB = getGameByIDResponse.teamB;


            GetFootballSquareGameByGameIDResponse getFootballSquareGameByGameIDResponse =
                await new FootballSquareGameMicroservice(this._appConfig).getFootballSquareGameByGameIDAsync(
                    new GetFootballSquareGameByGameIDRequest(getGameByIDResponse.gameID)
                );
            
            if (getFootballSquareGameByGameIDResponse.errorMessage != ""
                || (getFootballSquareGameByGameIDResponse.footballSquares != null
                    && getFootballSquareGameByGameIDResponse.footballSquares.Count < 1)) {

                gameResponse.errorMessage = getFootballSquareGameByGameIDResponse.errorMessage;
                return gameResponse;
            }

            var footballSquares = getFootballSquareGameByGameIDResponse.footballSquares 
                ?? new List<FootballSquare>(){new FootballSquare(0, 0, 0, 0, false, 0, 0, 0, "", "")};


            GetSquareBySquareIDResponse getSquareBySquareIDResponse = await new SquareMicroservice(this._appConfig)
                .getSquareBySquareIDAsync(new GetSquareBySquareIDRequest(footballSquares[0].squareID));

            if (getSquareBySquareIDResponse.errorMessage != "") {
                gameResponse.errorMessage = getSquareBySquareIDResponse.errorMessage;
                return gameResponse;
            }

            gameResponse.squareSize = getSquareBySquareIDResponse.squareSize;
            gameResponse.rowPoints = getSquareBySquareIDResponse.rowPoints;
            gameResponse.columnPoints = getSquareBySquareIDResponse.columnPoints;


            UserMicroservice userMicroservice = new(this._appConfig);
            List<SquareRow> squareRows = [];

            foreach (FootballSquare footballSquare in footballSquares){
                if (footballSquare.userID == 0) {
                    continue;
                }

                GetUserByUserIDResponse getUserByUserIDResponse = await userMicroservice.getUserByUserIDAsync(
                    new GetUserByUserIDRequest(footballSquare.userID)
                );

                if (getUserByUserIDResponse.errorMessage != "") {
                    gameResponse.errorMessage = getUserByUserIDResponse.errorMessage;
                    return gameResponse;
                }

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

            gameResponse.squareData = squareRows;

            return gameResponse;
        }

        [HttpPost("/CreateGame")]
        public async Task<Dtos.CreateGameResponse> CreateGameAsync(Dtos.CreateGameRequest createGameRequest) {
            Console.WriteLine(JsonSerializer.Serialize(createGameRequest));
            var createGameResponse = new Dtos.CreateGameResponse();

            ConnectedServices.CreateGameResponse createGameMicroserviceResponse = await
                new GameMicroservice(this._appConfig).createGameAsync(
                    new ConnectedServices.CreateGameRequest(createGameRequest.Sport, createGameRequest.TeamA, createGameRequest.TeamB)
                );
            
            if (createGameMicroserviceResponse.errorMessage != "") {
                createGameResponse.errorMessage = createGameMicroserviceResponse.errorMessage;
                return createGameResponse;
            }

            CreateSquareResponse createSquareResponse = await
                new SquareMicroservice(this._appConfig).createSquareAsync(
                    new CreateSquareRequest(createGameRequest.SquareSize)
                );

            if (createSquareResponse.errorMessage != "") {
                createGameResponse.errorMessage = createSquareResponse.errorMessage;
                return createGameResponse;
            }

            CreateFootballSquareGameResponse createFootballSquareGameResponse = await 
                new FootballSquareGameMicroservice(this._appConfig).createFootballSquareGame(
                    new CreateFootballSquareGameRequest(
                        createSquareResponse.squareID,
                        createGameMicroserviceResponse.gameID,
                        createGameRequest.SquareSize
                    )
                );
            
            if (createFootballSquareGameResponse.errorMessage != "") {
                createGameResponse.errorMessage = createFootballSquareGameResponse.errorMessage;
                return createGameResponse;
            }

            createGameResponse.gameGUID = createGameMicroserviceResponse.gameGUID;
            createGameResponse.sport = createGameRequest.Sport;

            return createGameResponse;
        }

        [HttpPost("/ReserveSquares")]
        public async Task<ReserveGameResponse> ReserveSquaresAsync(ReserveGameRequest reserveGameRequest){
            Console.WriteLine(JsonSerializer.Serialize(reserveGameRequest));
            ReserveGameResponse reserveGameResponse = new ReserveGameResponse();

            GetGameByGUIDResponse getGameByIDResponse =  await new GameMicroservice(this._appConfig)
                .getGameByGUIDAsync(new GetGameByGUIDRequest(reserveGameRequest.GameGUID));

            if (getGameByIDResponse.errorMessage != "") {
                reserveGameResponse.errorMessage = getGameByIDResponse.errorMessage;
                return reserveGameResponse;
            }

            GetUserByUserGUIDResponse getUserByUserGUIDResponse = await new UserMicroservice(this._appConfig)
                .getUserByUserGUIDAsync(new GetUserByUserGUIDRequest(reserveGameRequest.UserGUID));

            if (getUserByUserGUIDResponse.errorMessage != "") {
                reserveGameResponse.errorMessage = getUserByUserGUIDResponse.errorMessage;
                return reserveGameResponse;
            }

            ReserveFootballSquareResponse reserveFootballSquareResponse = await new FootballSquareGameMicroservice(this._appConfig)
                .reserveFootballSquare(new ReserveFootballSquareRequest(
                    getUserByUserGUIDResponse.userID,
                    getGameByIDResponse.gameID,
                    reserveGameRequest.ColumnIndex,
                    reserveGameRequest.RowIndex
                ));
            
            if (reserveFootballSquareResponse.errorMessage != "") {
                reserveGameResponse.errorMessage = reserveFootballSquareResponse.errorMessage;
                return reserveGameResponse;
            }

            reserveGameResponse.reserved = true;
            return reserveGameResponse;
        }
    }
}