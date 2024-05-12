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
    public class MotherShip : GameObject
    {
        public bool isActive { get; set; }
        public float Health { get; set; }
        public Player Player { get; set; }

        public int playerdeaths;

        public float respawnTimer;

        public MotherShip(Player player, ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light) : base()
        {
            isActive = true;
            Health = GameConstant.MaxMotherHealth;
            Player = player;
            playerdeaths = 0;

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
            sphereCollider.Radius = GameConstant.MotherBoundingSphereScale;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);
            //---------------------------------------------------

            // Step 3 Add a Renderer
            //--------------------------------------------------------------------------
            Texture2D Texture = Content.Load<Texture2D>("Resources/Textures/Square");
            Renderer renderer = new Renderer(Content.Load<Model>("Resources/Models/hangar_roundB"),
                                             Transform, camera, Content, graphicsDevice,
                                             light, 1, null, 20f, Texture);
            Add<Renderer>(renderer);
            //--------------------------------------------------------------------------
        }

        public override void Update()
        {
            if (!isActive) return;

            if (Player.justdied)
            {
                Player.justdied = false;
                Player.Transform.LocalPosition = this.Transform.LocalPosition + Vector3.Backward * 60f;
                Player.Transform.LocalRotation = this.Transform.LocalRotation;
                Player.CurrentWeapon = GameConstant.Weapons.Default;
                playerdeaths++;
                respawnTimer = MathHelper.Clamp(GameConstant.MinRespawnTimer * playerdeaths, GameConstant.MinRespawnTimer, GameConstant.MaxRespawnTimer);
            }

            else if(!Player.isActive)
            {
                if (respawnTimer <= 0)
                {
                    Player.Health = GameConstant.MaxPlayerHealth;
                    Player.isActive = true;
                }

                else
                {
                    respawnTimer -= Time.ElapsedGameTime;
                }
            }

            if (Health <= 0)
            {
                isActive = false;
            }

            base.Update();
        }

        public override void Draw()
        {
            if (!isActive) return;

            base.Draw();
        }
    }
}
