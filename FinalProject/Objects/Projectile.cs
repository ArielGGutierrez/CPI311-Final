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
    public class Projectile : GameObject
    {
        public bool isActive { get; set; }
        public float projectileSpeed { get; set; }
        public GameConstant.Projectiles ProjectileType { get; set; }

        public bool Friendly;

        public Projectile(bool friendly, GameConstant.Projectiles type, ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light) : base()
        {
            isActive = true;
            ProjectileType = type;
            Friendly = friendly;

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

            Model model;

            if (ProjectileType == GameConstant.Projectiles.bullet) model = Content.Load<Model>("Resources/Models/bullet");
            else model = Content.Load<Model>("Resources/Models/Rocket");

            Renderer renderer = new Renderer(model,
                                             Transform, camera, Content, graphicsDevice,
                                             light, 1, null, 20f, Texture);
            Add<Renderer>(renderer);
            //--------------------------------------------------------------------------
        }

        public override void Update()
        {
            if (!isActive) return;

            // Out of Bounds Check
            //------------------------------------------------------------------
            if (MathF.Abs(Transform.Position.X) > GameConstant.PlayfieldSizeX ||
                MathF.Abs(Transform.Position.Z) > GameConstant.PlayfieldSizeY)
            {
                isActive = false;
                Rigidbody.Velocity = Vector3.Zero;
            }
            //------------------------------------------------------------------

            base.Update();
        }

        public override void Draw()
        {
            if (isActive) base.Draw();
        }
    }
}
