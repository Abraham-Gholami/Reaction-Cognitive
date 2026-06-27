using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace gamingCloud.Network
{
    [System.Serializable]
    public class RoomModel
    {
        [ShowOnly] public string id;
        [ShowOnly] public string name;
        [ShowOnly] public int? maxPlayers;
        [ShowOnly] public bool isPrivate;
        public List<PlayerModel> players;
        public PlayerModel roomCreator;
        [ShowOnly] public string turn;
        public JObject data;

        public PlayerModel CurrentTurn
        {
            get
            {
                return players.Find(v => v.netId == turn);
            }
            set { return; }
        }
    }
}