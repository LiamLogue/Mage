using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Mage
{
    class Zombie
    {
        #region Variables/Members
        //Variables/members needed for this class
        Texture2D zombieTexture;
        Texture2D zombieHoverTexture;
        Texture2D zombieTargetTexture;

        public Point frameSize;
        Point currentFrame;
        Point sheetSize;

        public int collisionOffset;
        public int randomDirection;

        Random randNum = new Random();
        int detectionDistance = 50;
        int millisecondsPerFrame;
        int timeSinceLastFrame = 0;
        public int zombieHealth;
        float timer;
        float elapsedTime;

        public Vector2 speed;
        public Vector2 position;

        public bool showNametag;
        public bool isClicked;
        public bool isAlive = false;
        public bool isImmune = false;

        public Rectangle rectangle;
        public string zombieName;
        public string direction;
        #endregion
        
        #region Constructor
        /// <summary>
        /// Main constructor for creating an enemy zombie.
        /// </summary>
        /// <param name="pTexture">Texture for the Zombie.</param>
        /// <param name="pHoverTexture">Texture for our zombie when we hover over it.</param>
        /// <param name="pTargetTexture">Texture for our zombie when it's clicked on.</param>
        /// <param name="pHealthTexture">Texture for zombie health.</param>
        /// <param name="pPosition">Positiong to initially draw the Mage.</param>
        /// <param name="pFrameSize">Size of each frame in the sprite sheet.</param>
        /// <param name="pCollisionOffset">Offset for more accurate collision detection.</param>
        /// <param name="pCurrentFrame">The current frame used fr animation.</param>
        /// <param name="pSheetSize">The total size of the sprite sheet in columns/rows.</param>
        /// <param name="pSpeed">Speed the Mage moves at.</param>
        /// <param name="pMillisecondsPerFrame">The milliseconds to animate per frame.</param>
        /// <param name="pZombieName">The unique zombie name to give to each zombie.</param>
        /// <param name="pZombieHealth">The amount of health this zombie has.</param>
        public Zombie(Texture2D pTexture, Texture2D pHoverTexture, Texture2D pTargetTexture, Vector2 pPosition, Point pFrameSize, int pCollisionOffset, Point pCurrentFrame, Point pSheetSize, Vector2 pSpeed, int pMillisecondsPerFrame, string pZombieName, int pZombieHealth)
        {
            //Instantiate our members using the passed values
            this.zombieTexture = pTexture;
            this.zombieHoverTexture = pHoverTexture;
            this.zombieTargetTexture = pTargetTexture;
            this.position = pPosition;
            this.frameSize = pFrameSize;
            this.collisionOffset = pCollisionOffset;
            this.currentFrame = pCurrentFrame;
            this.sheetSize = pSheetSize;
            this.speed = pSpeed;
            this.millisecondsPerFrame = pMillisecondsPerFrame;
            this.rectangle = new Rectangle((int)pPosition.X, (int)pPosition.Y, (int)pTexture.Width/2, (int)pTexture.Height);
            this.zombieName = pZombieName;
            this.zombieHealth = pZombieHealth;
            this.isAlive = true;
        }
        #endregion

        #region Update
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="clientBounds">Used to detect when objects leave the game screen</param>
        /// <param name="pPlayerPos">The position of the player we are going to chase.</param>
        public void Update(GameTime gameTime, Rectangle clientBounds, Vector2 pPlayerPos)
        {
            //Add the elapsed milliseconds to the time since the last frame
            this.timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;

            //Time to animate?
            if (this.timeSinceLastFrame > this.millisecondsPerFrame)
            {
                //Reduce the time since last frame
                this.timeSinceLastFrame -= this.millisecondsPerFrame;

                //Increment the X of currentFrame
                ++currentFrame.X;

                //Are we off/on the edge?!
                if (this.currentFrame.X >= this.sheetSize.X)
                {
                    //Yes. Set the X to 0
                    this.currentFrame.X = 0;
                }
            }

            //Update our zombie rectangle
            this.rectangle.X = (int)this.position.X;
            this.rectangle.Y = (int)this.position.Y;

            //Bounds checking
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

            //Check our keyboard state and act accordingly            
            KeyboardState keyboardState = Keyboard.GetState();

            //Have we selected a target?
            if (this.isClicked)
            {
                //Increment our timer
                this.timer += gameTime.ElapsedGameTime.Milliseconds;

                //Add or remove health
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    if (this.timer >= 1000)
                    {
                        this.zombieHealth += 1;
                        this.timer = 0.0f;
                    }
                }
                else if (keyboardState.IsKeyDown(Keys.Down))
                {
                    if (this.timer >= 1000)
                    {
                        this.zombieHealth -= 1;
                        this.timer = 0.0f;                     
                    }
                }

                //Zombies have a min/max of 0/100 respectively
                this.zombieHealth = (int)MathHelper.Clamp(this.zombieHealth, 0, 100);
            }

            //Should we chase?
            CheckForChase(pPlayerPos, gameTime, clientBounds);
        }
        #endregion

        #region Draw
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="spriteBatch">Needed to draw our textures in this class.</param>
        /// <param name="spriteFont">Needed to draw our zombie nametag.</param>
        /// <param name="pPlayerPos">The position of the player we are going to chase.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont spriteFont, Vector2 pPlayerPos)
        {
            //Draw the zombie nametag?
            if (!showNametag)
            {
                //Has the zombie been clicked on?
                if (!isClicked)
                {
                    //Draw the zombie either flipped or not flipped depending on the players X
                    if (direction == "left")
                    {
                        spriteBatch.Draw(zombieTexture, position, new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
                    }
                    else
                    {
                        spriteBatch.Draw(zombieTexture, position, new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.FlipHorizontally, 0.0f);
                    }
                }
                else
                {
                    //Draw our zombie nametag and health with bonus drop shadow text effect!
                    spriteBatch.DrawString(spriteFont, "zombie", new Vector2(position.X - 5, position.Y - 25), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                    spriteBatch.DrawString(spriteFont, zombieHealth.ToString(), new Vector2(position.X + 2, position.Y - 13), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                    spriteBatch.DrawString(spriteFont, "zombie", new Vector2(position.X - 5, position.Y - 26), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                    spriteBatch.DrawString(spriteFont, zombieHealth.ToString(), new Vector2(position.X + 2, position.Y - 14), Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 1);

                    //Draw the zombie either flipped or not flipped depending on the players X
                    if (direction == "left")
                    {
                        spriteBatch.Draw(zombieTargetTexture, position, new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
                    }
                    else
                    {
                        spriteBatch.Draw(zombieTargetTexture, position, new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.FlipHorizontally, 0.0f);
                    }
                }
            }
            else
            {
                //Draw our zombie nametag and health with bonus drop shadow text effect!
                spriteBatch.DrawString(spriteFont, "zombie", new Vector2(position.X - 5, position.Y - 25), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                spriteBatch.DrawString(spriteFont, zombieHealth.ToString(), new Vector2(position.X + 2, position.Y - 13), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                spriteBatch.DrawString(spriteFont, "zombie", new Vector2(position.X - 5, position.Y - 26), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                spriteBatch.DrawString(spriteFont, zombieHealth.ToString(), new Vector2(position.X + 2, position.Y - 14), Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 1);

                //Draw the zombie either flipped or not flipped depending on the players X
                if (direction == "left")
                {
                    spriteBatch.Draw(zombieHoverTexture, position, new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
                }
                else
                {
                    spriteBatch.Draw(zombieHoverTexture, position, new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.FlipHorizontally, 0.0f);
                }
            }
        }
        #endregion

        #region CheckForChase
        /// <summary>
        /// Sets our zombie behaviours based on distance.
        /// </summary>
        /// <param name="pPlayerPos">The position of the player we are going to chase.</param>
        public void CheckForChase(Vector2 pPlayerPos, GameTime gameTime, Rectangle clientBounds)
        {
            //Can we detect the player?
            if (Vector2.Distance(pPlayerPos, position) < detectionDistance)
            {
                ///////////////////////////////////////
                //
                // THIS NEEDS DONE. HURTS HEAD.
                //
                ///////////////////////////////////////
                /*
                //Compare the X values and decide to chase or not
                if (this.position.X > pPlayerPos.X)
                {
                    //Give chase!
                    this.position.X -= this.speed.X;
                    this.direction = "left";
                }
                else if (this.position.X < pPlayerPos.X)
                {
                    //Give chase!
                    this.position.X += this.speed.X;
                    this.direction = "right";
                }

                //Compare the Y values and decide to chase or not
                if (this.position.Y > pPlayerPos.Y)
                {
                    //Give chase!
                    this.position.Y -= this.speed.Y;
                }
                else if (this.position.Y < pPlayerPos.Y)
                {
                    //Give chase!
                    this.position.Y += this.speed.Y;
                }*/
            }
            else
            {

                //Add to our elapsed time
                elapsedTime += gameTime.ElapsedGameTime.Milliseconds;

                //Longer than 2 seconds?
                if (elapsedTime > 2000)
                {
                    //Bottom right
                    if (randomDirection == 0)
                    {
                        speed.X = 1.0f;
                        speed.Y = 1.0f;
                        direction = "right";
                    }
                    //Bottom left
                    if (randomDirection == 1)
                    {
                        speed.X = -1.0f;
                        speed.Y = 1.0f;
                        direction = "left";
                    }
                    //Top left
                    if (randomDirection == 2)
                    {
                        speed.X = -1.0f;
                        speed.Y = -1.0f;
                        direction = "left";
                    }
                    //Top right
                    if (randomDirection == 3)
                    {
                        speed.X = 1.0f;
                        speed.Y = -1.0f;
                        direction = "right";
                    }

                    //Reset elapsed time
                    elapsedTime = 0.0f;
                }

                //Update the position.
                position += speed;
            }
        }
        #endregion

        #region StopChasing
        /// <summary>
        /// Stops the zombie chasing the player
        /// </summary>
        public void StopChasing()
        {
            this.speed.X = 0;
            this.speed.Y = 0;
        }
        #endregion

        #region TakeDamage
        /// <summary>
        /// Make the zombie take damage.
        /// </summary>
        public void TakeDamage(int pDamage)
        {
            this.zombieHealth -= pDamage;
        }
        #endregion
    }
}
