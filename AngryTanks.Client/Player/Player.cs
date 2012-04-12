using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using log4net;

using AngryTanks.Common;
using AngryTanks.Common.Messages;
using AngryTanks.Common.Protocol;

namespace AngryTanks.Client
{
    public abstract class Player : DynamicSprite, IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Player Properties

        protected PlayerInformation playerInfo;

        public PlayerInformation PlayerInfo
        {
            get { return playerInfo; }
        }

        public Byte Slot
        {
            get { return PlayerInfo.Slot; }
        }

        public String Callsign
        {
            get { return PlayerInfo.Callsign; }
        }

        public String Tag
        {
            get { return PlayerInfo.Tag; }
        }

        public TeamType Team
        {
            get { return PlayerInfo.Team; }
        }

        protected PlayerState state = PlayerState.None;

        public PlayerState State
        {
            get { return state; }
        }

        #endregion

        public Player(World world, PlayerInformation playerInfo)
            : base(world, GetTexture(world, playerInfo), Vector2.Zero, GetTankSize(world, playerInfo), 0)
        {
            this.playerInfo = playerInfo;

            World.ServerLink.MessageReceivedEvent += HandleReceivedMessage;
        }

        ~Player()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            World.ServerLink.MessageReceivedEvent -= HandleReceivedMessage;
        }

        protected static Texture2D GetTexture(World world, PlayerInformation playerInfo)
        {
            // TODO get the correct texture depending on team
            return world.Content.Load<Texture2D>("textures/tank_white");
        }

        protected static Vector2 GetTankSize(World world, PlayerInformation playerInfo)
        {
            // TODO get size from variable database
            return new Vector2(4.86f, 6);
        }

        protected virtual void HandleReceivedMessage(object sender, ServerLinkMessageEvent message)
        {
            return;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            DrawStretched(gameTime, spriteBatch);
        }
    }
}