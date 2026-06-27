using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using gamingCloud.templates;
using System;

namespace gamingCloud
{

    public class PlayerStorage
    {
        #region Set
        class ResponseData
        {
            public bool ok;
            public JObject data;
            public int ecode;

        }
        class tmp_json
        {
            public string key;
            public Dictionary<string, dynamic> value;
        }
        /// <summary>
        /// Create Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        ///  <param name="value"> the value of PlayerExtra </param>
        /// <return></return>
        public static async Task<RestfulMessages> SetJson(string key, Dictionary<string, dynamic> json)
        {

            if (Players.IsLogin == false)
                return RestfulMessages.failure;


            tmp_json tj = new tmp_json();
            tj.key = key;
            tj.value = json;

            string j = JsonConvert.SerializeObject(tj);

            ServerResponse req = await HttpRequest.PostRequestAsync("/players/v2/storage", JObject.Parse(j));

            if (req.IsSuccess)
            {

                return RestfulMessages.successful;
            }
            else
                return RestfulMessages.failure;
        }
        /// <summary>
        /// Create Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        ///  <param name="value"> the value of PlayerExtra </param>
        /// <return></return>
        public static async Task<RestfulMessages> SetJson<T>(string key, T json)
        {
            if (Players.IsLogin == false)
                return RestfulMessages.failure;


            Dictionary<string, dynamic> body = new Dictionary<string, dynamic>();

            body.Add("key", key);

            body.Add("value", json);

            ServerResponse req = await HttpRequest.PostRequestAsync("/players/v2/storage", body);

            if (req.IsSuccess)
            {

                return RestfulMessages.successful;
            }
            else
                return RestfulMessages.failure;
        }
        /// <summary>
        /// Create Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        ///  <param name="value"> the value of PlayerExtra </param>
        /// <return></return>
        public static async Task<RestfulMessages> SetString(string key, string value)
        {
            if (Players.IsLogin == false)
                return RestfulMessages.failure;

            JObject body = new JObject();
            body.Add("key", key);
            body.Add("value", value);
            ServerResponse req = await HttpRequest.PostRequestAsync("/players/v2/storage", body);
            var resp = JObject.Parse(req.responseMessage).ToObject<ResponseData>();



            if (req.IsSuccess)
            {

                return RestfulMessages.successful;
            }
            else
                return RestfulMessages.failure;
        }
        /// <summary>
        /// Create Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        ///  <param name="value"> the value of PlayerExtra </param>
        /// <return></return>
        public static async Task<RestfulMessages> SetInt(string key, int value)
        {
            if (Players.IsLogin == false)
                return RestfulMessages.failure;

            JObject body = new JObject();
            body.Add("key", key);
            body.Add("value", value);
            ServerResponse req = await HttpRequest.PostRequestAsync("/players/v2/storage", body);
            var resp = JObject.Parse(req.responseMessage).ToObject<ResponseData>();


            if (req.IsSuccess)
            {

                return RestfulMessages.successful;
            }
            else
                return RestfulMessages.failure;
        }

        /// <summary>
        /// Create Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        ///  <param name="value"> the value of PlayerExtra </param>
        /// <return></return>
        public static async Task<RestfulMessages> SetBool(string key, bool value)
        {
            if (Players.IsLogin == false)
                return RestfulMessages.failure;
            JObject body = new JObject();
            body.Add("key", key);
            body.Add("value", value);
            ServerResponse req = await HttpRequest.PostRequestAsync("/players/v2/storage", body);
            var resp = JObject.Parse(req.responseMessage).ToObject<ResponseData>();


            if (req.IsSuccess)
            {

                return RestfulMessages.successful;
            }
            else
                return RestfulMessages.failure;
        }
        /// <summary>
        /// Create Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        ///  <param name="value"> the value of PlayerExtra </param>
        /// <return></return>

