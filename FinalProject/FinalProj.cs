using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using CPI311.GameEngine;


namespace FinalProject
{
    public class FinalProj : Game
    {
        //--------------------------------------------------
        public class Scene
        {
            public delegate void CallMethod(GameTime gameTime);
            public CallMethod Update;
            public CallMethod Draw;
            public Scene(CallMethod update, CallMethod draw)
            { Update = update; Draw = draw; }
        }
        //--------------------------------------------------

        // Scene Stuff
        //-------------------------------
        Dictionary<String, Scene> scenes;
        Scene currentScene;
        List<GUIElement> menuGUI;
        List<GUIElement> controlsGUI;
        List<GUIElement> playGUI;
        //-------------------------------

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Camera and Light;
        Camera camera;
        Light light;

        // Visual Content
        Player player;
        MotherShip mother;
        Projectile[] projectiles;
        SimpleEnemy[] enemies;
        Powerup[] powerups;

        // Sound
        SoundEffect confirmSound;
        SoundEffect gunSound;
        SoundEffect expSound;
        SoundEffect bigExpSound;
        SoundEffect enemySpawn;
        SoundEffectInstance soundInstance;

        // Gameplay UI
        Texture2D stars;
        Texture2D D_frame;
        Texture2D M_frame;
        Texture2D S_frame;
        SpriteFont lucidaConsole;
        float Timer;
        int score;
        Vector2 timerPosition = new Vector2(50, 50);
        Vector2 scorePosition = new Vector2(50, 80);
        ProgressBar playerHealthBar;
        ProgressBar motherHealthBar;
        Vector2 playerHealthPosition = new Vector2(250, 50);
        Vector2 motherHealthPosition = new Vector2(250, 80);

        Random random; // Random

        float enemySpawnTimer;

        GameConstant.GameStates currentGameState = GameConstant.GameStates.Play;

        public FinalProj()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = 1280;  // set this value to the desired width of your window
            _graphics.PreferredBackBufferHeight = 720;   // set this value to the desired height of your window
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            // Initialize CPI311 GameEngine Stuff
            //----------------------------------
            Time.Initialize();
            InputManager.Initialize();
            ScreenManager.Initialize(_graphics);
            //----------------------------------

            random = new Random(); // Initialize random

            enemySpawnTimer = GameConstant.MaxEnemySpawnTime;

            // Arrays
            //--------------------------------------------------------
            projectiles = new Projectile[GameConstant.MaxProjectiles];
            enemies = new SimpleEnemy[GameConstant.MaxEnemies];
            powerups = new Powerup[GameConstant.MaxEnemies];
            //--------------------------------------------------------

            // GUI Elements
            //-----------------------------------
            menuGUI = new List<GUIElement>();
            controlsGUI = new List<GUIElement>();
            playGUI = new List<GUIElement>();
            //-----------------------------------

            // Scenes
            //----------------------------------------------------------
            scenes = new Dictionary<string, Scene>();
            scenes.Add("Menu", new Scene(MainMenuUpdate, MainMenuDraw));
            scenes.Add("Controls", new Scene(ControlsUpdate, ControlsDraw));
            scenes.Add("Play", new Scene(PlayUpdate, PlayDraw));
            currentScene = scenes["Menu"];
            //----------------------------------------------------------

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Initialize Square Texture for Stuff
            //-------------------------------------------------------//
            Texture2D square = Content.Load<Texture2D>("Resources/Textures/Square");
            //-------------------------------------------------------//

            // GUI Stuff
            //--------------------------------------------------------------------------------------
            Button PlayButton = new Button();
            PlayButton.Texture = square;
            PlayButton.Text = "Play";
            PlayButton.Bounds = new Rectangle(GraphicsDevice.Viewport.Width/2 - 100,
                                              GraphicsDevice.Viewport.Height/2, 200, 50);
            PlayButton.Action += PlayGame;
            menuGUI.Add(PlayButton);

