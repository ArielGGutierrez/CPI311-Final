using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;
using CPI311.GameEngine.Physics;

namespace FinalProject
{
    public class Player : GameObject
    {
        public bool isActive { get; set; }
        public float Health { get; set; }
        public float PlayerSpeed { get; set; }
        public GameConstant.Weapons CurrentWeapon { get; set; }

        public float fireCooldown;

        public bool justdied;

        public Player(ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light) : base()
        {
            isActive = true;
            Health = GameConstant.MaxPlayerHealth;
            PlayerSpeed = GameConstant.MinPlayerSpeed;
            CurrentWeapon = GameConstant.Weapons.Default;

            // Step 1: Add a RigidBody
            //------------------------------------
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);
            //------------------------------------

            // Step 2 Add a Collider
            //---------------------------------------------------
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = GameConstant.PlayerBoundingSphereScale;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);
            //---------------------------------------------------

            // Step 3 Add a Renderer
            //--------------------------------------------------------------------------
            Texture2D Texture = Content.Load<Texture2D>("Resources/Textures/Square");
            Renderer renderer = new Renderer(Content.Load<Model>("Resources/Models/craft_speederD"),
                                             Transform, camera, Content, graphicsDevice,
                                             light, 1, null, 20f, Texture);
            Add<Renderer>(renderer);
            //--------------------------------------------------------------------------
        }

        public override void Update()
        {
            if (!isActive) return;
            
            PlayerMovement();
            
            if (fireCooldown < 0) fireCooldown = 0;
            else if (fireCooldown > 0) fireCooldown -= Time.ElapsedGameTime;
            
            if (Health <= 0 && !justdied)
            {
                justdied = true;
                this.isActive = false;
            }

            base.Update();
        }

        public override void Draw()
        {
            if (!isActive) return;
            
            base.Draw();
        }

        private void PlayerMovement()
        {
            Vector3 nextMove = Vector3.Zero;
            if (InputManager.IsKeyDown(Keys.W)) nextMove += Vector3.Forward;
            if (InputManager.IsKeyDown(Keys.S)) nextMove += Vector3.Backward;
            if (InputManager.IsKeyDown(Keys.A)) nextMove += Vector3.Left;
            if (InputManager.IsKeyDown(Keys.D)) nextMove += Vector3.Right;

            Vector3 direction;

            if (nextMove.LengthSquared() == 0)
            {
                direction = this.Transform.Forward;
            }

            else
            {
                direction = Vector3.Normalize(nextMove);
            }

            float angle = MathF.Acos(MathHelper.Clamp(Vector3.Dot(this.Transform.Forward, direction), -1, 1));

            /*  Determines whether to turn left or right */
            Vector3 direction2 = new Vector3(direction.Z, 0, -direction.X);
            if (Vector3.Dot(direction2, this.Transform.Forward) > 0) angle *= -1;

            this.Transform.Rotate(Vector3.Up, angle * 1 / 10);
            if (nextMove.LengthSquared() != 0 && MathF.Abs(angle) < MathHelper.PiOver2)
            {
                this.Transform.LocalPosition += this.Transform.Forward * Time.ElapsedGameTime * PlayerSpeed;
            }

            //System.Diagnostics.Debug.WriteLine("Pos: " + this.Transform.Position);
        }
    }
}
