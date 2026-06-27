using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using gamingCloud.templates;
using Newtonsoft.Json.Linq;

namespace gamingCloud
{

    public class Players : HttpRequest
    {
        public static bool IsLogin
        {
            get { return PlayerPrefs.HasKey("gc-prefs-token"); }
            set { return; }
        }

        public static string PlayerToken
        {
            get { return _playerToken; }
            set { return; }
        }


        private static string _playerToken
        {
            get { return PlayerPrefs.GetString("gc-prefs-token"); }
            set
            {
                PlayerPrefs.SetString("gc-prefs-token", value);
            }
        }

        /// <summary>
        /// Create Your Player in your Game
        /// </summary>
        ///  <param name="PlayerSchema"> the Schema of player you want to create </param>
        /// <return></return>
        public async static Task<RankResponse> GetRankOfPlayer(string searchQuery, string field, sortMode mode)
        {
            ServerResponse resp = await GetRequestAsync($"/players/v2/rank?searchQuery={searchQuery}&sortMode={(int)mode}&field={field}");
            if (resp.IsSuccess)
                return new RankResponse()
                {
                    rank = JObject.Parse(resp.responseMessage)["rank"].ToObject<int>(),
                    status = RestfulMessages.successful
                };
            return new RankResponse()
            {
                rank = 0,
                status = RestfulMessages.failure
            };
        }



        /// <summary>
        /// Create Your Player in your Game
        /// </summary>
        ///  <param name="PlayerSchema"> the Schema of player you want to create </param>
        /// <return></return>
        public async static Task<ApiResponse> CreatePlayer<T>(T PlayerSchema, bool otp = false)
        {
            string stringJSON = JsonConvert.SerializeObject(PlayerSchema);
            var body = JObject.Parse(stringJSON);
            if (body["password"] != null)
            {

                body["password"] = GCPolicy.Md5Generator(body["password"].ToObject<string>());
            }
            else
            {
                return new ApiResponse(false, -751);
            }

            ServerResponse req = await PostRequestAsync("/players/v2/register", body);

            var resp = JObject.Parse(req.responseMessage);
            if (req.IsSuccess)
            {
                _playerToken = resp["token"].ToObject<string>();
                resp.Remove("token");
                return new ApiResponse(true, req.responseStatusCode, resp.ToObject<Dictionary<string, dynamic>>());
            }
            else if (resp["ecode"].ToObject<int>() == 10012)
            {
                return new ApiResponse(false, 10012, null);

            }

            return new ApiResponse(false, resp["ecode"].ToObject<int>(), null);
        }


        /// <summary>
        /// create your guest Player 
        /// </summary>
        ///  <param name="PlayerSchema"> the Schema of player you want to create </param>
        /// <return></return>
        public async static Task<ApiResponse> CreateGuestPlayer<T>(T PlayerSchema)
        {


            var stringJSON = JsonConvert.SerializeObject(PlayerSchema);
            var body = JObject.Parse(stringJSON);
            body["password"] = "guest";

            ServerResponse req = await PostRequestAsync("/players/v2/register?createMode=guest", body);
            var resp = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(req.responseMessage);
            if (req.IsSuccess)
            {
                _playerToken = resp["token"];
                resp.Remove("token");
                return new ApiResponse(true, req.responseStatusCode, resp);
            }
            else if (resp["ecode"] == 10012)
            {
                return new ApiResponse(false, 10012, null);

            }

            return new ApiResponse(false, resp["ecode"].ToObject<int>(), null);
        }

