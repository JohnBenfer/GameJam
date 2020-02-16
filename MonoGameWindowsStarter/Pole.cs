using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace MonoGameWindowsStarter
{
    class Pole
    {
        Texture2D currentTexture;
        public double X;
        public double Y;
        public int width;
        public int height;
        public Hitbox hitbox;
        Game1 game;
        Vector2 origin;
        public bool offScreen;
        double poleSpeed;

        public Pole(Game1 game)
        {
            poleSpeed = 0;
            width = 140;
            height = 320;
            LoadContent(game.Content);
            this.game = game;
            
            Y = game.SCREEN_HEIGHT - 240;
            X = game.SCREEN_WIDTH + width;

            hitbox = new Hitbox(height/2, width/2, (int)X, (int)Y);
            origin = new Vector2((float)(width / 2), (float)(height / 2));
            offScreen = false;
        }
        public void Update()
        {
            X -= game.backgroundSpeed + poleSpeed;
            hitbox.Move((int)X, (int)Y);
            if (X < -1 * width)
            {
                offScreen = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(currentTexture, new Rectangle((int)X, (int)Y, width, height), null, Color.White, 0f, origin, SpriteEffects.None, 0);
        }

        public void LoadContent(ContentManager content)
        {
            
            currentTexture = content.Load<Texture2D>("Pole");

        }
    }
}
