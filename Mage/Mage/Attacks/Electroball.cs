using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Mage
{
    class Electroball
    {
        #region Variables/members
        Texture2D electroBallTexture;

        public Vector2 position;
        public Vector2 velocity;
        public Vector2 origin;

        public bool hasTarget;
        public bool isVisible;

        public int damage = 5;

        public Rectangle rectangle;
        #endregion

        #region Constructor
        /// <summary>
        /// Main constructor for creating a "frostspear" object.
        /// </summary>
        /// <param name="pTexture">Texture for the frostspear.</param>
        public Electroball(Texture2D pTexture)
        {
            //Set the texture and make our frostspear invisible
            this.electroBallTexture = pTexture;
            isVisible = false;

            //Create the rectangle
            rectangle = new Rectangle((int)position.X, (int)position.Y, pTexture.Width, pTexture.Height);
        }
        #endregion

        #region Update
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        public void Update()
        {
            //Update the fireball's rectangle
            this.rectangle.X = (int)this.position.X;
            this.rectangle.Y = (int)this.position.Y;
        }
        #endregion

        #region Draw
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="spriteBatch">Needed to draw our textures in this class.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw the fireball attack
            spriteBatch.Draw(electroBallTexture, position, null, Color.White, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);
        }
        #endregion
    }
}
