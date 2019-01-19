using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MyAsteroidsGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        //Variables
        protected enum mode { Menu, Playing, Game_Over };
        protected enum objectName { Space_Asteroid, Space_Chaser, Blind_Zerg };
        mode currentMode;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        PathFinder pathFinder;
        Random rand;
        float difficulty;
        bool hitboxOn;
        bool nodesOn;
        bool recentlySelceted;
        bool recentlyUnselected;
        int pathUpdateRate;
        bool respawning;
        bool muted;
        bool selected;
        bool displayingHelpMenu;
        int level;
        int respawnClearDelay;
        int respawnTime;
        int respawnCounter;
        int playerLives;
        int pathUpdateCounter;
        int voidZone;
        bool pathOn;
        float asteroidMapWeight;
        int enemyCount;
        int enemyMineCount;
        int enemyMineCountMin;
        int enemyCountMin;
        int asteroidMapWeightMin;
        Int64 score;
        Int64 scoreCounter;
        Int64 relativeScoreHolder;

        Thread threadPathFinding;

        //Keyboard
        KeyboardState ks;

        //Objects
        List<SpaceObject> objects;
        List<Texture2D> asteroidTextures;
        List<Texture2D> explosionTextures;
        List<Texture2D> shrapnel;
        List<Texture2D> thrustParticles;
        Texture2D PlayerShip;
        Texture2D PlayerShipThrusting;
        Texture2D PlayerShipDead;
        Texture2D EnemyShip1;
        Texture2D EnemyMine;

        //Sounds and Music
        SoundEffect background;
        SoundEffectInstance bgMusic;
        SoundEffect mainMenuMusicIntro;
        SoundEffect mainMenuMusicLoop;
        SoundEffect makeItRainSound;
        SoundEffect gameOverSound;
        SoundEffect goSound;
        SoundEffect levelUpSound;
        SoundEffect respawnSound;
        SoundEffect respawnSoundWarp;
        SoundEffect youDaBest;
        SoundEffect forcePushSound;
        SoundEffect mineDetect;
        SoundEffect MedASound;
        SoundEffect SmallASound;
        SoundEffect scoutHatesYou;
        SoundEffect thrustSound;
        SoundEffectInstance thrustSoundInstance;
        SoundEffect sadEndingSong;
        SoundEffectInstance sadEndingSongInstance;
        SoundEffectInstance mainMenuMusicInstance;
        List<SoundEffect> explosionSounds;
        List<SoundEffect> radioChatter;

        int gamespeed;

        // background texture layers
        Texture2D texture1;
        List<Particle> particleList;

        //HUD
        Texture2D Bar;
        Texture2D Health;
        Texture2D Shield;
        Texture2D pauseMenu;
        Texture2D mainMenu;
        Texture2D HelpMenu;
        SpriteFont font;
        SpriteFont gameOverFont;

        //Random Textures
        Texture2D Cracks;
        Texture2D Error;
        Texture2D blank;
        Texture2D Hitbox;
        Texture2D HitboxSquare;
        Texture2D playerShield;
        Texture2D chaseModeCover;
        Texture2D patrolModeCover;
        Texture2D invisible;

        // player
        Vector2 camera;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            recentlySelceted = false;
            recentlyUnselected = false;
            selected = true;
            currentMode = mode.Menu;
            muted = false;
            respawnClearDelay = 0;
            respawnCounter = 0;
            respawnTime = 200;
            relativeScoreHolder = 0;
            scoreCounter = 0;
            respawning = false;
            playerLives = 3;
            level = 1;
            enemyMineCountMin = 1;
            displayingHelpMenu = false;
            enemyCountMin = 1;
            asteroidMapWeightMin = 30;
            score = 0;
            difficulty = 1;
            pathFinder = new PathFinder();
            rand = new Random();
            gamespeed = 1;
            enemyMineCount = 0;
            pathUpdateRate = 5;
            pathUpdateCounter = 0;
            voidZone = 100;
            hitboxOn = false;
            pathOn = false;
            nodesOn = false;
            this.IsFixedTimeStep = false;
            asteroidMapWeight = 0;
            enemyCount = 0;
            ks = Keyboard.GetState();

            ////Objects
            particleList = new List<Particle>();
            asteroidTextures = new List<Texture2D>();
            explosionTextures = new List<Texture2D>();
            shrapnel = new List<Texture2D>();
            thrustParticles = new List<Texture2D>();
            objects = new List<SpaceObject>();
            pathFinder.Pathfinder(GraphicsDevice, voidZone, (float)0.025, objects);
            threadPathFinding = new Thread(updatePaths);

            //Sounds and Music Stuff
            explosionSounds = new List<SoundEffect>();
            radioChatter = new List<SoundEffect>();


            //Stopping any already playing sounds
            if (bgMusic != null)
                bgMusic.Stop();
            if (mainMenuMusicInstance != null)
                mainMenuMusicInstance.Stop();
            if (sadEndingSongInstance != null)
                sadEndingSongInstance.Stop();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Sounds and Music
            //Main Game Music
            background = Content.Load<SoundEffect>("Audio/Music/SMB");
            bgMusic = background.CreateInstance();
            bgMusic.IsLooped = true;
            //Main Menu Music
            mainMenuMusicLoop = Content.Load<SoundEffect>("Audio/Music/MoonLoop");
            mainMenuMusicIntro = Content.Load<SoundEffect>("Audio/Music/MoonStart");
            mainMenuMusicInstance = mainMenuMusicIntro.CreateInstance();
            //Explosions
            explosionSounds.Add(Content.Load<SoundEffect>("Audio/Sound Effects/Explosions/ExplosionSound0"));
            explosionSounds.Add(Content.Load<SoundEffect>("Audio/Sound Effects/Explosions/ExplosionSound1"));
            explosionSounds.Add(Content.Load<SoundEffect>("Audio/Sound Effects/Explosions/ExplosionSound2"));
            //Asteroid Deaths
            MedASound = Content.Load<SoundEffect>("Audio/Sound Effects/Asteroids/MediumAsteroidBreaking");
            SmallASound = Content.Load<SoundEffect>("Audio/Sound Effects/Asteroids/SmallAsteroidBreaking");
            //Other Game Sound effects
            makeItRainSound = Content.Load<SoundEffect>("Audio/Sound Effects/Level Sounds/Soundbites/MakeItRain");
            gameOverSound = Content.Load<SoundEffect>("Audio/Sound Effects/Level Sounds/Status Sounds/GameOverSound");
            goSound = Content.Load<SoundEffect>("Audio/Sound Effects/Level Sounds/Status Sounds/GoSound");
            levelUpSound = Content.Load<SoundEffect>("Audio/Sound Effects/Level Sounds/Status Sounds/LevelUpSound");
            respawnSound = Content.Load<SoundEffect>("Audio/Sound Effects/Level Sounds/Status Sounds/RespawnSound");
            respawnSoundWarp = Content.Load<SoundEffect>("Audio/Sound Effects/Level Sounds/Status Sounds/RespawnSoundWarp");
            thrustSound = Content.Load<SoundEffect>("Audio/Sound Effects/PlayerShip/ThrustSound");
            thrustSoundInstance = thrustSound.CreateInstance();
            thrustSoundInstance.IsLooped = true;
            thrustSoundInstance.Play();
            thrustSoundInstance.Pause();
            mineDetect = Content.Load<SoundEffect>("Audio/Sound Effects/Enemies/Mine/MineDetect");
            scoutHatesYou = Content.Load<SoundEffect>("Audio/Sound Effects/Level Sounds/Soundbites/ScoutHatesYou");
            youDaBest = Content.Load<SoundEffect>("Audio/Sound Effects/Level Sounds/Soundbites/YouDaBest");
            forcePushSound = Content.Load<SoundEffect>("Audio/Sound Effects/Weapons/ForcePushSound");
            //RadioChatter
            for (int i = 0; i < 5; i++)
            {
                String tempString = "Audio/Sound Effects/Enemies/Chaser/RadioChatter" + i.ToString();
                radioChatter.Add(Content.Load<SoundEffect>(tempString));
            }
            //Sad End song
            thrustSoundInstance.Pause();
            sadEndingSong = Content.Load<SoundEffect>("Audio/Music/SadEndSong");
            sadEndingSongInstance = sadEndingSong.CreateInstance();
            sadEndingSongInstance.IsLooped = true;

            //Playing Main Menu Music
            mainMenuMusicInstance.Play();

            // TODO: use this.Content to load your game content here
            texture1 = Content.Load<Texture2D>("Textures/Backgrounds/4");

            //Sprites
            PlayerShip = Content.Load<Texture2D>("Textures/Player Ship/Ship");
            PlayerShipThrusting = Content.Load<Texture2D>("Textures/Player Ship/ShipThrust");
            PlayerShipDead = Content.Load<Texture2D>("Textures/Player Ship/ShipDead");
            EnemyShip1 = Content.Load<Texture2D>("Textures/Enemies/EShip3");
            EnemyMine = Content.Load<Texture2D>("Textures/Enemies/EShip5");

            //HUD
            mainMenu = Content.Load<Texture2D>("Textures/Backgrounds/MainMenu");
            Bar = Content.Load<Texture2D>("Textures/HUD/Bar");
            Shield = Content.Load<Texture2D>("Textures/HUD/Shield");
            Health = Content.Load<Texture2D>("Textures/HUD/Health");
            pauseMenu = Content.Load<Texture2D>("Textures/Pause Menu/Pause");
            HelpMenu = Content.Load<Texture2D>("Textures/Pause Menu/Help");
            font = Content.Load<SpriteFont>("Fonts/MyFont");
            gameOverFont = Content.Load<SpriteFont>("Fonts/MyFontGameOver");

            //Random Textures
            asteroidTextures.Add(Content.Load<Texture2D>("Textures/Asteroids/MTestG1"));
            asteroidTextures.Add(Content.Load<Texture2D>("Textures/Asteroids/MTestG2"));
            asteroidTextures.Add(Content.Load<Texture2D>("Textures/Asteroids/MTestG3"));
            asteroidTextures.Add(Content.Load<Texture2D>("Textures/Asteroids/MTest1"));
            asteroidTextures.Add(Content.Load<Texture2D>("Textures/Asteroids/MTest2"));
            asteroidTextures.Add(Content.Load<Texture2D>("Textures/Asteroids/MTest3"));
            asteroidTextures.Add(Content.Load<Texture2D>("Textures/Asteroids/MTest4"));
            blank = Content.Load<Texture2D>("Textures/General Use/White");

            //Particles
            for (int i = 0; i < 6; i++)
            {
                String temp = "Textures/Particles/Shrapnel/Shrapnel";
                temp += i.ToString();
                shrapnel.Add(Content.Load<Texture2D>(temp));
            }
            for (int i = 0; i < 7; i++)
            {
                String temp = "Textures/Particles/Thrust/Thrust";
                temp += i.ToString();
                thrustParticles.Add(Content.Load<Texture2D>(temp));
            }
            explosionTextures.Add(Content.Load<Texture2D>("Textures/Particles/Explosions/Explosion0"));
            explosionTextures.Add(Content.Load<Texture2D>("Textures/Particles/Explosions/Explosion5"));

            //Other
            invisible = Content.Load<Texture2D>("Textures/General Use/Invisible");
            chaseModeCover = Content.Load<Texture2D>("Textures/Enemies/Danger");
            patrolModeCover = Content.Load<Texture2D>("Textures/Enemies/Safe");
            playerShield = Content.Load<Texture2D>("Textures/Player Ship/SheildTestBMP");
            Cracks = Content.Load<Texture2D>("Textures/Asteroids/CracksTest");
            Error = Content.Load<Texture2D>("Textures/General Use/ERROR");
            Hitbox = Content.Load<Texture2D>("Textures/Debugging Textures/Hitbox");
            HitboxSquare = Content.Load<Texture2D>("Textures/Debugging Textures/HitBoxSquare");

            ////Objects
            PlayerShip player = new PlayerShip();

            //Weapons
            player.addWeaponTexture(Content.Load<Texture2D>("Textures/Weapons/Wave2"));
            objects.Add(player);
            objects.ElementAt(0).init(objects, difficulty);
            objects.ElementAt(0).setSprite(PlayerShip);
            objects.ElementAt(0).setX(GraphicsDevice.Viewport.Width / 2);
            objects.ElementAt(0).setY(GraphicsDevice.Viewport.Height / 2);
            objects.ElementAt(0).setVelocity((float)0.2);
            objects.ElementAt(0).setCurrentNode(pathFinder.spaceObjectLoc(objects.ElementAt(0)));

            camera = Vector2.Zero;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        //CCreates the particle effect of an asteroid explosion on the "parent"
        private void AddAsteroidExplosion(SpaceObject parent)
        {
            for (int i = 0; i < parent.getRealSpriteHeight(); i++)
            {
                float tempSpin = ((float)rand.Next(-50, 51) / 10);
                float tempSpeedX = ((float)(rand.Next(-15, 16)) / 10) + parent.getSpeedX();
                float tempSpeedY = ((float)(rand.Next(-15, 16)) / 10) + parent.getSpeedY();
                int tempLifeTime = rand.Next(20, 60);

                int noSpawnZoneGap = (int)((parent.getRealSpriteWidth() / (float)6));
                int minX = (int)((parent.getX() + noSpawnZoneGap) - parent.getRealSpriteWidth() / 2);
                int maxX = (int)(((parent.getX() + parent.getRealSpriteWidth()) - noSpawnZoneGap) - parent.getRealSpriteWidth() / 2);
                int minY = (int)((parent.getY() + noSpawnZoneGap) - parent.getRealSpriteHeight() / 2);
                int maxY = (int)(((parent.getY() + parent.getRealSpriteHeight()) - noSpawnZoneGap) - parent.getRealSpriteHeight() / 2);

                float tempSize = ((float)rand.Next(50, 150) / 10);
                float tempX = ((float)rand.Next(minX, maxX));
                float tempY = ((float)rand.Next(minY, maxY));

                int temp = rand.Next(0, asteroidTextures.Count);
                AddParticleXY(asteroidTextures.ElementAt(temp), tempX, tempY, tempSpeedX, tempSpeedY, tempSpin, tempSize, tempLifeTime);
            }
            if (parent.getSize() < 3.5)
                SmallASound.Play();
            else
                MedASound.Play();
        }

        //Creates the particle effect of the players thrust engines on the "parent"
        private void AddPlayerThrustJet(SpaceObject parent)
        {
            for (int i = 0; i < 3; i++)
            {
                float tempSpin = 0;
                float tempDirection = (180 + parent.getSpriteDirection() + ((float)(rand.Next(-300, 300)) / 10));
                float tempVel = ((float)(rand.Next(15, 31)) / 10);
                int tempLifeTime = (int)(rand.Next(50, 100));
                float tempSize = 2;


                float tempX = (float)(parent.getX() - (Math.Cos(parent.getSpriteDirectionRadians()) * (parent.getRealSpriteWidth() / 2.4)));
                float tempY = (float)(parent.getY() - (Math.Sin(parent.getSpriteDirectionRadians()) * (parent.getRealSpriteHeight() / 2.4)));

                //float tempX = parent.getX();
                //float tempY = parent.getY();

                int tempTex = rand.Next(0, thrustParticles.Count);
                AddParticleVelDir(thrustParticles.ElementAt(tempTex), tempX, tempY, tempVel, tempDirection, tempSpin, tempSize, tempLifeTime);
            }
        }

        //Creates the particle effect of the chaser class enemy ship's thrust engines (parent)
        private void AddChaserThrustJet(SpaceObject parent)
        {
            for (int i = 0; i < 3; i++)
            {
                float tempSpin = 0;
                float tempDirection = (180 + parent.getSpriteDirection() + ((float)(rand.Next(-300, 300)) / 10));
                float tempVel = ((float)(rand.Next(7, 21)) / 10);
                int tempLifeTime = (int)(rand.Next(20, 50));
                float tempSize = 2;


                float tempX = (float)(parent.getX() - (Math.Cos(parent.getSpriteDirectionRadians()) * (parent.getRealSpriteWidth() / 2.4)));
                float tempY = (float)(parent.getY() - (Math.Sin(parent.getSpriteDirectionRadians()) * (parent.getRealSpriteHeight() / 2.4)));

                //float tempX = parent.getX();
                //float tempY = parent.getY();

                int tempTex = rand.Next(0, explosionTextures.Count);
                AddParticleVelDir(explosionTextures.ElementAt(tempTex), tempX, tempY, tempVel, tempDirection, tempSpin, tempSize, tempLifeTime);
            }
        }

        //Creates an explosion of "size" on a "parent"
        //This will not only create the particles for it, but also the actual "Explosion" object that can be collided with
        private void AddExplosion(SpaceObject parent, float size)
        {
            for (int i = 0; i < ((parent.getRealSpriteHeight() * parent.getRealSpriteWidth()) / 6) * size; i++)
            {
                float tempSpin = 0;
                float tempDirection = ((float)(rand.Next(0, 3600)) / 10);
                float tempVel = ((float)(rand.Next(-50, 51)) / 10);
                int tempLifeTime = (int)(rand.Next(20, 40) * size);

                int noSpawnZoneGap = (int)((parent.getRealSpriteWidth() / (float)6));
                int minX = (int)((parent.getX() + noSpawnZoneGap) - parent.getRealSpriteWidth() / 2);
                int maxX = (int)(((parent.getX() + parent.getRealSpriteWidth()) - noSpawnZoneGap) - parent.getRealSpriteWidth() / 2);
                int minY = (int)((parent.getY() + noSpawnZoneGap) - parent.getRealSpriteHeight() / 2);
                int maxY = (int)(((parent.getY() + parent.getRealSpriteHeight()) - noSpawnZoneGap) - parent.getRealSpriteHeight() / 2);

                float tempSize = 2;
                float tempX = ((float)rand.Next(minX, maxX));
                float tempY = ((float)rand.Next(minY, maxY));

                int tempTex = rand.Next(0, explosionTextures.Count);
                AddParticleVelDir(explosionTextures.ElementAt(tempTex), tempX, tempY, tempVel, tempDirection, tempSpin, tempSize, tempLifeTime);
            }
            for (int i = 0; i < (parent.getRealSpriteHeight()) * size; i++)
            {
                float tempSpin = ((float)rand.Next(-50, 51) / 10);
                float tempDirection = ((float)(rand.Next(0, 3600)) / 10);
                float tempVel = ((float)(rand.Next(-30, 31)) / 10);
                int tempLifeTime = (int)(rand.Next(50, 150));

                int noSpawnZoneGap = (int)((parent.getRealSpriteWidth() / (float)6));
                int minX = (int)((parent.getX() + noSpawnZoneGap) - parent.getRealSpriteWidth() / 2);
                int maxX = (int)(((parent.getX() + parent.getRealSpriteWidth()) - noSpawnZoneGap) - parent.getRealSpriteWidth() / 2);
                int minY = (int)((parent.getY() + noSpawnZoneGap) - parent.getRealSpriteHeight() / 2);
                int maxY = (int)(((parent.getY() + parent.getRealSpriteHeight()) - noSpawnZoneGap) - parent.getRealSpriteHeight() / 2);

                float tempSize = ((float)rand.Next(20, 100) / 10);
                float tempX = ((float)rand.Next(minX, maxX));
                float tempY = ((float)rand.Next(minY, maxY));

                int tempTex = rand.Next(0, shrapnel.Count);
                AddParticleVelDir(shrapnel.ElementAt(tempTex), tempX, tempY, tempVel, tempDirection, tempSpin, tempSize, tempLifeTime);
            }
            objects.Add(new Explosion(parent, chaseModeCover, size));
            explosionSounds.ElementAt(rand.Next(0, explosionSounds.Count)).Play();
        }

        //Adds a particle to the particle list of the spesified parameters (speed based on speedX speedY)
        void AddParticleXY(Texture2D particleTex, float xIn, float yIn, float speedXIn, float speedYIn, float rotationIn, float diameterIn, int lifeTimeIn)
        {
            Particle particle = new Particle();

            particle.tex = particleTex;
            particle.x = xIn;
            particle.realSpriteHeight = (int)diameterIn;
            particle.realSpriteWidth = (int)diameterIn;
            particle.y = yIn;
            particle.speedX = speedXIn;
            particle.speedY = speedYIn;
            particle.spin = rotationIn;
            particle.maxLifeTime = lifeTimeIn;
            particle.lifeTime = particle.maxLifeTime;

            particle.calculateVelAndDirBasedOnSpeedXY();
            particle.remakeRec();
            particleList.Add(particle);
        }
        //Adds a particle to the particle list of the spesified parameters (speed based on Direction and Velocity)
        void AddParticleVelDir(Texture2D particleTex, float xIn, float yIn, float velocityIn, float directionIn, float rotationIn, float diameterIn, int lifeTimeIn)
        {
            Particle particle = new Particle();

            particle.tex = particleTex;
            particle.x = xIn;
            particle.realSpriteHeight = (int)diameterIn;
            particle.realSpriteWidth = (int)diameterIn;
            particle.y = yIn;
            particle.direction = directionIn;
            particle.velocity = velocityIn;
            particle.spin = rotationIn;
            particle.maxLifeTime = lifeTimeIn;
            particle.lifeTime = particle.maxLifeTime;

            particle.calculateXYBasedOnDirVel();
            particle.remakeRec();
            particleList.Add(particle);
        }

        //Creates a child asteroid inside of the "parent"
        //Returns false if after multiple attempts the asteroid can not be placed without overlapping something
        protected bool createChildAsteroid(SpaceObject parent)
        {
            SpaceObject SO = null;
            bool returnVal = true;
            //Small asteroids will not create children
            if (parent.getSize() > 3)
            {
                SO = new Asteroid();
                int attempts = 50;
                int temp = rand.Next(0, asteroidTextures.Count - 1);
                bool colission = true;
                int count = 0;

                SO.setSprite(asteroidTextures.ElementAt(temp));
                SO.setSpin((float)rand.Next(-50, 51) / 10);

                //Inherits parents speed as well as a randomised amount
                SO.setSpeedX(((float)(rand.Next(-15, 16)) / 10) + parent.getSpeedX());
                SO.setSpeedY(((float)(rand.Next(-15, 16)) / 10) + parent.getSpeedY());
                SO.calculateVelAndDirBasedOnSpeedXY();

                //Calculating the minimum and maximum areas it can spawn based on its parent
                int noSpawnZoneGap = (int)((parent.getRealSpriteWidth() / (float)6));
                int minX = (int)((parent.getX() + noSpawnZoneGap) - parent.getRealSpriteWidth() / 2);
                int maxX = (int)(((parent.getX() + parent.getRealSpriteWidth()) - noSpawnZoneGap) - parent.getRealSpriteWidth() / 2);
                int minY = (int)((parent.getY() + noSpawnZoneGap) - parent.getRealSpriteHeight() / 2);
                int maxY = (int)(((parent.getY() + parent.getRealSpriteHeight()) - noSpawnZoneGap) - parent.getRealSpriteHeight() / 2);

                while (colission)
                {
                    SO.setSize((float)rand.Next(10, (int)(parent.getSize() / 3) * 10) / 10);
                    SO.setX((float)rand.Next(minX, maxX));
                    SO.setY((float)rand.Next(minY, maxY));

                    colission = false;

                    if (checkCollissionForObject(SO) != SO)
                    {
                        colission = true;
                    }
                    else
                    {
                        SO.makeInvunerable(10);
                    }
                    //Break put in so that it will fail if an asteroid can not be placed
                    count++;
                    if (count > attempts)
                    {
                        returnVal = false;
                        colission = false;
                    }
                }
            }
            else
            {
                returnVal = false;
            }
            if (returnVal)
                if (SO != null)
                {
                    SO.setCurrentNode(pathFinder.spaceObjectLoc(SO));
                    objects.Add(SO);
                }

            return returnVal;
        }

        //Creates a "Space Object" at a randomised location (outside of view)
        protected bool createObject(objectName nameIn)
        {
            //Number of attempts it will make to place the object before giving up
            int attempts = 200;
            bool colission = true;
            bool testing = true;
            int count = 0;
            SpaceObject SO = null;
            //Creating Different Objects and initialising them acordingly
            switch (nameIn)
            {
                case objectName.Space_Asteroid:
                    SO = new Asteroid();
                    int temp = rand.Next(0, asteroidTextures.Count - 1);
                    SO.setSprite(asteroidTextures.ElementAt(temp));
                    SO.setDirection((float)rand.Next(0, 361));
                    SO.setSpin((float)rand.Next(-50, 51) / 10);
                    SO.setVelocity((float)rand.Next(5, 35) / 10);
                    SO.setSize((float)rand.Next(10, 100) / 10);
                    break;

                case objectName.Space_Chaser:
                    SO = new Chaser();
                    SO.init(objects, difficulty);
                    SO.setSprite(EnemyShip1);
                    break;

                case objectName.Blind_Zerg:
                    SO = new BlindZerg();
                    SO.init(objects, difficulty);
                    SO.setDirection((float)rand.Next(0, 361));
                    SO.setVelocity((float)rand.Next(5, 25) / 10);
                    SO.calculateXYBasedOnDirVel();
                    SO.setSprite(EnemyMine);
                    break;

                default:
                    colission = false;
                    testing = false;
                    break;
            }
            if (SO != null)
                SO.initiateLevelSize(pathFinder.levelWidth, pathFinder.levelHeight);

            //Creating Object on the edge of the map (preferably in the "void")
            while (colission)
            {
                float tempX = 0;
                float tempY = 0;
                if (voidZone > SO.getRec().Width / 2)
                {
                    while (tempX > 0 - SO.getRec().Width / 2)
                        tempX = rand.Next(-voidZone, GraphicsDevice.Viewport.Width + voidZone);
                    while (tempY > 0 - SO.getRec().Height / 2)
                        tempY = rand.Next(-voidZone, GraphicsDevice.Viewport.Height + voidZone);
                }
                SO.setX(tempX);
                SO.setY(tempY);

                SO.updateRec();
                colission = false;

                if (checkCollissionForObject(SO) != SO)
                    colission = true;
                count++;
                if (count > attempts)
                {
                    colission = false;
                    testing = false;
                }
            }
            if (SO != null)
            {
                if (testing)
                {
                    SO.setCurrentNode(pathFinder.spaceObjectLoc(SO));
                    objects.Add(SO);
                }
            }
            return testing;
        }

        //Returns true if the two objects are colliding (intersecting)
        protected bool checkSpesificCollision(SpaceObject SO1, SpaceObject SO2)
        {
            bool returnVal = false;
            if (SO1.getRec().Intersects(SO2.getRec()) && SO2 != SO1)
            {
                if (SO1.getTargetCol() != SO2)
                {
                    float temptemptemp = ((SO1.getRealSpriteWidth() / 2) + (SO2.getRealSpriteWidth() / 2));
                    SO1.setCollisionClose(true);
                    float distanceX = SO1.getX() - SO2.getX();
                    float distanceY = SO1.getY() - SO2.getY();
                    if (distanceX < 0)
                        distanceX *= -1;
                    if (distanceY < 0)
                        distanceY *= -1;
                    double distance = (double)Math.Sqrt(Math.Pow(distanceX, 2) + Math.Pow(distanceY, 2));

                    if (distance < temptemptemp)
                    {
                        returnVal = true;
                    }
                }
            }
            return returnVal;
        }

        //Checks the object for a collision with another object
        //Returns the objects that it collides with or itself by default
        protected SpaceObject checkCollissionForObject(SpaceObject SO)
        {
            SpaceObject returnVal = SO;
            for (int j = 0; j < objects.Count; j++)
            {
                if (SO.getRec().Intersects(objects.ElementAt(j).getRec()) && objects.ElementAt(j) != SO)
                {
                    if (SO.getTargetCol() != objects.ElementAt(j))
                    {
                        float temptemptemp = ((SO.getRealSpriteWidth() / 2) + (objects.ElementAt(j).getRealSpriteWidth() / 2));
                        SO.setCollisionClose(true);
                        float distanceX = SO.getX() - objects.ElementAt(j).getX();
                        float distanceY = SO.getY() - objects.ElementAt(j).getY();
                        if (distanceX < 0)
                            distanceX *= -1;
                        if (distanceY < 0)
                            distanceY *= -1;
                        double distance = (double)Math.Sqrt(Math.Pow(distanceX, 2) + Math.Pow(distanceY, 2));

                        if (distance < temptemptemp)
                        {
                            returnVal = objects.ElementAt(j);
                        }
                    }
                }
            }
            return returnVal;
        }

        //Alters the two space objects that have collided
        protected void actOnCollision(SpaceObject SO1, SpaceObject SO2)
        {

            SO1.readyColission(SO2);
            SO2.readyColission(SO1);

            if ((SO1.getObjectId() != 6 && SO2.getObjectId() != 6) &&
                (SO1.getObjectId() != 7 && SO2.getObjectId() != 7))
            {
                //Step back to stop collisions
                SO1.backStep();
                recheckBounds(SO1);
                SO2.backStep();
                recheckBounds(SO2);

                //Bug Fix Where objects get stuck on corners
                while (checkSpesificCollision(SO1, SO2))
                {
                    if (SO1.getX() > SO2.getX())
                    {
                        SO1.setX(SO1.getX() + 1);
                        SO2.setX(SO2.getX() - 1);
                    }
                    else
                    {
                        SO1.setX(SO1.getX() - 1);
                        SO2.setX(SO2.getX() + 1);
                    }
                    if (SO1.getY() > SO2.getY())
                    {
                        SO1.setY(SO1.getY() + 1);
                        SO2.setY(SO2.getY() - 1);
                    }
                    else
                    {
                        SO1.setY(SO1.getY() - 1);
                        SO2.setY(SO2.getY() + 1);
                    }
                    recheckBounds(SO1);
                    recheckBounds(SO2);
                }
            }

            SO1.setColission();
            SO2.setColission();

            SO1.setTargetCol(SO2);
            SO2.setTargetCol(SO1);

            SO2.setCollided(true);
            SO1.setCollided(true);
        }

        //Makes sure the objects are within the map (changes x y to loop)
        protected void recheckBounds(SpaceObject SO)
        {
            //Left
            if (SO.getX() + (SO.getRealSpriteWidth() / 2) + voidZone < SO.getRealSpriteWidth() / 2)
            {
                SO.setX(SO.getX() + GraphicsDevice.Viewport.Width + voidZone * 2);
            }
            //Right
            else if (SO.getX() - voidZone - (SO.getRealSpriteWidth() / 2) > GraphicsDevice.Viewport.Width - (SO.getRealSpriteWidth() / 2))
            {
                SO.setX(SO.getX() - (GraphicsDevice.Viewport.Width + voidZone * 2));
            }
            //Up
            if (SO.getY() + voidZone + (SO.getRealSpriteWidth() / 2) < SO.getRealSpriteHeight() / 2)
            {
                SO.setY(SO.getY() + GraphicsDevice.Viewport.Height + voidZone * 2);
            }
            //Down
            else if (SO.getY() - voidZone - (SO.getRealSpriteWidth() / 2) > GraphicsDevice.Viewport.Height - (SO.getRealSpriteHeight() / 2))
            {
                SO.setY(SO.getY() - (GraphicsDevice.Viewport.Height + voidZone * 2));
            }
        }

        //Updates each space objects current node as well as updates all the weights for the nodes
        protected virtual void updateNodes()
        {
            foreach (SpaceObject SO in objects.ToList())
            {
                SearchNode tempTest = pathFinder.spaceObjectLoc(SO);
                if (SO != null)
                    if (SO.getCurrentNode() != tempTest)
                    {
                        SO.setCurrentNode(tempTest);
                    }
            }
            pathFinder.updateNodes();
        }

        //Updates all of the paths for the AI (who need paths)
        protected virtual void updatePaths()
        {
            updateNodes();
            pathUpdateCounter = 0;

            foreach (SpaceObject SO in objects.ToList())
            {
                if (SO != null)
                {
                    //Calculates only for chaser and mine
                    if (SO.getObjectId() == 2 || SO.getObjectId() == 3)
                    {
                        //Sets current node if it is somehow null still
                        if (SO.getCurrentNode() == null)
                            SO.setCurrentNode(pathFinder.spaceObjectLoc(SO));
                        //Finds path to player if it is chasing him
                        if (SO.getChaseMode())
                            SO.setPath(pathFinder.findRouteDijkstra(SO.getCurrentNode(), objects.ElementAt(0).getCurrentNode(), false));
                        //Otherwise it will find the lowest scoring agacent node
                        else
                            SO.setPath(pathFinder.findSafestNearestNode(SO.getCurrentNode()));
                    }
                }
            }
        }

       
        /// This is my hidiously un-OO spaghetti Update method
        /// MmmMmmMmMmmMmmmmmm spaghetti code
        protected override void Update(GameTime gameTime)
        {
            if (recentlySelceted)
            {
                //resuming music
                if (mainMenuMusicInstance.State == SoundState.Paused)
                    mainMenuMusicInstance.Play();
                else if (bgMusic.State == SoundState.Paused)
                    bgMusic.Play();
                else if (sadEndingSongInstance.State == SoundState.Paused)
                    sadEndingSongInstance.Play();

                recentlySelceted = false;
                if (muted)
                    SoundEffect.MasterVolume = 0;
                else
                    SoundEffect.MasterVolume = 1;
            }
            else if (recentlyUnselected)
            {
                recentlyUnselected = false;
                //pausing music
                if (mainMenuMusicInstance.State == SoundState.Playing)
                    mainMenuMusicInstance.Pause();
                else if (bgMusic.State == SoundState.Playing)
                    bgMusic.Pause();
                else if (sadEndingSongInstance.State == SoundState.Playing)
                    sadEndingSongInstance.Pause();
                SoundEffect.MasterVolume = 0;
            }
            //Activity checks
            if (IsActive)
            {
                if (selected == false)
                    recentlySelceted = true;
                selected = true;
            }
            else
            {
                SuppressDraw();
                if(selected == true)
                    recentlyUnselected = true;
                selected = false;
            }

            if (selected)
            {
                //Key Checks
                checkKeys();

                //Main game update
                if (currentMode == mode.Playing || currentMode == mode.Game_Over)
                {
                    if (gamespeed >= 1)
                    {
                        for (int l = 0; l < gamespeed; l++)
                        {
                            scoreCounter = score - relativeScoreHolder;
                            if (scoreCounter > 1000 * level)
                            {
                                makeItRainSound.Play();
                                levelUpSound.Play();
                                relativeScoreHolder = score;
                                level++;
                                //Every 3 Levels another mine can appear
                                if (level % 3 == 0)
                                    enemyMineCountMin++;
                                //Every 2 Levels another ship can appear
                                if (level % 2 == 0)
                                    enemyCountMin++;
                                asteroidMapWeightMin += 5;
                            }

                            //Map Balancing
                            float tempAsteroidMapWeight = 0;
                            int tempEnemyCount = 0;
                            int tempEnemyMineCounter = 0;
                            foreach (SpaceObject SO in objects)
                            {
                                if (SO.getObjectId() == 2)
                                {
                                    tempEnemyCount++;
                                }
                                else if (SO.getObjectId() == 3)
                                {
                                    tempEnemyMineCounter++;
                                }
                                else if (SO.getObjectId() == 4)
                                {
                                    tempAsteroidMapWeight += SO.getSize();
                                }
                            }
                            asteroidMapWeight = tempAsteroidMapWeight;
                            enemyCount = tempEnemyCount;
                            enemyMineCount = tempEnemyMineCounter;

                            //Chance to add enemies if they are below their respective minimums
                            if (enemyCount < enemyCountMin)
                            {
                                if (rand.Next(1, 101) > 99)
                                    createObject(objectName.Space_Chaser);
                            }
                            if (enemyMineCount < enemyMineCountMin)
                            {
                                if (rand.Next(1, 101) > 99)
                                    createObject(objectName.Blind_Zerg);
                            }
                            if (asteroidMapWeight < asteroidMapWeightMin)
                            {
                                if (rand.Next(1, 101) > 99)
                                    createObject(objectName.Space_Asteroid);
                            }

                            if (pathUpdateCounter >= pathUpdateRate)
                            {
                                if (threadPathFinding.IsAlive == false)
                                {
                                    threadPathFinding.Abort();
                                    threadPathFinding = new Thread(updatePaths);
                                    threadPathFinding.Start();
                                }
                            }
                            pathUpdateCounter++;

                            for (int i = 0; i < objects.Count; i++)
                            {
                                objects.ElementAt(i).resetTargetCol();
                                objects.ElementAt(i).setCollided(false);
                                objects.ElementAt(i).setCollisionClose(false);
                            }

                            //Player Updates
                            if (respawning)
                            {
                                respawnCounter--;
                                if (respawnCounter <= 0)
                                {
                                    bool safeToRespawn = true; ;
                                    objects.ElementAt(0).setX(GraphicsDevice.Viewport.Width / 2);
                                    objects.ElementAt(0).setY(GraphicsDevice.Viewport.Height / 2);

                                    //Clearing the way if it is blocked
                                    if (respawnClearDelay >= 100)
                                    {
                                        respawnClearDelay = 0;
                                        AddExplosion(objects.ElementAt(0), 2);
                                    }

                                    foreach (SpaceObject SO in objects)
                                    {
                                        if (safeToRespawn == true)
                                            if (SO != objects.ElementAt(0))
                                                if (checkSpesificCollision(SO, objects.ElementAt(0)))
                                                    safeToRespawn = false;
                                    }
                                    if (safeToRespawn)
                                    {
                                        objects.ElementAt(0).setHealth((int)objects.ElementAt(0).getMaxHealth());
                                        objects.ElementAt(0).setShieldVal((int)objects.ElementAt(0).getMaxShieldVal());
                                        objects.ElementAt(0).setSpeedX(0);
                                        objects.ElementAt(0).setSpeedY(0);
                                        objects.ElementAt(0).setDirection(0);
                                        objects.ElementAt(0).setVelocity((float)0.001);
                                        respawnSound.Play();
                                        respawnSoundWarp.Play();
                                        respawnClearDelay = 0;
                                        respawnCounter = 0;
                                        respawning = false;
                                    }
                                    else
                                    {
                                        respawnClearDelay++;
                                        respawnCounter++;
                                        objects.ElementAt(0).setX(1000000);
                                        objects.ElementAt(0).setY(1000000);
                                    }
                                }
                            }

                            //Space Object Updates
                            foreach (SpaceObject SO in objects.ToList())
                            {
                                SO.step();
                                recheckBounds(SO);

                                if (SO.getThrust())
                                {
                                    if (SO.getObjectId() == 1)
                                        AddPlayerThrustJet(SO);
                                    else if (SO.getObjectId() == 2)
                                        AddChaserThrustJet(SO);
                                }

                                if (respawning)
                                {

                                }
                                else if (currentMode != mode.Game_Over)
                                    if (SO.checkAwareness(objects.ElementAt(0)))
                                    {
                                        if (SO.getObjectId() == 3)
                                            mineDetect.Play();
                                        else if (SO.getObjectId() == 2)
                                            radioChatter.ElementAt(rand.Next(0, radioChatter.Count)).Play();
                                    }

                                //Destorying Dead Objects
                                if (SO.getLifeTime() == 0 || SO.getHealth() <= 0)
                                {
                                    if (SO.getObjectId() != 1)
                                    {
                                        float scoreMultiplier = (float)(difficulty * Math.Sqrt(level));
                                        SO.destroyed();
                                        int tempScore = 0;
                                        switch (SO.getObjectId())
                                        {
                                            case 4:
                                                while (createChildAsteroid(SO)) { }
                                                AddAsteroidExplosion(SO);
                                                tempScore += (int)(SO.getSize() * scoreMultiplier);
                                                break;
                                            case 2:
                                                AddExplosion(SO, (float)0.8);
                                                tempScore += (int)(200 * scoreMultiplier);
                                                break;
                                            case 3:
                                                AddExplosion(SO, (float)1.5);
                                                tempScore += (int)(50 * scoreMultiplier);
                                                break;
                                        }
                                        if (currentMode != mode.Game_Over)
                                            score += tempScore;

                                        objects.Remove(SO);
                                    }
                                    else
                                    {
                                        if (thrustSoundInstance.State == SoundState.Playing)
                                        {
                                            thrustSoundInstance.Pause();
                                        }

                                        if (respawning == false)
                                        {
                                            if (currentMode != mode.Game_Over)
                                            {
                                                AddExplosion(SO, 3);
                                                //WHEN PLAYER DIES
                                                playerLives--;
                                                if (playerLives < 0)
                                                {
                                                    //GAME OVER STUFF
                                                    if (score < 2000)
                                                        scoutHatesYou.Play();
                                                    else if (score > 20000)
                                                        youDaBest.Play();
                                                    currentMode = mode.Game_Over;
                                                    objects.ElementAt(0).setThrust(false);
                                                    objects.ElementAt(0).setTurningRight(true);
                                                    objects.ElementAt(0).setSprite(PlayerShipDead);
                                                    bgMusic.Stop();
                                                    sadEndingSongInstance.Play();
                                                    gameOverSound.Play();
                                                }
                                                else
                                                {
                                                    //RESPAWN STUFF
                                                    respawning = true;
                                                    respawnCounter = respawnTime;
                                                }
                                                foreach (SpaceObject spaceObject in objects)
                                                {
                                                    spaceObject.setChaseMode(false);
                                                }
                                            }
                                            else
                                            {
                                                objects.ElementAt(0).setShieldVal(0);
                                            }
                                        }
                                        else
                                        {
                                            objects.ElementAt(0).setShieldVal(0);
                                            objects.ElementAt(0).setX(100000);
                                            objects.ElementAt(0).setY(100000);
                                        }
                                    }
                                }
                            }
                            //Collisions
                            foreach (SpaceObject SO in objects.ToList())
                            {
                                bool clipX = false;
                                bool clipY = false;
                                bool clipXEvent = false;
                                bool clipYEvent = false;
                                bool clipXY = false;
                                bool stuffToDo = true;
                                float startX = SO.getX();
                                float startY = SO.getY();

                                //If object is near edge: act past it
                                while (stuffToDo)
                                {
                                    stuffToDo = false;
                                    SO.setX(startX);
                                    SO.setY(startY);
                                    if (clipX == false)
                                    {
                                        if (clipXY == false)
                                            clipX = true;
                                        //Left
                                        if (SO.getX() + voidZone < SO.getRealSpriteWidth() / 2)
                                        {
                                            SO.setX(SO.getX() + GraphicsDevice.Viewport.Width + voidZone * 2);
                                            clipXEvent = true;
                                            stuffToDo = true;
                                        }
                                        //Right
                                        else if (SO.getX() - voidZone > GraphicsDevice.Viewport.Width - (SO.getRealSpriteWidth() / 2))
                                        {
                                            SO.setX(SO.getX() - (GraphicsDevice.Viewport.Width + voidZone * 2));
                                            clipXEvent = true;
                                            stuffToDo = true;
                                        }
                                    }
                                    if (clipY == false)
                                    {
                                        if (clipXY == false)
                                            clipY = true;
                                        //Up
                                        if (SO.getY() + voidZone < SO.getRealSpriteHeight() / 2)
                                        {
                                            SO.setY(SO.getY() + GraphicsDevice.Viewport.Height + voidZone * 2);
                                            clipYEvent = true;
                                            stuffToDo = true;
                                        }
                                        //Down
                                        else if (SO.getY() - voidZone > GraphicsDevice.Viewport.Height - (SO.getRealSpriteHeight() / 2))
                                        {
                                            SO.setY(SO.getY() - (GraphicsDevice.Viewport.Height + voidZone * 2));
                                            clipYEvent = true;
                                            stuffToDo = true;
                                        }
                                    }
                                    if (clipXEvent == true && clipYEvent == true)
                                    {
                                        clipXY = true;
                                        if (clipX == true && clipY == true)
                                        {
                                            stuffToDo = true;
                                            clipX = false;
                                        }
                                        else if (clipX == false && clipY == true)
                                        {
                                            stuffToDo = true;
                                            clipX = true;
                                            clipY = false;
                                            clipXY = false;
                                            clipXEvent = false;
                                            clipYEvent = false;
                                        }
                                    }
                                    SpaceObject tempSO = checkCollissionForObject(SO);
                                    while (tempSO != SO)
                                    {
                                        actOnCollision(SO, tempSO);
                                        tempSO = checkCollissionForObject(SO);
                                        if ((tempSO.getObjectId() == 6 || SO.getObjectId() == 6) ||
                                            (tempSO.getObjectId() == 7 || SO.getObjectId() == 7))
                                            tempSO = SO;
                                    }
                                }
                                SO.setX(startX);
                                SO.setY(startY);
                            }
                        }
                        //Updating Particles
                        foreach (Particle P in particleList.ToList())
                        {
                            P.step();
                            if (P.lifeTime <= 0)
                                particleList.Remove(P);
                        }
                    }
                }
                else if (currentMode == mode.Menu)
                {
                    if (mainMenuMusicInstance.State == SoundState.Stopped)
                    {
                        mainMenuMusicInstance = mainMenuMusicLoop.CreateInstance();
                        mainMenuMusicInstance.IsLooped = true;
                        mainMenuMusicInstance.Play();
                    }
                }
            }
            base.Update(gameTime);
        }

        //Draws all of the current particles in the particle list
        private void DrawParticles()
        {
            foreach (Particle P in particleList)
            {
                Rectangle srcRectangle = new Rectangle(0, 0, P.tex.Width, P.tex.Height);
                Vector2 origin = new Vector2(P.tex.Width / 2, P.tex.Height / 2);
                Color tempCol = new Color(P.alphaVal, P.alphaVal, P.alphaVal, P.alphaVal);

                P.prepareRecForDraw();

                spriteBatch.Draw(P.tex, P.rec, srcRectangle, tempCol,
                    P.direction, origin, SpriteEffects.None, 0);

                P.fixRecAfterDraw();
            }
        }

        //Draws all of the nodes of alpha value relative to their weight
        //Also draws enemy paths if enabled
        protected void drawNodes()
        {
            foreach (SearchNode SN in pathFinder.searchNodes)
            {
                if (nodesOn)
                {
                    float tempAlphaVAl = (float)SN.weight / (float)pathFinder.getMaxVal();
                    Color tempColor = new Color(tempAlphaVAl, tempAlphaVAl, tempAlphaVAl, 1);
                    spriteBatch.Draw(HitboxSquare, SN.rec, tempColor);
                }
            }
            //Draws the enemy ship path of nodes (colour red) over the current nodes to show pathways
            foreach (SpaceObject ES in objects)
            {
                if (ES.getObjectId() == 2 || ES.getObjectId() == 3)
                    if (ES.getPath() != null)
                    {
                        if (pathOn)
                        {
                            foreach (SearchNode SN in ES.getPath().ToList())
                            {
                                spriteBatch.Draw(HitboxSquare, SN.rec, Color.Red);
                            }
                            spriteBatch.Draw(Error, new Rectangle((int)ES.getTargetX() - 5, (int)ES.getTargetY() - 5, 10, 10), Color.Red);
                        }
                    }
            }
        }

        //Draws all status and information about the game
        protected void drawHUD()
        {
            int leftAlign = 10;

            //Hud information if the game is in session
            if (currentMode == mode.Playing)
            {
                //Score
                Color fontColour = new Color((int)148, (int)150, (int)124, (int)150);
                String tempString = "Score: " + score.ToString();
                spriteBatch.DrawString(font, tempString, new Vector2(leftAlign, 5), fontColour);

                //Health Bar
                int digits = 0;
                int tempNum = objects.ElementAt(0).getHealth();
                while (tempNum >= 10)
                {
                    tempNum /= 10;
                    digits++;
                }
                Rectangle healthbarRec = new Rectangle(leftAlign, 35, 145, 25);
                Rectangle healthbarPercentSourceRec = new Rectangle(0, 0, (int)(Health.Width * objects.ElementAt(0).getHealthPercent()), healthbarRec.Height);
                Rectangle healthbarRecPercent = new Rectangle(healthbarRec.X + 10, healthbarRec.Y, (int)(objects.ElementAt(0).getHealthPercent() * healthbarRec.Width) - 20, healthbarRec.Height);
                spriteBatch.Draw(Health, healthbarRecPercent, healthbarPercentSourceRec, Color.White);
                spriteBatch.Draw(Bar, healthbarRec, Color.White);

                if (objects.ElementAt(0).getHealth() > 0)
                {
                    tempString = objects.ElementAt(0).getHealth().ToString();
                }
                else
                {
                    tempString = "DEAD";
                }
                Vector2 textVec = font.MeasureString(tempString);
                spriteBatch.DrawString(font, tempString, new Vector2((healthbarRec.X + healthbarRec.Width / 2) - (textVec.X / 2), (healthbarRec.Y + healthbarRec.Height / 2) - (textVec.Y / 2)), fontColour);


                //Shield Bar
                Rectangle shieldLevelRec = new Rectangle(leftAlign, 64, 96, 25);
                Rectangle shieldbarPercentSourceRec = new Rectangle(0, 0, (int)(Shield.Width * objects.ElementAt(0).getShieldvalPercent()), shieldLevelRec.Height);
                Rectangle shieldLevelRecPercent = new Rectangle(shieldLevelRec.X + 7, shieldLevelRec.Y, (int)(objects.ElementAt(0).getShieldvalPercent() * shieldLevelRec.Width) - 15, shieldLevelRec.Height);
                spriteBatch.Draw(Shield, shieldLevelRecPercent, shieldbarPercentSourceRec, Color.White);
                spriteBatch.Draw(Bar, shieldLevelRec, Color.White);

                if (objects.ElementAt(0).getShieldVal() > 0)
                {
                    int tempShieldHolder = (int)objects.ElementAt(0).getShieldVal();
                    tempString = tempShieldHolder.ToString();
                }
                else
                {
                    tempString = "DANGER";
                }
                textVec = font.MeasureString(tempString);
                spriteBatch.DrawString(font, tempString, new Vector2((shieldLevelRec.X + shieldLevelRec.Width / 2) - (textVec.X / 2), (shieldLevelRec.Y + shieldLevelRec.Height / 2) - (textVec.Y / 2)), fontColour);


                //Level
                tempString = "Level: " + level.ToString();
                spriteBatch.DrawString(font, tempString, new Vector2(leftAlign, 91), fontColour);


                //Lives
                int lifeSpriteSize = 25;
                for (int i = 0; i < playerLives; i++)
                {
                    spriteBatch.Draw(PlayerShip, new Rectangle(leftAlign + ((lifeSpriteSize + 5) * i), 117, lifeSpriteSize, lifeSpriteSize), Color.White);
                }
            }
            else if (currentMode == mode.Game_Over)
            {
                //Displaying Game Over
                Color fontColour = new Color((int)148, (int)150, (int)124, (int)150);
                String tempString = "GAME OVER";
                String tempString2 = "FINAL SCORE:   " + score.ToString();
                Vector2 textVec = gameOverFont.MeasureString(tempString);
                Vector2 textVec2 = gameOverFont.MeasureString(tempString2);
                spriteBatch.DrawString(gameOverFont, tempString, new Vector2((GraphicsDevice.Viewport.Width / 2) - (textVec.X / 2), (GraphicsDevice.Viewport.Height / 2) - (textVec.Y / 2) - (textVec2.Y / 2)), fontColour);
                spriteBatch.DrawString(gameOverFont, tempString2, new Vector2((GraphicsDevice.Viewport.Width / 2) - (textVec2.X / 2), (GraphicsDevice.Viewport.Height / 2) - (textVec2.Y / 2) + (textVec.Y / 2)), fontColour);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            int width = GraphicsDevice.Viewport.Width;
            int height = GraphicsDevice.Viewport.Height;

            // spriteBatch.Begin - SamplerState.LinearWrap allows textures to repeat
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null);

            if (currentMode == mode.Playing || currentMode == mode.Game_Over)
            {
                //Draw Background
                spriteBatch.Draw(texture1, Vector2.Zero, new Rectangle((int)(0), (int)(0), width, height), Color.White);

                //Drawing Nodes
                drawNodes();

                foreach (SpaceObject SO in objects.ToList())
                {
                    bool clipX = false;
                    bool clipY = false;
                    bool clipXEvent = false;
                    bool clipYEvent = false;
                    bool clipXY = false;
                    bool stuffToDo = true;
                    float startX = SO.getX();
                    float startY = SO.getY();

                    //If object is near edge: act past it
                    while (stuffToDo)
                    {
                        stuffToDo = false;
                        SO.setX(startX);
                        SO.setY(startY);
                        if (clipX == false)
                        {
                            if (clipXY == false)
                                clipX = true;
                            //Left
                            if (SO.getX() + voidZone < SO.getRealSpriteWidth() / 2)
                            {
                                SO.setX(SO.getX() + GraphicsDevice.Viewport.Width + voidZone * 2);
                                clipXEvent = true;
                                stuffToDo = true;
                            }
                            //Right
                            else if (SO.getX() - voidZone > GraphicsDevice.Viewport.Width - (SO.getRealSpriteWidth() / 2))
                            {
                                SO.setX(SO.getX() - (GraphicsDevice.Viewport.Width + voidZone * 2));
                                clipXEvent = true;
                                stuffToDo = true;
                            }
                        }
                        if (clipY == false)
                        {
                            if (clipXY == false)
                                clipY = true;
                            //Up
                            if (SO.getY() + voidZone < SO.getRealSpriteHeight() / 2)
                            {
                                SO.setY(SO.getY() + GraphicsDevice.Viewport.Height + voidZone * 2);
                                clipYEvent = true;
                                stuffToDo = true;
                            }
                            //Down
                            else if (SO.getY() - voidZone > GraphicsDevice.Viewport.Height - (SO.getRealSpriteHeight() / 2))
                            {
                                SO.setY(SO.getY() - (GraphicsDevice.Viewport.Height + voidZone * 2));
                                clipYEvent = true;
                                stuffToDo = true;
                            }
                        }
                        if (clipXEvent == true && clipYEvent == true)
                        {
                            clipXY = true;
                            if (clipX == true && clipY == true)
                            {
                                stuffToDo = true;
                                clipX = false;
                            }
                            else if (clipX == false && clipY == true)
                            {
                                stuffToDo = true;
                                clipX = true;
                                clipY = false;
                                clipXY = false;
                                clipXEvent = false;
                                clipYEvent = false;
                            }
                        }

                        Rectangle srcRectangle = new Rectangle(0, 0, SO.getSpriteWidth(), SO.getSpriteHeight());
                        Vector2 origin = new Vector2(SO.getSpriteWidth() / 2, SO.getSpriteHeight() / 2);
                        Color tempColorSprite = new Color(SO.getAlpha(), SO.getAlpha(), SO.getAlpha(), SO.getAlpha());

                        SO.prepareRecForDraw();

                        //Enemy Status Overlay (Chase/Patrol)
                        //This must go first 
                        if (SO.getObjectId() == 2 || SO.getObjectId() == 3)
                        {
                            Texture2D tempTex;
                            if (SO.getChaseMode())
                                tempTex = chaseModeCover;
                            else
                                tempTex = patrolModeCover;

                            int inflateRate = SO.getRec().Width;
                            Rectangle tempOverlayRec = new Rectangle(SO.getRec().X - inflateRate, SO.getRec().Y - inflateRate, SO.getRec().Width + inflateRate, SO.getRec().Height + inflateRate);
                            spriteBatch.Draw(tempTex, tempOverlayRec, Color.White);
                        }


                        spriteBatch.Draw(SO.getSprite(), SO.getRec(), srcRectangle, tempColorSprite,
                            SO.getSpriteDirectionRadians(), origin, SpriteEffects.None, 0);

                        //Cracks
                        if (SO.getObjectId() == 4)
                        {
                            Color tempCol = new Color(1 - SO.getHealthPercent(), 1 - SO.getHealthPercent(), 1 - SO.getHealthPercent(), 1 - SO.getHealthPercent());
                            spriteBatch.Draw(Cracks, SO.getRec(), srcRectangle, tempCol,
                                SO.getSpriteDirectionRadians(), origin, SpriteEffects.None, 0);
                        }

                        SO.fixRecAfterDraw();

                        //IF PLAYER STUFF:
                        if (SO == objects.ElementAt(0))
                        {
                            if (SO.getShieldAlpha() > 0)
                            {
                                Color tempColor = new Color(SO.getShieldAlpha(), SO.getShieldAlpha(), SO.getShieldAlpha(), SO.getShieldAlpha());
                                spriteBatch.Draw(playerShield, SO.getRec(), tempColor);
                            }
                        }

                        //Drawing Healthbars
                        if (SO.getObjectId() == 2)
                        {
                            Rectangle tempRec = new Rectangle(SO.getRec().X, SO.getRec().Y + SO.getRec().Height + 5, SO.getRec().Width, 5);
                            spriteBatch.Draw(blank, tempRec, Color.Red);

                            tempRec.Width = (int)(SO.getRec().Width * SO.getHealthPercent());
                            spriteBatch.Draw(blank, tempRec, Color.LimeGreen);
                        }

                        //Drawing Hitboxes
                        if (hitboxOn)
                        {
                            if (SO.getCollisionDelay() > 0)
                            {
                                spriteBatch.Draw(Hitbox, SO.getRec(), Color.Red);
                                spriteBatch.Draw(HitboxSquare, SO.getRec(), Color.Red);
                            }
                            else if (SO.getCollisionClose())
                            {
                                spriteBatch.Draw(Hitbox, SO.getRec(), Color.Green);
                                spriteBatch.Draw(HitboxSquare, SO.getRec(), Color.Green);
                            }
                            else
                            {
                                spriteBatch.Draw(Hitbox, SO.getRec(), Color.White);
                                spriteBatch.Draw(HitboxSquare, SO.getRec(), Color.White);
                            }
                        }
                    }
                }


                //Drawing Particles
                DrawParticles();

                //Drawing HUD last
                drawHUD();

                if (gamespeed == 0)
                {
                    //Pause Menu
                    int alphaVal = 200;
                    Color tempCol = new Color(alphaVal, alphaVal, alphaVal, alphaVal);
                    spriteBatch.Draw(pauseMenu, new Rectangle(0, 0, width, height), tempCol);

                    //Help Menu
                    if (displayingHelpMenu)
                        spriteBatch.Draw(HelpMenu, new Rectangle(0, 0, width, height), Color.White);

                }
            }
            else if (currentMode == mode.Menu)
            {
                spriteBatch.Draw(mainMenu, new Rectangle(0, 0, width, height), Color.White);

                //Main Menu Prompt
                double alphaMultiplier = Math.Abs(Math.Sin((gameTime.TotalGameTime.TotalSeconds * 2)));
                Color fontColour = new Color((int)(255 * alphaMultiplier), (int)(251 * alphaMultiplier),
                    (int)(175 * alphaMultiplier), (int)(255 * alphaMultiplier));
                String tempString = "Press space to continue";
                Vector2 stringVec = gameOverFont.MeasureString(tempString);

                spriteBatch.DrawString(gameOverFont, tempString, new Vector2((width / 2) - (stringVec.X / 2),
                    (int)((height * 0.9) - (stringVec.Y / 2))), fontColour);

            }
            // complete the spriteBatch
            spriteBatch.End();

            base.Draw(gameTime);
        }

        //Scans keys for relivant input
        void checkKeys()
        {
            KeyboardState oldKS = ks;
            ks = Keyboard.GetState();

            if (currentMode == mode.Playing || currentMode == mode.Game_Over)
            {
                //Toggle pause
                if (oldKS.IsKeyUp(Keys.P) && ks.IsKeyDown(Keys.P))
                {
                    if (gamespeed == 0)
                    {
                        gamespeed = 1;
                    }
                    else
                    {
                        gamespeed = 0;
                    }
                }

                //Debugging stuff
                if (oldKS.IsKeyUp(Keys.End) && ks.IsKeyDown(Keys.End))
                {
                    hitboxOn = !hitboxOn;
                }
                if (oldKS.IsKeyUp(Keys.PageDown) && ks.IsKeyDown(Keys.PageDown))
                {
                    nodesOn = !nodesOn;
                }
                if (oldKS.IsKeyUp(Keys.PageUp) && ks.IsKeyDown(Keys.PageUp))
                {
                    pathOn = !pathOn;
                }

                //Pause sub-controlls
                if (gamespeed == 0)
                {
                    if (ks.IsKeyDown(Keys.H))
                    {
                        displayingHelpMenu = true;
                    }
                    else
                    {
                        displayingHelpMenu = false;
                    }
                }

                //Player Controlls
                if (currentMode == mode.Playing)
                {
                    if (respawning == false)
                    {
                        if (oldKS.IsKeyUp(Keys.Space) && ks.IsKeyDown(Keys.Space))
                        {
                            objects.ElementAt(0).fireWeapon(objects);
                            forcePushSound.Play();
                        }

                        if (ks.IsKeyDown(Keys.Up))
                        {
                            if (thrustSoundInstance.State == SoundState.Paused)
                                thrustSoundInstance.Resume();

                            if (objects.ElementAt(0).getThrust() == false)
                            {
                                objects.ElementAt(0).setThrust(true);
                                objects.ElementAt(0).setSprite(PlayerShipThrusting);
                            }
                        }
                        else
                        {
                            if (thrustSoundInstance.State == SoundState.Playing)
                                thrustSoundInstance.Pause();

                            if (objects.ElementAt(0).getThrust())
                            {
                                objects.ElementAt(0).setThrust(false);
                                objects.ElementAt(0).setSprite(PlayerShip);
                            }
                        }

                        if (ks.IsKeyDown(Keys.Left))
                        {
                            objects.ElementAt(0).setTurningLeft(true);
                        }
                        else
                            objects.ElementAt(0).setTurningLeft(false);

                        if (ks.IsKeyDown(Keys.Right))
                        {
                            objects.ElementAt(0).setTurningRight(true);
                        }
                        else
                            objects.ElementAt(0).setTurningRight(false);

                        if (ks.IsKeyDown(Keys.Down))
                        {
                            //Anything to do?
                        }
                    }
                }

                //Restart game
                if (ks.IsKeyDown(Keys.Back))
                {
                    restartGame();
                }
            }
            else if (currentMode == mode.Menu)
            {
                if (ks.IsKeyDown(Keys.Space))
                {
                    mainMenuMusicInstance.Stop();
                    bgMusic.Play();
                    goSound.Play();
                    currentMode = mode.Playing;
                }
            }

            //Constant Buttons Regardless of Mode
            if (ks.IsKeyDown(Keys.Enter))
            {
                graphics.ToggleFullScreen();
            }
            //Toggle Mute
            if (oldKS.IsKeyUp(Keys.M) && ks.IsKeyDown(Keys.M))
            {
                if (muted == true)
                {
                    SoundEffect.MasterVolume = 1;
                    muted = false;
                }
                else
                {
                    SoundEffect.MasterVolume = 0;
                    muted = true;
                }
            }
            //Exit Game
            if (oldKS.IsKeyUp(Keys.Escape) && ks.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }
        }

        //Restarts the game
        protected void restartGame()
        {
            Initialize();
        }
    }
}
