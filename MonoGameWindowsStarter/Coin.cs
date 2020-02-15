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
    class Coin
    {
        Texture2D currentTexture;
        double X;
        double Y;

        public Coin(Game1 game)
        {
            Random random = new Random();
            random.Next(100, game.SCREEN_HEIGHT);
        }
        public void Update()
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {

        }

        public void LoadContent(ContentManager content)
        {
            currentTexture = content.Load<Texture2D>("Coin");
        }
    }
}
