using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using gamingCloud.Network.tcp;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using UnityEngine.Networking;
using System.Reflection;
using System.Linq;
using System.Management.Instrumentation;
using gamingCloud.templates;

namespace gamingCloud.Network
{
	public class GCMultiPlayer : MonoBehaviour
	{
		#region Fields
		private const string BASE_URL = "multiplayer.gcsc.ir";
		//private const string BASE_URL = "localhost";
		private const int PORT = 10010;
		///<summary>
		///if you want test with test player, fill it, else remove value in it.
		///</summary>
		[SerializeField] string testPlayerId;
		private GCNetworkAnimation animation;

		///<summary>
		///check internet connection
		///</summary>
		public bool internetConnection
		{
			get { return Application.internetReachability != NetworkReachability.NotReachable; }
		}

		#region ping
		///<summary>
		///Get the Ping of connetion to GamnigCloud data center
		///</summary>
		public int ping
		{
			get { return PingCalculator.instance ? PingCalculator.instance.Ping : 0; }
		}
		#endregion


		public enum FriendRequestResponse
		{
			RequestSent = 0,
			PlayerNotFound = 91001,
			PlayerisOffline = 91002,
			ExistedInRoom = 91003,
			RequestIsExpired = 2,
			FirstCreateRoom = 91004,
			RequestAcceptedAndJoined = 1,
			RequestRejected = -1,
		}

		#region is connected
		[SerializeField] [ShowOnly] private bool _isConnected = false;
		///<summary>
		///check connection to multiplayer server
		///</summary>
		public bool isConnected
		{
			get { return _isConnected; }
		}
		#endregion

		#region master player
		[SerializeField] [ShowOnly] private bool _isMasterPlayer = false;
		///<summary>
		///if player created the room, then he is a room master.
		///you can use this property for manage your game, like room admin! 
		///</summary>
		public bool isMasterPlayer
		{
			get { return _isMasterPlayer; }
		}
		#endregion

		#region net id
		[SerializeField] [ShowOnly] private string _netId;
		///<summary>
		///player netId
		///</summary>
		public string netId
		{
			get { return _netId; }
		}
		#endregion

		#region player model
		[SerializeField] private PlayerModel _PlayerInfo;
		///<summary>
		///info of player
		///</summary>
		public PlayerModel PlayerInfo
		{
			get { return _PlayerInfo; }
		}
		#endregion

		#region room
		[SerializeField] private RoomModel _room;
		///<summary>
		///room info
		///</summary>
		public RoomModel room
		{
			get { return _room; }
		}
		#endregion

		#region packet size
		private ulong _packet = 1024 * 1024 * 100;
		///<summary>
		///size of packets:
		///if you have error for covnvert of packets, increase this property
		///</summary>
		public ulong packetSize
		{
			get { return _packet; }
			set
			{
				if (_isConnected)
					return;
				else
					_packet = value;
			}
		}
		#endregion

		#region Spawn index
		private int _spawnIndex = -1;
		public int spawnIndex
		{
			get
			{
				if (this._room != null && _spawnIndex == -1)
				{
					for (int i = 0; i < this._room.players.Count; i++)
					{
						if (this._room.players[i].netId == this._netId)
						{
							_spawnIndex = i;
							break;
						}
					}
				}

				return _spawnIndex;
			}
		}
		#endregion

		#region server time
		private long _serverTs;
		private bool _serverTimerTickEnabled = false;

		public long serverTimeStamp
		{
			get { return _isConnected ? _serverTs : GCPolicy.getTimeStamp(); }
			set { return; }
		}

		public DateTime serverDateTime
		{
			get { return JavaTimeStampToDateTime(serverTimeStamp); }
			set { return; }
		}

		private DateTime JavaTimeStampToDateTime(double javaTimeStamp)
		{
			// Java timestamp is milliseconds past epoch
			System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
			dtDateTime = dtDateTime.AddMilliseconds(javaTimeStamp).ToLocalTime();
			return dtDateTime;
		}

		#endregion

		[Header("Clients Settings:")]
		public GameObject[] ClientObjects;

		#region private Fields
		[SerializeField] private bool removeClientsAfterDC = true;
		[SerializeField] private bool instatiateClientsOnJoin = true;
		private Dictionary<string, List<StreambleObjectTag>> _clients;
		private tcpStreamer streamer;
		private bool isTurn;
		int index = 0;

		#endregion

		#endregion

		#region json schemas
		class disconnectPlayer
		{
			public PlayerModel player;
			public bool isLeave = false;
			public RoomModel room;
		}
		class joinPlayerModel
		{
			[ShowOnly] public string netId;
			[ShowOnly] public string name;
			public Role role;
		}

		class OnjoinPlayer
		{
			public bool pre;
			public RoomModel room;
			public joinPlayerModel player;

		}

		class eventSender
		{
			public string body = null;
			public string eventName;
			public string getter = null;
			public bool nsp = false;
		}

