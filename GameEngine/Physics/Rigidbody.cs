using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace CPI311.GameEngine.Physics
{
    public class Rigidbody : Component, IUpdateable
    {
        public Vector3 Velocity { get; set; }
        public float Mass { get; set; }
        public Vector3 Acceleration { get; set; }
        public Vector3 Impulse { get; set; }
        public float ElapsedGameTime { get; set; }

        public Rigidbody()
        {
            ElapsedGameTime = Time.ElapsedGameTime;
        }

        public void Update()
        {
            Velocity += Acceleration * ElapsedGameTime + Impulse / Mass;
            Transform.LocalPosition += Velocity * ElapsedGameTime;
            Impulse = Vector3.Zero;
        }
    }
}
