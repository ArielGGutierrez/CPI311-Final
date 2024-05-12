using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CPI311.GameEngine
{
    public class AnimatedSprite : Sprite
    {
        /* Properties */
        public int Animations { get; set; } // Number of total animations
        public int Animation { get; set; } // Current animation
        public int Frames { get; set; } // Total number of frames of animation
        public float Frame { get; set; } // Current frame of animation
        public float Speed { get; set; } // Speed to cycle through the frames of animation
        
        /* Variables */
        public bool Play;        // Is animation playing or not?
        private int frameWidth;  // Width of the frame of animation
        private int frameHeight; // Height of the frame of animation
        
        public AnimatedSprite(Texture2D texture, int animations = 1, int frames = 1) : base(texture)
        {
            Animations = animations;
            Frames = frames;
            Animation = 0;
            Frame = 0;
            Speed = 1;
            frameWidth = Texture.Width / Frames;
            frameHeight = Texture.Height / Animations;
            Play = false;
            Origin = new Vector2(frameWidth/2f, frameHeight/2f);
        }

        public void Update()
        {
            /* If animation is playing, cycle through the frames */
            if (Play)
            {
                Frame += Speed * Time.ElapsedGameTime;
            }

            /* If you reach the end of the animation, start from the beginning again */
            if ((int) Frame >= Frames)
            {
                Frame = 0;
            }

            Source = new Rectangle((int)(Frame) * frameWidth, Animation * frameHeight, frameWidth, frameHeight); // Move source rectangle as needed
        }

        /* Plays the current animation on loop */
        public void PlayAnim()
        {
            Play = true;
        }

        /* Stops playing the current animation */
        public void StopAnim()
        {
            Play = false;
        }

        /* Resets the animation back to the first frame */
        public void ResetAnim()
        {
            Frame = 0;
        }
    }
}