        /// <summary>
        /// create your guest Player 
        /// </summary>
        public async static Task<ApiResponse> CreateGuestPlayer()
        {

            ServerResponse req = await PostRequestAsync("/players/v2/register?createMode=guest", new JObject());
            var resp = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(req.responseMessage);
            if (req.IsSuccess)
            {
                _playerToken = resp["token"];
                resp.Remove("token");
                return new ApiResponse(true, 200, resp);
            }
            else if (resp["ecode"] == 10012)
            {
                return new ApiResponse(false, 10012, null);

            }

            return new ApiResponse(false, resp["ecode"].ToObject<int>(), null);
        }
        /// <summary>
        /// login your player in game
        /// </summary>
        ///  <param name="playerName"> the username of your player </param>
        ///  <param name="password"> the password of your player </param>
        /// <return></return>
        public static async Task<ApiResponse> Login(string playerName, string password)
        {
            string pass = "";
            if (password == "guest")
                pass = "guest";
            else
                pass = GCPolicy.Md5Generator(password);
            ServerResponse req = await GetRequestAsync("/players/v2/login/" + playerName + "/" + pass);
            var resp = JObject.Parse(req.responseMessage);

            if (req.IsSuccess)
            {
                _playerToken = resp["token"].ToObject<string>();
                resp.Remove("token");
                return new ApiResponse(true, 200, resp.ToObject<Dictionary<string, dynamic>>());
            }
            else if (resp["ecode"].ToObject<int>() == 404)
            {
                return new ApiResponse(false, 404, null);

            }
            else if (resp["ecode"].ToObject<int>() == 10090)
            {
                return new ApiResponse(false, 10090, null);

            }
            else if (resp["ecode"].ToObject<int>() == 10091)
            {
                return new ApiResponse(false, 10091, null);

            }
            return new ApiResponse(false, resp["ecode"].ToObject<int>(), null);

        }

        /// <summary>
        /// login your Guest player in game
        /// </summary>
        ///  <param name="playerName"> the username of your player </param>
        /// <return></return>
        public static async Task<ApiResponse> LoginGuestPlayer(string playerName)
        {
            return await Login(playerName, "guest");

        }
        /// <summary>
        /// get your player info with this method
        /// </summary>
        public static async Task<ApiResponse> GetPlayerInfo()
        {
            ServerResponse req = await GetRequestAsync("/players/v2/");
            var resp = JObject.Parse(req.responseMessage);

            if (req.IsSuccess)
            {

                return new ApiResponse(true, 200, resp.ToObject<Dictionary<string, dynamic>>());
            }
            else if (resp["ecode"].ToObject<int>() == 404)
            {
                return new ApiResponse(false, 404, null);

            }
            return new ApiResponse(false, resp["ecode"].ToObject<int>(), null);
        }
        /// <summary>
        /// get your player info with this method
        /// </summary>
        public static async Task<ApiResponse> GetPlayerInfo(string PlayerId)
        {
            ServerResponse req = await GetRequestAsync("/players/v2?pid=" + PlayerId);
            var resp = JObject.Parse(req.responseMessage);
            Debug.Log(resp);
            if (req.IsSuccess)
            {

                return new ApiResponse(true, 200, resp.ToObject<Dictionary<string, dynamic>>());
            }
            else if (resp["ecode"].ToObject<int>() == 404)
            {
                return new ApiResponse(false, 404, null);

            }
            return new ApiResponse(false, resp["ecode"].ToObject<int>(), null);
        }
        /// <summary>
        /// get your player's info as a leaderboard 
        /// </summary>
        ///  <param name="field"> the field of your table that want to sort</param>
        ///  <param name="sortMode"> the type of sort </param>
        ///  <param name="documentLimit"> the number of document to want show </param>
        /// <return></return>
        public static async Task<LeaderBoard> GetLeaderBoardInfo(string field, sortMode sortMode, int documentLimit = 100)
        {
            LeaderBoard leaderBoard = new LeaderBoard();
            ServerResponse req = await GetRequestAsync("/players/v2/leaderboard?field=" + field + "&sortMode=" + sortMode + "&documentLimit=" + documentLimit);
            if (req.IsSuccess)
            {
                JObject j = JObject.Parse(req.responseMessage);
                List<Dictionary<string, dynamic>> list = j["players"].ToObject<List<Dictionary<string, dynamic>>>();
                leaderBoard.pages = j["pages"].ToObject<int>();
                leaderBoard.players = list;

                return leaderBoard;
            }
            return null;
        }
        /// <summary>
        /// get your player's info as a leaderboard 
        /// </summary>
        ///  <param name="field"> the field of your table that want to sort</param>
        ///  <param name="sortMode"> the type of sort </param>
        ///  <param name="documentLimit"> the number of document to want show </param>
        ///  <param name="page"> the number page of data </param>
        /// <return></return>
        public static async Task<LeaderBoard> GetLeaderBoardInfo(string field, sortMode sortMode, int documentLimit = 100, int page = 1)
        {
            LeaderBoard leaderBoard = new LeaderBoard();
            ServerResponse req = await GetRequestAsync("/players/v2/leaderboard?field=" + field + "&sortMode=" + sortMode + "&documentLimit=" + documentLimit + "@pagination=on" + "&page=" + page);
            JObject j = JObject.Parse(req.responseMessage);
            if (req.IsSuccess)
            {
                List<Dictionary<string, dynamic>> list = j["players"].ToObject<List<Dictionary<string, dynamic>>>();
                leaderBoard.pages = j["pages"].ToObject<int>();
                leaderBoard.players = list;

                return leaderBoard;
            }
            return null;
        }

