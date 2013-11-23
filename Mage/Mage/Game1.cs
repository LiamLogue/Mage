using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Mage
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Variables/members
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        Random randNum = new Random();

        Vector2 zombiePos;
        Vector2 mouseToTarget;

        KeyboardState keyboardState;
        KeyboardState lastKeyboardState;
        Texture2D cursorTexture;
        Texture2D mapTexture;
        Texture2D humpTexture;
        float timer;
        float attackTimer;
        float healthTimer;
        float earthgrabLifespan;
        float spikesLifespan;
        float spikesTimer;

        bool isPlaying = false;

        int zombieCount = 0;

        #region Sound effects / songs
        SoundEffect playerHitEffect;
        SoundEffect hitEffect;
        SoundEffect fireballShoot;
        SoundEffect frostballShoot;
        SoundEffect electroballShoot;

        Song winningSong;
        Song deathSong;
        Song themeSong;
        #endregion
        
        Mage player;
        
        List<Fireball> fireballs = new List<Fireball>();
        List<Frostball> frostballs = new List<Frostball>();
        List<Electroball> electroballs = new List<Electroball>();

        Fireball newFireball;
        Frostball newFrostball;
        Electroball newElectroball;
        HealthTotem newHealthTotem;
        Texture2D healthTotemPixel; 
        Earthgrab newEarthgrab;
        Texture2D earthgrabPixel;
        Spikes newSpikes;
        Texture2D spikesPixel;

        Texture2D testBorder;

        List<Zombie> zombies = new List<Zombie>();
        AttackBar attackBar;

        #region GameState Enum
        enum GameState
        {
            SplashScreen,
            Controls,
            GetReady,
            Pause,
            Playing,
            Dead,
            Win,
        }

        GameState currentGameState = GameState.SplashScreen;
        #endregion

        #region Level Enum
        enum LevelState
        {
            One,
            Two,
            Three,
        }

        LevelState currentLevelState = LevelState.One;
        #endregion

        #endregion

        #region Constructor
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 800;
            graphics.IsFullScreen = false;
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            this.IsMouseVisible = false;
            this.Window.Title = "Mage";

            base.Initialize();
        }
        #endregion

        #region LoadContent
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Setup the pixel details for our border
            healthTotemPixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            healthTotemPixel.SetData(new[] { Color.White });
            earthgrabPixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            earthgrabPixel.SetData(new[] { Color.White });
            spikesPixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            spikesPixel.SetData(new[] { Color.White });
            testBorder = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            testBorder.SetData(new[] { Color.White });

            //Sound effect
            playerHitEffect = Content.Load<SoundEffect>(@"Sounds\Player\PlayerHit");
            hitEffect = Content.Load<SoundEffect>(@"Sounds\Enemy\EnemyHit");
            fireballShoot = Content.Load<SoundEffect>(@"Sounds\Player\FireballShoot");
            frostballShoot = Content.Load<SoundEffect>(@"Sounds\Player\FrostballShoot");
            electroballShoot = Content.Load<SoundEffect>(@"Sounds\Player\ElectroballShoot");

            //Songs
            deathSong = Content.Load<Song>(@"Sounds\Player\DeathSound");
            winningSong = Content.Load<Song>(@"Sounds\Player\Winning");
            themeSong = Content.Load<Song>(@"Sounds\Theme\Theme");
            MediaPlayer.Play(themeSong);
            MediaPlayer.IsRepeating = true;

            // TODO: use this.Content to load your game content here
            spriteFont = Content.Load<SpriteFont>(@"Fonts\font");
            attackBar = new AttackBar(Content.Load<Texture2D>(@"Images\HUD\AttackHUD"));
            player = new Mage(Content.Load<Texture2D>(@"Images\Player\Mage"), Vector2.Zero, new Point(26, 35), 10, new Point(0, 0), new Point(2, 6), 1.0f, 200, 100);
            cursorTexture = Content.Load<Texture2D>(@"Images\HUD\Cursor");
            mapTexture = Content.Load<Texture2D>(@"Images\Map\Map");
            humpTexture = Content.Load<Texture2D>(@"Images\Map\Humps");
            newHealthTotem = new HealthTotem(Content.Load<Texture2D>(@"Images\Player\Attacks\HealthTotem"));
            newHealthTotem.position = new Vector2(player.position.X + player.frameSize.X / 2, player.position.Y + player.frameSize.Y / 2);
            newEarthgrab = new Earthgrab(Content.Load<Texture2D>(@"Images\Player\Attacks\Earthgrab"));
            newEarthgrab.position = new Vector2(player.position.X + player.frameSize.X / 2, player.position.Y + player.frameSize.Y / 2);
            newSpikes = new Spikes(Content.Load<Texture2D>(@"Images\Player\Attacks\Spikes"));
            newSpikes.position = new Vector2(player.position.X + player.frameSize.X / 2, player.position.Y + player.frameSize.Y / 2);

            //Load our zombies
            LoadNewZombies(1, 10);
        }
        #endregion

        #region UnloadContent
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {

        }

        #endregion

        #region Update
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //Check our keyboard and mouse state          
            keyboardState = Keyboard.GetState();

            //Setup our cursor target
            mouseToTarget = new Vector2((float)Mouse.GetState().X, (float)Mouse.GetState().Y);

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            #region Gamestate specific updating
            //Which gamestate are we in?
            switch (currentGameState)
            {
                case GameState.SplashScreen:
                    #region SplashScreen updates
                    if (keyboardState.IsKeyDown(Keys.Enter) && lastKeyboardState.IsKeyUp(Keys.Enter))
                    {
                        //Stop the song and play the winners theme!
                        MediaPlayer.Stop();
                        MediaPlayer.Play(themeSong);

                        //Change to the next screen
                        currentGameState = GameState.Controls;
                    }
                    #endregion
                break;

                case GameState.Controls:
                    #region Controls screen updates
                    if (keyboardState.IsKeyDown(Keys.Enter) && lastKeyboardState.IsKeyUp(Keys.Enter))
                    {
                        //Change to the next screen
                        currentGameState = GameState.Playing;
                    }
                    #endregion
                break;

                case GameState.GetReady:
                    #region GetReady screen updates
                    if (keyboardState.IsKeyDown(Keys.Enter) && lastKeyboardState.IsKeyUp(Keys.Enter))
                    {
                        //Change to the next screen
                        currentGameState = GameState.Playing;
                    }
                    #endregion
                break;

                case GameState.Playing:
                    #region All in-game updates
                   //Wait.. are we alive?!
                    if (player.mageHealth <= 0)
                    {
                        //Add to a timer
                        timer += gameTime.ElapsedGameTime.Milliseconds;

                        //Play the death sound
                        MediaPlayer.Play(deathSong);                   

                        //Time to exit the game?
                        if (timer >= 500)
                        {
                            //Nope.. we're not
                            currentGameState = GameState.Dead;
                        }
                    }

                    //Any zombies left?
                    if (zombies.Count <= 0)
                    {
                        switch (currentLevelState)
                        {
                            case LevelState.One:
                                LoadNewZombies(2, 20);
                                player.mageHealth = 80;
                                currentLevelState = LevelState.Two;
                                currentGameState = GameState.GetReady;
                                break;

                            case LevelState.Two:
                                LoadNewZombies(3, 30);
                                player.mageHealth = 60;
                                currentLevelState = LevelState.Three;
                                currentGameState = GameState.GetReady;
                                break;

                            case LevelState.Three:
                                LoadNewZombies(1, 10);
                                player.mageHealth = 100;
                                currentLevelState = LevelState.One;
                                currentGameState = GameState.Win;
                                break;
                        }
                    }

                    //Check for pause
                    if (keyboardState.IsKeyDown(Keys.Escape) && lastKeyboardState.IsKeyUp(Keys.Escape))
                    {
                        //Pause the game!
                        currentGameState = GameState.Pause;
                    }

                    //Check our mouse state
                    MouseState mouseState = Mouse.GetState();

                    //Setup a mouse rectangle for intersecting checks
                    Rectangle mouseRect = new Rectangle(mouseState.X + 2, mouseState.Y + 2, 0, 0);

                    //Update our player
                    player.Update(gameTime, Window.ClientBounds);

                    //Hovering over our mage?
                    if (mouseRect.Intersects(player.rectangle))
                    {
                        player.showHealth = true;
                    }

                    #region Zombie collision (w/ player) / hover / targeted detection and attacking
                    //Loop our zombies
                    foreach (Zombie z in zombies)
                    {
                        //Random zombie direction
                        z.randomDirection = randNum.Next(0, 4);

                        //Mouse hovering over him?
                        if (mouseRect.Intersects(z.rectangle))
                        {
                            //Show the zombies name
                            z.showNametag = true;

                            //Clicked on the zombie?
                            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                            {
                                //Set our zombie to 'isClicked' to enable the red outline
                                z.isClicked = true;
                                z.showNametag = true;

                                //Loop the zombies again
                                foreach (Zombie z2 in zombies)
                                {
                                    //Are we on the same zombie?
                                    if (z.zombieName != z2.zombieName)
                                    {
                                        //Sets every zombie (apart from the selected zombie) to not clicked
                                        z2.isClicked = false;
                                        z2.showNametag = false;
                                    }
                                }
                            }
                        }
                        else if (z.rectangle.Intersects(player.rectangle))
                        {
                            //Random damage 
                            int randomDamage = randNum.Next(3, 10);

                            //Increment our timer
                            timer += gameTime.ElapsedGameTime.Milliseconds;

                            //Cooldown finish?
                            if (timer >= 1000)
                            {
                                //Play sound
                                playerHitEffect.Play();

                                //Decrease the Mage's health by the random damage and reset out timer
                                player.mageHealth -= randomDamage;
                                player.lastDamage = randomDamage;
                                timer = 0.0f;
                            }

                            //Player have a min/max of 0/100 respectively
                            player.mageHealth = (int)MathHelper.Clamp(player.mageHealth, 0, 100);
                        }
                        else
                        {
                            //Clicked on blank part of the screen?
                            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                            {
                                //No zombies should be set to isClicked
                                z.isClicked = false;
                            }
                            //Nope, not hovering so there's no point in showing the nametag!
                            z.showNametag = false;
                        }
                    }
                    #endregion

                    //Add to the attack timer
                    attackTimer += gameTime.ElapsedGameTime.Milliseconds;

                    #region Attack button checks
                    //Check for which attack to use
                    #region Fireball
                    if (keyboardState.IsKeyDown(Keys.D1) && lastKeyboardState.IsKeyUp(Keys.D1))
                    {
                        //Timer cooldown at 0.8 of a second?
                        if (attackTimer >= 800)
                        {
                            //Play sound effect
                            fireballShoot.Play(0.2f, 0.0f, 0.0f);

                            //Shoot fireball and reset timer
                            Shoot("fireball");
                            attackTimer = 0.0f;
                        }
                    }
                    #endregion

                    #region Frostball
                    if (keyboardState.IsKeyDown(Keys.D2) && lastKeyboardState.IsKeyUp(Keys.D2))
                    {
                        //Timer cooldown at 0.5 of a second?
                        if (attackTimer >= 500)
                        {
                            //Play sound effect
                            frostballShoot.Play(0.2f, 0.0f, 0.0f);

                            //Shoot frostball and reset timer
                            Shoot("frostball");
                            attackTimer = 0.0f;
                        }
                    }
                    #endregion

                    #region Electroball
                    if (keyboardState.IsKeyDown(Keys.D3) && lastKeyboardState.IsKeyUp(Keys.D3))
                    {

                        //Timer cooldown at 0.3 of a second?
                        if (attackTimer >= 300)
                        {
                            //Play sound effect
                            electroballShoot.Play(0.2f, 0.0f, 0.0f);

                            //Shoot electroball and reset timer
                            Shoot("electroball");
                            attackTimer = 0.0f;
                        }
                    }
                    #endregion

                    #region Spikes
                    if (keyboardState.IsKeyDown(Keys.D4) && lastKeyboardState.IsKeyUp(Keys.D4))
                    {
                        //Have we already got spikes planted?
                        if (!newSpikes.isPlanted)
                        {
                            //Nope, we don't. Set the positionToSet and "shoot" it.
                            newSpikes.positionToSet = player.position;
                            Shoot("spikes");
                        }
                    }
                    #endregion

                    #region Earthgrab
                    if (keyboardState.IsKeyDown(Keys.D5) && lastKeyboardState.IsKeyUp(Keys.D5))
                    {
                        //Have we already got an earthgrab down?
                        if (!newEarthgrab.isPlanted)
                        {
                            //Nope, we don't. Set the positionToSet of the earthgrab and shoot it
                            newEarthgrab.positionToSet = player.position;
                            Shoot("earthgrab");
                        }
                    }
                    #endregion

                    #region Health Totem
                    if (keyboardState.IsKeyDown(Keys.D6) && lastKeyboardState.IsKeyUp(Keys.D6))
                    {
                        //Have we already got a totem?
                        if (!newHealthTotem.isPlanted)
                        {
                            //Nope, we don't. Set the positionToSet the totem and "shoot" it.
                            newHealthTotem.positionToSet = player.position;
                            Shoot("healthtotem");
                        }
                    }
                    #endregion
                    #endregion

                    #region Attack collision detection
                    #region Fireballs
                    //Loop the fireballs
                    foreach (Fireball fireball in fireballs)
                    {
                        //Update the fireball
                        fireball.Update();

                        //And also the zombies
                        foreach (Zombie z in zombies)
                        {
                            //Is this our target?
                            if (z.isClicked)
                            {
                                //Set the fireball's target
                                zombiePos = z.position;
                                fireball.hasTarget = true;
                            }

                            //Collision?
                            if (fireball.rectangle.Intersects(z.rectangle))
                            {
                                //Hurt the zombie and make the fireball not active
                                fireball.isVisible = false;
                                z.TakeDamage(fireball.damage);

                                //Play sound effect
                                hitEffect.Play();
                            }
                        }
                    }
                    #endregion

                    #region Frostballs
                    //Loop the frostspears
                    foreach (Frostball frostball in frostballs)
                    {
                        //Update the fireball
                        frostball.Update();

                        //And also the zombies
                        foreach (Zombie z in zombies)
                        {
                            //Is this our target?
                            if (z.isClicked)
                            {
                                //Set the fireball's target
                                zombiePos = z.position;
                                frostball.hasTarget = true;
                            }

                            //Collision?
                            if (frostball.rectangle.Intersects(z.rectangle))
                            {
                                //Hurt the zombie and make the fireball not active
                                frostball.isVisible = false;
                                z.TakeDamage(frostball.damage);

                                //Play sound effect
                                hitEffect.Play();
                            }
                        }
                    }
                    #endregion

                    #region Electroball
                    //Loop the Electroballs
                    foreach (Electroball electroball in electroballs)
                    {
                        //Update the fireball
                        electroball.Update();

                        //And also the zombies
                        foreach (Zombie z in zombies)
                        {
                            //Is this our target?
                            if (z.isClicked)
                            {
                                //Set the fireball's target
                                zombiePos = z.position;
                                electroball.hasTarget = true;
                            }

                            //Collision?
                            if (electroball.rectangle.Intersects(z.rectangle))
                            {
                                //Hurt the zombie and make the fireball not active
                                electroball.isVisible = false;
                                z.TakeDamage(electroball.damage);

                                //Play sound effect
                                hitEffect.Play();
                            }
                        }
                    }
                    #endregion

                    #region Spikes
                    //Have we planted our health totem?
                    if (newSpikes.isPlanted)
                    {
                        //Add to the timer
                        spikesLifespan += gameTime.ElapsedGameTime.Milliseconds;

                        //Spikes up long enough?
                        if (spikesLifespan >= 10000)
                        {
                            newSpikes.isPlanted = false;
                            spikesLifespan = 0.0f;
                        }

                        //Update totem
                        newSpikes.Update();

                        //Check for intersection
                        foreach (Zombie z in zombies)
                        {
                            //Zombie inside our border?
                            if (z.rectangle.Intersects(newSpikes.borderRectangle))
                            {
                                //Stop chasing
                                z.StopChasing();

                                //Add to the timer
                                spikesTimer += gameTime.ElapsedGameTime.Milliseconds;

                                //Cooldown enough to take damage?
                                if (spikesTimer >= 1000)
                                {
                                    //New random damage
                                    int randomDamage = randNum.Next(11);

                                    //Zombie take that damage!
                                    z.TakeDamage(randomDamage);

                                    //Reset timer
                                    spikesTimer = 0.0f;

                                    //Start chasing again
                                    z.CheckForChase(player.position, gameTime, Window.ClientBounds);
                                }
                            }
                        }
                    }
                    #endregion

                    #region Earthgrab
                    //Have we planted our health totem?
                    if (newEarthgrab.isPlanted)
                    {
                        //Add to the timer
                        earthgrabLifespan += gameTime.ElapsedGameTime.Milliseconds;

                        //Earthgrab up long enough?
                        if (earthgrabLifespan >= 15000)
                        {
                            newEarthgrab.isPlanted = false;
                            earthgrabLifespan = 0.0f;
                        }

                        //Update totem
                        newEarthgrab.Update();

                        //Check for intersection
                        foreach (Zombie z in zombies)
                        {
                            //Zombie inside our border?
                            if (z.rectangle.Intersects(newEarthgrab.borderRectangle))
                            {
                                //Stop chasing
                                z.StopChasing();
                            }
                        }
                    }
                    #endregion

                    #region Health Totem
                    //Have we planted our health totem?
                    if (newHealthTotem.isPlanted == true)
                    {
                        //Update totem
                        newHealthTotem.Update();

                        //Check for intersection
                        if (player.rectangle.Intersects(newHealthTotem.borderRectangle))
                        {
                            //Add to timer
                            healthTimer += gameTime.ElapsedGameTime.Milliseconds;

                            //We only want to add 10 health every second, not millisecond
                            if (healthTimer >= 1000)
                            {
                                //Add random health and reset timer
                                int randomHealth = randNum.Next(5, 10);
                                player.mageHealth += randomHealth;
                                player.lastHealth = randomHealth;
                                healthTimer = 0.0f;

                                //Player have a min/max of 0/100 respectively
                                player.mageHealth = (int)MathHelper.Clamp(player.mageHealth, 0, 100);
                            }
                        }
                    }
                    #endregion
                    #endregion

                    //Update all of the attacks
                    UpdateAttacks();

                    #region Zombie updating
                    //Loop the zombies
                    foreach (Zombie z in zombies)
                    {
                        //Is the zombie dead?!
                        if (z.zombieHealth <= 0)
                        {
                            //Unlucky fella
                            z.isAlive = false;
                        }
                        else
                        {
                            //He's not dead (technically zombies are though!) so update him
                            z.Update(gameTime, Window.ClientBounds, player.position);
                        }

                        //Loop the zombies
                        foreach (Zombie z2 in zombies)
                        {
                            //Are we comparing the same zombie? We shouldn't be!
                            if (z.zombieName != z2.zombieName)
                            {
                                //Hey you, are you in my rectangle!?
                                if (z.rectangle.Intersects(z2.rectangle))
                                {
                                    //Do stuff..
                                }
                            }
                        }
                    }
                    #endregion

                    #region Checking for dead zombies and removing them
                    //Loop the zombies
                    for (int i = 0; i < zombies.Count; i++)
                    {
                        //Are you here?!
                        if (!zombies[i].isAlive)
                        {
                            //No. I guess not. Remove it.
                            zombies.RemoveAt(i);
                            i--;
                        }
                    }
                    #endregion
                    #endregion
                break;

                case GameState.Pause:
                    #region Pause screen updates
                    if (keyboardState.IsKeyDown(Keys.Enter) && lastKeyboardState.IsKeyUp(Keys.Enter))
                    {
                        //Play on
                        currentGameState = GameState.Playing;
                    }
                    if (keyboardState.IsKeyDown(Keys.Escape) && lastKeyboardState.IsKeyUp(Keys.Escape))
                    {
                        //Unload content and have the garbage collector grab it.
                        UnloadContent();
                        GC.Collect();

                        //Reload our content
                        LoadContent();

                        //Go to the splash screen
                        currentGameState = GameState.SplashScreen;
                    }
                    #endregion
                break;

                case GameState.Dead:
                    #region Dead screen updates
                    if (keyboardState.IsKeyDown(Keys.Enter) && lastKeyboardState.IsKeyUp(Keys.Enter))
                    {
                        //Unload content and have the garbage collector grab it.
                        UnloadContent();
                        GC.Collect();

                        //Reload our content
                        LoadContent();

                        //Go to the splash screen
                        currentGameState = GameState.SplashScreen;
                    }
                    #endregion
                break;

                case GameState.Win:
                    #region Win screen updates
                    //Stop the song and play the winners theme!
                    if (!isPlaying)
                    {
                        MediaPlayer.Play(winningSong);
                        isPlaying = true;
                    }

                    if (keyboardState.IsKeyDown(Keys.Enter) && lastKeyboardState.IsKeyUp(Keys.Enter))
                    {                 
                        //Go to the splash screen
                        currentGameState = GameState.SplashScreen;
                    }
                    #endregion
                break;
            }
            #endregion

            //Update last keyboard state
            lastKeyboardState = Keyboard.GetState();

            base.Update(gameTime);
        }
        #endregion

        #region Draw
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //Begin spritebatch
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            //Which game state are we in?
            #region Gamestate specific drawing
            switch (currentGameState)
            {
                case GameState.SplashScreen:
                    #region Splashscreen drawing
                    spriteBatch.Draw(Content.Load<Texture2D>(@"Images\SplashScreen\SplashScreen"), new Rectangle(0, 0, 800, 600), Color.White);
                    #endregion
                break;

                case GameState.Controls:
                    #region Controls screen drawing
                    spriteBatch.Draw(Content.Load<Texture2D>(@"Images\ControlsScreen\ControlsScreen"), new Rectangle(0, 0, 800, 600), Color.White);
                    #endregion
                break;

                case GameState.GetReady:
                    #region GetReady screen drawing
                        spriteBatch.Draw(Content.Load<Texture2D>(@"Images\GetReady\GetReady"), new Rectangle(0, 0, 800, 600), Color.White);
                    #endregion
                break;

                case GameState.Playing:
                    #region All in-game drawing
                    //Draw the map
                    spriteBatch.Draw(mapTexture, new Vector2(0, 0), Color.White);

                    //Should we draw the health totem?
                    if (newHealthTotem.isPlanted)
                    {
                        //Draw totem and the border
                        newHealthTotem.Draw(spriteBatch, newHealthTotem.positionToSet);
                        newHealthTotem.DrawBorder(spriteBatch, healthTotemPixel, newHealthTotem.borderRectangle, 1, Color.LightGreen);
                    }

                    //Should we draw the earthgrab?
                    if (newEarthgrab.isPlanted)
                    {
                        //Draw the earthgrab and the border
                        newEarthgrab.Draw(spriteBatch, newEarthgrab.positionToSet);
                        newEarthgrab.DrawBorder(spriteBatch, earthgrabPixel, newEarthgrab.borderRectangle, 1, new Color(171, 0, 219));
                    }

                    //Spikes! Spikes everywhere!
                    if (newSpikes.isPlanted)
                    {
                        newSpikes.Draw(spriteBatch, newSpikes.positionToSet);
                        newSpikes.DrawBorder(spriteBatch, spikesPixel, newSpikes.borderRectangle, 1, new Color(209, 76, 0));
                    }

                    //Draw our zombie(s)
                    foreach (Zombie z in zombies)
                    {
                        z.Draw(gameTime, spriteBatch, spriteFont, player.position);
                    }

                    //Draw our player
                    player.Draw(gameTime, spriteBatch, spriteFont);

                    //Check level and draw info
                    switch (currentLevelState)
                    {
                        case LevelState.One:
                            spriteBatch.DrawString(spriteFont, "Level 1 - Zombie Count " + zombies.Count + "/" + zombieCount + " | Active Effects - None", new Vector2(0, 1), Color.Black);
                            spriteBatch.DrawString(spriteFont, "Level 1 - Zombie Count " + zombies.Count + "/" + zombieCount + " | Active Effects - None", new Vector2(0, 0), Color.White);
                            break;

                        case LevelState.Two:
                            spriteBatch.DrawString(spriteFont, "Level 2 - Zombie Count " + zombies.Count + "/" + zombieCount + " | Active Effects - Zombie health +20 / Mage Health -20", new Vector2(0, 1), Color.Black);
                            spriteBatch.DrawString(spriteFont, "Level 2 - Zombie Count " + zombies.Count + "/" + zombieCount + " | Active Effects - Zombie health +20 / Mage Health -20", new Vector2(0, 0), Color.White);
                            break;

                        case LevelState.Three:
                            spriteBatch.DrawString(spriteFont, "Level 3 - Zombie Count " + zombies.Count + "/" + zombieCount + " | Active Effects - Zombie health +40 / Mage Health -40", new Vector2(0, 1), Color.Black);
                            spriteBatch.DrawString(spriteFont, "Level 3 - Zombie Count " + zombies.Count + "/" + zombieCount + " | Active Effects - Zombie health +40 / Mage Health -40", new Vector2(0, 0), Color.White);
                            break;
                    }

                    //Loop zombies
                    foreach (Zombie z in zombies)
                    {
                        //Are you MY zombie?
                        if (z.isClicked)
                        {
                            //Draw some zombie info
                            spriteBatch.DrawString(spriteFont, "Zombie Name - " + z.zombieName, new Vector2(0, 21), Color.Black);
                            spriteBatch.DrawString(spriteFont, "Zombie Name - " + z.zombieName, new Vector2(0, 20), Color.White);
                            spriteBatch.DrawString(spriteFont, "Zombie Position - " + z.position.ToString(), new Vector2(0, 31), Color.Black);
                            spriteBatch.DrawString(spriteFont, "Zombie Position - " + z.position.ToString(), new Vector2(0, 30), Color.White);
                            spriteBatch.DrawString(spriteFont, "Zombie Health - " + z.zombieHealth.ToString(), new Vector2(0, 41), Color.Black);
                            spriteBatch.DrawString(spriteFont, "Zombie Health - " + z.zombieHealth.ToString(), new Vector2(0, 40), Color.White);
                        }
                    }

                    //Draw mouse target info
                    spriteBatch.DrawString(spriteFont, "Current Mouse Target - " + new Vector2((float)Mouse.GetState().X, (float)Mouse.GetState().Y).ToString(), new Vector2(580, 1), Color.Black);
                    spriteBatch.DrawString(spriteFont, "Current Mouse Target - " + new Vector2((float)Mouse.GetState().X, (float)Mouse.GetState().Y).ToString(), new Vector2(580, 0), Color.White);

                    //Draw terrain ibjects
                    spriteBatch.Draw(humpTexture, new Vector2(100, 100), Color.White);
                    spriteBatch.Draw(humpTexture, new Vector2(100, 400), Color.White);
                    spriteBatch.Draw(humpTexture, new Vector2(500, 100), Color.White);
                    spriteBatch.Draw(humpTexture, new Vector2(500, 400), Color.White);
                    
                    #region Attack Drawing (Fireball, Frostball, etc..)
                //Draw our fireball(s)
                foreach (Fireball fireball in fireballs)
                {
                    fireball.Draw(spriteBatch);
                }

                //Draw our frostspear(s)
                foreach (Frostball frostball in frostballs)
                {
                    frostball.Draw(spriteBatch);
                }

                //Draw our electroball(s)
                foreach (Electroball electroball in electroballs)
                {
                    electroball.Draw(spriteBatch);
                }
                #endregion

                    //Draw attack HUD
                    attackBar.Draw(gameTime, spriteBatch, Window.ClientBounds);

                    //Show the current FPS
                    //spriteBatch.DrawString(spriteFont, "FPS: " + (1000 / gameTime.ElapsedGameTime.Milliseconds), new Vector2(0, 1), Color.Black);
                    //spriteBatch.DrawString(spriteFont, "FPS: " + (1000 / gameTime.ElapsedGameTime.Milliseconds), Vector2.Zero, Color.White);
                    #endregion
                break;

                case GameState.Pause:
                    #region Pause screen drawing
                    spriteBatch.Draw(Content.Load<Texture2D>(@"Images\PauseScreen\PauseScreen"), new Rectangle(0, 0, 800, 600), Color.White);
                    #endregion
                break;

                case GameState.Dead:
                    #region Death screen drawing
                    spriteBatch.Draw(Content.Load<Texture2D>(@"Images\DeadScreen\DeadScreen"), new Rectangle(0, 0, 800, 600), Color.White);
                    #endregion
                break;

                case GameState.Win:
                    #region Win screen drawing
                    spriteBatch.Draw(Content.Load<Texture2D>(@"Images\WinningScreen\WinningScreen"), new Rectangle(0, 0, 800, 600), Color.White);           
                    #endregion
                break;
            }
            #endregion

            //Draw the custom cursor
            spriteBatch.Draw(cursorTexture, new Vector2(Mouse.GetState().X, Mouse.GetState().Y), Color.White);

            //Close our sprite batch
            spriteBatch.End();

            base.Draw(gameTime);
        }
        #endregion

        #region Misc (UpdateAttacks, Shoot, ..)
        /// <summary>
        /// Load new zombies
        /// </summary>
        public void LoadNewZombies(int level, int pZombieCount)
        {
            //Clear old assets
            zombies.Clear();
            fireballs.Clear();
            frostballs.Clear();
            electroballs.Clear();

            //False a few bools
            newEarthgrab.isPlanted = false;
            newHealthTotem.isPlanted = false;
            newSpikes.isPlanted = false;

            //Uppdate zombieCount
            zombieCount = pZombieCount;

            //Loop zombies
            for (int i = 0; i <= pZombieCount - 1; i++)
            {
                //Change health based on level
                switch (level)
                {
                    case 1:
                        //Easy zombies
                        zombies.Add(new Zombie(Content.Load<Texture2D>(@"Images\Enemies\Zombie"), Content.Load<Texture2D>(@"Images\Enemies\ZombieHover"), Content.Load<Texture2D>(@"Images\Enemies\ZombieTarget"), new Vector2(randNum.Next(680 - 60), randNum.Next(1100 - 60)), new Point(26, 35), 10, new Point(0, 0), new Point(2, 1), new Vector2(1, 1), 200, "Level1Zombie#" + (i + 1), 100));
                        break;

                    case 2:
                        //Medium zombies
                        zombies.Add(new Zombie(Content.Load<Texture2D>(@"Images\Enemies\Zombie"), Content.Load<Texture2D>(@"Images\Enemies\ZombieHover"), Content.Load<Texture2D>(@"Images\Enemies\ZombieTarget"), new Vector2(randNum.Next(680 - 60), randNum.Next(1100 - 60)), new Point(26, 35), 10, new Point(0, 0), new Point(2, 1), new Vector2(1, 1), 200, "Level2Zombie#" + (i + 1), 120));
                        break;

                    case 3:
                        //Hard zombies
                        zombies.Add(new Zombie(Content.Load<Texture2D>(@"Images\Enemies\Zombie"), Content.Load<Texture2D>(@"Images\Enemies\ZombieHover"), Content.Load<Texture2D>(@"Images\Enemies\ZombieTarget"), new Vector2(randNum.Next(680 - 60), randNum.Next(1100 - 60)), new Point(26, 35), 10, new Point(0, 0), new Point(2, 1), new Vector2(1, 1), 200, "Level3Zombie#" + (i + 1), 140));
                        break;
                }
            }
        }
        /// <summary>
        /// Method for firing attacks at the enemy
        /// </summary>
        /// <param name="pAttackType">The type of attack to fire.</param>
        public void Shoot(string pAttackType)
        {
            //Which attack are we using?
            if (pAttackType == "fireball")
            {
                #region Fireball Attack
                //Create a new fireball object at the position of the player
                newFireball = new Fireball(Content.Load<Texture2D>(@"Images\Player\Attacks\Fireball"));
                newFireball.position = new Vector2(player.position.X + player.frameSize.X / 2, player.position.Y + player.frameSize.Y / 2);

                //Calculate and normalize the direction of which to fire
                Vector2 dir = mouseToTarget - player.position;
                dir.Normalize();

                //Add velocity and update the position
                newFireball.velocity = dir * 4.0f;
                newFireball.position += newFireball.velocity;

                //I'm visible!
                newFireball.isVisible = true;

                //Less than 10 fireballs?
                if (fireballs.Count < 10)
                {
                    //Have one more
                    fireballs.Add(newFireball);
                }
                #endregion
            }
            else if (pAttackType == "frostball")
            {
                #region Frostball Attack
                //Create a new frostspear object at the position of the player
                newFrostball = new Frostball(Content.Load<Texture2D>(@"Images\Player\Attacks\Frostball"));
                newFrostball.position = new Vector2(player.position.X + player.frameSize.X / 2, player.position.Y + player.frameSize.Y / 2);

                //Calculate and normalize the direction of which to fire
                Vector2 dir = mouseToTarget - player.position;
                dir.Normalize();

                //Add velocity and update the position
                newFrostball.velocity = dir * 4.0f;
                newFrostball.position += newFrostball.velocity;

                //I'm visible!
                newFrostball.isVisible = true;

                //Less than 20 frostspear?
                if (frostballs.Count < 15)
                {
                    //Have one more
                    frostballs.Add(newFrostball);
                }
                #endregion
            }
            else if (pAttackType == "electroball")
            {
                #region Electroball Attack
                //Create a new frostspear object at the position of the player
                newElectroball = new Electroball(Content.Load<Texture2D>(@"Images\Player\Attacks\Electroball"));
                newElectroball.position = new Vector2(player.position.X + player.frameSize.X / 2, player.position.Y + player.frameSize.Y / 2);

                //Calculate and normalize the direction of which to fire
                Vector2 dir = mouseToTarget - player.position;
                dir.Normalize();

                //Add velocity and update the position
                newElectroball.velocity = dir * 4.0f;
                newElectroball.position += newElectroball.velocity;

                //I'm visible!
                newElectroball.isVisible = true;

                //Less than 20 frostspear?
                if (electroballs.Count < 15)
                {
                    //Have one more
                    electroballs.Add(newElectroball);
                }
                #endregion
            }
            else if (pAttackType == "spikes")
            {
                #region Spikes
                //I'm visible!
                newSpikes.isVisible = true;
                newSpikes.isPlanted = true;
                #endregion
            }
            else if (pAttackType == "earthgrab")
            {
                #region Plant Earthgrab
                //I'm visible!
                newEarthgrab.isVisible = true;
                newEarthgrab.isPlanted = true;
                #endregion
            }
            else if (pAttackType == "healthtotem")
            {
                #region Plant Health Totem
                //I'm visible!
                newHealthTotem.isVisible = true;
                newHealthTotem.isPlanted = true;
                #endregion
            }
        }

        /// <summary>
        /// Method for updating the attacks/visibility checks.
        /// </summary>
        public void UpdateAttacks()
        {
            #region Fireball update
            //Loop the fireballs
            foreach (Fireball fireball in fireballs)
            {
                //Update the position
                fireball.position += fireball.velocity;

                //How far away from the player is the fireball?
                if (Vector2.Distance(fireball.position, player.position) > 1100)
                {
                    //Too far. That's how far.
                    fireball.isVisible = false;
                }
            }

            //For loop this time. Switching it up.
            for (int i = 0; i < fireballs.Count; i++)
            {
                //Are you here?!
                if (!fireballs[i].isVisible)
                {
                    //No. I guess not. Remove it.
                    fireballs.RemoveAt(i);
                    i--;
                }
            }
            #endregion

            #region Frostball update
            //Loop the fireballs
            foreach (Frostball frostball in frostballs)
            {
                //Update the position
                frostball.position += frostball.velocity;

                //How far away from the player is the fireball?
                if (Vector2.Distance(frostball.position, player.position) > 1100)
                {
                    //Too far. That's how far.
                    frostball.isVisible = false;
                }
            }

            //For loop this time. Switching it up.
            for (int i = 0; i < frostballs.Count; i++)
            {
                //Are you here?!
                if (!frostballs[i].isVisible)
                {
                    //No. I guess not. Remove it.
                    frostballs.RemoveAt(i);
                    i--;
                }
            }
            #endregion

            #region Electroball update
            //Loop the fireballs
            foreach (Electroball electroball in electroballs)
            {
                //Update the position
                electroball.position += electroball.velocity;

                //How far away from the player is the fireball?
                if (Vector2.Distance(electroball.position, player.position) > 1100)
                {
                    //Too far. That's how far.
                    electroball.isVisible = false;
                }
            }

            //For loop this time. Switching it up.
            for (int i = 0; i < electroballs.Count; i++)
            {
                //Are you here?!
                if (!electroballs[i].isVisible)
                {
                    //No. I guess not. Remove it.
                    electroballs.RemoveAt(i);
                    i--;
                }
            }
            #endregion
        }
        #endregion
    }
}
