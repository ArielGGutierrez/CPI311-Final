using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CPI311.GameEngine
{
    public class Button : GUIElement
    {
        public override void Update()
        {
            if (InputManager.IsMouseReleased(0) &&
                    Bounds.Contains(InputManager.GetMousePosition()))
                OnAction();
        }
        public override void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            base.Draw(spriteBatch, font);
            Vector2 length = font.MeasureString(Text);
            spriteBatch.DrawString(font, Text,
                new Vector2(Bounds.X + Bounds.Width/2 - length.X/2, Bounds.Y + Bounds.Height/2 - length.Y/2), Color.Black);
        }
    }
}