        /// <summary>
        /// forgot your player password 
        /// </summary>
        ///  <param name="PhoneNumber"> the phone number that want to reset password</param>
        ///  <param name="type"> the type of code that want to send</param>
        /// <return></return>
        public static async Task<ForgotPasswordTemplate> ForgotPassworByPhone(string PhoneNumber, CodeType type)
        {
            JObject body = new JObject();
            if (PhoneNumber != null)
                body.Add("phone", PhoneNumber);
            else
                return new ForgotPasswordTemplate(null, RestfulMessages.failure);
            ServerResponse req = await PostRequestAsync("/players/v2/forgotPassword/" + SenderMode.sms + "?codeType=" + type, body);
            var resp = (JObject)JsonConvert.DeserializeObject(req.responseMessage);

            if (req.IsSuccess == true)
                return new ForgotPasswordTemplate(resp["mask"].ToString(), RestfulMessages.successful);
            else if (resp["ecode"].ToObject<int>() == 10070)
            {
                return new ForgotPasswordTemplate(null, RestfulMessages.SenderNotFound);

            }
            else
            {
                if (resp["ecode"].ToObject<int>() == 10073)
                    return new ForgotPasswordTemplate(null, RestfulMessages.EmailOrPhoneNotFound);
                if (resp["ecode"].ToObject<int>() == 10079)
                    return new ForgotPasswordTemplate(null, RestfulMessages.SMSwalletNotEnough);
            }

            return new ForgotPasswordTemplate(null, RestfulMessages.failure);

        }
        /// <summary>
        /// forgot your player password 
        /// </summary>
        ///  <param name="Email"> the email address that want to reset password</param>
        ///  <param name="type"> the type of code that want to send</param>
        /// <return></return>
        public static async Task<ForgotPasswordTemplate> ForgotPasswordByEmail(string Email, CodeType type)
        {
            JObject body = new JObject();
            if (Email != null)
            {
                body.Add("email", Email);
            }
            else
                return new ForgotPasswordTemplate(null, RestfulMessages.failure);
            ServerResponse req = await PostRequestAsync("/players/v2/forgotPassword/" + SenderMode.email + "?codeType=" + type, body);
            var resp = JObject.Parse(req.responseMessage);

            if (req.IsSuccess == true)
                return new ForgotPasswordTemplate(resp["mask"].ToString(), RestfulMessages.successful);
            else if (resp["ecode"].ToObject<int>() == 10070)
            {
                return new ForgotPasswordTemplate(null, RestfulMessages.SenderNotFound);

            }
            else
            {
                if (resp["ecode"].ToObject<int>() == 10073)
                    return new ForgotPasswordTemplate(null, RestfulMessages.EmailOrPhoneNotFound);
                if (resp["ecode"].ToObject<int>() == 10078)
                    return new ForgotPasswordTemplate(null, RestfulMessages.EmailwalletNotEnough);
            }
            return new ForgotPasswordTemplate(null, RestfulMessages.failure);
        }
        /// <summary>
        /// check the validation of code 
        /// </summary>
        ///  <param name="code"> the forgot password code that recieved</param>
        /// <return></return>
        public static async Task<bool> CheckForgotPasswordCode(string code)
        {
            ServerResponse req = await GetRequestAsync("/players/v2/checkTempCode/" + code);
            var resp = JObject.Parse(req.responseMessage);
            if (req.IsSuccess)
            {
                bool instance = resp["correct"].ToObject<bool>();
                return instance;
            }
            return false;

        }
        /// <summary>
        /// change player password
        /// </summary>
        ///  <param name="tmpCode"> the code that recieved by email or phone number</param>
        ///  <param name="newPassword"> the new password that want to be changed</param>
        /// <return></return>
        public static async Task<ApiResponse> ChangePassword(string tmpCode, string newPassword)
        {
            JObject body = new JObject();
            if (tmpCode != null)
                body.Add("tmpCode", tmpCode);
            if (newPassword != null)
                body.Add("newPassword", GCPolicy.Md5Generator(newPassword));

            ServerResponse req = await PutRequestAsync("players/v2/forgot/changePassword", body);

            var resp = (JObject)JsonConvert.DeserializeObject(req.responseMessage);

            if (req.IsSuccess)
            {
                bool instance = resp["change"].ToObject<bool>();
                return new ApiResponse(instance, req.responseStatusCode, null);
            }
            else
            {
                if (resp["ecode"].ToObject<int>() == 10074)
                    return new ApiResponse(false, 10074, null);


            }
            return new ApiResponse(false, resp["ecode"].ToObject<int>(), null);


        }



