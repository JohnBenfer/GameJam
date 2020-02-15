using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameWindowsStarter
{
    class Player
    {
        Texture2D currentTexture;
        public hitbox hitbox;
        public int X;
        public int Y;
        double MAX_VELOCITY;
        double TERMINAL_VELOCITY = 6;
        double velocity;
        double acceleration;
        double GRAVITY = 0.5;
        int SCREEN_WIDTH;
        int SCREEN_HEIGHT;
        Game1 game;

        public Player(Game1 game)
        {
            
            MAX_VELOCITY = 10;
            this.game = game;
            SCREEN_WIDTH = game.SCREEN_WIDTH;
            SCREEN_HEIGHT = game.SCREEN_HEIGHT;
            X = 100;
            Y = SCREEN_HEIGHT / 2;
            hitbox = new hitbox(50, 100, X, Y);

        }

        public void Update()
        {
            var keyboardState = Keyboard.GetState();
            if(keyboardState.IsKeyDown(Keys.Space))
            {
                if(velocity < MAX_VELOCITY)
                {
                    velocity += acceleration;
                }


            } else // space bar is not held down
            {
                if(Y < (SCREEN_HEIGHT - 100)) { // if we are not to the bottom of the screen
                    if(velocity < MAX_VELOCITY) // if we are not to terminal velocity
                    {
                        velocity -= GRAVITY;
                    }
                }
            }


            hitbox.Move(X, Y);
        }

        public void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