		class eventRecieve
		{
			public string eventName;
			public string body;
		}
		#endregion

		#region overrides

		///<summary>
		///when your connection to server is successful
		///</summary>
		public virtual void ConnectedToServer() { }

		///<summary>
		///when Friend Request is Recieved
		///</summary>
		public virtual void OnNewFriendRequest(string RequestId, string PlayerId) { }

		///<summary>
		///when Friend Request is Responed.
		///</summary>
		public virtual void OnFriendRequestResponse(FriendRequestResponse status) { }


		///<summary>
		///Return the Status of Player
		///</summary>
		public virtual void OnSendRequestStatus(FriendRequestResponse status) { }

		///<summary>
		///when new player join to room
		///</summary>
		public virtual void OnJoinNewPlayer(PlayerModel newPlayer) { }

		///<summary>
		///like loop! looping in previous players of room
		///</summary>
		public virtual void PreviousPlayerInRoom(PlayerModel previousPlayer) { }

		///<summary>
		///when current player join to room successfully
		///</summary>
		public virtual void OnJoined() { }

		///<summary>
		///when some player leave from room
		///</summary>
		public virtual void OnLeavePlayer(PlayerModel player) { }

		///<summary>
		///when some player disconneted from server
		///</summary>
		public virtual void OnDisconnetPlayer(PlayerModel player) { }

		///<summary>
		///when has room closed
		///</summary>
		public virtual void OnCloseRoom(RoomModel removedRoom, bool isYouMaster) { }

		///<summary>
		///when you have an error
		///</summary>
		public virtual void OnError(multiplayerErrors netError) { }

		///<summary>
		///when new data save on room
		///</summary>
		public virtual void NewDataOnRoomTrigger(PlayerModel recorder, string key) { }

		///<summary>
		///when new data delete from room
		///</summary>
		public virtual void DeleteDataFromRoomTrigger(PlayerModel recorder, string key) { }

		///<summary>
		///when new data delete from room
		///</summary>
		public virtual void OnChangeScoreOfSomePlayer(PlayerModel target, int newScore, ChangeScoreMode mode) { }
		public virtual void onChangeTurn(PlayerModel target) { }
		#endregion

		#region public methods

		public void AcceptFriendRequest(string RequestId)
		{
			ResponseFriendRequest(true, RequestId);
		}
		public void RejectFriendRequest(string RequestId)
		{
			ResponseFriendRequest(false, RequestId);

		}

		///<summary>
		///Join to room with id
		///</summary>

		public void JoinRoomWithId(string RoomId)
		{
			streamer.sendPacket(Packet.jsonToPacket("join", this._netId, new Dictionary<string, string>()
				{
					{"rid",RoomId }
				}));
		}

		/// ///<summary>
		///Get Game's Room id
		///</summary>
		public static async Task<List<string>> GetRooms()
		{
			ServerResponse res = await HttpRequest.GetRequestAsync("/game/rooms");
			if (res.IsSuccess)
			{
				JObject a = JObject.Parse(res.responseMessage);
				List<string> resp = JsonConvert.DeserializeObject<List<string>>(a["ids"].ToString());
				return resp;
			}
			else
			{
				return null;
			}
		}

		///<summary>
		///Change Score Of My Player
		///</summary>
		public void ChangeMyScore(uint newScore, ChangeScoreMode mode)
		{
			ChangeScore(PlayerInfo.netId, newScore, mode);
		}

		///<summary>
		///Change Score Of Some Player
		///</summary>
		public void ChangeScore(PlayerModel player, uint newScore, ChangeScoreMode mode)
		{
			ChangeScore(player.netId, newScore, mode);
		}

		///<summary>
		///Change Score Of Some Player
		///</summary>
		public void ChangeScore(string netId, uint newScore, ChangeScoreMode mode)
		{
			JObject packet = new JObject()
			{
				{"netId", netId},
				{"score",newScore},
				{"mode", (int)mode}
			};

			sendPacket("ch-point", PlayerInfo.netId, packet.ToString().Replace("\n", "").Replace(" ", "").Replace("\t", ""));
		}

		///<summary>
		///To Change Turn Of Room
		///</summary>
		public void ChangeTurn(ChangeTurnMode mode)
		{
			JObject packet = new JObject()
			{
				{"mode", (int)mode}
			};
			sendPacket("ch-turn", PlayerInfo.netId, packet.ToString().Replace("\n", "").Replace(" ", "").Replace("\t", ""));
		}
		///<summary>
		///To Change Turn to Specific Player
		///</summary>
		public void ChangeTurn(string netId)
		{
			JObject packet = new JObject()
			{
				{"target", netId},
			};
			sendPacket("ch-turn", PlayerInfo.netId, packet.ToString().Replace("\n", "").Replace(" ", "").Replace("\t", ""));
		}
		///<summary>
		///To Change Turn to Specific Player
		///</summary>
		public void ChangeTurn(PlayerModel model)
		{
			ChangeTurn(model.netId);
		}

