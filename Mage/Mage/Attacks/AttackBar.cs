using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Mage
{
    class AttackBar
    {
        #region Variables/Members
        //Variables/members needed for this class
        Texture2D attackBarTexture;
        #endregion

        #region Constructor
        /// <summary>
        /// Main constructor for creating an attack bar for our Mage.
        /// </summary>
        /// <param name="pTexture">Texture for the attack bar.</param>
        public AttackBar(Texture2D pTexture)
        {
            //Instantiate our members using the passed values
            this.attackBarTexture = pTexture;
        }
        #endregion

        #region Update
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {

        }
        #endregion

        #region Draw
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="spriteBatch">Needed to draw our textures in this class.</param>
        /// <param name="clientBounds">Needed to draw the texture at the bottom of the window.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle clientBounds)
        {
            //Draw our attack bar
            spriteBatch.Draw(attackBarTexture, new Rectangle((clientBounds.Width / 2) - 180, clientBounds.Height - attackBarTexture.Height, attackBarTexture.Width, attackBarTexture.Height), null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0f);
        }
        #endregion
    }
}
