using System.Collections;
using System.Collections.Generic;


namespace gamingCloud.Network
{
    [System.Serializable]
    public class Role
    {
        [ShowOnly]
        public string key;
        [ShowOnly]
        public string description;
    }

    [System.Serializable]
    public class PlayerModel
    {
        [ShowOnly] public string netId;
        [ShowOnly] public string name;
        [ShowOnly] public int score;

        public Role role;

        // Dictionary<string, object> _model;

        // public Dictionary<string, object> model
        // {
        //     set { return; }
        //     get { return _model; }
        // }

        public PlayerModel(/* Dictionary<string, object> model,  */string netId, string name , Role role)
        {
            this.netId = netId;
            this.name = name;
            this.role = role;
            // this._model = model;
        }

        // public T getPlayerModel<T>()
        // {
        //     return JsonParser.ConvertDictionaryToSchema<T>(this._model);
        // }
    }
}