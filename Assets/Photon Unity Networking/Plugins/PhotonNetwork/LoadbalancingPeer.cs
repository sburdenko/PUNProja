// ----------------------------------------------------------------------------
// <copyright file="LoadbalancingPeer.cs" company="Exit Games GmbH">
//   Loadbalancing Framework for Photon - Copyright (C) 2011 Exit Games GmbH
// </copyright>
// <summary>
//   Provides the operations needed to use the loadbalancing server app(s).
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.Lite;

public class LoadbalancingPeer : PhotonPeer
{
    public LoadbalancingPeer(IPhotonPeerListener listener, ConnectionProtocol protocolType) : base(listener, protocolType)
    {
    }

    public bool OpJoinLobby()
    {
        if (this.DebugOut >= DebugLevel.INFO)
        {
            this.Listener.DebugReturn(DebugLevel.INFO, "JoinLobby()");
        }

        return this.OpCustom(OperationCode.JoinLobby, null, true);
    }

    public bool OpLeaveLobby()
    {
        if (this.DebugOut >= DebugLevel.INFO)
        {
            this.Listener.DebugReturn(DebugLevel.INFO, "OpLeaveLobby()");
        }

        return this.OpCustom(OperationCode.LeaveLobby, null, true);
    }

    /// <summary>
    /// Don't use this method directly, unless you know how to cache and apply customActorProperties.
    /// The PhotonNetwork methods will handle player and room properties for you and call this method.
    /// </summary>
    public virtual bool OpCreateGame(string gameID, bool isVisible, bool isOpen, byte maxPlayers, bool autoCleanUp, Hashtable customGameProperties, Hashtable customActorProperties, string[] customRoomPropertiesForLobby)
    {
        if (this.DebugOut >= DebugLevel.INFO)
        {
            this.Listener.DebugReturn(DebugLevel.INFO, "OpCreateGame()");
        }

        Hashtable gameProperties = new Hashtable();
        gameProperties[GameProperties.IsOpen] = isOpen;
        gameProperties[GameProperties.IsVisible] = isVisible;
        gameProperties[GameProperties.PropsListedInLobby] = customRoomPropertiesForLobby;
        gameProperties.MergeStringKeys(customGameProperties);
        if (maxPlayers > 0)
        {
            gameProperties[GameProperties.MaxPlayers] = maxPlayers;
        }

        Dictionary<byte, object> op = new Dictionary<byte, object>();
        op[ParameterCode.GameProperties] = gameProperties;
        op[ParameterCode.ActorProperties] = customActorProperties;
        op[ParameterCode.Broadcast] = true;

        if (!string.IsNullOrEmpty(gameID))
        {
            op[ParameterCode.GameId] = gameID;
        }

        if (autoCleanUp)
        {
            op[ParameterCode.CleanupCacheOnLeave] = autoCleanUp;
            gameProperties[GameProperties.CleanupCacheOnLeave] = autoCleanUp;
        }

        Listener.DebugReturn(DebugLevel.INFO, OperationCode.CreateGame + ": " + SupportClass.DictionaryToString(op));
        return this.OpCustom(OperationCode.CreateGame, op, true);
    }

    public virtual bool OpJoin(string gameID, Hashtable actorProperties)
    {
        if (this.DebugOut >= DebugLevel.INFO)
        {
            this.Listener.DebugReturn(DebugLevel.INFO, "OpJoin()");
        }

        if (string.IsNullOrEmpty(gameID))
        {
            this.Listener.DebugReturn(DebugLevel.ERROR, "OpJoin() failed. Please specify a roomname.");
            return false;
        }

        Dictionary<byte, object> op = new Dictionary<byte, object>();
        op[ParameterCode.GameId] = gameID;
        op[ParameterCode.ActorProperties] = actorProperties;
        op[ParameterCode.Broadcast] = true;

        Listener.DebugReturn(DebugLevel.INFO, OperationCode.JoinGame + ": " + SupportClass.DictionaryToString(op));
        return this.OpCustom(OperationCode.JoinGame, op, true);
    }

    /// <remarks>the hashtable is (optionally) used to filter games: only those that fit the contained custom properties will be matched</remarks>
    public virtual bool OpJoinRandom(Hashtable expectedGameProperties)
    {
        if (this.DebugOut >= DebugLevel.INFO)
        {
            this.Listener.DebugReturn(DebugLevel.INFO, "OpJoinRandom()");
        }

        Dictionary<byte, object> op = new Dictionary<byte, object>();
        if (expectedGameProperties != null && expectedGameProperties.Count > 0)
        {
            op[ParameterCode.Properties] = expectedGameProperties;
        }

        Listener.DebugReturn(DebugLevel.INFO, OperationCode.JoinRandomGame + ": " + SupportClass.DictionaryToString(op));
        return this.OpCustom(OperationCode.JoinRandomGame, op, true);
    }

    public bool OpSetCustomPropertiesOfActor(int actorNr, Hashtable actorProperties, bool broadcast, byte channelId)
    {
        return this.OpSetPropertiesOfActor(actorNr, actorProperties.StripToStringKeys(), broadcast, channelId);
    }