        public static async Task<RestfulMessages> SetFloat(string key, float value)
        {
            if (Players.IsLogin == false)
                return RestfulMessages.failure;

            JObject body = new JObject();
            body.Add("key", key);
            body.Add("value", value);
            ServerResponse req = await HttpRequest.PostRequestAsync("/players/v2/storage", body);
            var resp = JObject.Parse(req.responseMessage).ToObject<ResponseData>();

            if (req.IsSuccess)
            {

                return RestfulMessages.successful;
            }
            else
                return RestfulMessages.failure;
        }
        /// <summary>
        /// Create Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        ///  <param name="value"> the value of PlayerExtra </param>
        /// <return></return>
        public static async Task<RestfulMessages> SetDouble(string key, double value)
        {
            if (Players.IsLogin == false)
                return RestfulMessages.failure;

            JObject body = new JObject();
            body.Add("key", key);
            body.Add("value", value);
            ServerResponse req = await HttpRequest.PostRequestAsync("/players/v2/storage", body);
            var resp = JObject.Parse(req.responseMessage).ToObject<ResponseData>();


            if (req.IsSuccess)
            {

                return RestfulMessages.successful;
            }
            else
                return RestfulMessages.failure;
        }
        /// <summary>
        /// Create Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        ///  <param name="value"> the value of PlayerExtra </param>
        /// <return></return>
        public static async Task<RestfulMessages> SetLong(string key, long value)
        {
            if (Players.IsLogin == false)
                return RestfulMessages.failure;

            JObject body = new JObject();
            body.Add("key", key);
            body.Add("value", value);
            ServerResponse req = await HttpRequest.PostRequestAsync("/players/v2/storage", body);
            var resp = JObject.Parse(req.responseMessage).ToObject<ResponseData>();

            if (req.IsSuccess)
            {

                return RestfulMessages.successful;
            }
            else
                return RestfulMessages.failure;
        }
        /// <summary>
        /// Create Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        ///  <param name="value"> the value of PlayerExtra </param>
        /// <return></return>
        /// 

        class ResponseDataArray
        {
            public bool ok;
            public List<string> data;
            public int ecode;
        }
        public static async Task<RestfulMessages> SetIntArray(string key, int[] value)
        {
            if (Players.IsLogin == false)
                return RestfulMessages.failure;

            Dictionary<string, dynamic> body = new Dictionary<string, dynamic>();
            body.Add("key", key);
            body.Add("value", value);
            ServerResponse req = await HttpRequest.PostRequestAsync("/players/v2/storage", body);

            if (req.IsSuccess)
            {

                return RestfulMessages.successful;
            }
            else
                return RestfulMessages.failure;

        }
        /// <summary>
        /// Create Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        ///  <param name="value"> the value of PlayerExtra </param>
        /// <return></return>
        /// 

        public static async Task<RestfulMessages> SetFloatArray(string key, float[] value)
        {
            if (Players.IsLogin == false)
                return RestfulMessages.failure;

            Dictionary<string, dynamic> body = new Dictionary<string, dynamic>();
            body.Add("key", key);
            body.Add("value", value);
            ServerResponse req = await HttpRequest.PostRequestAsync("/players/v2/storage", body);

            if (req.IsSuccess)
            {

                return RestfulMessages.successful;
            }
            else
                return RestfulMessages.failure;
        }
        /// <summary>
        /// Create Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        ///  <param name="value"> the value of PlayerExtra </param>
        /// <return></return>
        public static async Task<RestfulMessages> SetStringArray(string key, string[] value)
        {
            if (Players.IsLogin == false)
                return RestfulMessages.failure;

            Dictionary<string, dynamic> body = new Dictionary<string, dynamic>();
            body.Add("key", key);
            body.Add("value", value);
            ServerResponse req = await HttpRequest.PostRequestAsync("/players/v2/storage", body);

            if (req.IsSuccess)
            {

                return RestfulMessages.successful;
            }
            else
                return RestfulMessages.failure;
        }
        /// <summary>
        /// Create Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        ///  <param name="value"> the value of PlayerExtra </param>
        /// <return></return>
        public static async Task<RestfulMessages> SetDoubleArray(string key, double[] value)
        {
            if (Players.IsLogin == false)
                return RestfulMessages.failure;

            Dictionary<string, dynamic> body = new Dictionary<string, dynamic>();
            body.Add("key", key);
            body.Add("value", value);
            ServerResponse req = await HttpRequest.PostRequestAsync("/players/v2/storage", body);

            if (req.IsSuccess)
            {

                return RestfulMessages.successful;
            }
            else
                return RestfulMessages.failure;
        }
        /// <summary>
        /// Create Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        ///  <param name="value"> the value of PlayerExtra </param>
        /// <return></return>
        public static async Task<RestfulMessages> SetLongArray(string key, long[] value)
        {
            if (Players.IsLogin == false)
                return RestfulMessages.failure;

            Dictionary<string, dynamic> body = new Dictionary<string, dynamic>();
            body.Add("key", key);
            body.Add("value", value);
            ServerResponse req = await HttpRequest.PostRequestAsync("/players/v2/storage", body);

            if (req.IsSuccess)
            {

                return RestfulMessages.successful;
            }
            else
                return RestfulMessages.failure;
        }