		///<summary>
		///To Send Join Request to Spectific Player
		///</summary>
		public void SendFreindRequest(string PlayerParameter)
		{
			sendPacket("friend-request", PlayerInfo.netId, PlayerParameter);
		}

		///<summary>
		///Get All Game's Room details
		///</summary>
		public static async Task<List<RoomInfo>> GetAllRoomsWithDetails()
		{
			ServerResponse res = await HttpRequest.GetRequestAsync("/game/rooms/?mode=details");
			if (res.IsSuccess == true)
			{
				JObject result = JObject.Parse(res.responseMessage);
				return result["ids"].ToObject<List<RoomInfo>>();
			}
			return null;
		}

		private Animator anim;
		private AnimatorControllerParameter[] animatorControllerParameters;
		private bool lastbool;
		public void SendAnimation<T>(string AnimationName, T value)
		{
			anim = GetComponent<Animator>();
			AnimatorControllerParameter[] controllerParameters = anim.parameters;
			foreach (var VARIABLE in controllerParameters)
			{
				bool result;

				if (VARIABLE.type == AnimatorControllerParameterType.Bool)
				{
					result = anim.GetBool(VARIABLE.name);
					if (lastbool != result)
					{
						SendEvent("animator", result.ToString(), SendPacketMode.BroadCast);
					}

				}
			}
			eventSender event_sender = new eventSender();
			event_sender.eventName = "AnimationResult";
			event_sender.body = JsonConvert.SerializeObject(new Dictionary<string, dynamic>()
				{
					{"value",value},
				});
			event_sender.getter = null;
			event_sender.nsp = false;
			string packet = Packet.jsonToPacket<eventSender>("call-event", this.netId, event_sender);
			streamer.sendPacket(packet);
		}


		///<summary>
		///connect to server
		///</summary>
		public void ConnectToMultiPlayerServer()
		{
			if (!internetConnection)
			{
				Debug.LogError("[GamingCloud] Internet Connection Not Reachable! Please Try Again Later");
				return;
			}

			if (isConnected)
			{
				Debug.LogWarning("[GamingCloud] You are Connected To Server");
				return;
			}


			if (gameObject.GetComponent<UnityMainThreadDispatcher>() == null)
				gameObject.AddComponent(typeof(UnityMainThreadDispatcher));

			streamer = new tcpStreamer(BASE_URL, PORT, packetSize);
			streamer.OnPacketRecieve += listenData;
			streamer.OnConnected += () =>
			{
				UnityMainThreadDispatcher.Instance().Enqueue(() =>
					{

						this._isConnected = true;
						Dictionary<string, string> payload = GCPolicy.GetRequiredQueries(GCPolicy.QueryMode.TCP);

						string pid;
						if (this.testPlayerId.Length > 0)
						{
							pid = this.testPlayerId;
						}
						else if (!Players.IsLogin)
						{
							Debug.LogError("[GamingCloud] You are supposed to login to gamingcloud player account!");
							return;
						}
						else
							pid = Players.PlayerToken;

						if (pid == null || pid.Trim().Length == 0)
						{
							Debug.LogError("[GamingCloud] You are supposed to login to gamingcloud player account!");
							return;
						}
						payload.Add("PlayerId", pid);
						this.sendPacket("authentication", this._netId, JsonConvert.SerializeObject(payload));
					});

			};

		}

		///<summary>
		///join to some room
		///<param name="roomSettings">pass room setting if you want customize your room setting</param>
		///</summary>
		public void JoinToServer(RoomSetting roomSettings)
		{
			streamer.sendPacket(Packet.jsonToPacket("join", this._netId, roomSettings));
		}

		///<summary>
		///destroy all clients of some player as localy
		///<param name="netId">player netId</param>
		///</summary>
		public void DestroyAllClients(string netId)
		{
			if (!_clients.ContainsKey(netId))
				return;

			List<StreambleObjectTag> _objects = _clients[netId];
			foreach (StreambleObjectTag item in _objects)
				Destroy(item.instance);
		}

		///<summary>
		///destroy specific client of some player as localy
		///<param name="netId">player netId</param>
		///<param name="clientId">client name</param>
		///</summary>
		public void DestroyAllClients(string netId, string clientId)
		{
			if (!_clients.ContainsKey(netId))
				return;

			List<StreambleObjectTag> _objects = _clients[netId];
			foreach (StreambleObjectTag item in _objects)
				if (item.id == clientId)
				{
					Destroy(item.instance);
					break;
				}
		}

		///<summary>
		///destroy all clients of some player as localy
		///<param name="player">player model</param>
		///</summary>
		public void DestroyAllClients(PlayerModel player)
		{
			if (!_clients.ContainsKey(player.netId))
				return;

			List<StreambleObjectTag> _objects = _clients[player.netId];
			foreach (StreambleObjectTag item in _objects)
				Destroy(item.instance);
		}

