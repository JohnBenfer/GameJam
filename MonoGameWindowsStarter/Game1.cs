﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace MonoGameWindowsStarter
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        List<Coin> coins;
        List<Gas> gasCans;
        List<Missle> missles;
        List<Bird> birds;
        Player player;
        public int SCREEN_WIDTH = 1920;
        public int SCREEN_HEIGHT = 1080;

        bool paused;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            
        }

        protected override void Initialize()
        {
            // Set the game screen size
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            graphics.ApplyChanges();

            player = new Player(this);
            paused = false;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        protected override void Update(GameTime gameTime)
        {
            

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (!paused)
            {

                player.Update();

                foreach (Coin c in coins)
                {
                    c.Update();
                }

                foreach (Gas g in gasCans)
                {
                    g.Update();
                }

                foreach (Missle m in missles)
                {
                    m.Update();
                }

                foreach (Bird b in birds)
                {
                    b.Update();
                }

            } else
            {
                SuppressDraw();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            player.Draw(spriteBatch);

            foreach (Coin c in coins)
            {
                c.Draw(spriteBatch);
            }

            foreach (Gas g in gasCans)
            {
                g.Draw(spriteBatch);
            }

            foreach (Missle m in missles)
            {
                m.Draw(spriteBatch);
            }

            foreach (Bird b in birds)
            {
                b.Draw(spriteBatch);
            }


            spriteBatch.End();

            base.Draw(gameTime);
        }


        public bool Collides(hitbox h1, hitbox h2)
        {
            Point l1 = new Point(h1.box.X, h1.box.Y);
            Point l2 = new Point(h2.box.X, h2.box.Y);
            Point r1 = new Point(h1.box.X + h1.box.Width, h1.box.Y + h1.box.Height);
            Point r2 = new Point(h2.box.X + h2.box.Width, h2.box.Y + h2.box.Height);

            // If one rectangle is on left side of other  
            if (l1.X > r2.X || l2.X > r1.X)
            {
                return false;
            }

            // If one rectangle is above other  
            if (l1.Y < r2.Y || l2.Y < r1.Y)
            {
                return false;
            }
            return true;

        }




    }
}