            Button ControlsButton = new Button();
            ControlsButton.Texture = square;
            ControlsButton.Text = "Controls";
            ControlsButton.Bounds = new Rectangle(GraphicsDevice.Viewport.Width / 2 - 100,
                                                  GraphicsDevice.Viewport.Height / 2 + 75, 200, 50);
            ControlsButton.Action += Controls;
            menuGUI.Add(ControlsButton);

            Button ExitButton = new Button();
            ExitButton.Texture = square;
            ExitButton.Text = "Exit";
            ExitButton.Bounds = new Rectangle(GraphicsDevice.Viewport.Width / 2 - 100,
                                              GraphicsDevice.Viewport.Height / 2 + 150, 200, 50);
            ExitButton.Action += ExitGame;
            menuGUI.Add(ExitButton);
            playGUI.Add(ExitButton);

            Button BackButton = new Button();
            BackButton.Texture = square;
            BackButton.Text = "Back";
            BackButton.Bounds = new Rectangle(GraphicsDevice.Viewport.Width / 2 - 100,
                                              GraphicsDevice.Viewport.Height / 2 + 150, 200, 50);
            BackButton.Action += Back;
            controlsGUI.Add(BackButton);
            //--------------------------------------------------------------------------------------

            stars = Content.Load<Texture2D>("Resources/bg_stars"); // Load Background

            // Load Frames
            //--------------------------------------------------------------
            D_frame = Content.Load<Texture2D>("Resources/Textures/D_Frame");
            M_frame = Content.Load<Texture2D>("Resources/Textures/M_Frame");
            S_frame = Content.Load<Texture2D>("Resources/Textures/S_Frame");
            //--------------------------------------------------------------

            lucidaConsole = Content.Load<SpriteFont>("HUD_Font"); // Load Font

            // Load Sounds
            //--------------------------------------------------------------------------------
            confirmSound = Content.Load<SoundEffect>("Resources/Sound/SFX/Flashpoint001a");
            gunSound = Content.Load<SoundEffect>("Resources/Sound/SFX/sfx_tx0_fire1");
            expSound = Content.Load<SoundEffect>("Resources/Sound/SFX/sfx_explosion2");
            bigExpSound = Content.Load<SoundEffect>("Resources/Sound/SFX/sfx_explosion3");
            enemySpawn = Content.Load<SoundEffect>("Resources/Sound/SFX/hyperspace_activate");
            //--------------------------------------------------------------------------------

            // Load Camera
            //----------------------------------------------------------------------
            camera = new Camera();
            camera.Transform = new Transform();
            camera.NearPlane = 0.01f;
            camera.FarPlane = GameConstant.CameraHeight + 100;
            camera.Transform.LocalPosition = Vector3.Up * GameConstant.CameraHeight;
            camera.Transform.Rotate(Vector3.Left, MathHelper.PiOver2);
            //----------------------------------------------------------------------

            // Load Light
            //--------------------------------------------------------------------------------
            light = new Light();
            light.Transform = new Transform();
            light.Transform.LocalPosition = Vector3.Backward * Vector3.Left * Vector3.Up * 5f;
            //--------------------------------------------------------------------------------

            // Initialize Player Health Bar
            //-------------------------------------------------------------------------------------------------------------------//
            playerHealthBar = new ProgressBar(square, new Vector2(3f * square.Width, square.Height/2), new Vector2(2f, 2f));
            playerHealthBar.Position = playerHealthPosition;
            playerHealthBar.Speed = 0f;
            playerHealthBar.Value = GameConstant.MaxPlayerHealth;
            playerHealthBar.MaxValue = GameConstant.MaxPlayerHealth;
            playerHealthBar.FillColor = Color.Red;
            playerHealthBar.Update();
            //-------------------------------------------------------------------------------------------------------------------//

            // Initialize Mother Health Bar
            //-------------------------------------------------------------------------------------------------------------------//
            motherHealthBar = new ProgressBar(square, new Vector2(3f * square.Width, square.Height/2), new Vector2(2f, 2f));
            motherHealthBar.Position = motherHealthPosition;
            motherHealthBar.Speed = 0f;
            motherHealthBar.Value = GameConstant.MaxMotherHealth;
            motherHealthBar.MaxValue = GameConstant.MaxMotherHealth;
            motherHealthBar.FillColor = Color.Green;
            motherHealthBar.Update();
            //-------------------------------------------------------------------------------------------------------------------//