    protected bool OpSetPropertiesOfActor(int actorNr, Hashtable actorProperties, bool broadcast, byte channelId)
    {
        if (this.DebugOut >= DebugLevel.INFO)
        {
            this.Listener.DebugReturn(DebugLevel.INFO, "OpSetPropertiesOfActor()");
        }
            
        Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
        opParameters.Add(ParameterCode.Properties, actorProperties);
        opParameters.Add(ParameterCode.ActorNr, actorNr);
        if (broadcast)
        {
            opParameters.Add(ParameterCode.Broadcast, broadcast);
        }

        return this.OpCustom((byte)OperationCode.SetProperties, opParameters, broadcast, channelId);
    }

    protected void OpSetPropertyOfGame(byte propCode, object value)
    {
        Hashtable properties = new Hashtable();
        properties[propCode] = value;
        this.OpSetPropertiesOfGame(properties, true, (byte)0);
    }

    public bool OpSetCustomPropertiesOfGame(Hashtable gameProperties, bool broadcast, byte channelId)
    {
        return this.OpSetPropertiesOfGame(gameProperties.StripToStringKeys(), broadcast, channelId);
    }

    public bool OpSetPropertiesOfGame(Hashtable gameProperties, bool broadcast, byte channelId)
    {
        if (this.DebugOut >= DebugLevel.INFO)
        {
            this.Listener.DebugReturn(DebugLevel.INFO, "OpSetPropertiesOfGame()");
        }

        Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
        opParameters.Add(ParameterCode.Properties, gameProperties);
        if (broadcast)
        {
            opParameters.Add(ParameterCode.Broadcast, broadcast);
        }

        return this.OpCustom((byte)OperationCode.SetProperties, opParameters, broadcast, channelId);
    }

    protected bool OpAuthenticate(string appId, string appVersion)
    {
        if (this.DebugOut >= DebugLevel.INFO)
        {
            this.Listener.DebugReturn(DebugLevel.INFO, "OpAuthenticate()");
        }

        Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
        opParameters[ParameterCode.AppVersion] = appVersion;
        opParameters[ParameterCode.ApplicationId] = appId;

        return this.OpCustom(OperationCode.Authenticate, opParameters, true, (byte)0, false);
    }
        
    public virtual bool OpRaiseEvent(byte eventCode, Hashtable evData, bool sendReliable, byte channelId)
    {
        return this.OpRaiseEvent(eventCode, evData, sendReliable, channelId, EventCaching.DoNotCache, ReceiverGroup.Others);
    }

    public virtual bool OpRaiseEvent(byte eventCode, Hashtable evData, bool sendReliable, byte channelId, int[] targetActors)
    {
        return this.OpRaiseEvent(eventCode, evData, sendReliable, channelId, targetActors, EventCaching.DoNotCache);
    }

    public virtual bool OpRaiseEvent(byte eventCode, Hashtable evData, bool sendReliable, byte channelId, int[] targetActors, EventCaching cache)
    {
        if (this.DebugOut >= DebugLevel.INFO)
        {
            this.Listener.DebugReturn(DebugLevel.INFO, "OpRaiseEvent()");
        }

        Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
        opParameters[ParameterCode.Data] = evData;
        opParameters[ParameterCode.Code] = (byte)eventCode;

        if (cache != EventCaching.DoNotCache)
        {
            opParameters[ParameterCode.Cache] = (byte)cache;
        }

        if (targetActors != null)
        {
            opParameters[ParameterCode.ActorList] = targetActors;
        }

        return this.OpCustom(OperationCode.RaiseEvent, opParameters, sendReliable, channelId);
    }

    public virtual bool OpRaiseEvent(byte eventCode, Hashtable evData, bool sendReliable, byte channelId, EventCaching cache, ReceiverGroup receivers)
    {
        if (this.DebugOut >= DebugLevel.INFO)
        {
            this.Listener.DebugReturn(DebugLevel.INFO, "OpRaiseEvent()");
        }

        Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
        opParameters[ParameterCode.Data] = evData;
        opParameters[ParameterCode.Code] = (byte)eventCode;

        if (receivers != ReceiverGroup.Others)
        {
            opParameters[ParameterCode.ReceiverGroup] = (byte)receivers;
        }

        if (cache != EventCaching.DoNotCache)
        {
            opParameters[ParameterCode.Cache] = (byte)cache;
        }

        return this.OpCustom((byte)OperationCode.RaiseEvent, opParameters, sendReliable, channelId);
    }
}

public class ActorProperties
{
    public const byte PlayerName = 255; // was: 1
}

public class ErrorCode
{
    // server - Photon low(er) level: <= 0
    /// <summary>Operation can't be executed (e.g. RaiseEvent cant be used before getting into a room).</summary>
    public const int OperationNotAllowedInCurrentState = -3;
    /// <summary>The operation you called is not implemented on the server (application) you connect to. Make sure you run the fitting applications.</summary>
    public const int InvalidOperationCode = -2;
    /// <summary>Something went wrong in the server. Try to reproduce and contact Exit Games.</summary>
    public const int InternalServerError = -1;
    /// <summary>Operation done.</summary>
    public const int Ok = 0;

