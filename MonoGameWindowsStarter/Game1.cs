using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace MonoGameWindowsStarter
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        int boosterTwoPrice = 30;
        int boosterThreePrice = 100;
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
        Texture2D continueButton;
        SoundEffect CoinPickup;
        SoundEffect Thruster;
        SoundEffect Error;
        SoundEffect MissleLock;
        SoundEffect GasPickup;
        SoundEffect Electricity;
        SoundEffect GameOverSound;
        SoundEffect Mopar;
        Song BackgroundSong;
        Color gasTextColor;

        

        float soundVolume;
        float musicVolume;

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

        int coinSpawnProbability;
        int gasSpawnProbability;
        int birdSpawnProbability;
        int missleSpawnProbability;
        int poleSpawnProbability;

        Keys lastKey;

        int maxCoins;
        int maxGas;
        int maxBirds;
        int maxMissles;
        int maxPoles;

        double score = 0;
        int coinAmount = 0;
        double gas;

        Random random = new Random();

        int boosterLevel = 1;

        bool gamePaused = false;
        bool gameStart = true;
        bool gameOver = false;
        public bool gameUpgradeMenu = false;

        bool tempLock = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            
        }

        protected override void Initialize()
        {
            coinSpawnProbability = 10;
            gasSpawnProbability = 10;
            birdSpawnProbability = 2;
            missleSpawnProbability = 1;
            poleSpawnProbability = 1;
            maxCoins = 4;
            maxGas = 3;
            maxBirds = 2;
            maxMissles = 1;
            maxPoles = 1;
            soundVolume = 0.05f;
            musicVolume = 0.1f;
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
            if(gameStart)
            {
                MediaPlayer.Play(BackgroundSong);
                MediaPlayer.Volume = musicVolume;
            }
         }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ScoreFont = Content.Load<SpriteFont>("ScoreFont");
            BigScoreFont = Content.Load<SpriteFont>("BigScoreFont");
            background1 = Content.Load<Texture2D>("BackgroundBack");
            background2 = Content.Load<Texture2D>("BackgroundFront");
            coinTexture = Content.Load<Texture2D>("Coin");
            gasTexture = Content.Load<Texture2D>("Gas");
            play = Content.Load<Texture2D>("Play");
            exit = Content.Load<Texture2D>("Exit");
            logo = Content.Load<Texture2D>("Logo");
            upgradeMenu = Content.Load<Texture2D>("UpgradeMenu");
            continueButton = Content.Load<Texture2D>("Continue");
            CoinPickup = Content.Load<SoundEffect>("CoinPickup");
            Thruster = Content.Load<SoundEffect>("Thruster");
            Error = Content.Load<SoundEffect>("Error");
            MissleLock = Content.Load<SoundEffect>("MissileLock");
            GasPickup = Content.Load<SoundEffect>("GasPickup");
            Electricity = Content.Load<SoundEffect>("Electricity");
            GameOverSound = Content.Load<SoundEffect>("GameOver");
            Mopar = Content.Load<SoundEffect>("MoparFinal");
            BackgroundSong = Content.Load<Song>("BackgroundSong");

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
                score += 0.1;

                UpdateLevel();

                if(MediaPlayer.State == MediaState.Stopped)
                {
                    MediaPlayer.Play(BackgroundSong);
                }

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
                        player.boosterLevel = 2;
                        PlaySound("mopar");
                        tempLock = true;
                    } else if(tempLock == false)
                    {

                        // play sound of not enough money
                        PlaySound("error");
                    }

                } else if(keyboard.IsKeyDown(Keys.D3))
                {
                    if (boosterLevel == 2 && coinAmount >= boosterThreePrice)
                    {
                        boosterLevel = 3;
                        coinAmount -= boosterThreePrice;
                        player.boosterLevel = 3;
                        PlaySound("mopar");
                        tempLock = true;
                    } else if(tempLock == false)
                    {
                        // play sound of not enough money
                        PlaySound("error");
                    }

                } else if(keyboard.IsKeyDown(Keys.Space))
                {
                    gameUpgradeMenu = false;
                    Initialize();
                } else
                {
                    tempLock = false;
                }
            } else
            {
                SuppressDraw();
            }



            base.Update(gameTime);
        }

        private void UpdateLevel()
        {
            Console.WriteLine(gasCans.Count);
            if(score > 1000)
            {
                coinSpawnProbability = 10;
                gasSpawnProbability = 38;
                birdSpawnProbability = 50;
                missleSpawnProbability = 30;
                poleSpawnProbability = 40;
                maxCoins = 6;
                maxGas = 5;
                maxBirds = 8;
                maxMissles = 4;
                maxPoles = 3;
            } else if(score > 800)
            {
                coinSpawnProbability = 8;
                gasSpawnProbability = 34;
                birdSpawnProbability = 40;
                missleSpawnProbability = 15;
                poleSpawnProbability = 25;
                maxCoins = 5;
                maxGas = 4;
                maxBirds = 7;
                maxMissles = 3;
                maxPoles = 2;
            } else if(score > 600)
            {
                coinSpawnProbability = 8;
                gasSpawnProbability = 20;
                birdSpawnProbability = 25;
                missleSpawnProbability = 15;
                poleSpawnProbability = 20;
                maxCoins = 5;
                maxGas = 4;
                maxBirds = 6;
                maxMissles = 2;
                maxPoles = 2;
            } else if(score > 400)
            {
                coinSpawnProbability = 8;
                gasSpawnProbability = 20;
                birdSpawnProbability = 25;
                missleSpawnProbability = 15;
                poleSpawnProbability = 8;
                maxCoins = 5;
                maxGas = 3;
                maxBirds = 5;
                maxMissles = 2;
                maxPoles = 2;
            } else if(score > 300)
            {
                coinSpawnProbability = 8;
                gasSpawnProbability = 18;
                birdSpawnProbability = 20;
                missleSpawnProbability = 15;
                poleSpawnProbability = 18;
                maxCoins = 5;
                maxGas = 3;
                maxBirds = 5;
                maxMissles = 2;
                maxPoles = 2;
            } else if(score > 200)
            {
                coinSpawnProbability = 8;
                gasSpawnProbability = 20;
                birdSpawnProbability = 10;
                missleSpawnProbability = 6;
                poleSpawnProbability = 4;
                maxCoins = 5;
                maxGas = 3;
                maxBirds = 4;
                maxMissles = 2;
                maxPoles = 2;
            } else if(score > 100)
            {
                coinSpawnProbability = 8;
                gasSpawnProbability = 20;
                birdSpawnProbability = 5;
                missleSpawnProbability = 3;
                poleSpawnProbability = 3;
                maxCoins = 4;
                maxGas = 4;
                maxBirds = 4;
                maxMissles = 2;
                maxPoles = 1;
            } else if(score > 50)
            {
                coinSpawnProbability = 8;
                gasSpawnProbability = 12;
                birdSpawnProbability = 3;
                missleSpawnProbability = 1;
                poleSpawnProbability = 1;
                maxCoins = 4;
                maxGas = 3;
                maxBirds = 4;
                maxMissles = 1;
                maxPoles = 1;
            }
        }
        public void PlaySound(string sound)
        {
            if(sound.ToUpper().Equals("COIN"))
            {
                CoinPickup.Play(soundVolume, 0, 0);
            } else if(sound.ToUpper().Equals("THRUSTER"))
            {
                Thruster.Play(soundVolume, 0, 0);
            }
            else if (sound.ToUpper().Equals("ERROR"))
            {
                Error.Play((float)(soundVolume * 0.2), 0, 0);
            } else if(sound.ToUpper().Equals("MISSLE"))
            {
                Thruster.Play(soundVolume, 0, 0);
            }
            else if (sound.ToUpper().Equals("GAS"))
            {
                GasPickup.Play(soundVolume, 0, 0);
            }
            else if (sound.ToUpper().Equals("ELECTRICITY"))
            {
                Electricity.Play(soundVolume, 0, 0);
            }
            else if (sound.ToUpper().Equals("GAMEOVER"))
            {
                GameOverSound.Play(soundVolume, 0, 0);
            } else if(sound.ToUpper().Equals("MOPAR"))
            {
                Mopar.Play(soundVolume * 20, 0, 0);
            }
            

            /*
            CoinPickup = Content.Load<SoundEffect>("CoinPickup");
            Thruster = Content.Load<SoundEffect>("Thruster");
            Error = Content.Load<SoundEffect>("Error");
            MissleLock = Content.Load<SoundEffect>("Missle");
            GasPickup = Content.Load<SoundEffect>("GasPickup");
            Electricity = Content.Load<SoundEffect>("Electricity");
            GameOverSound = Content.Load<SoundEffect>("GameOver");
            */
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
                    PlaySound("coin");
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

                    PlaySound("gas");
                    
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
            
            int n = random.Next(0, 2000);
            if(n < 15)
            {
                if(coins.Count < maxCoins)
                {
                    SpawnCoin();
                }
                // spawn coin
            } else if(n < coinSpawnProbability + gasSpawnProbability)
            {
                if (gasCans.Count <= maxGas)
                {
                    SpawnGas();
                }
                // spawn gas

            } else if(n< coinSpawnProbability + gasSpawnProbability + birdSpawnProbability)
            {
                if (birds.Count <= maxBirds)
                {
                    SpawnBird();
                }
                // spawn bird
            } else if(n< coinSpawnProbability + gasSpawnProbability + birdSpawnProbability + missleSpawnProbability)
            {
                if (missles.Count <= maxMissles)
                {
                    SpawnMissle();
                }
                // spawn missile
            } else if(n< coinSpawnProbability + gasSpawnProbability + birdSpawnProbability + poleSpawnProbability)
            {
                if (poles.Count <= maxPoles)
                {
                    SpawnPole();
                }
                
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
            if(poles.Contains(c))
            {
                PlaySound("electricity");
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
            if (missles.Contains(c))
            {
                PlaySound("missle");
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            spriteBatch.Draw(background1, new Rectangle(new Point(0, 0), new Point(SCREEN_WIDTH, SCREEN_HEIGHT)), Color.White);
            spriteBatch.Draw(background2, new Rectangle(new Point((int)(backgroundX), 0), new Point(SCREEN_WIDTH, SCREEN_HEIGHT)), Color.White);
            spriteBatch.Draw(background2, new Rectangle(new Point((int)(backgroundX+SCREEN_WIDTH), 0), new Point(SCREEN_WIDTH, SCREEN_HEIGHT)), Color.White);

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
                spriteBatch.DrawString(ScoreFont, 
                    "Instructions: Collect coins for upgrades and gas cans for fuel. Avoid birds, missiles and power lines. ", 
                    new Vector2(50, ((SCREEN_HEIGHT / 2) + 330)), Color.Black);
                spriteBatch.DrawString(ScoreFont,
    "Music rights: [Merlin] XelonEntertainment(on behalf of 8 - Bit Arcade); CMRRA, and 6 Music Rights Societies ",
    new Vector2(50, (SCREEN_HEIGHT) - 50), Color.Black);
            } else if(gameOver)
            {
                spriteBatch.Draw(coinTexture, new Rectangle((SCREEN_WIDTH/2) - 100, (SCREEN_HEIGHT / 2) - 115, 100, 100), null, Color.White, 0f, new Vector2(20, 20), SpriteEffects.None, 0);
                spriteBatch.DrawString(BigScoreFont, coinAmount.ToString(), new Vector2((SCREEN_WIDTH / 2) + 10, (SCREEN_HEIGHT / 2) - 120), Color.Black);
                spriteBatch.DrawString(BigScoreFont, "Final Score " + (int)score + "m", new Vector2((SCREEN_WIDTH / 2) - 200, (SCREEN_HEIGHT / 2)), Color.Black);
                spriteBatch.Draw(continueButton, new Rectangle((SCREEN_WIDTH / 2) - 100, (SCREEN_HEIGHT / 2) + 100, 400, 300), null, Color.White, 0f, new Vector2(20, 20), SpriteEffects.None, 0);
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
            MediaPlayer.Stop();
            PlaySound("GameOver");
            gameOver = true;
            Initialize();
            

        }


    }
}
