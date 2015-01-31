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

namespace WindowsGame1
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    /// 
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        // Game World
        // These variables define the world 
        PlayerMover [] players = new PlayerMover[2];

        PhysicsMover enemy;

        int screenWidth;
        int screenHeight;

        Sprite background;
        Sprite topBar;

        List<Sprite> gameSprites = new List<Sprite>();

        SpriteFont messageFont;

        string messageString = "Welcome to TopGull 2.0";

        int timer;

        enum GameStates
        {
            Start_Screen,
            Playing_Game
        }

        GameStates state;

        void startPlayingGame()
        {
            foreach (Sprite s in gameSprites)
                s.Reset();

            foreach (PlayerMover p in players)
                p.Reset();

            messageString = "Cracker Chase";

            timer = 2400;

            state = GameStates.Playing_Game;
        }

        void gameOver(string winner)
        {
            messageString = string.Format("{0} won the game", winner);
            state = GameStates.Start_Screen;
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 1080;
            graphics.PreferredBackBufferWidth = 1920;

            graphics.IsFullScreen = true;

            Content.RootDirectory = "Content";
        }

        #region Loading / Unloading and init
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
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

            messageFont = Content.Load<SpriteFont>("MessageFont");

            screenWidth = GraphicsDevice.Viewport.Width;
            screenHeight = GraphicsDevice.Viewport.Height;

            //Load textures here
            Texture2D playerBlue = Content.Load<Texture2D>("Textures/Players/PlayerBlue");
            Texture2D playerYellow = Content.Load<Texture2D>("Textures/Players/PlayerYellow");
            Texture2D playerRed = Content.Load<Texture2D>("Textures/Players/PlayerRed");

            Texture2D topBarTexture = Content.Load<Texture2D>("Textures/TopBar");
            Texture2D backgroundTexture = Content.Load<Texture2D>("Textures/Background");
            Texture2D enemyTexture = Content.Load<Texture2D>("Textures/Enemy");

            background = new Sprite(screenWidth, screenHeight, backgroundTexture, screenWidth, 0, 0);
            gameSprites.Add(background);

            topBar = new Sprite(screenWidth, screenHeight, topBarTexture, screenWidth, 0, 0);
            gameSprites.Add(topBar);

            int playerWidth = screenWidth / 30;
            int enemyWidth = screenWidth / 20;

            //add player one
            players[0] = new PlayerMover(screenWidth, screenHeight, playerBlue, playerRed, playerWidth, (screenWidth / 4), screenHeight / 2, 7, 7, PlayerIndex.One);
            gameSprites.Add(players[0]);
            //add player two
            players[1] = new PlayerMover(screenWidth, screenHeight, playerYellow, playerRed, playerWidth, (screenWidth / 4)*3, screenHeight / 2, 16, 16, PlayerIndex.Two);
            gameSprites.Add(players[1]);

            //Add the enemys
            enemy = new PhysicsMover(screenWidth, screenHeight, enemyTexture, enemyWidth, (screenWidth / 2) - (enemyWidth / 2), screenHeight / 2, 0, 0, 2000, 2000, 1f);
            gameSprites.Add(enemy);
            // go to the start screen state

           // gameOver();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        #endregion

        #region Update Methods
        void updateGamePlay(GameTime gameTime)
        {
            foreach (Sprite s in gameSprites)
                s.Update(1.0f / 60.0f);

            timer = timer - 1;

            int secsLeft = timer / 60;
            messageString = string.Format("Player 1: {0,-20}Time: {1,-20}Player 2: {2}", players[0].getScore(), secsLeft, players[1].getScore());

            if (timer == 0 || players[0].getScore() <= 0 || players[1].getScore() <= 0)
            {
                if (players[0].getScore() > players[1].getScore())
                    gameOver("Player 1");
                else if (players[0].getScore() < players[1].getScore())
                    gameOver("Player 2");
                else
                    gameOver("Everyone");
            }

            float distPlayer1;
            float distPlayer2;
            int closestPlayer = 0;

            distPlayer1 = players[0].GetDistanceFrom(enemy);
            distPlayer2 = players[1].GetDistanceFrom(enemy);

            if (distPlayer1 < distPlayer2)
                closestPlayer = 0;
            else 
                closestPlayer = 1;

            //move on the horizontal axis
            if (enemy.getXPosition() >= players[closestPlayer].getXPosition())
            {
                enemy.StopMovingRight();
                enemy.StartMovingLeft();
            }
            else
            {
                enemy.StopMovingLeft();
                enemy.StartMovingRight();
            }
            //move on the vertical axis
            if (enemy.getYPosition() >= players[closestPlayer].getYPosition())
            {
                enemy.StopMovingDown();
                enemy.StartMovingUp();
            }
            else
            {
                enemy.StopMovingUp();
                enemy.StartMovingDown();
            }

            foreach(PlayerMover p in players)
            {
                if (p.IntersectsWith(enemy))
                    p.Damaged();
            }
        }

        void updateStartScreen(GameTime gameTime)
        {
            GamePadState pad1State = GamePad.GetState(PlayerIndex.One);
            GamePadState pad2State = GamePad.GetState(PlayerIndex.Two);

            if (pad1State.Buttons.Start == ButtonState.Pressed || pad2State.Buttons.Start == ButtonState.Pressed)
                startPlayingGame();
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            GamePadState pad1State = GamePad.GetState(PlayerIndex.One);
            GamePadState pad2State = GamePad.GetState(PlayerIndex.Two);

            if (pad1State.Buttons.Back == ButtonState.Pressed || pad2State.Buttons.Back == ButtonState.Pressed)
                Exit();

            switch (state)
            {
                case GameStates.Start_Screen:
                    updateStartScreen(gameTime);
                    break;

                case GameStates.Playing_Game:
                    updateGamePlay(gameTime);
                    break;

            }
            base.Update(gameTime);
        }
        #endregion

        #region Draw Methods
        void drawStartScreen()
        {
            spriteBatch.Begin();

            foreach (Sprite s in gameSprites)
                s.Draw(spriteBatch);

            float xPos = (screenWidth - messageFont.MeasureString(messageString).X) / 2;

            Vector2 statusPos = new Vector2(xPos, (screenHeight / 2) - 64);

            spriteBatch.DrawString(messageFont, messageString, statusPos, Color.White);

            spriteBatch.End();
        }

        void drawGamePlay()
        {
            spriteBatch.Begin();

            foreach (Sprite s in gameSprites)
                s.Draw(spriteBatch);

            float xPos = (screenWidth - messageFont.MeasureString(messageString).X) / 2;

            Vector2 statusPos = new Vector2(xPos, 7);

            spriteBatch.DrawString(messageFont, messageString, statusPos, Color.Black);

            spriteBatch.End();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            switch (state)
            {
                case GameStates.Start_Screen:
                    drawStartScreen();
                    break;

                case GameStates.Playing_Game:
                    drawGamePlay();
                    break;

            }


            base.Draw(gameTime);
        }
        #endregion
    }
}