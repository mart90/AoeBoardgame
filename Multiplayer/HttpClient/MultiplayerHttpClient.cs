using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace AoeBoardgame.Multiplayer
{
    class MultiplayerHttpClient
    {
        const string OUR_VERSION = "0.1";

        public User AuthenticatedUser { get; set; }

        private readonly HttpClient _client;
        private readonly string _baseUrl;

        public MultiplayerHttpClient()
        {
            //_baseUrl = "http://85.146.99.136:7275/";
            _baseUrl = "http://localhost:5000/";

            _client = new HttpClient()
            {
                BaseAddress = new Uri(_baseUrl)
            };
        }

        public bool RunningLatestVersion()
        {
            string serverVersion = Post("latest_version");

            return OUR_VERSION == serverVersion;
        }

        public void RegisterUser(string username, string plainTextPassword)
        {
            string password = HashPassword(plainTextPassword);
            int userId = int.Parse(Post("register_user", new { username, password }));

            AuthenticatedUser = new User
            {
                Id = userId,
                Username = username,
                Password = password,
                LastElo = 1500
            };
        }

        public int? GetUserIdByName(string username)
        {
            string result = Post("get_user_id_by_name", new { username });
            return result != null ? int.Parse(result) : (int?)null;
        }

        public bool Login(string username, string plainTextPassword, string password = null)
        {
            if (password == null)
            {
                password = HashPassword(plainTextPassword);
            }

            string result = Post("login", new { username, password });

            if (result == null)
            {
                return false;
            }

            var dto = JsonConvert.DeserializeObject<UserDto>(result);

            AuthenticatedUser = new User
            {
                Id = dto.Id,
                Username = username,
                Password = password,
                LastElo = dto.LastElo
            };

            return true;
        }

        public void RefreshUser()
        {
            Login(AuthenticatedUser.Username, "", AuthenticatedUser.Password);
        }

        public int CreateLobby(MultiplayerGameSettings settings)
        {
            string result = Post("create_lobby", new
            {
                username = AuthenticatedUser.Username,
                password = AuthenticatedUser.Password,

                timeControlEnabled = settings.TimeControlEnabled,
                startTimeMinutes = settings.StartTimeMinutes,
                timeIncrementSeconds = settings.TimeIncrementSeconds,
                minRating = settings.MinRating,
                maxRating = settings.MaxRating,
                mapSeed = settings.MapSeed
            });

            return int.Parse(result);
        }

        public void CancelLobby(int id)
        {
            Post("cancel_lobby", new
            {
                username = AuthenticatedUser.Username,
                password = AuthenticatedUser.Password,
                id
            });
        }

        public int JoinLobby(Lobby lobby)
        {
            string result = Post("join_lobby", new
            {
                username = AuthenticatedUser.Username,
                password = AuthenticatedUser.Password,
                lobbyId = lobby.Id,
                mapSeed = lobby.Settings.MapSeed
            });

            return int.Parse(result);
        }

        public string GetGeneratedMapSeed(int lobbyId)
        {
            return Post("get_generated_seed", new
            {
                username = AuthenticatedUser.Username,
                password = AuthenticatedUser.Password,
                lobbyId
            });
        }

        public int GetGameId(int lobbyId)
        {
            string result = Post("get_game_id", new
            {
                username = AuthenticatedUser.Username,
                password = AuthenticatedUser.Password,
                lobbyId
            });

            return int.Parse(result);
        }

        public User GetOpponent(int gameId)
        {
            string result = Post("get_opponent", new
            {
                username = AuthenticatedUser.Username,
                password = AuthenticatedUser.Password,
                gameId
            });

            return JsonConvert.DeserializeObject<UserDto>(result).ToUser();
        }

        public void MakeBid(int gameId, int bid)
        {
            Post("make_bid", new
            {
                username = AuthenticatedUser.Username,
                password = AuthenticatedUser.Password,
                gameId,
                bid
            });
        }

        public void SetWhite(int gameId)
        {
            Post("set_white", new
            {
                username = AuthenticatedUser.Username,
                password = AuthenticatedUser.Password,
                gameId
            });
        }

        public void SetTime(bool isWhite, int gameId, int timeMs)
        {
            Post("set_time", new
            {
                username = AuthenticatedUser.Username,
                password = AuthenticatedUser.Password,
                gameId,
                isWhite,
                timeMs
            });
        }

        //public PlayerTimesDto GetTimes(int gameId)
        //{
        //    string result = Post("get_player_times", new
        //    {
        //        username = AuthenticatedUser.Username,
        //        password = AuthenticatedUser.Password,
        //        gameId
        //    });

        //    return JsonConvert.DeserializeObject<PlayerTimesDto>(result);
        //}

        public List<Lobby> GetActiveLobbies()
        {
            string result = Post("get_open_lobbies", new
            {
                username = AuthenticatedUser.Username,
                password = AuthenticatedUser.Password
            });

            if (result == null)
            {
                return new List<Lobby>();
            }

            List<LobbyDto> dtos = JsonConvert.DeserializeObject<List<LobbyDto>>(result);

            return dtos.Select(e => e.ToLobby()).ToList();
        }

        public PollLastMovesDto LatestMoves(int gameId, int minimumMoveNumber)
        {
            string result = Post("poll_last_moves", new
            {
                username = AuthenticatedUser.Username,
                password = AuthenticatedUser.Password,
                gameId,
                minimumMoveNumber
            });

            if (result == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<PollLastMovesDto>(result);
        }

        public void MakeMove(GameMove dto)
        {
            Post("make_move", new
            {
                username = AuthenticatedUser.Username,
                password = AuthenticatedUser.Password,

                gameId = dto.GameId,
                moveNumber = dto.MoveNumber,
                playerId = dto.PlayerId,
                originTileId = dto.OriginTileId,
                destinationTileId = dto.DestinationTileId,
                isMovement = dto.IsMovement,
                isAttack = dto.IsAttack,
                isWaypoint = dto.IsWaypoint,
                isQueueBuilding = dto.IsQueueBuilding,
                isQueueUnit = dto.IsQueueUnit,
                isQueueResearch = dto.IsQueueResearch,
                subselectedUnitHitpoints = dto.SubselectedUnitHitpoints,
                buildingTypeName = dto.BuildingTypeName,
                unitTypeName = dto.UnitTypeName,
                researchId = dto.ResearchId,
                isEndOfTurn = dto.IsEndOfTurn
            });
        }

        public void SetResult(int gameId, string result)
        {
            Post("set_result", new
            {
                username = AuthenticatedUser.Username,
                password = AuthenticatedUser.Password,
                gameId,
                result
            });
        }

        //public LeaderboardDto GetLeaderboard()
        //{
        //    string result = Post("get_leaderboard", new
        //    {
        //        userId = AuthenticatedUser?.Id
        //    });

        //    return JsonConvert.DeserializeObject<LeaderboardDto>(result);
        //}

        private string Post(string url, object convertableObject = null)
        {
            HttpRequestMessage request;
            if (convertableObject != null)
            {
                var jsonString = JsonConvert.SerializeObject(convertableObject);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                request = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
            }
            else
            {
                request = new HttpRequestMessage(HttpMethod.Post, url);
            }

            var response = _client.SendAsync(request).Result;
            var stringResult = response.Content.ReadAsStringAsync().Result;

            return stringResult == "None" ? null : stringResult;
        }

        private string HashPassword(string password)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}