        /// <summary>
        /// Create Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        ///  <param name="value"> the value of PlayerExtra </param>
        /// <return></return>
        public static async Task<RestfulMessages> SetBoolArray(string key, bool[] value)
        {
            if (Players.IsLogin == false)
                return RestfulMessages.failure;

            Dictionary<string, dynamic> body = new Dictionary<string, dynamic>();
            body.Add("key", key);
            body.Add("value", value);
            ServerResponse req = await HttpRequest.PostRequestAsync("/players/v2/storage", body);

            if (req.IsSuccess)
            {

                return RestfulMessages.successful;
            }
            else
                return RestfulMessages.failure;
        }

        #endregion

        #region Get
        /// <summary>
        /// get Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        /// <return></return>


        public static async Task<PlayerStorageResponseJson> GetJson(string key)
        {
            ServerResponse req = await HttpRequest.GetRequestAsync("/players/v2/storage/" + key);
            JObject data = JObject.Parse(req.responseMessage);
            if (req.IsSuccess)
            {
                var rr = data["data"]["value"];
                return new PlayerStorageResponseJson(true, req.responseStatusCode, rr.ToObject<Dictionary<string, dynamic>>());
            }

            return new PlayerStorageResponseJson(false, (int)data["ecode"]);
        }
        /// <summary>
        /// get Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        /// <return></return>


        public static async Task<T> GetJson<T>(string key)
        {

            ServerResponse req = await HttpRequest.GetRequestAsync("/players/v2/storage/" + key);
            if (req.IsSuccess)
            {
                ResponseData data = JObject.Parse(req.responseMessage).ToObject<ResponseData>();
                string sjson = JsonConvert.SerializeObject(data.data["value"]);

                return JObject.Parse(sjson).ToObject<T>();
            }

            return default(T);
        }
        /// <summary>
        /// get Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        /// <return></return>


        public static async Task<PlayerStorageResponseInt> GetInt(string key)
        {
            ServerResponse req = await HttpRequest.GetRequestAsync("/players/v2/storage/" + key);

            var resp = JObject.Parse(req.responseMessage).ToObject<ResponseData>();


            if (req.IsSuccess)
            {

                return new PlayerStorageResponseInt(true, req.responseStatusCode, resp.data["value"].ToObject<int>());
            }
            else
                return new PlayerStorageResponseInt(false, resp.ecode);
        }
        /// <summary>
        /// get Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        /// <return></return>
        public static async Task<PlayerStorageResponseIntArray> GetIntArray(string key)
        {
            ServerResponse req = await HttpRequest.GetRequestAsync("/players/v2/storage/" + key);
            JObject data = JObject.Parse(req.responseMessage);
            if (req.IsSuccess)
            {
                int[] rr = data["data"]["value"].ToObject<int[]>();
                return new PlayerStorageResponseIntArray(true, req.responseStatusCode, rr);
            }

            return new PlayerStorageResponseIntArray(false, (int)data["ecode"]);
        }
        /// <summary>
        /// get Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        /// <return></return>

