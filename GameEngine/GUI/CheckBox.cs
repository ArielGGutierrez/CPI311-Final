using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CPI311.GameEngine
{
    public class CheckBox : GUIElement
    {
        public Texture2D Box { get; set; }
        public bool Checked { get; set; }
        public override void Update()
        {
            if (InputManager.IsMouseReleased(0) &&
                    Bounds.Contains(InputManager.GetMousePosition()))
            {
                OnAction();
                Checked = !Checked;
            }
            base.Update();
        }

        public override void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            base.Draw(spriteBatch, font);

            int width = Math.Min(Bounds.Width, Bounds.Height);
            spriteBatch.Draw(Box, new Rectangle(Bounds.X, Bounds.Y, width, width), Checked ? Color.Red : Color.White);
            spriteBatch.DrawString(font, Text, new Vector2(Bounds.X + width, Bounds.Y), Color.Black);
        }
    }
}
