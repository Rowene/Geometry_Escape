using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Obstacle_Courses
{
    class Player
    {
        Texture2D _texture;

        Rectangle _spawn = new Rectangle(210, 410, 12,12);
        Rectangle _location;
        
        Rectangle _playerBottom;
        Rectangle _playerTop;
        Rectangle _playerLeft;
        Rectangle _playerRight;

        bool leftBorder = false;
        bool rightBorder = false;
        bool wallJump = false;
        bool wallJumpL = false;
        bool wallJumpR = false;

        Vector2 _velocity;

        bool onGround = false;
        bool onWall = false;
        bool hasJumped;

        public Player(Texture2D texture, int x, int y)
        {
            _texture = texture;
            _location = new Rectangle(x, y, 12, 12);
            hasJumped = false;
        }

        public void Update(GameTime gt, List<Rectangle> walls, List<Rectangle> spikes, List<Rectangle> yellows, List<Rectangle> teleports, Rectangle yellowSplat1, Rectangle yellowSplat2, Rectangle yellowSplat3)
        {
            _location.X += (int)_velocity.X;
            _location.Y += (int)_velocity.Y;

            onGround = false;
            onWall = false;
            

            _velocity.X = 0f;

            //wall jump
            if ((Keyboard.GetState().IsKeyDown(Keys.W) || Keyboard.GetState().IsKeyDown(Keys.Space)) && rightBorder && !wallJumpR)
            {
                _location.X -= (int)5f;
                _location.Y -= (int)10f;
                _velocity.Y = -4f;
                hasJumped = true;
                onWall = false;
                wallJumpR = true;
                wallJump = true;
            }

            if ((Keyboard.GetState().IsKeyDown(Keys.W) || Keyboard.GetState().IsKeyDown(Keys.Space)) && leftBorder && !wallJumpL)
            {
                _location.X += (int)5f;
                _location.Y -= (int)10f;
                _velocity.Y = -4f;
                hasJumped = true;
                onWall = false;
                wallJumpL = true;
                wallJump = true;
            }

            //basic controls
            if (Keyboard.GetState().IsKeyDown(Keys.D) & !rightBorder)
                _velocity.X = 2f;

            if (Keyboard.GetState().IsKeyDown(Keys.A) & !leftBorder)
                _velocity.X = -2f;

            leftBorder = false; rightBorder = false;

            if ((Keyboard.GetState().IsKeyDown(Keys.W) || Keyboard.GetState().IsKeyDown(Keys.Space)) && !hasJumped)
            {
                _location.Y -= (int)10f;
                _velocity.Y = -5f;
                hasJumped = true;
            }

            if (hasJumped)
            {
                float i = 1;
                _velocity.Y += 0.15f * i;
            }

            //player dimensions
            _playerBottom = new Rectangle(_location.X + 4, _location.Y + 13, 4, 2);
            _playerTop = new Rectangle(_location.X + 4, _location.Y - 1, 4, 2);
            _playerLeft = new Rectangle(_location.X, _location.Y + 3, 1, 6);
            _playerRight = new Rectangle(_location.X + 12, _location.Y + 3, 1, 6);

            //borders up & down
            for (int i = 0; i < walls.Count; i++)
            {
                if (_playerTop.Intersects(walls[i]))
                { 
                    if (_velocity.Y < 1) { _velocity.Y *= -1; }
                    else { _velocity.Y += 2; }
                }
                if (_playerBottom.Intersects(walls[i]))
                { hasJumped = false; onGround = true; wallJump = false; wallJumpR = false; wallJumpL = false; }
            }

            //border left & right
            for (int i = 0; i < walls.Count; i++)
            {
                if (_playerLeft.Intersects(walls[i]))
                {
                    leftBorder = true;
                    wallJumpR = false;
                    onWall = true;
                }

                if (_playerRight.Intersects(walls[i]))
                {
                    rightBorder = true;
                    wallJumpL = false;
                    onWall = true;
                }
            }

            //deaths
            for (int i = 0; i < spikes.Count; i++)
            {
                if (spikes[i].Intersects(_location)) { _location = _spawn; }
            }
            for (int i = 0; i < yellows.Count; i++)
            {
                if (yellows[i].Intersects(_location)) { _location = _spawn; }
            }

            if (yellowSplat1.Intersects(_location)) { _location = _spawn;}
            if (yellowSplat2.Intersects(_location)) { _location = _spawn; }
            if (yellowSplat3.Intersects(_location)) { _location = _spawn; }

            //teleports
            for (int i = 0; i < teleports.Count; i += 2)
            {
                if (_location.Intersects(teleports[i])) { _location.X = teleports[i + 1].X + teleports[i+1].Width/2; _location.Y = teleports[i+1].Y + teleports[i+1].Height/2; }
            }




                    if (onWall & !onGround) { _velocity.Y = 1f; }

            if (!hasJumped) _velocity.Y = 0f;

            if (!onGround && !hasJumped)
            {
                hasJumped = true;
                _location.Y += 2;
            }

            if (_velocity.Y > 5f) { _velocity.Y = 5f; }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _location, Color.White);
        }

    }
}