        public static async Task<PlayerStorageResponseStringArray> GetStringArray(string key)
        {
            ServerResponse req = await HttpRequest.GetRequestAsync("/players/v2/storage/" + key);
            JObject data = JObject.Parse(req.responseMessage);
            if (req.IsSuccess)
            {
                string[] rr = data["data"]["value"].ToObject<string[]>();
                return new PlayerStorageResponseStringArray(true, req.responseStatusCode, rr);
            }

            return new PlayerStorageResponseStringArray(false, (int)data["ecode"]);
        }
        /// <summary>
        /// get Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        /// <return></return>

        public static async Task<PlayerStorageResponseFloatArray> GetFloatArray(string key)
        {
            ServerResponse req = await HttpRequest.GetRequestAsync("/players/v2/storage/" + key);
            JObject data = JObject.Parse(req.responseMessage);
            if (req.IsSuccess)
            {
                float[] rr = data["data"]["value"].ToObject<float[]>();
                return new PlayerStorageResponseFloatArray(true, req.responseStatusCode, rr);
            }

            return new PlayerStorageResponseFloatArray(false, (int)data["ecode"]);
        }
        /// <summary>
        /// get Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        /// <return></return>
        /// 

        public static async Task<PlayerStorageResponseDoubleArray> GetDoubleArray(string key)
        {
            ServerResponse req = await HttpRequest.GetRequestAsync("/players/v2/storage/" + key);
            JObject data = JObject.Parse(req.responseMessage);
            if (req.IsSuccess)
            {
                double[] rr = data["data"]["value"].ToObject<double[]>();
                return new PlayerStorageResponseDoubleArray(true, req.responseStatusCode, rr);
            }

            return new PlayerStorageResponseDoubleArray(false, (int)data["ecode"]);
        }
        public static async Task<PlayerStorageResponseBoolArray> GetBoolArray(string key)
        {
            ServerResponse req = await HttpRequest.GetRequestAsync("/players/v2/storage/" + key);
            JObject data = JObject.Parse(req.responseMessage);
            if (req.IsSuccess)
            {
                bool[] rr = data["data"]["value"].ToObject<bool[]>();
                return new PlayerStorageResponseBoolArray(true, req.responseStatusCode, rr);
            }

            return new PlayerStorageResponseBoolArray(false, (int)data["ecode"]);
        }

        /// <summary>
        /// get Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        /// <return></return>
        /// 

        public static async Task<PlayerStorageResponseLongArray> GetLongArray(string key)
        {
            ServerResponse req = await HttpRequest.GetRequestAsync("/players/v2/storage/" + key);
            JObject data = JObject.Parse(req.responseMessage);
            if (req.IsSuccess)
            {
                long[] rr = data["data"]["value"].ToObject<long[]>();
                return new PlayerStorageResponseLongArray(true, req.responseStatusCode, rr);
            }

            return new PlayerStorageResponseLongArray(false, (int)data["ecode"]);
        }

        /// <summary>
        /// get Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        /// <return></return>
        /// 