    // server - PhotonNetwork: 0x7FFF and down
    // logic-level error codes start with short.max

    /// <summary>Authentication failed. Possible cause: AppId is unknown to Photon (in cloud service).</summary>
    public const int InvalidAuthentication = 0x7FFF;
    /// <summary>GameId (name) already in use (can't create another). Change name.</summary>
    public const int GameIdAlreadyExists = 0x7FFF - 1;
    /// <summary>Game is full. This can when players took over while you joined the game.</summary>
    public const int GameFull = 0x7FFF - 2;
    /// <summary>Game is closed and can't be joined. Join another game.</summary>
    public const int GameClosed = 0x7FFF - 3;
    [Obsolete("No longer used, cause random matchmaking is no longer a process.")]
    public const int AlreadyMatched = 0x7FFF - 4;
    /// <summary>Not in use currently.</summary>
    public const int ServerFull = 0x7FFF - 5;
    /// <summary>Not in use currently.</summary>
    public const int UserBlocked = 0x7FFF - 6;
    /// <summary>Random matchmaking only succeeds if a room exists thats neither closed nor full. Repeat in a few seconds or create a new room.</summary>
    public const int NoRandomMatchFound = 0x7FFF - 7;
    /// <summary>Join can fail if the room (name) is not existing (anymore). This can happen when players leave while you join.</summary>
    public const int GameDoesNotExist = 0x7FFF - 9;
}

public class GameProperties
{
    public const byte MaxPlayers = 255;
    public const byte IsVisible = 254;
    public const byte IsOpen = 253;
    public const byte PlayerCount = 252;
    public const byte Removed = 251;
    public const byte PropsListedInLobby = 250;
    /// <summary>Equivalent of Operation Join parameter CleanupCacheOnLeave.</summary>
    public const byte CleanupCacheOnLeave = 249;
}

public class EventCode
{
    public const byte GameList = 230;
    public const byte GameListUpdate = 229;
    public const byte QueueState = 228;
    public const byte Match = 227;
    public const byte AppStats = 226;
    public const byte AzureNodeInfo = 210;
    public const byte Join = (byte)LiteEventCode.Join;
    public const byte Leave = (byte)LiteEventCode.Leave;
    public const byte SetProperties = (byte)LiteEventCode.PropertiesChanged;
}

public class ParameterCode
{
    public const byte Address = 230;
    public const byte PeerCount = 229;
    public const byte GameCount = 228;
    public const byte MasterPeerCount = 227;
    public const byte UserId = 225;
    public const byte ApplicationId = 224;
    public const byte Position = 223;
    public const byte GameList = 222;
    public const byte Secret = 221;
    public const byte AppVersion = 220;
    public const byte AzureNodeInfo = 210;	// only used within events, so use: EventCode.AzureNodeInfo
    public const byte AzureLocalNodeId = 209;
    public const byte AzureMasterNodeId = 208;

    public const byte GameId = (byte)LiteOpKey.GameId;
    public const byte Broadcast = (byte)LiteOpKey.Broadcast;
    public const byte ActorList = (byte)LiteOpKey.ActorList;
    public const byte ActorNr = (byte)LiteOpKey.ActorNr;
    public const byte ActorProperties = (byte)LiteOpKey.ActorProperties;
    public const byte CustomEventContent = (byte)LiteOpKey.Data;
    public const byte Data = (byte)LiteOpKey.Data;
    public const byte Code = (byte)LiteOpKey.Code;
    public const byte GameProperties = (byte)LiteOpKey.GameProperties;
    public const byte Properties = (byte)LiteOpKey.Properties;
    public const byte TargetActorNr = (byte)LiteOpKey.TargetActorNr;
    public const byte ReceiverGroup = (byte)LiteOpKey.ReceiverGroup;
    public const byte Cache = (byte)LiteOpKey.Cache;
    /// <summary>(241) Cleans up roomcache on leave of player. Also known as autoCleanup property of rooms.</summary>
    public const byte CleanupCacheOnLeave = (byte)241;
}

public class OperationCode
{
    public const byte Authenticate = 230;
    public const byte JoinLobby = 229;
    public const byte LeaveLobby = 228;
    public const byte CreateGame = 227;
    public const byte JoinGame = 226;
    public const byte JoinRandomGame = 225;
    // public const byte CancelJoinRandom = 224; // obsolete, cause JoinRandom no longer is a "process". now provides result immediately
    public const byte Leave = (byte)LiteOpCode.Leave;
    public const byte RaiseEvent = (byte)LiteOpCode.RaiseEvent;
    public const byte SetProperties = (byte)LiteOpCode.SetProperties;
    public const byte GetProperties = (byte)LiteOpCode.GetProperties;
}
