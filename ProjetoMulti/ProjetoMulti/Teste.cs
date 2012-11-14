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
using System.IO;

namespace ProjetoMulti
{
    class Teste
    {
        /// <summary>
        /// This is the main type for your game
        /// </summary>
        public class AlphabetDemoApp : Microsoft.Xna.Framework.Game
        {
            DemoScreen screen;

            GraphicsDeviceManager graphics;
            SpriteBatch spriteBatch;
            SpriteFont outputFont;

            public AlphabetDemoApp()
            {
                graphics = new GraphicsDeviceManager(this);
                Content.RootDirectory = "Content";
                graphics.PreferredBackBufferWidth = 1024;
                graphics.PreferredBackBufferHeight = 768;
            }

            /// <summary>
            /// Allows the game to perform any initialization it needs to before starting to run.
            /// This is where it can query for any required services and load any non-graphic
            /// related content.  Calling base.Initialize will enumerate through any components
            /// and initialize them as well.
            /// </summary>
            protected override void Initialize()
            {

                Stream stream = TitleContainer.OpenStream(Content.RootDirectory + "\\Config\\teste-multitouch.ksvm");
                screen = new DemoScreen();
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

                outputFont = Content.Load<SpriteFont>("OutputStringFont");
            }

            /// <summary>
            /// UnloadContent will be called once per game and is the place to unload
            /// all content.
            /// </summary>
            protected override void UnloadContent()
            {
                // TODO: Unload any non ContentManager content here
            }

            /// <summary>
            /// Allows the game to run logic such as updating the world,
            /// checking for collisions, gathering input, and playing audio.
            /// </summary>
            /// <param name="gameTime">Provides a snapshot of timing values.</param>
            protected override void Update(GameTime gameTime)
            {
                // Allows the game to exit
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    this.Exit();
                if (screen.IsClearButtonPressed())
                {
                    screen.ResetString();
                }
                screen.UpdateLetter();

                base.Update(gameTime);
            }

            /// <summary>
            /// This is called when the game should draw itself.
            /// </summary>
            /// <param name="gameTime">Provides a snapshot of timing values.</param>
            protected override void Draw(GameTime gameTime)
            {
                Rectangle titleSafeArea = GraphicsDevice.Viewport.TitleSafeArea;
                Vector2 hudLocation = new Vector2(titleSafeArea.X, titleSafeArea.Y);
                Vector2 hudCenter = new Vector2(titleSafeArea.X + titleSafeArea.Width / 2.0f,
                                             titleSafeArea.Y + titleSafeArea.Height / 2.0f);

                GraphicsDevice.Clear(Color.CornflowerBlue);

                spriteBatch.Begin();

                titleSafeArea = GraphicsDevice.Viewport.TitleSafeArea;
                hudLocation = new Vector2(titleSafeArea.X, titleSafeArea.Y);
                hudCenter = new Vector2(0, 0);

                spriteBatch.DrawString(outputFont, "UHUUU", hudLocation, Color.Black);

                drawString(screen.GetOutputString(), 50, 150);

                spriteBatch.End();

                // FIXME!!!
                //spriteBatch.DrawString(screen.GetOutputString());
                base.Draw(gameTime);
            }

            private void drawString(List<Letter> outputString, int leftMargin, int height)
            {
                int i = 0;
                foreach (Letter l in outputString)
                {
                    spriteBatch.DrawString(outputFont, l.getText(), new Vector2(leftMargin + i * 65 * l.getScale().X, height), l.getColor(), 0.0f, new Vector2(0, 0), l.getScale(), SpriteEffects.None, 0.0f);
                    i++;
                }
            }
        }

    }
}