        public static async Task<PlayerStorageResponseBool> GetBool(string key)
        {
            ServerResponse req = await HttpRequest.GetRequestAsync("/players/v2/storage/" + key);
            var resp = JObject.Parse(req.responseMessage);

            if (req.IsSuccess)
            {

                return new PlayerStorageResponseBool(true, req.responseStatusCode, resp["data"]["value"].ToObject<bool>());
            }
            else
                return new PlayerStorageResponseBool(false, resp["ecode"].ToObject<int>());
        }
        /// <summary>
        /// get Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        /// <return></return>
        public static async Task<PlayerStorageResponseString> GetString(string key)
        {
            ServerResponse req = await HttpRequest.GetRequestAsync("/players/v2/storage/" + key);
            var resp = JsonConvert.DeserializeObject<ResponseData>(req.responseMessage);

            if (req.IsSuccess)
            {

                return new PlayerStorageResponseString(true, resp.ecode, resp.data["value"].ToObject<string>());
            }
            else
                return new PlayerStorageResponseString(false, resp.ecode);
        }
        /// <summary>
        /// get Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        /// <return></return>
        public static async Task<PlayerStorageResponseLong> GetLong(string key)
        {
            ServerResponse req = await HttpRequest.GetRequestAsync("/players/v2/storage/" + key);
            var resp = JsonConvert.DeserializeObject<ResponseData>(req.responseMessage);

            if (req.IsSuccess)
            {

                return new PlayerStorageResponseLong(true, resp.ecode, resp.data["value"].ToObject<long>());
            }
            else
                return new PlayerStorageResponseLong(false, resp.ecode);
        }

        /// <summary>
        /// get Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        /// <return></return>
        public static async Task<PlayerStorageResponseDouble> GetDouble(string key)
        {
            ServerResponse req = await HttpRequest.GetRequestAsync("/players/v2/storage/" + key);
            var resp = JsonConvert.DeserializeObject<ResponseData>(req.responseMessage);

            if (req.IsSuccess)
            {

                return new PlayerStorageResponseDouble(true, resp.ecode, resp.data["value"].ToObject<double>());
            }
            else
                return new PlayerStorageResponseDouble(false, resp.ecode);
        }

        /// <summary>
        /// get Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        /// <return></return>
        public static async Task<PlayerStorageResponseFloat> GetFloat(string key)
        {
            ServerResponse req = await HttpRequest.GetRequestAsync("/players/v2/storage/" + key);
            var resp = JsonConvert.DeserializeObject<ResponseData>(req.responseMessage);

            if (req.IsSuccess)
            {

                return new PlayerStorageResponseFloat(true, resp.ecode, resp.data["value"].ToObject<float>());
            }
            else
                return new PlayerStorageResponseFloat(false, resp.ecode);
        }

        #endregion

        #region edit
        /// <summary>
        /// Edit Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        ///  <param name="value"> the value of PlayerExtra </param>
        /// <return></return>
        public static async Task<RestfulMessages> EditJson(string key, Dictionary<string, dynamic> json)
        {
            if (Players.IsLogin == false)
                return RestfulMessages.failure;

            tmp_json tj = new tmp_json();
            tj.key = key;
            tj.value = json;
            string j = JsonConvert.SerializeObject(tj);

            ServerResponse req = await HttpRequest.PutRequestAsync("/players/v2/storage/" + key, JObject.Parse(j));


            if (req.IsSuccess)
            {

                return RestfulMessages.successful;
            }
            else
                return RestfulMessages.failure;
        }
        /// <summary>
        /// Edit Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        ///  <param name="value"> the value of PlayerExtra </param>
        /// <return></return>
        public static async Task<RestfulMessages> EditJson<T>(string key, T json)
        {
            if (Players.IsLogin == false)
                return RestfulMessages.failure;

            Dictionary<string, dynamic> body = new Dictionary<string, dynamic>();
            body.Add("key", key);
            body.Add("value", json);
            ServerResponse req = await HttpRequest.PutRequestAsync("/players/v2/storage/" + key, body);



            if (req.IsSuccess)
            {

                return RestfulMessages.successful;
            }
            else
                return RestfulMessages.failure;
        }


