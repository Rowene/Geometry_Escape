using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;

namespace Obstacle_Courses
{
    public enum Screen { Intro, Game, HowToPlay, Won }

    public class Game1 : Game
    {
        public GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;

        int tutorialFrame = 0;

        public Screen screen;

        Rectangle mouseLocation;

        Texture2D GeometryIntroTexture;
        Texture2D ClickHereTexture;
        Texture2D IntroArt;
        Texture2D controlsButton;
        Texture2D playAgainTexture;

        SoundEffect dyingSound;
        SoundEffect gameMusic;
        SoundEffectInstance gameMusicInst;

        List<Rectangle> borders = new List<Rectangle>();
        List<Rectangle> spikes = new List<Rectangle>();
        List<Rectangle> yellows = new List<Rectangle>();
        List<Rectangle> teleports = new List<Rectangle>();

        Rectangle yellowSplat1 = new Rectangle(505, 300, 15, 15);
        int speedYS1 = 3;

        Rectangle yellowSplat2 = new Rectangle(245, 115, 15, 15);
        int speedYS2 = 3;

        Rectangle yellowSplat3 = new Rectangle(555, 170, 15, 15);
        int speedYS3 = 3;

        Texture2D yellowSplatTexture;
        Texture2D borderTexture;

        Texture2D spikeTexture;
        Texture2D spikeTextureFlip;

        Texture2D yellowTexture;
        Texture2D teleportTexture;

        Texture2D flagTexture;

        Player player;
        Texture2D playerTexture;

        SpriteFont tutorialText;
        SpriteFont wonText;

        List<Texture2D> frameTextures = new List<Texture2D>();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            this.Window.Title = "Geometry Escape";

            _graphics.PreferredBackBufferHeight = 540;
            _graphics.PreferredBackBufferWidth = 960;
            _graphics.ApplyChanges();

            //Borders
            borders.Add(new Rectangle(180, 105, 595, 5));
            borders.Add(new Rectangle(180, 105, 5, 335));
            borders.Add(new Rectangle(185, 435, 595, 5));
            borders.Add(new Rectangle(775, 105, 5, 335));

            //walls
            borders.Add(new Rectangle(250, 330, 5, 105));
            borders.Add(new Rectangle(250, 330, 120, 5));
            borders.Add(new Rectangle(185, 275, 540, 5));
            borders.Add(new Rectangle(405, 275, 5, 160));
            borders.Add(new Rectangle(265, 375, 80, 5));
            borders.Add(new Rectangle(345, 375, 5, 25));
            borders.Add(new Rectangle(720, 110, 5, 200));
            borders.Add(new Rectangle(465, 330, 30, 5));
            borders.Add(new Rectangle(530, 345, 90, 5));
            borders.Add(new Rectangle(710, 350, 20, 5));
            borders.Add(new Rectangle(675, 370, 20, 5));
            borders.Add(new Rectangle(185, 160, 50, 5));
            borders.Add(new Rectangle(270, 110, 5, 125));
            borders.Add(new Rectangle(310, 140, 5, 30));
            borders.Add(new Rectangle(310, 140, 40, 5));
            borders.Add(new Rectangle(350, 140, 5, 135));
            borders.Add(new Rectangle(370, 110, 5, 120));
            borders.Add(new Rectangle(400, 135, 5, 140));
            borders.Add(new Rectangle(465, 200, 20, 5));
            borders.Add(new Rectangle(635, 260, 20, 5));
            borders.Add(new Rectangle(640, 165, 30, 5));

            //spikes
            spikes.Add(new Rectangle(275, 320, 10, 10));
            spikes.Add(new Rectangle(575, 335, 10, 10));
            spikes.Add(new Rectangle(360, 265, 10, 10));

            //spikes flipped
            spikes.Add(new Rectangle(565, 280, 10, 10));

            //yellows
            yellows.Add(new Rectangle(255, 430, 150, 5));
            yellows.Add(new Rectangle(475, 430, 300, 5));
            yellows.Add(new Rectangle(265, 110, 5, 125));
            yellows.Add(new Rectangle(275, 150, 5, 15));
            yellows.Add(new Rectangle(405, 270, 315, 5));
            yellows.Add(new Rectangle(420, 150, 40, 5));

            //teleporter
            teleports.Add(new Rectangle(275, 345, 5, 30));
            teleports.Add(new Rectangle(410, 400, 5, 30));
            teleports.Add(new Rectangle(735, 110, 30, 5));
            teleports.Add(new Rectangle(185, 115, 5, 30));

            base.Initialize();
            player = new Player(playerTexture, 210, 410, dyingSound);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            GeometryIntroTexture = Content.Load<Texture2D>("GeometryEscape");
            ClickHereTexture = Content.Load<Texture2D>("clickhere");
            IntroArt = Content.Load<Texture2D>("IntroArt");
            controlsButton = Content.Load<Texture2D>("howtoplaybutton");

            tutorialText = Content.Load<SpriteFont>("tutorialText");
            wonText = Content.Load<SpriteFont>("winText");
            playAgainTexture = Content.Load<Texture2D>("playAgain");

            dyingSound = Content.Load<SoundEffect>("dyingSound");
            gameMusic = Content.Load<SoundEffect>("gameMusic");