		///<summary>
		///destroy specific client of some player as localy
		///<param name="player">player model</param>
		///<param name="clientId">client name</param>
		///</summary>
		public void DestroyAllClients(PlayerModel player, string clientId)
		{
			if (!_clients.ContainsKey(player.netId))
				return;

			List<StreambleObjectTag> _objects = _clients[player.netId];
			foreach (StreambleObjectTag item in _objects)
				if (item.id == clientId)
				{
					Destroy(item.instance);
					break;
				}
		}


		///<summary>
		///instatiate clients manualy
		///<param name="PlayerNetId">player netId</param>
		///</summary>
		public void InstatiateStreambleObjects(string PlayerNetId)
		{

			if (PlayerNetId == _netId)
			{
				Debug.LogError("[GamingCloud] you Can't instatiate clients for Player!");
				return;
			}

			if (_clients.ContainsKey(PlayerNetId))
			{
				Debug.LogError("[GamingCloud] you Can't instatiate all client for twice!");
				return;
			}

			UnityMainThreadDispatcher.Instance().Enqueue(() =>
				{
					List<StreambleObjectTag> _objects = new List<StreambleObjectTag>();

					foreach (GameObject item in ClientObjects)
					{
						StreambleObjectTag sot = item.GetComponent<StreambleObjectTag>();
						sot.netId = PlayerNetId;

						Vector3 pos = Vector3.zero;
						ClientSpawnPoint spawnPoint = item.GetComponent<ClientSpawnPoint>();
						if (!spawnPoint)
							Debug.LogWarning("[GamingCloud] Use Spawn Point For Your Client to spawn automaticly!");
						else
						{
							if (spawnPoint.SpawnPointName == null || spawnPoint.SpawnPointName == string.Empty)
								Debug.LogError("[GamingCloud] " + sot.id + " Have not any SpawnPointName!");
							else
							{
								GameObject findedSpPoint;
								string query;

								int client_spawnIndex = -1;
								for (int i = 0; i < this._room.players.Count; i++)
								{
									if (this._room.players[i].netId == PlayerNetId)
									{
										client_spawnIndex = i;
										break;
									}
								}
								if (spawnPoint.SearchType == SpawnClientType.ByObjectName)
								{
									query = spawnPoint.SpawnPointName;
									if (spawnPoint.IsPrefix)
										query += client_spawnIndex;

									findedSpPoint = GameObject.Find(query);
								}
								else
								{
									query = spawnPoint.SpawnPointName;
									if (spawnPoint.IsPrefix)
										query += client_spawnIndex;

									findedSpPoint = GameObject.FindGameObjectWithTag(query);
								}

								if (findedSpPoint == null)
									Debug.LogError("[GamingCloud] any object couldn't find with this " + (spawnPoint.SearchType == SpawnClientType.ByTag ? "tag" : "") + " name: " + query);
								else
									pos = findedSpPoint.transform.position;
							}
						}

						GameObject _iobj = Instantiate(item, pos, Quaternion.identity);
						_iobj.name = PlayerNetId + ":" + sot.id;
						_objects.Add(_iobj.GetComponent<StreambleObjectTag>());
					}

					_clients.Add(PlayerNetId, _objects);
				});
		}



		#region Save Data on Room
		public void SaveDataOnRoom<T>(string key, T value)
		{
			Dictionary<string, dynamic> body = new Dictionary<string, dynamic>();
			body.Add("key", key);
			body.Add("value", value);

			sendPacket("set-data-room", netId, JsonConvert.SerializeObject(body));
		}
		public T getDataOnRoom<T>(string key)
		{
			return room.data[key].ToObject<T>();
		}
		public bool existRoomData(string key)
		{
			return room.data[key] != null;
		}
		public void DeleteDataFromRoom(string key)
		{
			sendPacket("del-data-room", netId, key);
		}
		#endregion

		#region Events
		public void SendEvent(string eventName, SendPacketMode mode = SendPacketMode.BroadCast)
		{
			eventSender event_sender = new eventSender();
			event_sender.eventName = eventName;
			event_sender.body = null;
			event_sender.getter = null;
			event_sender.nsp = mode == SendPacketMode.nspMode;
			string packet = Packet.jsonToPacket<eventSender>("call-event", this.netId, event_sender);
			streamer.sendPacket(packet);
		}
		public void SendEvent(string eventName, SendPacketMode mode, string clientId)
		{
			eventSender event_sender = new eventSender();
			event_sender.eventName = eventName;
			event_sender.body = null;
			List<string> ids = new List<string>();
			ids.Add(clientId);
			event_sender.getter = JsonConvert.SerializeObject(ids);
			event_sender.nsp = mode == SendPacketMode.nspMode;
			string packet = Packet.jsonToPacket<eventSender>("call-event", this.netId, event_sender);
			streamer.sendPacket(packet);
		}
		public void SendEvent(string eventName, SendPacketMode mode, string[] clientIds)
		{
			eventSender event_sender = new eventSender();
			event_sender.eventName = eventName;
			event_sender.body = null;
			event_sender.getter = JsonConvert.SerializeObject(clientIds);
			event_sender.nsp = mode == SendPacketMode.nspMode;
			string packet = Packet.jsonToPacket<eventSender>("call-event", this.netId, event_sender);
			streamer.sendPacket(packet);
		}