        /// <summary>
        /// edit Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        ///  <param name="value"> the value of PlayerExtra </param>
        /// <return></return>
        public static async Task<RestfulMessages> EditDoubleArray(string key, double[] value)
        {
            if (Players.IsLogin == false)
                return RestfulMessages.failure;

            Dictionary<string, dynamic> body = new Dictionary<string, dynamic>();
            body.Add("value", value);

            ServerResponse req = await HttpRequest.PutRequestAsync("/players/v2/storage/" + key, body);

            if (req.IsSuccess)
            {

                return RestfulMessages.successful;
            }
            else
                return RestfulMessages.failure;
        }
        /// <summary>
        /// edit Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        ///  <param name="value"> the value of PlayerExtra </param>
        /// <return></return>
        public static async Task<RestfulMessages> EditStringArray(string key, string[] value)
        {
            if (Players.IsLogin == false)
                return RestfulMessages.failure;

            Dictionary<string, dynamic> body = new Dictionary<string, dynamic>();
            body.Add("value", value);

            ServerResponse req = await HttpRequest.PutRequestAsync("/players/v2/storage/" + key, body);

            if (req.IsSuccess)
            {

                return RestfulMessages.successful;
            }
            else
                return RestfulMessages.failure;
        }
        /// <summary>
        /// edit Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        ///  <param name="value"> the value of PlayerExtra </param>
        /// <return></return>
        public static async Task<RestfulMessages> EditLongArray(string key, long[] value)
        {
            if (Players.IsLogin == false)
                return RestfulMessages.failure;

            Dictionary<string, dynamic> body = new Dictionary<string, dynamic>();
            body.Add("value", value);

            ServerResponse req = await HttpRequest.PutRequestAsync("/players/v2/storage/" + key, body);

            if (req.IsSuccess)
            {

                return RestfulMessages.successful;
            }
            else
                return RestfulMessages.failure;
        }
        /// <summary>
        /// edit Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        ///  <param name="value"> the value of PlayerExtra </param>
        /// <return></return>
        public static async Task<RestfulMessages> EditIntArray(string key, int[] value)
        {
            if (Players.IsLogin == false)
                return RestfulMessages.failure;

            Dictionary<string, dynamic> body = new Dictionary<string, dynamic>();
            body.Add("value", value);
            ServerResponse req = await HttpRequest.PutRequestAsync("/players/v2/storage/" + key, body);

            if (req.IsSuccess)
            {

                return RestfulMessages.successful;
            }
            else
                return RestfulMessages.failure;
        }