            gameMusicInst = gameMusic.CreateInstance();

            playerTexture = Content.Load<Texture2D>("player");
            borderTexture = Content.Load<Texture2D>("black");

            spikeTexture = Content.Load<Texture2D>("Triangle");
            spikeTextureFlip = Content.Load<Texture2D>("Triangleflip");

            yellowTexture = Content.Load<Texture2D>("yellow");
            teleportTexture = Content.Load<Texture2D>("blue");
            yellowSplatTexture = Content.Load<Texture2D>("yellowsplat");

            flagTexture = Content.Load<Texture2D>("Flag");

            for (int i = 0; i < 100; i++)
            {
                frameTextures.Add(Content.Load<Texture2D>("" + i));
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) & screen == Screen.Game){ screen = Screen.Intro; }

            var MouseState = Mouse.GetState();
            mouseLocation.X = MouseState.X;
            mouseLocation.Y = MouseState.Y;

            if (screen == Screen.Intro) 
            {
                gameMusicInst.IsLooped = true;
                gameMusicInst.Play();

                if (MouseState.LeftButton == ButtonState.Pressed && mouseLocation.Intersects(new Rectangle(275, 375, 410, 52)))
                {
                    screen = Screen.Game; 
                    
                }
                if (MouseState.LeftButton == ButtonState.Pressed && mouseLocation.Intersects(new Rectangle(10, 10, 235, 52))) { screen = Screen.HowToPlay; }
            }
            else if (screen == Screen.Game)
            {
                if (yellowSplat1.Intersects(borders[2]) || yellowSplat1.Intersects(borders[6])) { speedYS1 *= -1; }
                yellowSplat1.Y += speedYS1;

                if (yellowSplat2.Intersects(borders[0]) || yellowSplat2.Intersects(borders[6])) { speedYS2 *= -1; }
                yellowSplat2.Y += speedYS2;

                if (yellowSplat3.Intersects(borders[0]) || yellowSplat3.Intersects(borders[6])) { speedYS3 *= -1; }
                yellowSplat3.Y += speedYS3;

                screen = player.Update(gameTime, borders, spikes, yellows, teleports, yellowSplat1, yellowSplat2, yellowSplat3);
            }
            else if (screen == Screen.Won)
            {
                if (MouseState.LeftButton == ButtonState.Pressed && mouseLocation.Intersects(new Rectangle(250, 350, 410, 52)))
                {
                    screen = Screen.Game;
                }     
            }
            else if (screen == Screen.HowToPlay)
            {
                tutorialFrame++;
                if (tutorialFrame >= 100)
                    tutorialFrame = 0;
                if (MouseState.LeftButton == ButtonState.Pressed && !mouseLocation.Intersects(new Rectangle(10, 10, 235, 52)))
                {
                    screen = Screen.Intro;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DimGray);
            _spriteBatch.Begin();

            if (screen == Screen.Game)
            {
                player.Draw(_spriteBatch);

                for (int i = 0; i < borders.Count; i++)
                    _spriteBatch.Draw(borderTexture, borders[i], Color.Gray);

                for (int i = 0; i < spikes.Count - 1; i++)
                    _spriteBatch.Draw(spikeTexture, spikes[i], Color.White);

                _spriteBatch.Draw(spikeTextureFlip, spikes[3], Color.White);

                for (int i = 0; i < yellows.Count; i++)
                    _spriteBatch.Draw(yellowTexture, yellows[i], Color.White);

                for (int i = 0; i < teleports.Count; i++)
                    _spriteBatch.Draw(teleportTexture, teleports[i], Color.White);

                _spriteBatch.Draw(yellowSplatTexture, yellowSplat1, Color.White);
                _spriteBatch.Draw(yellowSplatTexture, yellowSplat2, Color.White);
                _spriteBatch.Draw(yellowSplatTexture, yellowSplat3, Color.White);

                _spriteBatch.Draw(flagTexture, new Rectangle(640, 140, 25, 25),Color.White);
            }
            else if (screen == Screen.Intro)
            {
                _spriteBatch.Draw(controlsButton, new Rectangle(10,10, 235,52), Color.White);
                _spriteBatch.Draw(GeometryIntroTexture, new Rectangle(200, 110 ,560, 93), Color.White);
                _spriteBatch.Draw(ClickHereTexture, new Rectangle(275, 375, 410, 52), Color.White);
                _spriteBatch.Draw(IntroArt, new Rectangle(250, 0, 460, 460), Color.White);
            }
            else if (screen == Screen.HowToPlay)
            {
                _spriteBatch.Draw(frameTextures[tutorialFrame], new Rectangle(0,0,960,540), Color.White);
                _spriteBatch.DrawString(tutorialText, "Click Anywhere to go back." ,new Microsoft.Xna.Framework.Vector2(450, 250), Color.White);
            }
            else
            {
                _spriteBatch.Draw(playAgainTexture, new Rectangle(250, 350, 410, 52), Color.Black);
                _spriteBatch.DrawString(wonText, "You Won! That's Very impressive.", new Microsoft.Xna.Framework.Vector2(175, 100), Color.White);
            }

                _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}