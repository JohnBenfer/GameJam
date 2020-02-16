using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameWindowsStarter
{
    class Badge
    {
        Texture2D currentTexture;
        int X;
        int Y;
        int width;
        int height;
        Vector2 origin;

        public Badge(Game1 game, SpriteBatch spriteBatch, int w, int h, int X, int Y)
        {
            this.X = X;
            this.Y = Y;
            width = w;
            height = h;
            origin = new Vector2((float)(width / 2), (float)(height / 2));
            spriteBatch.Draw(currentTexture, new Rectangle((int)X, (int)Y, width, height), null, Color.White, 0f, origin, SpriteEffects.None, 0);
        }
    }
}