        /// <summary>
        /// edit Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        ///  <param name="value"> the value of PlayerExtra </param>
        /// <return></return>
        public static async Task<RestfulMessages> EditFloatArray(string key, float[] value)
        {
            if (Players.IsLogin == false)
                return RestfulMessages.failure;

            Dictionary<string, dynamic> body = new Dictionary<string, dynamic>();
            body.Add("value", value);

            ServerResponse req = await HttpRequest.PutRequestAsync("/players/v2/storage/" + key, body);

            if (req.IsSuccess)
            {

                return RestfulMessages.successful;
            }
            else
                return RestfulMessages.failure;
        }
        /// <summary>
        /// edit Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        ///  <param name="value"> the value of PlayerExtra </param>
        /// <return></return>
        public static async Task<RestfulMessages> EditBoolArray(string key, bool[] value)
        {
            if (Players.IsLogin == false)
                return RestfulMessages.failure;

            Dictionary<string, dynamic> body = new Dictionary<string, dynamic>();
            body.Add("value", value);

            ServerResponse req = await HttpRequest.PutRequestAsync("/players/v2/storage/" + key, body);

            if (req.IsSuccess)
            {

                return RestfulMessages.successful;
            }
            else
                return RestfulMessages.failure;
        }
        /// <summary>
        /// edit Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        ///  <param name="value"> the value of PlayerExtra </param>
        /// <return></return>
        public static async Task<RestfulMessages> EditFloat(string key, float value)
        {

            if (Players.IsLogin == false)
                return RestfulMessages.failure;

            Dictionary<string, dynamic> body = new Dictionary<string, dynamic>();
            body.Add("value", value.ToString());
            ServerResponse req = await HttpRequest.PutRequestAsync("/players/v2/storage/" + key, body);

            if (req.IsSuccess)
            {

                return RestfulMessages.successful;
            }
            else
                return RestfulMessages.failure;

        }
        /// <summary>
        /// edit Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        ///  <param name="value"> the value of PlayerExtra </param>
        /// <return></return>
        public static async Task<RestfulMessages> EditInt(string key, int value)
        {

            if (Players.IsLogin == false)
                return RestfulMessages.failure;

            Dictionary<string, dynamic> body = new Dictionary<string, dynamic>();
            body.Add("value", value.ToString());
            ServerResponse req = await HttpRequest.PutRequestAsync("/players/v2/storage/" + key, body);

            if (req.IsSuccess)
            {

                return RestfulMessages.successful;
            }
            else
                return RestfulMessages.failure;
        }
        /// <summary>
        /// edit Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        ///  <param name="value"> the value of PlayerExtra </param>
        /// <return></return>
        public static async Task<RestfulMessages> EditBool(string key, bool value)
        {

            if (Players.IsLogin == false)
                return RestfulMessages.failure;

            Dictionary<string, dynamic> body = new Dictionary<string, dynamic>();
            body.Add("value", value.ToString());
            ServerResponse req = await HttpRequest.PutRequestAsync("/players/v2/storage/" + key, body);

            if (req.IsSuccess)
            {

                return RestfulMessages.successful;
            }
            else
                return RestfulMessages.failure;
        }
        /// <summary>
        /// edit Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        ///  <param name="value"> the value of PlayerExtra </param>
        /// <return></return>
        public static async Task<RestfulMessages> EditDouble(string key, double value)
        {

            if (Players.IsLogin == false)
                return RestfulMessages.failure;

            Dictionary<string, dynamic> body = new Dictionary<string, dynamic>();
            body.Add("value", value.ToString());
            ServerResponse req = await HttpRequest.PutRequestAsync("/players/v2/storage/" + key, body);

            if (req.IsSuccess)
            {

                return RestfulMessages.successful;
            }
            else
                return RestfulMessages.failure;
        }
        /// <summary>
        /// edit Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        ///  <param name="value"> the value of PlayerExtra </param>
        /// <return></return>
        public static async Task<RestfulMessages> EditLong(string key, long value)
        {

            if (Players.IsLogin == false)
                return RestfulMessages.failure;

            Dictionary<string, dynamic> body = new Dictionary<string, dynamic>();
            body.Add("value", value.ToString());
            ServerResponse req = await HttpRequest.PutRequestAsync("/players/v2/storage/" + key, body);

            if (req.IsSuccess)
            {

                return RestfulMessages.successful;
            }
            else
                return RestfulMessages.failure;
        }
        /// <summary>
        /// edit Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        ///  <param name="value"> the value of PlayerExtra </param>
        /// <return></return>
        public static async Task<RestfulMessages> EditString(string key, string value)
        {

            if (Players.IsLogin == false)
                return RestfulMessages.failure;

            Dictionary<string, dynamic> body = new Dictionary<string, dynamic>();
            body.Add("value", value.ToString());
            ServerResponse req = await HttpRequest.PutRequestAsync("/players/v2/storage/" + key, body);

            if (req.IsSuccess)
            {

                return RestfulMessages.successful;
            }
            else
                return RestfulMessages.failure;
        }


        #endregion


        #region delete
        /// <summary>
        /// delete Your Player Storage  Data
        /// </summary>
        ///  <param name="key"> the key of PlayerExtra </param>
        /// <return></return>
        public static async Task<bool> DeleteKey(string key)
        {
            if (Players.IsLogin == false)
                return false;

            ServerResponse req = await HttpRequest.DeleteRequestAsync("/players/v2/storage/" + key);

            if (req.IsSuccess)
            {

                return true;
            }
            else
                return false;
        }


        #endregion

        public async static Task<bool> HasKey(string key)
        {
            ServerResponse req = await HttpRequest.GetRequestAsync("/players/v2/storage/" + key);
            return req.IsSuccess;
        }

    }
}