		public void SendEvent(string eventName, string body, SendPacketMode mode = SendPacketMode.BroadCast)
		{
			eventSender event_sender = new eventSender();
			event_sender.eventName = eventName;
			event_sender.body = body;
			event_sender.getter = null;
			event_sender.nsp = mode == SendPacketMode.nspMode;
			string packet = Packet.jsonToPacket<eventSender>("call-event-with-body", this.netId, event_sender);
			streamer.sendPacket(packet);
		}
		public void SendEvent(string eventName, string body, SendPacketMode mode, string clientId)
		{
			eventSender event_sender = new eventSender();
			event_sender.eventName = eventName;
			event_sender.body = body;
			List<string> ids = new List<string>();
			ids.Add(clientId);
			event_sender.getter = JsonConvert.SerializeObject(ids);
			event_sender.nsp = mode == SendPacketMode.nspMode;
			string packet = Packet.jsonToPacket<eventSender>("call-event-with-body", this.netId, event_sender);
			streamer.sendPacket(packet);
		}
		public void SendEvent(string eventName, string body, SendPacketMode mode, string[] clientIds)
		{
			eventSender event_sender = new eventSender();
			event_sender.eventName = eventName;
			event_sender.body = body;
			event_sender.getter = JsonConvert.SerializeObject(clientIds);
			event_sender.nsp = mode == SendPacketMode.nspMode;
			string packet = Packet.jsonToPacket<eventSender>("call-event-with-body", this.netId, event_sender);
			streamer.sendPacket(packet);
		}
		public void SendEvent(string eventName, string body, SendPacketMode mode, PlayerModel model)
		{
			eventSender event_sender = new eventSender();
			event_sender.eventName = eventName;
			event_sender.body = body;
			List<string> ids = new List<string>();
			ids.Add(model.netId);
			event_sender.getter = JsonConvert.SerializeObject(ids);
			event_sender.nsp = mode == SendPacketMode.nspMode;
			string packet = Packet.jsonToPacket<eventSender>("call-event-with-body", this.netId, event_sender);
			streamer.sendPacket(packet);
		}
		public void SendEvent(string eventName, string body, SendPacketMode mode, PlayerModel[] models)
		{
			eventSender event_sender = new eventSender();
			event_sender.eventName = eventName;
			event_sender.body = body;
			List<string> ids = new List<string>();
			foreach (PlayerModel item in models)
			{
				ids.Add(item.netId);
			}
			event_sender.getter = JsonConvert.SerializeObject(ids);
			event_sender.nsp = mode == SendPacketMode.nspMode;
			string packet = Packet.jsonToPacket<eventSender>("call-event-with-body", this.netId, event_sender);
			streamer.sendPacket(packet);
		}
		public void SendEvent(string eventName, SendPacketMode mode, PlayerModel model)
		{
			eventSender event_sender = new eventSender();
			event_sender.eventName = eventName;
			event_sender.body = null;
			List<string> ids = new List<string>();
			ids.Add(model.netId);
			event_sender.getter = JsonConvert.SerializeObject(ids);
			event_sender.nsp = mode == SendPacketMode.nspMode;
			string packet = Packet.jsonToPacket<eventSender>("call-event", this.netId, event_sender);
			streamer.sendPacket(packet);
		}
		public void SendEvent(string eventName, SendPacketMode mode, PlayerModel[] models)
		{
			eventSender event_sender = new eventSender();
			event_sender.eventName = eventName;
			event_sender.body = null;
			List<string> ids = new List<string>();
			foreach (PlayerModel item in models)
			{
				ids.Add(item.netId);
			}
			event_sender.getter = JsonConvert.SerializeObject(ids);
			event_sender.nsp = mode == SendPacketMode.nspMode;
			string packet = Packet.jsonToPacket<eventSender>("call-event", this.netId, event_sender);
			streamer.sendPacket(packet);
		}


		#endregion

		#region GetClients
		List<StreambleObjectTag> _getClients(string netId)
		{
			return _clients[netId];
		}

		///<summary>
		///get list of clients
		///<param name="netId">player netId</param>
		///</summary>
		public List<StreambleObjectTag> getClients(string netId)
		{
			return _getClients(netId);
		}

		///<summary>
		///get list of clients
		///<param name="player">player model</param>
		///</summary>
		public List<StreambleObjectTag> getClients(PlayerModel player)
		{
			return _getClients(player.netId);
		}
		#endregion

		#region compare netId
		///<summary>
		///check input its current player or not!
		///<param name="netId">player netId</param>
		///</summary>
		public bool compareIdWithMe(string netId)
		{
			return this._netId == netId;
		}

