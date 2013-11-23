using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Mage
{
    class Mage
    {
        #region Variables/Members
        //Variables/members needed for this class
        Texture2D mageTexture;
 
        public Point frameSize;
        Point currentFrame;
        Point sheetSize;

        public int collisionOffset;
        public int mageHealth;
        public int lastDamage;
        public int lastHealth;
        public bool showHealth;
        int millisecondsPerFrame;
        int timeSinceLastFrame = 0;

        float speed;
        float sprintTimer;
        float cooldownTimer;
        float timer;
        public Vector2 position;
        public Rectangle rectangle;
        #endregion

        #region Constructor
        /// <summary>
        /// Main constructor for creating a "mage" player/object.
        /// </summary>
        /// <param name="pTexture">Texture for the Mage.</param>
        /// <param name="pPosition">Positiong to initially draw the Mage.</param>
        /// <param name="pFrameSize">Size of each frame in the sprite sheet.</param>
        /// <param name="pCollisionOffset">Offset for more accurate collision detection.</param>
        /// <param name="pCurrentFrame">The current frame used fr animation.</param>
        /// <param name="pSheetSize">The total size of the sprite sheet in columns/rows.</param>
        /// <param name="pSpeed">Speed the Mage moves at.</param>
        /// <param name="pMillisecondsPerFrame">The milliseconds delay in each frame.</param>
        public Mage(Texture2D pTexture, Vector2 pPosition, Point pFrameSize, int pCollisionOffset, Point pCurrentFrame, Point pSheetSize, float pSpeed, int pMillisecondsPerFrame, int pMageHealth)
        {
            //Instantiate our members using the passed values
            this.mageTexture = pTexture;
            this.position = pPosition;
            this.frameSize = pFrameSize;
            this.collisionOffset = pCollisionOffset;
            this.currentFrame = pCurrentFrame;
            this.sheetSize = pSheetSize;
            this.speed = pSpeed;
            this.millisecondsPerFrame = pMillisecondsPerFrame;
            this.rectangle = new Rectangle((int)pPosition.X, (int)pPosition.Y, (int)pTexture.Width, (int)pTexture.Height / 6 + 10);
            this.mageHealth = pMageHealth;
        }
        #endregion

        #region Update
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="clientBounds">Used to detect when objects leave the game screen.</param>
        public void Update(GameTime gameTime, Rectangle clientBounds)
        {
            //Check our keyboard state and act accordingly            
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.A))
            {
                //Let's go left!
                this.Animate("Left", gameTime);
                position.X -= speed;
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                //Let's go right!
                this.Animate("Right", gameTime);
                position.X += speed;
            }
            if (keyboardState.IsKeyDown(Keys.W))
            {
                //Let's go up!
                this.Animate("Up", gameTime);
                position.Y -= speed;
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                //Let's go down!
                this.Animate("Down", gameTime);
                position.Y += speed;
            }
            if (keyboardState.IsKeyDown(Keys.W) && keyboardState.IsKeyDown(Keys.A))
            {
                //Seeing as 'W' is already down, thus moving our Mage, let's just change the anim for left diagonal vertical movements!
                this.Animate("UpLeft", gameTime);
            }
            if (keyboardState.IsKeyDown(Keys.W) && keyboardState.IsKeyDown(Keys.D))
            {
                //Seeing as 'W' is already down, thus moving our Mage, let's just change the anim for right diagonal vertical movements!
                this.Animate("UpRight", gameTime);
            }

            //Sprint function - For when the zombies get too 'cray!
            if (keyboardState.IsKeyDown(Keys.LeftShift))
            {
                //Add to our sprint timer
                sprintTimer += gameTime.ElapsedGameTime.Milliseconds;

                //Check how long we have been sprinting
                if (sprintTimer <= 6000)
                {
                    //Run like the wind bullseye!
                    speed = 2.0f;
                }
                else if (sprintTimer >= 6000)
                {
                    //No moar speedz :(
                    speed = 1.0f;

                    //Add to the cooldown timer
                    cooldownTimer += gameTime.ElapsedGameTime.Milliseconds;

                    //Have we cooled down?
                    if (cooldownTimer >= 2000)
                    {
                        //Reset both timers
                        cooldownTimer = 0.0f;
                        sprintTimer = 0.0f;
                    }
                } 
            }

            
            //Stop sprinting check
            if (keyboardState.IsKeyUp(Keys.LeftShift))
            {
                //Back to normal speed
                speed = 1.0f;
            }


            //Bounds checking
            if (position.Y < 0)
            {
                //Top of screen
                position.Y = 0;
            }
            if (position.X < 0)
            {
                //Left side of screen
                position.X = 0;
            }
            if (position.X > clientBounds.Width - frameSize.X)
            {
                //Right side of screen
                position.X = clientBounds.Width - frameSize.X;
            }
            if (position.Y > clientBounds.Height - frameSize.Y)
            {
                //Bottom of screen
                position.Y = clientBounds.Height - frameSize.Y;
            }

            //Update the Mage's rectangle
            this.rectangle.X = (int)this.position.X - 10;
            this.rectangle.Y = (int)this.position.Y - 5;

        }
        #endregion

        #region Draw
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="spriteBatch">Needed to draw our textures in this class.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            //Draw our mage and his health with the last damage value showing
            spriteBatch.Draw(mageTexture, position, new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(spriteFont, "mage", new Vector2(position.X + 1, position.Y - 24), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            spriteBatch.DrawString(spriteFont, "mage", new Vector2(position.X + 1, position.Y - 25), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            spriteBatch.DrawString(spriteFont, mageHealth.ToString(), new Vector2(position.X + 4, position.Y - 12), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            spriteBatch.DrawString(spriteFont, mageHealth.ToString(), new Vector2(position.X + 4, position.Y - 13), Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 1);

            //Damage to show?
            if (this.lastDamage != 0)
            {
                //Add to our timer
                timer += gameTime.ElapsedGameTime.Milliseconds;

                //Draw the damage dealt
                spriteBatch.DrawString(spriteFont, "-" + this.lastDamage.ToString(), new Vector2(position.X + 4, position.Y - 34), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                spriteBatch.DrawString(spriteFont, "-" + this.lastDamage.ToString(), new Vector2(position.X + 4, position.Y - 35), Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 1);

                //Damage shown for 2 secs?
                if (timer >= 2000)
                {
                    //Reset damage and timer
                    this.lastDamage = 0;
                    timer = 0.0f;
                }
            }

            //Health gains to show?
            if (this.lastHealth != 0)
            {
                //Add to our timer
                timer += gameTime.ElapsedGameTime.Milliseconds;

                //Draw the damage dealt
                spriteBatch.DrawString(spriteFont, "+" + this.lastHealth.ToString(), new Vector2(position.X + 4, position.Y - 34), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                spriteBatch.DrawString(spriteFont, "+" + this.lastHealth.ToString(), new Vector2(position.X + 4, position.Y - 35), Color.LightGreen, 0, Vector2.Zero, 1, SpriteEffects.None, 1);

                //Damage shown for 2 secs?
                if (timer >= 2000)
                {
                    //Reset damage and timer
                    this.lastHealth = 0;
                    timer = 0.0f;
                }
            }
        }
        #endregion

        #region Animate
        /// <summary>
        /// Animation method.
        /// </summary>
        /// <param name="pDirection">Direction in which we want to animate.</param>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Animate(string pDirection, GameTime gameTime)
        {
            //Check which direction we want to animate
            if (pDirection == "Up")
            {
                //Set the current frame to match our sprite sheets up anim
                currentFrame.Y = 3;
            }
            else if (pDirection == "Down")
            {
                //Set the current frame to match our sprite sheets down anim
                currentFrame.Y = 2;
            }
            else if (pDirection == "Left")
            {
                //Set the current frame to match our sprite sheets left anim
                currentFrame.Y = 0;
            }
            else if (pDirection == "Right")
            {
                //Set the current frame to match our sprite sheets right anim
                currentFrame.Y = 1;
            }
            else if (pDirection == "UpLeft")
            {
                //Set the current frame to match our sprite sheets up left anim
                currentFrame.Y = 4;
            }
            else if (pDirection == "UpRight")
            {
                //Set the current frame to match our sprite sheets up right anim
                currentFrame.Y = 5;
            }

            //Add the elapsed milliseconds to the time since the last frame
            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            
            //Time to animate?
            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                //Reduce the time since last frame
                timeSinceLastFrame -= millisecondsPerFrame;

                //Increment the X of currentFrame
                ++currentFrame.X;

                //Are we off/on the edge?!
                if (currentFrame.X >= sheetSize.X)
                {
                    //Yes. Set the X to 0
                    currentFrame.X = 0;
                }
            }
        }
        #endregion

        #region DrawBorder
        /// <summary>
        /// Will draw a border (hollow rectangle) of the given 'thicknessOfBorder' (in pixels)
        /// of the specified color.
        ///
        /// By Sean Colombo, from http://bluelinegamestudios.com/blog
        /// </summary>
        /// <param name="rectangleToDraw"></param>
        /// <param name="thicknessOfBorder"></param>
        public void DrawBorder(SpriteBatch spriteBatch, Texture2D pixel, Rectangle rectangleToDraw, int thicknessOfBorder, Color borderColor)
        {
            // Draw top line
            spriteBatch.Draw(pixel, new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, rectangleToDraw.Width, thicknessOfBorder), borderColor);

            // Draw left line
            spriteBatch.Draw(pixel, new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, thicknessOfBorder, rectangleToDraw.Height), borderColor);

            // Draw right line
            spriteBatch.Draw(pixel, new Rectangle((rectangleToDraw.X + rectangleToDraw.Width - thicknessOfBorder),
                                            rectangleToDraw.Y,
                                            thicknessOfBorder,
                                            rectangleToDraw.Height), borderColor);
            // Draw bottom line
            spriteBatch.Draw(pixel, new Rectangle(rectangleToDraw.X,
                                            rectangleToDraw.Y + rectangleToDraw.Height - thicknessOfBorder,
                                            rectangleToDraw.Width,
                                            thicknessOfBorder), borderColor);
        }
        #endregion
    }
}
