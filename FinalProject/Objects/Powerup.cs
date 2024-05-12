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
    public class Powerup : GameObject
    {
        public int Health { get; set; }
        public bool isActive { get; set; }
        public float Timer { get; set; }

        public GameConstant.Weapons weapon { get; set; }
        public Powerup(GameConstant.Weapons type, ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light) : base()
        {
            isActive = true; // Default is active
            weapon = type;
            Timer = 2f;

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
            sphereCollider.Radius = GameConstant.EnemyBoundingSphereScale;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);
            //---------------------------------------------------

            // Step 3 Add a Renderer
            //--------------------------------------------------------------------------
            Texture2D Texture = Content.Load<Texture2D>("Resources/Textures/Square");

            Model model = Content.Load<Model>("Resources/Models/Sphere");

            Renderer renderer = new Renderer(model,
                                             Transform, camera, Content, graphicsDevice,
                                             light, 1, null, 20f, Texture);
            Add<Renderer>(renderer);
            //--------------------------------------------------------------------------
        }
        public override void Update()
        {
            if (!isActive) return;

            if (Timer <= 0) isActive = false;

            // Out of Bounds Check
            //-----------------------------------------------------------------------------------------------------------------------------
            if (Transform.Position.X > GameConstant.PlayfieldSizeX) isActive = false;
            if (Transform.Position.X < -GameConstant.PlayfieldSizeX) isActive = false;
            if (Transform.Position.Z > GameConstant.PlayfieldSizeX) isActive = false;
            if (Transform.Position.Z < -GameConstant.PlayfieldSizeX) isActive = false;
            //-----------------------------------------------------------------------------------------------------------------------------

            Timer -= Time.ElapsedGameTime;

            base.Update();
        }

        public override void Draw()
        {
            if (isActive) base.Draw();
        }
    }
}