		///<summary>
		///check input its current player or not!
		///<param name="player">player model</param>
		///</summary>
		public bool compareIdWithMe(PlayerModel player)
		{
			return this._netId == player.netId;
		}
		#endregion

		#region find player in room
		///<summary>
		///find player model in room
		///<param name="netId">player netId</param>
		///</summary>
		public PlayerModel findPlayerInRoom(string netId)
		{
			if (_room == null)
				return null;

			PlayerModel player = null;

			foreach (PlayerModel _player in _room.players)
			{
				if (_player.netId == netId)
				{
					player = _player;
					break;
				}
			}

			return player;
		}

		///<summary>
		///find player model in room
		///<param name="player">player model</param>
		///</summary>
		public PlayerModel findPlayerInRoom(PlayerModel player)
		{
			if (_room == null)
				return null;

			PlayerModel found_player = null;

			foreach (PlayerModel _player in _room.players)
			{
				if (_player.netId == found_player.netId)
				{
					found_player = _player;
					break;
				}
			}

			return found_player;
		}
		#endregion

		///<summary>
		///leave player from room
		///</summary>
		public void leaveRoom()
		{
			streamer.sendPacket(Packet.packPacket("leave-player", this._netId, null));
			this._room = null;
			this._PlayerInfo = null;
			this._isMasterPlayer = false;

			this.removeAllClients();
		}

		///<summary>
		///disconnect from server
		///</summary>
		public void disconnect()
		{

			if (streamer != null)
			{
				streamer.OnPacketRecieve -= listenData;
				streamer.Disconnect();
			}

		}

		///<summary>
		///get player info by player model
		///<param name="player">player model</param>
		///</summary>
		public async Task<T> GetPlayerInfo<T>(PlayerModel player)
		{
			return await _GetPlayerInfo<T>(player.netId);
		}
		///<summary>
		///get player info by netId
		///<param name="netId">player netId</param>
		///</summary>
		public async Task<T> GetPlayerInfo<T>(string netId)
		{
			return await _GetPlayerInfo<T>(netId);
		}
		#endregion

		#region private methods
		private void removeAllClients()
		{

			foreach (KeyValuePair<string, List<StreambleObjectTag>> item in this._clients)
			{
				foreach (StreambleObjectTag _object in item.Value)
					Destroy(_object.instance);
			}

			this._clients = new Dictionary<string, List<StreambleObjectTag>>();
		}

		private void ResponseFriendRequest(bool isAccept, string RequestId)
		{
			sendPacket("friend-request-response", PlayerInfo.netId, JsonConvert.SerializeObject(new Dictionary<string, dynamic>()
				{
					{"status",(isAccept)?1:0},
					{"id",RequestId},
				}));
		}

