using Microsoft.Xna.Framework;
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

        Texture2D background1;
        Texture2D background2;
        SpriteFont ScoreFont;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // lists of objects
        List<Coin> coins;
        List<Coin> coinsCollected;
        List<Coin> coinsOffScreen;
        List<Gas> gasCans;
        List<Gas> gasCollected;
        List<Gas> gasOffScreen;
        List<Missle> missles;
        List<Missle> misslesOffScreen;
        List<Bird> birds;
        List<Bird> birdsOffScreen;


        Player player;
        public int SCREEN_WIDTH = 1920;
        public int SCREEN_HEIGHT = 1080;

        double backgroundX;
        public double backgroundSpeed;

        double coinSpawnProbability = 0.01;
        double gasSpawnProbability = 0.02;
        double birdSpawnProbability = 0.06;
        double missleSpawnProbability = 0.015;

        int maxCoins = 3;
        int maxGas = 3;
        int maxBirds = 3;
        int maxMissles = 1;

        double score;
        int coinAmount;
        double gas;

        Random random = new Random();

        int boosterLevel = 1;

        bool paused;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            
        }

        protected override void Initialize()
        {
            // Set the game screen size
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            graphics.ApplyChanges();

            player = new Player(this, GetAcceleration(), GetMaxVelocity());
            coins = new List<Coin>();
            coinsCollected = new List<Coin>();
            coinsOffScreen = new List<Coin>();
            gasCans = new List<Gas>();
            gasCollected = new List<Gas>();
            gasOffScreen = new List<Gas>();
            missles = new List<Missle>();
            misslesOffScreen = new List<Missle>();
            birds = new List<Bird>();
            birdsOffScreen = new List<Bird>();

            paused = false;
            backgroundX = 0;
            backgroundSpeed = 4;
            gas = 100;
            coinAmount = 0;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ScoreFont = Content.Load<SpriteFont>("ScoreFont");
            background1 = Content.Load<Texture2D>("Background");
            //background2 = Content.Load<Texture2D>("Background");
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
                score += 0.05;
                if(backgroundX < -SCREEN_WIDTH)
                {
                    backgroundX = 0;
                }
                backgroundX -= backgroundSpeed;

                SpawnObjects();

                UpdateObjects();


                if (gas > 100)
                {
                    gas = 100;
                } else if(gas <= 0)
                {
                    GameOver();
                }

            } else
            {
                SuppressDraw();
            }



            base.Update(gameTime);
        }

        private void UpdateObjects()
        {
            player.Update();

            foreach (Coin c in coins)
            {
                if(Collides(c.hitbox, player.hitbox))
                {
                    coinsCollected.Add(c);
                    coinAmount++;
                } else if(c.offScreen)
                {
                    coinsOffScreen.Add(c);
                }
                c.Update();
            }

            RemoveAll(coins, coinsOffScreen);
            RemoveAll(coins, coinsCollected);

            foreach (Gas g in gasCans)
            {
                if (Collides(g.hitbox, player.hitbox))
                {
                    gasCollected.Add(g);
                    
                    gas+=10;
                    
                }
                else if (g.offScreen)
                {
                    gasOffScreen.Add(g);
                }
                g.Update();
            }
            RemoveAll(gasCans, gasOffScreen);
            RemoveAll(gasCans, gasCollected);

            foreach (Missle m in missles)
            {
                if (Collides(m.hitbox, player.hitbox))
                {
                    GameOver();
                }
                else if (m.offScreen)
                {
                    misslesOffScreen.Add(m);
                }
                m.Update();
            }
            RemoveAll(missles, misslesOffScreen);

            foreach (Bird b in birds)
            {
                if (Collides(b.hitbox, player.hitbox))
                {
                    GameOver();
                }
                else if (b.offScreen)
                {
                    birdsOffScreen.Add(b);
                }
                b.Update();
            }
            RemoveAll(birds, birdsOffScreen);


        }

        private void RemoveAll(List<Coin> source, List<Coin> temp)
        {
            foreach(Coin o in temp)
            {
                source.Remove(o);
            }
        }

        private void RemoveAll(List<Bird> source, List<Bird> temp)
        {
            foreach (Bird o in temp)
            {
                source.Remove(o);
            }
        }

        private void RemoveAll(List<Gas> source, List<Gas> temp)
        {
            foreach (Gas o in temp)
            {
                source.Remove(o);
            }
        }

        private void RemoveAll(List<Missle> source, List<Missle> temp)
        {
            foreach (Missle o in temp)
            {
                source.Remove(o);
            }
        }

        private void SpawnObjects()
        {
            int n = random.Next(0, 500);
            if(n < 10)
            {
                Console.WriteLine("coin");
                SpawnCoin();
                // spawn coin
            } else if(n < 15)
            {
                SpawnGas();
                // spawn gas
                Console.WriteLine("gas");
            } else if(n<25)
            {
                SpawnBird();
                Console.WriteLine("bird");
                // spawn bird
            } else if(n<35)
            {
                SpawnMissle();
                // spawn missile
                Console.WriteLine("Missle");
            }
        }

        private void SpawnCoin()
        {
            Coin c = new Coin(this);
            coins.Add(c);
            foreach(Gas g in gasCans)
            {
                if(Collides(c.hitbox, g.hitbox))
                {
                    coins.Remove(c);
                }
            }
            foreach(Bird b in birds)
            {
                if (Collides(c.hitbox, b.hitbox))
                {
                    coins.Remove(c);
                }
            }
            foreach (Missle m in missles)
            {
                if (Collides(c.hitbox, m.hitbox))
                {
                    coins.Remove(c);
                }
            }

        }

        private void SpawnGas()
        {
            Gas c = new Gas(this);
            gasCans.Add(c);

            foreach (Coin g in coins)
            {
                if (Collides(c.hitbox, g.hitbox))
                {
                    gasCans.Remove(c);
                }
            }
            foreach (Bird b in birds)
            {
                if (Collides(c.hitbox, b.hitbox))
                {
                    gasCans.Remove(c);
                }
            }
            foreach (Missle m in missles)
            {
                if (Collides(c.hitbox, m.hitbox))
                {
                    gasCans.Remove(c);
                }
            }
        }

        private void SpawnBird()
        {
            Bird c = new Bird(this);
            birds.Add(c);
            foreach (Gas g in gasCans)
            {
                if (Collides(c.hitbox, g.hitbox))
                {
                    birds.Remove(c);
                }
            }
            foreach (Coin b in coins)
            {
                if (Collides(c.hitbox, b.hitbox))
                {
                    birds.Remove(c);
                }
            }
            foreach (Missle m in missles)
            {
                if (Collides(c.hitbox, m.hitbox))
                {
                    birds.Remove(c);
                }
            }
        }

        private void SpawnMissle()
        {
            Missle c = new Missle(this);
            missles.Add(c);
            foreach (Gas g in gasCans)
            {
                if (Collides(c.hitbox, g.hitbox))
                {
                    missles.Remove(c);
                }
            }
            foreach (Bird b in birds)
            {
                if (Collides(c.hitbox, b.hitbox))
                {
                    missles.Remove(c);
                }
            }
            foreach (Coin m in coins)
            {
                if (Collides(c.hitbox, m.hitbox))
                {
                    missles.Remove(c);
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            spriteBatch.Draw(background1, new Rectangle(new Point((int)(backgroundX), 0), new Point(SCREEN_WIDTH, SCREEN_HEIGHT)), Color.White);
            spriteBatch.Draw(background1, new Rectangle(new Point((int)(backgroundX+SCREEN_WIDTH), 0), new Point(SCREEN_WIDTH, SCREEN_HEIGHT)), Color.White);

            spriteBatch.DrawString(ScoreFont, (int)score + "m", new Vector2((float)(SCREEN_WIDTH - 160), (float)(20)), Color.Black);
            spriteBatch.DrawString(ScoreFont, (int)coinAmount + " coins", new Vector2((float)(100), (float)(20)), Color.Black);

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


        public bool Collides(Hitbox h1, Hitbox h2)
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
            if (l1.Y > r2.Y || l2.Y > r1.Y)
            {
                return false;
            }
            return true;
            
        }

        private double GetAcceleration()
        {
            switch (boosterLevel)
            {
                case 1:
                    return 1.1;
                case 2:
                    return 1.5;
                case 3:
                    return 1.7;
                default:
                    return 1;
            }
            
        }

        private double GetMaxVelocity()
        {
            switch (boosterLevel)
            {
                case 1:
                    return 10;
                case 2:
                    return 12;
                case 3:
                    return 14;
                default:
                    return 10;
            }
        }

        public void GameOver()
        {

            Initialize();

        }


    }
}
