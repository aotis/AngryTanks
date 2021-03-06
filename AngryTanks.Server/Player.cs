﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using log4net;
using Lidgren.Network;

using AngryTanks.Common;
using AngryTanks.Common.Messages;
using AngryTanks.Common.Protocol;

namespace AngryTanks.Server
{
    public class Player
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Player Properties

        public readonly Byte          Slot;
        public readonly NetConnection Connection;
        public readonly String        Callsign, Tag;
        public readonly TeamType      Team;

        private PlayerState playerState;
        public PlayerState State
        {
            get { return playerState; }
        }

        public PlayerInformation PlayerInfo
        {
            get
            {
                return new PlayerInformation(Slot, Callsign, Tag, Team);
            }
        }

        #endregion

        private GameKeeper gameKeeper;

        public Player(GameKeeper gameKeeper, Byte slot, NetConnection connection, PlayerInformation playerInfo)
        {
            this.Slot       = slot;
            this.Connection = connection;
            this.Team       = playerInfo.Team;
            this.Callsign   = playerInfo.Callsign;
            this.Tag        = playerInfo.Tag;

            this.gameKeeper = gameKeeper;

            this.playerState = PlayerState.Joining;

            Log.InfoFormat("Player #{0} \"{1}\" <{2}> created and joined to {3}", Slot, Callsign, Tag, Team);
        }

        public void HandleData(NetIncomingMessage incomingMsg)
        {
            MessageType messageType = (MessageType)incomingMsg.ReadByte();

            switch (messageType)
            {
                case MessageType.MsgState:
                    SendState();
                    break;

                case MessageType.MsgPlayerClientUpdate:
                    BroadcastUpdate(incomingMsg);
                    break;

                default:
                    break;
            }
        }

        private void SendState()
        {
            Log.DebugFormat("Sending state to #{0}...", Slot);

            // TODO we should clamp world size to no more than UInt16.MaxValue bytes large
            // first send the world
            NetOutgoingMessage worldMsg = gameKeeper.Server.CreateMessage(1 + 2 + (UInt16)gameKeeper.RawWorld.Length);
            worldMsg.Write((Byte)MessageType.MsgWorld);
            worldMsg.Write((UInt16)gameKeeper.RawWorld.Length);
            worldMsg.Write(gameKeeper.RawWorld);
            SendMessage(worldMsg, NetDeliveryMethod.ReliableOrdered, 0);

            // TODO send other state information... like flags

            // tell him about everyone else
            Log.DebugFormat("Sending MsgAddPlayer for each player to #{0}", Slot);

            NetOutgoingMessage addPlayerMessage;
            MsgAddPlayerPacket addPlayerPacket;

            foreach (Player otherPlayer in gameKeeper.Players)
            {
                // don't want to tell our player about himself just yet...
                if (otherPlayer.Slot == this.Slot)
                    continue;

                addPlayerMessage = gameKeeper.Server.CreateMessage();

                addPlayerPacket = new MsgAddPlayerPacket(otherPlayer.PlayerInfo, false);

                addPlayerMessage.Write((Byte)addPlayerPacket.MsgType);
                addPlayerPacket.Write(addPlayerMessage);

                Log.DebugFormat("MsgAddPlayer Compiled ({0} bytes) for player #{1} and being sent to player #{2}",
                                addPlayerMessage.LengthBytes, otherPlayer.Slot, this.Slot);

                SendMessage(addPlayerMessage, NetDeliveryMethod.ReliableOrdered, 0);
            }
            
            // TODO send scores and such...

            // let them know we're ready to move on, and give him his slot
            NetOutgoingMessage stateMessage = gameKeeper.Server.CreateMessage();

            MsgStatePacket statePacket = new MsgStatePacket(Slot);

            stateMessage.Write((Byte)MessageType.MsgState);
            statePacket.Write(stateMessage);

            SendMessage(stateMessage, NetDeliveryMethod.ReliableOrdered, 0);

            // send back one last MsgAddPlayer with their full information (which could be changed!)
            addPlayerMessage = gameKeeper.Server.CreateMessage();

            addPlayerPacket = new MsgAddPlayerPacket(PlayerInfo, true);

            addPlayerMessage.Write((Byte)MessageType.MsgAddPlayer);
            addPlayerPacket.Write(addPlayerMessage);

            SendMessage(addPlayerMessage, NetDeliveryMethod.ReliableOrdered, 0);

            // we're now ready to move to the spawn state
            this.playerState = PlayerState.Spawning;
        }

        private void BroadcastUpdate(NetIncomingMessage msg)
        {
            MsgPlayerClientUpdatePacket clientUpdatePacket = MsgPlayerClientUpdatePacket.Read(msg);

            NetOutgoingMessage serverUpdateMessage = gameKeeper.Server.CreateMessage();
            MsgPlayerServerUpdatePacket serverUpdatePacket = new MsgPlayerServerUpdatePacket(this.Slot, clientUpdatePacket);

            serverUpdateMessage.Write((Byte)MessageType.MsgPlayerServerUpdate);
            serverUpdatePacket.Write(serverUpdateMessage);

            // send to everyone but us
            gameKeeper.Server.SendToAll(serverUpdateMessage, this.Connection, NetDeliveryMethod.UnreliableSequenced, 0);
        }

        #region Connection Helpers

        public NetSendResult SendMessage(NetOutgoingMessage msg, NetDeliveryMethod method)
        {
            return gameKeeper.Server.SendMessage(msg, Connection, method);
        }

        public NetSendResult SendMessage(NetOutgoingMessage msg, NetDeliveryMethod method, int sequenceChannel)
        {
            return gameKeeper.Server.SendMessage(msg, Connection, method, sequenceChannel);
        }

        #endregion
    }
}