        /// <summary>
        /// edit player data
        /// </summary>
        ///  <param name="PlayerSchema"> schema of player to create</param>
        /// <return></return>
        public static async Task<ApiResponse> EditPlayer<T>(T PlayerSchema)
        {
            string stringJSON = JsonConvert.SerializeObject(PlayerSchema);
            var body = JObject.Parse(stringJSON);
            if (body["password"] != null)
                body["password"] = null;

            ServerResponse req = await PutRequestAsync("/players/v2/", body);

            var resp = JObject.Parse(req.responseMessage);
            if (req.IsSuccess)
            {
                return new ApiResponse(true, req.responseStatusCode, resp.ToObject<Dictionary<string, dynamic>>());
            }
            else if (resp["ecode"].ToObject<int>() == 404)
            {
                return new ApiResponse(false, 404, null);

            }
            else if (resp["ecode"].ToObject<int>() == 10012)
            {
                return new ApiResponse(false, 10012, null);

            }
            return new ApiResponse(false, resp["ecode"].ToObject<int>(), null);
        }
        /// <summary>
        /// edit other player data
        /// </summary>
        ///  <param name="PlayerSchema"> schema of player to edit</param>
        ///  <param name="playerId"> id of player to edit</param>
        /// <return></return>
        public static async Task<bool> EditOtherPlayer<T>(T PlayerSchema, string playerId)
        {
            string stringJSON = JsonConvert.SerializeObject(PlayerSchema);
            var body = JObject.Parse(stringJSON);
            ServerResponse req = await PutRequestAsync("/players/v2?playerId=" + playerId, body);

            if (req.IsSuccess)
                return true;
            return false;
        }
        /// <summary>
        /// edit other player data
        /// </summary>
        ///  <param name="PlayerSchema"> schema of player to edit</param>
        ///  <param name="playerName"> name of player to edit</param>
        /// <return></return>
        public static async Task<bool> EditOtherPlayer<T>(string playerName, T PlayerSchema)
        {
            string stringJSON = JsonConvert.SerializeObject(PlayerSchema);
            var body = JObject.Parse(stringJSON);
            ServerResponse req = await PutRequestAsync("/players/v2?playerName=" + playerName, body);
            if (req.IsSuccess)
                return true;
            return false;
        }

        /// <summary>
        /// promote your player from guest
        /// </summary>
        ///  <param name="PlayerSchema"> schema of player to create</param>
        /// <return></return>
        public static async Task<ApiResponse> PromoteGuestPlayer<T>(T PlayerSchema)
        {
            return await EditPlayer<T>(PlayerSchema);
        }
        /// <summary>
        /// to Check if player is Loged In 
        /// </summary>
        public static async Task<bool> IsDeviceLogedIn()
        {
            ServerResponse res = await GetRequestAsync("/players/v2/isLogin");
            if (res.IsSuccess == true)
            {
                var resq = JObject.Parse(res.responseMessage);
                if (resq["isLogin"].ToObject<bool>() == true)
                {
                    return true;
                }
            }
            return false;

        }


        /// <summary>
        /// to logout your player form device 
        /// </summary>
        public static async Task<bool> LogOut()
        {
            if (PlayerPrefs.HasKey("gc-prefs-token") == false)
                return false;
            else
            {
                ServerResponse res = await DeleteRequestAsync("/players/v2/logout");
                var resp = JObject.Parse(res.responseMessage);
                if (resp["logout"].ToObject<bool>() == true)
                {

                    PlayerPrefs.DeleteKey("gc-prefs-token");
                    return true;

                }

            }
            return false;
        }
    }
}
