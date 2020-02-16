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
        int boosterTwoPrice = 1;
        int boosterThreePrice = 2;
        Texture2D background1;
        Texture2D background2;
        SpriteFont ScoreFont;
        SpriteFont BigScoreFont;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D coinTexture;
        Texture2D gasTexture;
        Texture2D play;
        Texture2D exit;
        Texture2D logo;
        Texture2D upgradeMenu;
        Color gasTextColor;

        // lists of objects
        List<Coin> coins;
        List<Coin> coinsCollected = new List<Coin>();
        List<Coin> coinsOffScreen;
        List<Gas> gasCans;
        List<Gas> gasCollected;
        List<Gas> gasOffScreen;
        List<Missle> missles;
        List<Missle> misslesOffScreen;
        List<Bird> birds;
        List<Bird> birdsOffScreen;
        List<Pole> poles;
        List<Pole> polesOffScreen;

        bool hitDetection = true;
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

        double score = 0;
        int coinAmount = 0;
        double gas;

        Random random = new Random();

        int boosterLevel = 1;

        bool gamePaused = false;
        bool gameStart = true;
        bool gameOver = false;
        bool gameUpgradeMenu = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            
        }

        protected override void Initialize()
        {
            
            gasTextColor = Color.Green;
            // Set the game screen size
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            graphics.ApplyChanges();

            player = new Player(this, GetAcceleration(), GetMaxVelocity(), boosterLevel);
            coins = new List<Coin>();
            coinsOffScreen = new List<Coin>();
            gasCans = new List<Gas>();
            gasCollected = new List<Gas>();
            gasOffScreen = new List<Gas>();
            missles = new List<Missle>();
            misslesOffScreen = new List<Missle>();
            birds = new List<Bird>();
            birdsOffScreen = new List<Bird>();
            poles = new List<Pole>();
            polesOffScreen = new List<Pole>();
            //score = 0;
            gamePaused = false;
            backgroundX = 0;
            backgroundSpeed = 4;
            gas = 100;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ScoreFont = Content.Load<SpriteFont>("ScoreFont");
            BigScoreFont = Content.Load<SpriteFont>("BigScoreFont");
            background1 = Content.Load<Texture2D>("Background");
            coinTexture = Content.Load<Texture2D>("Coin");
            gasTexture = Content.Load<Texture2D>("Gas");
            play = Content.Load<Texture2D>("Play");
            exit = Content.Load<Texture2D>("Exit");
            logo = Content.Load<Texture2D>("Logo");
            upgradeMenu = Content.Load<Texture2D>("UpgradeMenu");
            //background2 = Content.Load<Texture2D>("Background");
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        protected override void Update(GameTime gameTime)
        {

            var keyboard = Keyboard.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (backgroundX < -SCREEN_WIDTH)
            {
                backgroundX = 0;
            }
            backgroundX -= backgroundSpeed;
            if (!gamePaused && !gameStart && !gameOver && !gameUpgradeMenu)
            {
                score += 0.05;


                SpawnObjects();

                UpdateObjects();

                gas -= 0.01;
                if(keyboard.IsKeyDown(Keys.Space)) {
                    gas -= 0.05;
                }
                if (gas > 100)
                {
                    gas = 100;
                } else if(gas <= 0)
                {
                    GameOver();
                } else if(gas <= 20)
                {
                    gasTextColor = Color.Red;
                } else if(gas <= 30)
                {
                    gasTextColor = Color.Yellow;
                } else
                {
                    gasTextColor = Color.Green;
                }

            } else if(gameStart)
            {

                
                if (keyboard.IsKeyDown(Keys.Space))
                {
                    gameStart = false;
                    gamePaused = false;

                }
            } else if(gameOver)
            {
                if (keyboard.IsKeyDown(Keys.Enter))
                {
                    gameUpgradeMenu = true;
                    gameOver = false;
                    score = 0;
                }
            } 
            else if(gameUpgradeMenu)
            {
                if(keyboard.IsKeyDown(Keys.D2))
                {
                    if (coinAmount >= boosterTwoPrice && boosterLevel == 1)
                    {
                        boosterLevel = 2;
                        coinAmount -= boosterTwoPrice;
                    } else
                    {
                        // play sound of not enough money
                    }

                } else if(keyboard.IsKeyDown(Keys.D3))
                {
                    if (boosterLevel == 2 && coinAmount >= boosterThreePrice)
                    {
                        boosterLevel = 3;
                        coinAmount -= boosterThreePrice;
                    } else
                    {
                        // play sound of not enough money
                    }

                } else if(keyboard.IsKeyDown(Keys.Space))
                {
                    gameUpgradeMenu = false;
                    Initialize();
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
                if (hitDetection)
                {
                    if (Collides(m.hitbox, player.hitbox))
                    {
                        GameOver();
                    }
                }
                if (m.offScreen)
                {
                    misslesOffScreen.Add(m);
                }
                m.Update();
            }
            RemoveAll(missles, misslesOffScreen);

            foreach (Bird b in birds)
            {
                if (hitDetection)
                {
                    if (Collides(b.hitbox, player.hitbox))
                    {
                        GameOver();
                    }
                }
                if (b.offScreen)
                {
                    birdsOffScreen.Add(b);
                }
                b.Update();
            }
            RemoveAll(birds, birdsOffScreen);

            foreach (Pole b in poles)
            {
                if (hitDetection)
                {
                    if (Collides(b.hitbox, player.hitbox))
                    {
                        GameOver();
                    }
                }
                if (b.offScreen)
                {
                    polesOffScreen.Add(b);
                }
                b.Update();
            }
            RemoveAll(poles, polesOffScreen);


        }

        private void RemoveAll(List<Coin> source, List<Coin> temp)
        {
            foreach(Coin o in temp)
            {
                source.Remove(o);
            }
        }

        private void RemoveAll(List<Pole> source, List<Pole> temp)
        {
            foreach (Pole o in temp)
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
                SpawnCoin();
                // spawn coin
            } else if(n < 15)
            {
                SpawnGas();
                // spawn gas

            } else if(n<25)
            {
                SpawnBird();
                // spawn bird
            } else if(n<35)
            {
                SpawnMissle();
                // spawn missile
            } else if(n<40)
            {
                SpawnPole();
                
            }
        }

        private void SpawnPole()
        {
            Pole c = new Pole(this);
            poles.Add(c);
            foreach (Pole coin in poles)
            {
                if (Collides(c.hitbox, coin.hitbox))
                {
                    if (c != coin)
                    {
                        c.X = coin.X + coin.width + 20;
                        c.Y = coin.Y;
                    }
                }
            }
            foreach (Gas g in gasCans)
            {
                if (Collides(c.hitbox, g.hitbox))
                {
                    poles.Remove(c);
                }
            }
            foreach (Bird b in birds)
            {
                if (Collides(c.hitbox, b.hitbox))
                {
                    poles.Remove(c);
                }
            }
            foreach (Missle m in missles)
            {
                if (Collides(c.hitbox, m.hitbox))
                {
                    poles.Remove(c);
                }
            }

            foreach(Coin p in coins)
            {
                if(Collides(p.hitbox, c.hitbox))
                {
                    poles.Remove(c);
                }
            }

        }

        private void SpawnCoin()
        {
            Coin c = new Coin(this);
            coins.Add(c);
            foreach(Coin coin in coins)
            {
                if(Collides(c.hitbox, coin.hitbox))
                {
                    if(c != coin)
                    {
                        c.X = coin.X + coin.width + 20;
                        c.Y = coin.Y;
                    }
                }
            }
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
            foreach (Pole p in poles)
            {
                if (Collides(c.hitbox, p.hitbox))
                {
                    coins.Remove(c);
                }
            }

        }

        private void SpawnGas()
        {
            Gas c = new Gas(this);
            gasCans.Add(c);

            foreach (Gas coin in gasCans)
            {
                if (Collides(c.hitbox, coin.hitbox))
                {
                    if (c != coin)
                    {
                        c.X = coin.X + coin.width + 20;
                        c.Y = coin.Y;
                    }
                }
            }
            foreach (Pole p in poles)
            {
                if (Collides(c.hitbox, p.hitbox))
                {
                    gasCans.Remove(c);
                }
            }
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
            foreach (Bird coin in birds)
            {
                if (Collides(c.hitbox, coin.hitbox))
                {
                    if (c != coin)
                    {
                        c.X = coin.X + coin.width + 20;
                        c.Y = coin.Y;
                    }
                }
            }
            foreach (Pole p in poles)
            {
                if (Collides(c.hitbox, p.hitbox))
                {
                    birds.Remove(c);
                }
            }
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
            foreach (Missle coin in missles)
            {
                if (Collides(c.hitbox, coin.hitbox))
                {
                    if (c != coin)
                    {
                        c.X = coin.X + coin.width + 20;
                        c.Y = coin.Y;
                    }
                }
            }
            foreach (Pole p in poles)
            {
                if (Collides(c.hitbox, p.hitbox))
                {
                    missles.Remove(c);
                }
            }
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
            spriteBatch.DrawString(ScoreFont, coinAmount.ToString(), new Vector2((float)(100), (float)(20)), Color.Black);
            spriteBatch.DrawString(ScoreFont, (int)gas + "%", new Vector2((float)(290), (float)(20)), gasTextColor);

            spriteBatch.Draw(coinTexture, new Rectangle(50, 20, 40, 40), null, Color.White, 0f, new Vector2(20, 20), SpriteEffects.None, 0);
            spriteBatch.Draw(gasTexture, new Rectangle(250, 20, 40, 40), null, Color.White, 0f, new Vector2(20, 20), SpriteEffects.None, 0);

            if(gameStart)
            {

                spriteBatch.Draw(logo, new Rectangle(SCREEN_WIDTH / 2, 250, 700, 400), null, Color.White, 0f, new Vector2(logo.Width / 2, logo.Height / 2), SpriteEffects.None, 1);
                spriteBatch.Draw(play, new Rectangle(SCREEN_WIDTH / 2, (SCREEN_HEIGHT / 2), 400, 240), null, Color.White, 0f, new Vector2(play.Width/2, play.Height/2), SpriteEffects.None, 0);
                spriteBatch.Draw(exit, new Rectangle(SCREEN_WIDTH / 2, (SCREEN_HEIGHT / 2) + 200, 400, 240), null, Color.White, 0f, new Vector2(exit.Width/2, exit.Height/2), SpriteEffects.None, 0);
            } else if(gameOver)
            {
                spriteBatch.Draw(coinTexture, new Rectangle((SCREEN_WIDTH/2) - 100, (SCREEN_HEIGHT / 2) - 115, 100, 100), null, Color.White, 0f, new Vector2(20, 20), SpriteEffects.None, 0);
                spriteBatch.DrawString(BigScoreFont, coinAmount.ToString(), new Vector2((SCREEN_WIDTH / 2) + 10, (SCREEN_HEIGHT / 2) - 120), Color.Black);
                spriteBatch.DrawString(BigScoreFont, "Final Score " + (int)score + "m", new Vector2((SCREEN_WIDTH / 2) - 200, (SCREEN_HEIGHT / 2)), Color.Black);
            } else if(gameUpgradeMenu)
            {
                spriteBatch.Draw(upgradeMenu, new Rectangle((SCREEN_WIDTH / 2) - 150, (SCREEN_HEIGHT / 2) - 300, 700, 700), null, Color.White, 0f, new Vector2(200, 200), SpriteEffects.None, 0);
            }

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

            foreach (Pole b in poles)
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
            gameOver = true;
            Initialize();
            

        }


    }
}
