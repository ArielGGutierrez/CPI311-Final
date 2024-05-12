using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CPI311.GameEngine
{
    public class ProgressBar : Sprite
    {
        /* Properties */
        public Color FillColor { get; set; }  // Color to fill inside of bar
        public Vector2 BarSize { get; set; }  // Size of bar in pixels
        public Vector2 Padding { get; set; }  // Padding between inner and outer bar
        public Vector2 Progress { get; set; } // Size of progress bar to be drawn
        public float Value { get; set; }      // Current value of the bar
        public float MaxValue { get; set; }   // Max value of the bar
        public float Speed { get; set; }      // Speed of progress through the bar

        /* Variables */
        public Vector2 ProgressPosition;     // Position of inner bar

        public ProgressBar(Texture2D texture, Vector2 barSize, Vector2 padding) : base(texture)
        {
            Progress = new Vector2(0, 0);
            Origin = new Vector2(0, 0);

            BarSize = barSize;
            Padding = padding;
            Scale = new Vector2(BarSize.X / Texture.Width, BarSize.Y / Texture.Height);
            ProgressPosition = new Vector2(Position.X + Padding.X, Position.Y + Padding.Y);
            
            FillColor = Color.Red;

            Value = 0f;
            MaxValue = 100f;
            Speed = 1f;

            
        }

        public void Update()
        {
            // Update the position of the inner bar
            ProgressPosition = new Vector2(Position.X + Padding.X/2f, Position.Y + Padding.Y/2f);

            // Update Value and Progress Bar
            //---------------------------//
            if (Value < MaxValue)
            {
                Value += Speed * Time.ElapsedGameTime;
                Progress = new Vector2((Value/MaxValue) * (BarSize.X - Padding.X) / Texture.Width, (BarSize.Y - Padding.Y) / Texture.Height);
            }
            else if (Value >= MaxValue)
            {
                Value = MaxValue;
                Progress = new Vector2((float)(BarSize.X - Padding.X) / Texture.Width, (float)(BarSize.Y - Padding.Y) / Texture.Height);
            }
            //---------------------------//
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Source, Color, Rotation, Origin, Scale, Effect, Layer);
            spriteBatch.Draw(Texture, ProgressPosition, Source, FillColor, Rotation, Origin, Progress, Effect, Layer);
        }

        /* This method lets you manually decrement the value of the bar without going into a negative value */
        public void Decrement(float decrement)
        {
            if (Value - decrement < 0)
            {
                Value = 0;
            }

            else
            {
                Value -= decrement;
            }
        }

        /* This method lets you manually increment the value of the bar without going over the max value */
        public void Increment(float increment)
        {
            if (Value - increment > MaxValue)
            {
                Value = MaxValue;
            }

            else
            {
                Value += increment;
            }
        }
    }
}
