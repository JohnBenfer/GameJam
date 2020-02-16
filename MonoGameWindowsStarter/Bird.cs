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
    class Bird
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
        double birdSpeed;

        public Bird(Game1 game)
        {
            birdSpeed = 0.5;
            width = 150;
            height = 150;
            LoadContent(game.Content);
            this.game = game;
            Random random = new Random();
            Y = random.Next(100, game.SCREEN_HEIGHT - 100);
            X = game.SCREEN_WIDTH + width;
            hitbox = new Hitbox(height-100, width-100, (int)X, (int)Y);
            origin = new Vector2((float)(width / 2), (float)(height / 2));
            offScreen = false;
        }
        public void Update()
        {
            X -= game.backgroundSpeed + birdSpeed;
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
            Random random = new Random();
            if(random.Next(0, 2) == 1)
            {
                currentTexture = content.Load<Texture2D>("Bird1");
            } else
            {
                currentTexture = content.Load<Texture2D>("Bird2");
            }
            
        }
    }
}
