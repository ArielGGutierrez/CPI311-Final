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
    class SimpleEnemy : GameObject
    {
        public int Health { get; set; }
        public bool isActive { get; set; }
        public float EnemySpeed { get; set; }
        public GameConstant.Enemies EnemyType { get; set; }
        public Player PlayerTarget { get; set; }
        public MotherShip MotherTarget { get; set; }
        public enum Enemy_State
        {
            wander,
            aim_at_player,
            aim_at_mother,
            crash_into_mother
        }

        public Enemy_State currentState;
        public SimpleEnemy(MotherShip mother, Player player, GameConstant.Enemies type, ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light) : base()
        {
            isActive = true; // Default is active
            EnemyType = type;
            MotherTarget = mother;
            PlayerTarget = player;

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

            Model model;

            if (EnemyType == GameConstant.Enemies.asteroid)
            {
                model = Content.Load<Model>("Resources/Models/meteor_detailed");
                currentState = Enemy_State.crash_into_mother;

            }

            else
            {
                model = Content.Load<Model>("Resources/Models/craft_miner");
                currentState = Enemy_State.wander;
            }

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
            //-----------------------------------------------------------------------------------------------------------------------------
            if (Transform.Position.X > GameConstant.PlayfieldSizeX) Transform.Position -= Vector3.UnitX * 2 * GameConstant.PlayfieldSizeX;
            if (Transform.Position.X < -GameConstant.PlayfieldSizeX) Transform.Position += Vector3.UnitX * 2 * GameConstant.PlayfieldSizeX;
            if (Transform.Position.Z > GameConstant.PlayfieldSizeX) Transform.Position -= Vector3.UnitZ * 2 * GameConstant.PlayfieldSizeY;
            if (Transform.Position.Z < -GameConstant.PlayfieldSizeX) Transform.Position += Vector3.UnitZ * 2 * GameConstant.PlayfieldSizeY;
            //-----------------------------------------------------------------------------------------------------------------------------

            base.Update();
        }

        public override void Draw()
        {
            if (isActive) base.Draw();
        }
    }
}
