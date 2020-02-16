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
    class Player
    {
        Texture2D currentTexture;
        Texture2D[] sprites = new Texture2D[4];
        public Hitbox hitbox;
        public double X;
        public double Y;
        double MAX_VELOCITY;
        double TERMINAL_VELOCITY = 6;
        double velocity;
        double acceleration;
        double GRAVITY = 1;
        int SCREEN_WIDTH;
        int SCREEN_HEIGHT;
        int width;
        int height;
        Game1 game;
        Vector2 origin;
        public int boosterLevel;

        public Player(Game1 game, double acceleration, double MAX_VELOCITY, int boosterLevel)
        {
            this.boosterLevel = boosterLevel;
            LoadContent(game.Content);
            currentTexture = sprites[boosterLevel];
            velocity = 0;
            this.game = game;
            SCREEN_WIDTH = game.SCREEN_WIDTH;
            SCREEN_HEIGHT = game.SCREEN_HEIGHT;
            width = 300;
            height = 100;
            X = 300;
            Y = SCREEN_HEIGHT / 2;
            hitbox = new Hitbox(height-50, width-60, (int)X, (int)Y);
            this.acceleration = acceleration;
            this.MAX_VELOCITY = MAX_VELOCITY * -1;
            origin = new Vector2(currentTexture.Width / 2, currentTexture.Height / 2);

        }

        public void Update()
        {

            UpdateVelocity();


            hitbox.Move((int)X-30, (int)Y - 50);
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(game.gameUpgradeMenu)
            {
                currentTexture = sprites[boosterLevel];
            }
            //spriteBatch.Draw(currentTexture, new Rectangle((int)X, (int)Y, 400, 200), null, Color.White, (float)0, origin, SpriteEffects.None, 0);
            spriteBatch.Draw(currentTexture, new Rectangle((int)X, (int)Y, width, height), null, Color.White, 0f, origin, SpriteEffects.None, 0);
        }


        private void UpdateVelocity()
        {
            var keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                currentTexture = sprites[boosterLevel];
                if (Y > 120) // if we are not to the top yet
                {
                    if (velocity > MAX_VELOCITY) // if we are under the speed limit 
                    {
                        velocity -= acceleration;
                    }
                }
                else // we are at the top
                {
                    velocity = 0;
                }
                MovePlayer();

            }
            else // space bar is not held down
            {
                currentTexture = sprites[0];
                if (Y < (SCREEN_HEIGHT - 100))
                { // if we are not to the bottom of the screen
                    if (velocity < TERMINAL_VELOCITY) // if we are not to terminal velocity
                    {
                        velocity += GRAVITY;

                    }
                    MovePlayer();
                }
                else
                {
                    velocity = 0;
                }
            }
            
        }
        private void MovePlayer()
        {

            Y += velocity;
        }

        public void LoadContent(ContentManager content)
        {
            //currentTexture = content.Load<Texture2D>("PlayerRising1");
            sprites[0] = content.Load<Texture2D>("PlayerFalling");
            sprites[1] = content.Load<Texture2D>("PlayerRising1");
            sprites[2] = content.Load<Texture2D>("PlayerRising2");
            sprites[3] = content.Load<Texture2D>("PlayerRising3");
        }
    }
}
