using Force.DeepCloner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AchtuurCore.Utility;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace AchtuurCore.Framework
{
    public abstract class Particle : ICloneable
    {
        /// <summary>
        /// Maximum speed of particle (in in-game coordinates)
        /// </summary>
        protected Vector2 maxSpeed = new Vector2(15f, 13f);

        /// <summary>
        /// Current speed of particle
        /// </summary>
        protected Vector2 speed;

        protected Vector2 maxAccel = new Vector2(4f, 3.2f);
        /// <summary>
        /// Acceleration increase per tick
        /// </summary>
        protected float accel_per_tick = 0.05f;

        /// <summary>
        /// Current acceleration
        /// </summary>
        protected Vector2 accel;

        /// <summary>
        /// Maximum number of ticks this particle can stay alive for
        /// </summary>
        protected int maxLifeSpanTicks = 60 * 5;

        /// <summary>
        /// Number of ticks this particle has been alive
        /// </summary>
        protected int ticks = 0;

        protected Vector2 Position { get; set; }
        protected Vector2 intialPosition;
        protected Vector2 targetPosition;

        protected Farmer targetFarmer;

        protected Color color = Color.White;
        protected float particleColorOpacity = 0.5f;
        protected Vector2 size = new Vector2(10f, 10f);


        public bool Started { get; protected set; }
        public bool ReachedTarget { get; protected set; }

        public Particle(Vector2 position, Vector2 targetPosition, Color color, Vector2 size)
        {
            this.Position = position;
            this.intialPosition = position;
            this.targetPosition = targetPosition;
            this.speed = new Vector2();
            this.accel = new Vector2();
            ReachedTarget = false;

            this.color = color;
            this.size = size;
        }
        protected virtual void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            this.ticks++;
            this.accel_per_tick = this.ticks / 10f;

            this.Position = GetNextPosition();
            if (hasReachedTargetPosition())
            {
                OnReachTargetPosition();
            }
        }
        protected void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            this.DrawToScreen(e.SpriteBatch);
        }

        public virtual void Start()
        {
            this.Started = true;
            ModEntry.Instance.Helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            ModEntry.Instance.Helper.Events.Display.RenderedWorld += this.OnRenderedWorld;

            // start with random speed to curve the particle a bit (cool effect)
            // generate random direction between 0.5 and -0.5 for x and y
            Random r = new Random();
            this.speed = new Vector2((float) r.NextDouble() - 0.5f, (float) r.NextDouble() - 0.5f);
            // Set speed to 0.1 max speed;
            this.speed.Normalize();
            this.speed *= maxSpeed * 1f;
        }

        public virtual void Reset()
        {
            this.Position = intialPosition;
            this.speed = new Vector2();
            this.accel = new Vector2();
            this.ReachedTarget = false;
            this.Start();
        }
        public object Clone()
        {
            return MemberwiseClone();
        }

        public void Destroy()
        {
            // Unsubscribe from events
            ModEntry.Instance.Helper.Events.GameLoop.UpdateTicked -= this.OnUpdateTicked;
            ModEntry.Instance.Helper.Events.Display.RenderedWorld -= this.OnRenderedWorld;

            // pray to god nothing references it anymore and the garbage collector fixes my problems
        }

        public virtual void SetColor(Color color)
        {
            this.color = color;
        }

        public virtual void SetSize(Vector2 size)
        {
            this.size = size;
        }

        public virtual void SetInitialPosition(Vector2 initialPosition)
        {
            this.intialPosition = initialPosition;
            this.Position = initialPosition;
        }

        /// <summary>
        /// Set target position
        /// </summary>
        /// <param name="position">Target position</param>
        public void SetTarget(Vector2 position)
        {
            this.targetPosition = position;
        }

        /// <summary>
        /// Set target Farmer, the particle will try and reach the farmer's position
        /// </summary>
        /// <param name="farmer">Farmer who's position will be targeted</param>
        public void SetTarget(Farmer farmer)
        {
            this.targetFarmer = farmer;
            this.targetPosition = farmer.Position;
        }

        protected Vector2 GetNextPosition()
        {
            if (this.targetFarmer is not null)
                this.targetPosition = this.targetFarmer.Position;

            Vector2 targetDirection = targetPosition - Position;
            targetDirection.Normalize();

            // increase acceleration towards target direction
            this.accel = accel_per_tick * targetDirection;
            this.accel = Vector2.Clamp(this.accel, -maxAccel, maxAccel);

            // increase speed based on acceleration
            this.speed += this.accel;
            this.speed = Vector2.Clamp(this.speed, -maxSpeed, maxSpeed);

            // return new position based on speed
            return this.Position + this.speed;
        }

        protected bool hasReachedTargetPosition()
        {
            // Distance scales from 0 to 64 based on percentage of max speed
            float d = 72f * this.accel.LengthSquared() / this.maxAccel.LengthSquared();
            return this.ticks > this.maxLifeSpanTicks || 
                (this.Position - this.targetPosition).LengthSquared() < d*d;
        }
        protected void OnReachTargetPosition()
        {
            this.ReachedTarget = true;

            // place out of bounds
            this.speed = new Vector2(0, 0);
            this.accel = new Vector2(0, 0);
            this.Position = new Vector2(3000, 3000);

            this.Destroy();
        }

        public virtual void DrawToScreen(SpriteBatch spriteBatch)
        {
            if (this.ReachedTarget || !this.Started)
                return;

            Vector2 screenCoords = Drawing.GetPositionScreenCoords(Position);
            Drawing.DrawLine(spriteBatch, screenCoords, this.size, this.color * particleColorOpacity);
            Drawing.DrawBorder(spriteBatch, screenCoords, this.size, color, bordersize:3);

            #if DEBUG
            Vector2 initialCoords = Drawing.GetPositionScreenCoords(this.intialPosition);
            Vector2 targetCoords = Drawing.GetPositionScreenCoords(this.targetPosition);

            Drawing.DrawLine(spriteBatch, initialCoords, this.size, Color.Yellow);
            Drawing.DrawLine(spriteBatch, targetCoords, this.size, Color.Magenta);

            #endif
        }
    }
}