		private void CallEvent(string eventName, PlayerModel player)
		{
			Type parentType = typeof(GCMultiPlayer);
			Assembly assembly = Assembly.GetExecutingAssembly();
			Type[] types = assembly.GetTypes();

			IEnumerable<Type> subclasses = types.Where(t => t.IsSubclassOf(parentType));

			foreach (Type type in subclasses)
			{
				Type instanceClass = Type.GetType(type.Name);

				MethodInfo[] methodInfos = instanceClass.GetMethods(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);
				object[] _params = { player };
				foreach (MethodInfo item in methodInfos)
				{
					if (item.Name == eventName)
						UnityMainThreadDispatcher.Instance().Enqueue(() => item.Invoke(this, _params));
				}
			}
		}
		private void CallEvent(string eventName, PlayerModel player, string body)
		{
			Type parentType = typeof(GCMultiPlayer);
			Assembly assembly = Assembly.GetExecutingAssembly();
			Type[] types = assembly.GetTypes();

			IEnumerable<Type> subclasses = types.Where(t => t.IsSubclassOf(parentType));

			foreach (Type type in subclasses)
			{
				Type instanceClass = Type.GetType(type.Name);

				MethodInfo[] methodInfos = instanceClass.GetMethods(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);
				object[] _params = { player, body };
				foreach (MethodInfo item in methodInfos)
				{
					if (item.Name == eventName)
						UnityMainThreadDispatcher.Instance().Enqueue(() => item.Invoke(this, _params));
				}
			}
		}
		private StreambleObjectTag findStreambleObjectInfo(string name)
		{
			if (isTurn == false)
			{
				foreach (StreambleObjectTag tag in StreamObjects.instance.streambleObjectTags)
					if (tag.name == name)
						return tag;
			}
			else
				Debug.LogWarning("[GamingCloud] Your game type is turnbased!");

			return null;
		}
		private void sendPacket(string issue, string sender, string body)
		{
			string _packet = Packet.packPacket(issue, sender, body);
			streamer.sendPacket(_packet);
		}
		private void listenData(string recievePacket)
		{
			string[] messages = recievePacket.Substring(0, recievePacket.Length - 1).Split(';');
			foreach (string message in messages)
			{
				Packet packet = Packet.unPackPacket(message);
				_serverTs = packet.ts;
				switch (packet.issue)
				{
				case "authOK2":
					JObject joo = JObject.Parse(packet.body);
					isTurn = joo["isTurn"].ToObject<bool>();
					_netId = joo["netId"].ToObject<string>();
					UnityMainThreadDispatcher.Instance().Enqueue(() =>
						{
							_clients = new Dictionary<string, List<StreambleObjectTag>>();

							if (isTurn == false)
							{
								gameObject.AddComponent<StreamObjects>();
								gameObject.AddComponent<GCAnimatorHelper>();
								StreamObjects.instance.streamer = streamer;
								GCAnimatorHelper.instance.streamer = streamer;
								StreamObjects.instance.SetNetId(netId);
							}

							gameObject.AddComponent<PingCalculator>();
							PingCalculator.instance.serverConnected = true;
							ConnectedToServer();

							UnityMainThreadDispatcher.Instance().Enqueue(() => InvokeRepeating("timerTick", 0, 1f));

						});
					break;

				case "call-event":
					PlayerModel player = this.findPlayerInRoom(packet.sender);
					eventRecieve eve_recv = JsonConvert.DeserializeObject<eventRecieve>(packet.body);
					this.CallEvent(eve_recv.eventName, player);
					break;

				case "call-event-with-body":
					PlayerModel player2 = this.findPlayerInRoom(packet.sender);
					eventRecieve eve_recv2 = JsonConvert.DeserializeObject<eventRecieve>(packet.body);
					this.CallEvent(eve_recv2.eventName, player2, eve_recv2.body);
					break;

				case "updateQeue":
					if (isTurn == false)
					{

						UnityMainThreadDispatcher.Instance().Enqueue(() =>
							{
								NetWorkTransform transform = Packet.convertTotransform(packet.body);
								StreamObjects.instance.updateQeue(packet.sender, transform);
							});
					}
					else
						Debug.LogWarning("[GamingCloud] Your game type is turnbased!");
					break;

				case "join":
					OnjoinPlayer joinPlayerStatus = JsonConvert.DeserializeObject<OnjoinPlayer>(packet.body);

					PlayerModel playerModel = new PlayerModel(joinPlayerStatus.player.netId, joinPlayerStatus.player.name, joinPlayerStatus.player.role);

					this._isMasterPlayer = joinPlayerStatus.room.roomCreator.netId == _netId;
					this._room = joinPlayerStatus.room;

					UnityMainThreadDispatcher.Instance().Enqueue(() =>
						{
							// اگر جدید است
							if (!joinPlayerStatus.pre)
							{
								// اگر فرستنده با بازیکن الان یکی باشد
								if (packet.sender == _netId)
								{
									_PlayerInfo = playerModel;
									this.OnJoined(); // جوین شدی!
									if (isTurn == false)
										StreamObjects.instance.StartStream();
								}
								else
								{
									// بازیکن جدید اومده
									if (!isTurn)
                                    {
										foreach (var VARIABLE in GCAnimatorHelper.instance.anims)
										{
											VARIABLE.AddLayersToPacket(this.netId);
											streamer.sendPacket(Packet.packPacket("update-anim",
												this.netId + ":" + VARIABLE.streambleObjectTags.id, JsonConvert.SerializeObject(VARIABLE.lastPacketModel)));
										}
									}
									this.OnJoinNewPlayer(playerModel);
								}
							}
							else
							{
								// این بازیکن قبلا توی اتاق بوده
								foreach (var VARIABLE in joinPlayerStatus.room.players)
								{
									if (PlayerInfo.netId != VARIABLE.netId)
										this.PreviousPlayerInRoom(VARIABLE);
								}
							}

							if (instatiateClientsOnJoin && _netId != packet.sender)
								this.InstatiateStreambleObjects(packet.sender);

						});
					break;

				case "removeRoom":
					OnCloseRoom(_room, _isMasterPlayer);
					_room = null;
					_isMasterPlayer = false;
					break;

				case "update-anim":
					UnityMainThreadDispatcher.Instance().Enqueue(() => StartCoroutine(PlayAnimation(packet.body, packet.sender)));
					break;
				case "update-room-data":
					JObject jo = JObject.Parse(packet.body);
					string key = jo["key"].ToObject<string>();

					if (jo["dl"].ToObject<bool>())
					{
						_room.data.Remove(key);
						UnityMainThreadDispatcher.Instance().Enqueue(() =>
							DeleteDataFromRoomTrigger(findPlayerInRoom(packet.sender), key));
					}
					else
					{
						_room.data = jo["data"].ToObject<JObject>();
						UnityMainThreadDispatcher.Instance().Enqueue(() =>
							NewDataOnRoomTrigger(findPlayerInRoom(packet.sender), key));
					}

					break;
				case "friend-response":

					UnityMainThreadDispatcher.Instance().Enqueue(() =>
						OnSendRequestStatus((FriendRequestResponse)int.Parse(packet.body)));

					break;

				case "friend-request":
					UnityMainThreadDispatcher.Instance().Enqueue(() =>
						{
							JObject jo2 = JObject.Parse(packet.body);
							OnNewFriendRequest(jo2["requestId"].ToObject<string>(), jo2["playerId"].ToObject<string>());
						});

					break;

				case "friend-request-response":
					UnityMainThreadDispatcher.Instance().Enqueue(() =>
						{
							JObject jo1 = JObject.Parse(packet.body);

							if (jo1["cid"].ToObject<string>() != _netId)
								OnFriendRequestResponse(jo1["status"].ToObject<string>() == "1" ? FriendRequestResponse.RequestAcceptedAndJoined : FriendRequestResponse.RequestRejected);
							else if (jo1["status"].ToObject<string>() == "1")
								JoinRoomWithId(jo1["rid"].ToObject<string>());

						});
					break;

				case "ch-turn":
					JObject jbody_turn = JObject.Parse(packet.body);
					room.turn = jbody_turn["nturn"].ToObject<string>();

					onChangeTurn(room.CurrentTurn);
					break;


				case "ch-point":
					JObject jbody = JObject.Parse(packet.body);
					string recv_netId = jbody["netId"].ToObject<string>();
					int nscore = jbody["nscore"].ToObject<int>();

					print("asd"+nscore);

					int index = room.players.FindIndex(v => v.netId == recv_netId);

					if (recv_netId == netId)
						PlayerInfo.score = nscore;
					if (recv_netId == room.roomCreator.netId)
						room.roomCreator.score = nscore;
					room.players[index].score = nscore;


					OnChangeScoreOfSomePlayer(room.players[index], nscore, (ChangeScoreMode)jbody["mode"].ToObject<int>());
					break;

				case "leave-player":
				case "dissconnect":
					UnityMainThreadDispatcher.Instance().Enqueue(() =>
						{
							disconnectPlayer dc = JsonConvert.DeserializeObject<disconnectPlayer>(packet.body);

							if (this.removeClientsAfterDC)
								DestroyAllClients(dc.player.netId);

							_clients.Remove(dc.player.netId);

							_room = dc.room;
							_isMasterPlayer = _room.roomCreator.netId == _netId;

							if (dc.isLeave)
								OnLeavePlayer(dc.player);
							else
								OnDisconnetPlayer(dc.player);
						});

					break;

				case "error":
					try
					{
						int status = int.Parse(packet.body);
						multiplayerErrors error = (multiplayerErrors)status;
						UnityMainThreadDispatcher.Instance().Enqueue(() =>
							{
								OnError(error);
							});

					}
					catch (System.Exception) { }
					break;
				}
			}
		}
		private Dictionary<string, GCNetworkAnimation> _animatorsCache = new Dictionary<string, GCNetworkAnimation>();
		IEnumerator PlayAnimation(string body, string sender)
		{

			yield return new WaitForSeconds(0.05f);
			UnityMainThreadDispatcher.Instance().Enqueue(() =>
				{
					AnimationPacketModel model = JsonConvert.DeserializeObject<AnimationPacketModel>(body);
					string _netId = sender;
					if (_netId.Split(':')[0] == this.netId)
						return;

					if (!_animatorsCache.ContainsKey(_netId))
					{
						animation = GameObject.Find(_netId).GetComponent<GCNetworkAnimation>();
						_animatorsCache.Add(_netId, animation);
					}
					else
					{
						animation = _animatorsCache[_netId];
					}
					if (model.up == false)
					{
						foreach (var VARIABLE in model.animations)
						{
							animation.ApplyAnimation(_netId, VARIABLE, model);
						}
					}
					else
					{
						LastAnimationPacketModel LastModel = JsonConvert.DeserializeObject<LastAnimationPacketModel>(body);
						StartCoroutine(PlayRestAnimation(LastModel, sender));
					}
				});
		}

		IEnumerator PlayRestAnimation(LastAnimationPacketModel model, string sender)
		{
			yield return new WaitForSeconds(0.05f);
			UnityMainThreadDispatcher.Instance().Enqueue(() =>
				{
					foreach (var VARIABLE in model.animations)
						animation.PlayRestOfAnimation(VARIABLE, sender, model);

				});
		}
		private void timerTick()
		{
			_serverTs += 1000;
		}
		private async Task<T> _GetPlayerInfo<T>(string netId)
		{
			string _parsedRoomId = UnityWebRequest.EscapeURL(room.id);
			string _parsedNetId = UnityWebRequest.EscapeURL(netId);

			var resp = await HttpRequest.GetRequestAsync("/players/v2/multi/" + _parsedRoomId + "/" + _parsedNetId);
			return JsonConvert.DeserializeObject<T>(resp.responseMessage);
		}
		void OnDestroy()
		{
			this.disconnect();
		}
		#endregion
	}
}
