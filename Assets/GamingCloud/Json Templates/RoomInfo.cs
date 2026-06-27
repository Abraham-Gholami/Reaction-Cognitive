using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gamingCloud.templates
{
    public class RoomInfo
    {
        public string id, name, createdAt;
        public bool isPrivate;
        public int maxPlayers, playersCount;
    }
}
