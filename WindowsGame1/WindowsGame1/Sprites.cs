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
    /// Provides a sprite on the screen
    /// 
    /// </summary>
    class Sprite
    {

        protected int screenWidth;
        protected int screenHeight;

        protected Texture2D texture;
        protected Rectangle rectangle;

        protected float xPosition;
        protected float yPosition;

        protected float xResetPosition;
        protected float yResetPosition;

        public Sprite(int inScreenWidth, int inScreenHeight, Texture2D inSpriteTexture, int inDrawWidth, float inResetX, float inResetY)
        {

            screenWidth = inScreenWidth;
            screenHeight = inScreenHeight;
            texture = inSpriteTexture;
            xResetPosition = inResetX;
            yResetPosition = inResetY;

            float aspect = inSpriteTexture.Width / inSpriteTexture.Height;
            int height = (int)Math.Round(inDrawWidth * aspect);
            rectangle = new Rectangle(0, 0, inDrawWidth, height);

            Reset();
        }

        public void SetPosition(float x, float y)
        {
            xPosition = (int)Math.Round(x);
            yPosition = (int)Math.Round(y);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            rectangle.X = (int)Math.Round(xPosition);
            rectangle.Y = (int)Math.Round(yPosition);
            spriteBatch.Draw(texture, rectangle, Color.White);
        }

        public virtual void Update(float deltaTime)
        {

        }

        public void SetResetPosition(float x, float y)
        {
            xResetPosition = x;
            yResetPosition = y;
        }

        public virtual void Reset()
        {
            SetPosition(xResetPosition, yResetPosition);
        }

        public Vector2 GetCentre()
        {
            float x = xPosition + rectangle.Width / 2;
            float y = yPosition + rectangle.Height / 2;
            return new Vector2(x, y);
        }

        public float GetDistanceFrom(Sprite s)
        {
            Vector2 v1 = s.GetCentre();
            Vector2 v2 = GetCentre();
            float dx = v1.X - v2.X;
            float dy = v1.Y - v2.Y;
            return (float)Math.Sqrt((dx * dx) + (dy * dy));
        }

        public bool IntersectsWith(Sprite s)
        {
            return rectangle.Intersects(s.rectangle);
        }
    }

    class Mover : Sprite
    {
        public void StartMovingUp()
        {
            MovingUp = true;
        }
        public void StopMovingUp()
        {
            MovingUp = false;
        }

        public void StartMovingDown()
        {
            MovingDown = true;
        }
        public void StopMovingDown()
        {
            MovingDown = false;
        }

        public void StartMovingLeft()
        {
            MovingLeft = true;
        }
        public void StopMovingLeft()
        {
            MovingLeft = false;
        }

        public void StartMovingRight()
        {
            MovingRight = true;
        }
        public void StopMovingRight()
        {
            MovingRight = false;
        }


        protected bool MovingUp;
        protected bool MovingDown;
        protected bool MovingLeft;
        protected bool MovingRight;

        protected float resetXSpeed;
        protected float resetYSpeed;

        protected float xSpeed;
        protected float ySpeed;

        public Mover(int inScreenWidth, int inScreenHeight, Texture2D inSpriteTexture, int inDrawWidth, float inResetX, float inResetY, float inResetXSpeed, float inResetYSpeed) :
            base(inScreenWidth, inScreenHeight, inSpriteTexture, inDrawWidth, inResetX, inResetY)
        {
            resetXSpeed = inResetXSpeed;
            resetYSpeed = inResetYSpeed;
            Reset();
        }

        public override void Reset()
        {
            MovingDown = false;
            MovingUp = false;
            MovingLeft = false;
            MovingRight = false;
            SetSpeed(resetXSpeed, resetYSpeed);
            base.Reset();
        }

        public void SetSpeed(float inXSpeed, float inYSpeed)
        {
            xSpeed = inXSpeed;
            ySpeed = inYSpeed;
        }

        public override void Update(float deltaTime)
        {
            if (MovingLeft) xPosition = xPosition - (xSpeed * deltaTime);
            if (MovingRight) xPosition = xPosition + (xSpeed * deltaTime);
            if (MovingUp) yPosition = yPosition - (ySpeed * deltaTime);
            if (MovingDown) yPosition = yPosition + (ySpeed * deltaTime);

            if (xPosition < 0) xPosition = 0;
            if (xPosition + rectangle.Width > screenWidth) xPosition = screenWidth - rectangle.Width;

            if (yPosition < 0) yPosition = 0;
            if (yPosition + rectangle.Height > screenHeight) yPosition = screenHeight - rectangle.Height;

            base.Update(deltaTime);
        }

    }

    class PlayerMover : Mover
    {
        GamePadState padState;
        PlayerIndex playerNumber;

        bool isHit = false;
        Texture2D standardTexture;
        Texture2D altTexture;

        int resetXPos;
        int resetYPos;

        float RotationAngle;
        private Vector2 origin;
        private Vector2 screenpos;

        int score;

        public PlayerMover(int inScreenWidth, int inScreenHeight, Texture2D inSpriteTexture, Texture2D inAltSpriteTexture , int inDrawWidth, float inResetX, float inResetY, float inResetXSpeed, float inResetYSpeed, PlayerIndex inPlayerIndex) :
            base(inScreenWidth, inScreenHeight, inSpriteTexture, inDrawWidth, inResetX, inResetY, inResetXSpeed, inResetYSpeed)
        {
            playerNumber = inPlayerIndex;
            
            origin.X = inSpriteTexture.Width / 2;
            origin.Y = inSpriteTexture.Height / 2;
            resetXPos = (int)inResetX;
            resetYPos = (int)inResetY;

            standardTexture = inSpriteTexture;
            altTexture = inAltSpriteTexture;

            resetXSpeed = inResetXSpeed;
            resetYSpeed = inResetYSpeed;
            Reset();
        }

        public int getScore()
        {
            return score;
        }

        public override void Reset()
        {
            score = 100;
            screenpos.X = resetXPos;
            screenpos.Y = resetYPos;
            base.Reset();
        }

        public float getXPosition()
        {
            return screenpos.X;
        }

        public float getYPosition()
        {
            return screenpos.Y;
        }

        public void Damaged()
        {
            isHit = true;
            score-=4;
        }

        public override void Update(float deltaTime)
        {
            padState = GamePad.GetState(playerNumber);

            screenpos.X += padState.ThumbSticks.Left.X * xSpeed;
            screenpos.Y -= padState.ThumbSticks.Left.Y * ySpeed;

            if (screenpos.X < 0 + (rectangle.Width / 2)) screenpos.X = 0 + (rectangle.Width / 2);
            if (screenpos.X + (rectangle.Width / 2) > screenWidth) screenpos.X = screenWidth - (rectangle.Width / 2);

            if (screenpos.Y < 58 + (rectangle.Height / 2)) screenpos.Y = 58 + (rectangle.Height / 2);
            if (screenpos.Y + (rectangle.Height / 2) > screenHeight) screenpos.Y = screenHeight - (rectangle.Height / 2);


            RotationAngle += deltaTime * 5;
            float circle = MathHelper.Pi * 2;
            RotationAngle = RotationAngle % circle;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            rectangle.X = (int)Math.Round(screenpos.X);
            rectangle.Y = (int)Math.Round(screenpos.Y);
            if (!isHit)
            {
                spriteBatch.Draw(standardTexture, screenpos, null, Color.White, RotationAngle, origin, 1.0f, SpriteEffects.None, 0f);
                GamePad.SetVibration(playerNumber, 0.0f, 0.0f);
            }
            else
            {
                spriteBatch.Draw(altTexture, screenpos, null, Color.White, RotationAngle, origin, 1.0f, SpriteEffects.None, 0f);
                GamePad.SetVibration(playerNumber, 1.0f, 1.0f);
                isHit = false;
            }
        }
    }

    class PhysicsMover : Mover
    {
        protected float xAcceleration;
        protected float yAcceleration;

        protected float resetXAcceleration;
        protected float resetYAcceleration;

        protected float friction;
        protected float resetFriction;



        public override void Reset()
        {
            xAcceleration = resetXAcceleration;
            yAcceleration = resetYAcceleration;
            friction = resetFriction;
            base.Reset();
        }

        public float getXPosition()
        {
            return xPosition + 48;
        }

        public float getYPosition()
        {
            return yPosition + 48;
        }

        public PhysicsMover(int inScreenWidth, int inScreenHeight, Texture2D inSpriteTexture, int inDrawWidth, float inResetX, float inResetY, float inResetXSpeed, float inResetYSpeed, float inResetXAccel, float inResetYAccel, float inResetFriction) :
            base(inScreenWidth, inScreenHeight, inSpriteTexture, inDrawWidth, inResetX, inResetY, inResetXSpeed, inResetYSpeed)
        {
            resetXAcceleration = inResetXAccel;
            resetYAcceleration = inResetYAccel;
            resetFriction = inResetFriction;
            Reset();
        }

        public void SetAcceleration(int inX, int inY)
        {
            xAcceleration = inX;
            yAcceleration = inY;
        }

        public override void Update(float deltaTime)
        {
            if (MovingLeft) xSpeed = xSpeed - (xAcceleration * deltaTime);
            if (MovingRight) xSpeed = xSpeed + (xAcceleration * deltaTime);
            if (MovingUp) ySpeed = ySpeed - (yAcceleration * deltaTime);
            if (MovingDown) ySpeed = ySpeed + (yAcceleration * deltaTime);

            xPosition = xPosition + (xSpeed * deltaTime);
            yPosition = yPosition + (ySpeed * deltaTime);

            if (xPosition < 0) xSpeed = Math.Abs(xSpeed);
            if (xPosition + rectangle.Width > screenWidth) xSpeed = Math.Abs(xSpeed) * -1;

            if (yPosition < 60) ySpeed = Math.Abs(ySpeed);
            if (yPosition + rectangle.Height > screenHeight) ySpeed = Math.Abs(ySpeed) * -1;

            xSpeed = xSpeed * (1 - deltaTime * friction);
            ySpeed = ySpeed * (1 - deltaTime * friction);

            if (Math.Abs(xSpeed) < 0.01) xSpeed = 0;
            if (Math.Abs(ySpeed) < 0.01) ySpeed = 0;
        }
    }
}