            // Load Player
            //----------------------------------------------------------
            player = new Player(Content, camera, GraphicsDevice, light);
            player.Transform.LocalPosition += Vector3.Backward * 60f;
            //----------------------------------------------------------

            mother = new MotherShip(player, Content, camera, GraphicsDevice, light); // Load Mothership
        }

        protected override void Update(GameTime gameTime)
        {
            // Update CPI311 Stuff
            //--------------------
            Time.Update(gameTime);
            InputManager.Update();
            //--------------------

            if (InputManager.IsKeyPressed(Keys.Escape)) Exit();

            currentScene.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            currentScene.Draw(gameTime);

            base.Draw(gameTime);
        }

        // Helper Methods
        //---------------------------------------------------------------------------------
        private void PlayerShooting()
        {
            bool press = InputManager.IsKeyPressed(Keys.Space);
            bool held = InputManager.IsKeyDown(Keys.Space);

            if (player.fireCooldown == 0)
            {
                switch (player.CurrentWeapon)
                {
                    case GameConstant.Weapons.Default:
                        if (press)
                        {
                            Projectile bullet = new Projectile(true, GameConstant.Projectiles.bullet, Content, camera, GraphicsDevice, light);
                            bullet.Transform.LocalPosition = player.Transform.LocalPosition + player.Transform.Forward * 10;
                            bullet.Rigidbody.Velocity = player.Transform.Forward * GameConstant.BulletSpeed;

                            if (AddProjectile(bullet))
                            {
                                player.fireCooldown = GameConstant.FireCooldown;
                                PlaySound(gunSound, false, 0.25f, 0.25f);
                            }
                        }
                        break;
                    case GameConstant.Weapons.machine_gun:
                        if (held)
                        {
                            Projectile bullet = new Projectile(true, GameConstant.Projectiles.bullet, Content, camera, GraphicsDevice, light);
                            bullet.Transform.LocalPosition = player.Transform.LocalPosition + player.Transform.Forward * 10;
                            bullet.Rigidbody.Velocity = player.Transform.Forward * GameConstant.BulletSpeed;

                            if (AddProjectile(bullet))
                            {
                                player.fireCooldown = GameConstant.FireCooldown;
                                PlaySound(gunSound, false, 0.125f, 0.25f);
                            }
                        }
                        break;
                    case GameConstant.Weapons.spread_gun:
                        if (press)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                Projectile bullet = new Projectile(true, GameConstant.Projectiles.bullet, Content, camera, GraphicsDevice, light);
                                bullet.Transform.LocalPosition = player.Transform.LocalPosition + player.Transform.Forward * 10;

                                if (i == 0)
                                {
                                    bullet.Rigidbody.Velocity = player.Transform.Forward * GameConstant.BulletSpeed;
                                }
                                else if (i == 1)
                                {
                                    Matrix rotation = Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.Pi / 15);
                                    bullet.Rigidbody.Velocity = Vector3.Transform(player.Transform.Forward, rotation) * GameConstant.BulletSpeed;
                                }
                                else if (i == 2)
                                {
                                    Matrix rotation = Matrix.CreateFromAxisAngle(Vector3.Up, -MathHelper.Pi / 15);
                                    bullet.Rigidbody.Velocity = Vector3.Transform(player.Transform.Forward, rotation) * GameConstant.BulletSpeed;
                                }

                                if (AddProjectile(bullet))
                                {
                                    player.fireCooldown = GameConstant.FireCooldown;
                                    PlaySound(gunSound, false, 0.083f, 0.25f);
                                }
                            }
                        }
                        break;
                }
            }
        }

        private bool AddProjectile(Projectile projectile)
        {
            for (int i = 0; i < projectiles.Length; i++)
            {
                if (projectiles[i] == null)
                {
                    projectiles[i] = projectile;
                    return true;
                }
            }
            return false;
        }

        private void CreateEnemy()
        {
            float xStart;
            float yStart;

            int randSpawn = random.Next(1, 5);

            for (int i = 0; i < randSpawn; i++)
            {
                if (random.Next(2) == 0)
                {
                    xStart = -(float)GameConstant.PlayfieldSizeX;
                }
                else
                {
                    xStart = (float)GameConstant.PlayfieldSizeX;
                }
                yStart = (float)random.NextDouble() * GameConstant.PlayfieldSizeY;

                if (enemies[i] == null)
                {
                    if (random.Next(2) == 0)
                    {
                        enemies[i] = new SimpleEnemy(mother, player, GameConstant.Enemies.asteroid, Content, camera, GraphicsDevice, light);
                    }

                    else
                    {
                        enemies[i] = new SimpleEnemy(mother, player, GameConstant.Enemies.ship, Content, camera, GraphicsDevice, light);
                    }

                    enemies[i].Transform.LocalPosition = new Vector3(xStart, 0.0f, yStart);

                    Vector3 direction = Vector3.Normalize(Vector3.Zero - enemies[i].Transform.LocalPosition);

                    float angle = MathF.Acos(MathHelper.Clamp(Vector3.Dot(enemies[i].Transform.Forward, direction), -1, 1));

                    /*  Determines whether to turn left or right */
                    Vector3 direction2 = new Vector3(direction.Z, 0, -direction.X);
                    if (Vector3.Dot(direction2, enemies[i].Transform.Forward) > 0) angle *= -1;

                    enemies[i].Transform.Rotate(Vector3.Up, angle);

                    enemies[i].Rigidbody.Velocity = enemies[i].Transform.Forward * GameConstant.MinEnemySpeed;
                    enemies[i].isActive = true;
                }
            }

            float currentMax = MathHelper.Clamp(GameConstant.MaxEnemySpawnTime - 0.5f * ((int)(Timer) / 10), GameConstant.MinEnemySpawnTime, GameConstant.MaxEnemySpawnTime);

            enemySpawnTimer = MathHelper.Clamp(currentMax * (float) (random.NextDouble()) + GameConstant.MinEnemySpawnTime, GameConstant.MinRespawnTimer, currentMax);
            PlaySound(enemySpawn, false, 0.25f, 1f);
        }

        private void PlaySound(SoundEffect sound, bool isLooped, float volume, float pitch)
        {
            soundInstance = sound.CreateInstance();
            soundInstance.IsLooped = isLooped;
            soundInstance.Volume = volume;
            soundInstance.Pitch = pitch;
            soundInstance.Play();
        }
        //---------------------------------------------------------------------------------

        // Scenes Stuff
        //--------------------------------------------
        private void MainMenuUpdate(GameTime gameTime)
        {
            foreach (GUIElement element in menuGUI)
                element.Update();
        }

        private void MainMenuDraw(GameTime gameTime)
        {
            // Draw BG
            //--------------------------------------------------------------------------------------
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            _spriteBatch.Begin();
            _spriteBatch.Draw(stars, new Rectangle(0, 0, 1280, 720), Color.White);
            _spriteBatch.End();
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            //--------------------------------------------------------------------------------------

            // Draw UI
            //-------------------------------------------------------------------------------------------------------------------
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            _spriteBatch.Begin();
            Vector2 center = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            Vector2 title = lucidaConsole.MeasureString("Protect the Ship!");
            _spriteBatch.DrawString(lucidaConsole, "Protect the Ship!", new Vector2(center.X - title.X / 2, center.Y - title.Y / 2 - 50), Color.White);
            foreach (GUIElement element in menuGUI)
                element.Draw(_spriteBatch, lucidaConsole);
            _spriteBatch.End();
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            //-------------------------------------------------------------------------------------------------------------------
        }

        private void PlayUpdate(GameTime gameTime)
        {
            if (currentGameState == GameConstant.GameStates.Play)
            {
                if (InputManager.IsKeyDown(Keys.Space)) PlayerShooting(); // Player Shooting Controls

                // Update Projectiles
                //------------------------------------------
                for (int i = 0; i < projectiles.Length; i++)
                {
                    if (projectiles[i] != null)
                    {
                        if (projectiles[i].isActive)
                        {
                            projectiles[i].Rigidbody.ElapsedGameTime = Time.ElapsedGameTime;
                            projectiles[i].Update();

                            Vector3 normal;
                            for (int j = 0; j < enemies.Length; j++)
                            {
                                if (enemies[j] != null)
                                {
                                    if (projectiles[i].Collider.Collides(enemies[j].Collider, out normal) && projectiles[i].Friendly)
                                    {
                                        // blows up
                                        PlaySound(expSound, false, 0.25f, 0.2f);

                                        enemies[j].isActive = false;
                                        projectiles[i].isActive = false;
                                        

                                        if (random.Next(4) > 2)
                                        {
                                            if (random.Next(2) == 0)
                                            {
                                                powerups[j] = new Powerup(GameConstant.Weapons.machine_gun, Content, camera, GraphicsDevice, light);

                                            }

                                            else
                                            {
                                                powerups[j] = new Powerup(GameConstant.Weapons.spread_gun, Content, camera, GraphicsDevice, light);
                                            }

                                            powerups[j].Transform.LocalPosition = enemies[j].Transform.LocalPosition;
                                        }

                                        enemies[j] = null;

                                        score += GameConstant.KillBonus;
                                        break; //no need to check other bullets
                                    }
                                }
                            }

                        }

                        else
                        {
                            projectiles[i] = null;
                        }
                    }
                }
                //------------------------------------------

                // Update Enemies
                //--------------------------------------
                for (int i = 0; i < enemies.Length; i++)
                {
                    if (enemies[i] != null)
                    {
                        if (enemies[i].isActive)
                        {
                            enemies[i].Rigidbody.ElapsedGameTime = Time.ElapsedGameTime;
                            enemies[i].Update();

                            Vector3 normal;
                            if (enemies[i].Collider.Collides(player.Collider, out normal))
                            {
                                // blows up
                                PlaySound(expSound, false, 0.25f, 0.2f);
                                enemies[i].isActive = false;
                                player.Health -= GameConstant.EnemyDamage;

                                playerHealthBar.Decrement(GameConstant.EnemyDamage);
                                playerHealthBar.Update();

                                score -= GameConstant.DamagePenalty;
                            }

                            else if (enemies[i].Collider.Collides(mother.Collider, out normal))
                            {
                                // blows up
                                PlaySound(expSound, false, 0.25f, 0.2f);
                                enemies[i].isActive = false;
                                mother.Health -= GameConstant.EnemyDamage;

                                if (mother.Health <= 0) PlaySound(bigExpSound, false, 0.25f, 1f);

                                motherHealthBar.Decrement(GameConstant.EnemyDamage);
                                motherHealthBar.Update();

                                score -= GameConstant.DamagePenalty;
                            }
                        }

                        else
                        {
                            enemies[i] = null;
                        }
                    }
                }
                //--------------------------------------

                // Spawn Enemies
                //-----------------------
                if (enemySpawnTimer <= 0)
                {
                    CreateEnemy();
                }
                //-----------------------

                // Update Powerups
                //------------------------------------------
                for (int i = 0; i < enemies.Length; i++)
                {
                    if (powerups[i] != null)
                    {
                        if (powerups[i].isActive)
                        {
                            powerups[i].Rigidbody.ElapsedGameTime = Time.ElapsedGameTime;
                            powerups[i].Update();

                            Vector3 normal;
                            if (powerups[i].Collider.Collides(player.Collider, out normal))
                            {
                                // blows up
                                //PlaySound(expSound, false, 0.25f, 0.2f);
                                powerups[i].isActive = false;
                                player.CurrentWeapon = powerups[i].weapon;
                            }
                        }

                        else
                        {
                            powerups[i] = null;
                        }
                    }
                }
                //------------------------------------------

                player.Update();
                mother.Update();

                enemySpawnTimer -= Time.ElapsedGameTime;

                if (mother.Health <= 0) currentGameState = GameConstant.GameStates.Lose;
            }

            else
            {
                foreach (GUIElement element in playGUI)
                    element.Update();
            }
        }

        private void PlayDraw(GameTime gameTime)
        {
            if (currentGameState == GameConstant.GameStates.Play)
            {
                Timer += Time.ElapsedGameTime;
                
                // Draw BG
                //--------------------------------------------------------------------------------------
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                _spriteBatch.Begin();
                _spriteBatch.Draw(stars, new Rectangle(0, 0, 1280, 720), Color.White);
                _spriteBatch.End();
                GraphicsDevice.RasterizerState = RasterizerState.CullNone;
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                //--------------------------------------------------------------------------------------
                
                mother.Draw();
                player.Draw();

                for (int i = 0; i < projectiles.Length; i++)
                {
                    if (projectiles[i] != null)
                    {
                        projectiles[i].Draw();
                    }
                }

                for (int i = 0; i < enemies.Length; i++)
                {
                    if (enemies[i] != null)
                    {
                        enemies[i].Draw();
                    }
                }

                for (int i = 0; i < powerups.Length; i++)
                {
                    if (powerups[i] != null)
                    {
                        powerups[i].Draw();
                    }
                }
                
                // Draw UI
                //-------------------------------------------------------------------------------------------------------------------
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                _spriteBatch.Begin();
                _spriteBatch.DrawString(lucidaConsole, "Time: " + (int)Timer, timerPosition, Color.White);
                _spriteBatch.DrawString(lucidaConsole, "Score: " + score, scorePosition, Color.White);

                Vector2 center = lucidaConsole.MeasureString("" + (int)mother.respawnTimer);
                if (player.Health <= 0) _spriteBatch.DrawString(lucidaConsole, "" + (int)mother.respawnTimer,
                    new Vector2(GraphicsDevice.Viewport.Width / 2 - center.X / 2, GraphicsDevice.Viewport.Height / 2 - center.Y / 2),
                    Color.Black);

                switch (player.CurrentWeapon)
                {
                    case GameConstant.Weapons.Default:
                        _spriteBatch.Draw(D_frame, new Rectangle(350, 0, 152, 152), Color.White);
                        break;
                    case GameConstant.Weapons.machine_gun:
                        _spriteBatch.Draw(M_frame, new Rectangle(350, 0, 152, 152), Color.White);
                        break;
                    case GameConstant.Weapons.spread_gun:
                        _spriteBatch.Draw(S_frame, new Rectangle(350, 0, 152, 152), Color.White);
                        break;
                }

                playerHealthBar.Draw(_spriteBatch);
                motherHealthBar.Draw(_spriteBatch);
                _spriteBatch.DrawString(lucidaConsole, "Player Health: ", new Vector2(150, 50), Color.White);
                _spriteBatch.DrawString(lucidaConsole, "Hangar Health: ", new Vector2(150, 80), Color.White);

                _spriteBatch.End();
                GraphicsDevice.RasterizerState = RasterizerState.CullNone;
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                //-------------------------------------------------------------------------------------------------------------------
            }

            else
            {
                // Draw BG
                //--------------------------------------------------------------------------------------
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                _spriteBatch.Begin();
                _spriteBatch.Draw(stars, new Rectangle(0, 0, 1280, 720), Color.White);
                _spriteBatch.End();
                GraphicsDevice.RasterizerState = RasterizerState.CullNone;
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                //--------------------------------------------------------------------------------------

                // Draw UI
                //-------------------------------------------------------------------------------------------------------------------
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                _spriteBatch.Begin();

                Vector2 center = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
                Vector2 gameOver = lucidaConsole.MeasureString("Game Over");
                Vector2 time = lucidaConsole.MeasureString("Time: " + (int) Timer);
                Vector2 points = lucidaConsole.MeasureString("Score: " + score);

                _spriteBatch.DrawString(lucidaConsole, "Game Over",
                    new Vector2(center.X - gameOver.X / 2, center.Y - gameOver.Y / 2 - time.Y), Color.White);
                _spriteBatch.DrawString(lucidaConsole, "Time: " + (int)Timer,
                    new Vector2(center.X - time.X/2, center.Y - time.Y/2), Color.White);
                _spriteBatch.DrawString(lucidaConsole, "Score: " + score,
                    new Vector2(center.X - points.X / 2, center.Y - points.Y / 2 + time.Y), Color.White);

                foreach (GUIElement element in playGUI)
                    element.Draw(_spriteBatch, lucidaConsole);

                _spriteBatch.End();
                GraphicsDevice.RasterizerState = RasterizerState.CullNone;
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                //-------------------------------------------------------------------------------------------------------------------
            }
        }

        void ControlsUpdate(GameTime gameTime)
        {
            foreach (GUIElement element in controlsGUI)
                element.Update();
        }

        void ControlsDraw(GameTime gameTime)
        {
            // Draw BG/UI
            //-------------------------------------------------------------------------------------------------------------------
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            _spriteBatch.Begin();
            _spriteBatch.Draw(stars, new Rectangle(0, 0, 1280, 720), Color.White);

            Vector2 center = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            Vector2 controls = lucidaConsole.MeasureString("Controls:");
            Vector2 movement = lucidaConsole.MeasureString("WASD: Move/Rotate Ship");
            Vector2 shoot = lucidaConsole.MeasureString("Space: Shoot");
            Vector2 escape = lucidaConsole.MeasureString("Escape: Exit Game");
            Vector2 defGun = lucidaConsole.MeasureString("Shoot Bullets by Pressing Space!");
            Vector2 macGun = lucidaConsole.MeasureString("Shoot Bullets by Holding Space!");
            Vector2 sprGun = lucidaConsole.MeasureString("Shoot 3 Bullets by Pressing Space!");

            _spriteBatch.DrawString(lucidaConsole, "Controls:", new Vector2(center.X - controls.X/2, 150), Color.White);
            _spriteBatch.DrawString(lucidaConsole, "WASD: Move/Rotate Ship", new Vector2(center.X - movement.X / 2, 170), Color.White);
            _spriteBatch.DrawString(lucidaConsole, "Space: Shoot", new Vector2(center.X - shoot.X / 2, 190), Color.White);
            _spriteBatch.DrawString(lucidaConsole, "Escape: Exit Game", new Vector2(center.X - escape.X / 2, 210), Color.White);

            _spriteBatch.Draw(D_frame, new Rectangle((int)(center.X) - 76 - 300, (int)(center.Y) - 76, 152, 152), Color.White);
            _spriteBatch.Draw(M_frame, new Rectangle((int)(center.X) - 76, (int)(center.Y) - 76, 152, 152), Color.White);
            _spriteBatch.Draw(S_frame, new Rectangle((int)(center.X) - 76 + 300, (int)(center.Y) - 76, 152, 152), Color.White);

            _spriteBatch.DrawString(lucidaConsole, "Shoot Bullets by Pressing Space!"  , new Vector2((int)(center.X) - 300 - defGun.X / 2, (int)(center.Y) + 86), Color.White);
            _spriteBatch.DrawString(lucidaConsole, "Shoot Bullets by Holding Space!"   , new Vector2((int)(center.X) - macGun.X / 2,       (int)(center.Y) + 86), Color.White);
            _spriteBatch.DrawString(lucidaConsole, "Shoot 3 Bullets by Pressing Space!", new Vector2((int)(center.X) +300 - sprGun.X / 2, (int)(center.Y) + 86), Color.White);

            foreach (GUIElement element in controlsGUI)
                element.Draw(_spriteBatch, lucidaConsole);
            _spriteBatch.End();
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            //-------------------------------------------------------------------------------------------------------------------
        }
        //--------------------------------------------

        // GUI Elements
        //-------------------------------
        void PlayGame(GUIElement element)
        {
            PlaySound(confirmSound, false, 0.25f, 1f);
            currentScene = scenes["Play"];
        }

        void Controls(GUIElement element)
        {
            PlaySound(confirmSound, false, 0.25f, 1f);
            currentScene = scenes["Controls"];
        }

        void ExitGame(GUIElement element)
        {
            PlaySound(confirmSound, false, 0.25f, 1f);
            Exit();
        }

        void Back(GUIElement element)
        {
            PlaySound(confirmSound, false, 0.25f, 1f);
            currentScene = scenes["Menu"];
        }
        //-------------------------------

    }
}
