using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Mage
{
    class Earthgrab
    {
        #region Variables/members
        Texture2D earthgrabTexture;

        public Vector2 position;
        public Vector2 velocity;
        public Vector2 origin;
        public Vector2 positionToSet;

        public bool isPlanted = false;
        public bool isVisible;

        public Rectangle rectangle;
        public Rectangle borderRectangle;
        #endregion

        #region Constructor
        /// <summary>
        /// Main constructor for creating a "health totem" object.
        /// </summary>
        /// <param name="pTexture">Texture for the health totem.</param>
        public Earthgrab(Texture2D pTexture)
        {
            //Set the texture and make our health totem invisible
            this.earthgrabTexture = pTexture;
            isVisible = false;
        }
        #endregion

        #region Update
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        public void Update()
        {

        }
        #endregion

        #region Draw
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="spriteBatch">Needed to draw our textures in this class.</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 pPlayerPos)
        {
             //Update the earthgrab's rectangle
            rectangle = new Rectangle((int)pPlayerPos.X, (int)pPlayerPos.Y, earthgrabTexture.Width, earthgrabTexture.Height);
            borderRectangle = new Rectangle((int)pPlayerPos.X - 60, (int)pPlayerPos.Y - 55, earthgrabTexture.Width * 6, earthgrabTexture.Height * 6);

            //Draw the earthgrab
            spriteBatch.Draw(earthgrabTexture, pPlayerPos, null, Color.White, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);   
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
