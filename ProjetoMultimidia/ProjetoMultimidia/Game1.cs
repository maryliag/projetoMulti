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

namespace ProjetoMultimidia
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Model player;
        Model pista;

        Texture2D texturaChaoPista;

        Matrix visao;
        Matrix projecao;

        Vector3 posicaoPlayer;
        Vector3 posicaoCamera;

        List<Area> obstaculos;

        float rotacaoPlayer = 0.0f;
        float velocidade;
        float velocidadeMaxima = 10.0f;
        float velocidadeMAximaRe = -0.1f;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            posicaoPlayer = new Vector3(0, 0, 20);
            posicaoCamera = new Vector3(0, 10, 0);

            projecao = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45),
                graphics.GraphicsDevice.Viewport.AspectRatio, 1.0f, 100.0f);
            visao = Matrix.CreateLookAt(posicaoCamera, posicaoPlayer, Vector3.Up);

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

            player = Content.Load<Model>("modelos\\man");
            pista = Content.Load<Model>("modelos\\pista");
            texturaChaoPista = Content.Load<Texture2D>("texturas\\chao");
         
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

            KeyboardState teclado = Keyboard.GetState();
            if (teclado.IsKeyDown(Keys.Left))
            {
                rotacaoPlayer += 0.05f;
            }
            if (teclado.IsKeyDown(Keys.Right))
            {
                rotacaoPlayer -= 0.05f;
            }
            if (teclado.IsKeyDown(Keys.Up))
            {
                if (velocidade < velocidadeMaxima)
                {
                    velocidade += 0.01f;
                }
            }
            else
            {
                if (velocidade > 0)
                {
                    velocidade -= 0.01f;
                }
            }

            if (teclado.IsKeyDown(Keys.Down))
            {
                if (velocidade > velocidadeMAximaRe)
                {
                    velocidade -= 0.01f;
                }
            }
            else
            {
                if (velocidade < 0)
                {
                    velocidade = 0;
                }
            }

            Vector3 novaPosicaoPlayer = new Vector3(0, 0, velocidade);
            posicaoPlayer.Z += Vector3.Transform(novaPosicaoPlayer, Matrix.CreateRotationY(rotacaoPlayer)).Z;
            posicaoPlayer.X += Vector3.Transform(novaPosicaoPlayer, Matrix.CreateRotationY(rotacaoPlayer)).X;

            Vector3 novaPosicaoCamera = new Vector3(0, 10, -20);
            novaPosicaoCamera = Vector3.Transform(novaPosicaoCamera, Matrix.CreateRotationY(rotacaoPlayer));
            posicaoCamera = novaPosicaoCamera + posicaoPlayer;
            visao = Matrix.CreateLookAt(posicaoCamera, posicaoPlayer, Vector3.Up);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            DrawModelo(player,  Matrix.CreateRotationY(rotacaoPlayer) * Matrix.CreateTranslation(posicaoPlayer),null);
            DrawModelo(pista, Matrix.CreateRotationX(MathHelper.ToRadians(-90)) * Matrix.CreateTranslation(new Vector3(0,-1,-20)), texturaChaoPista);

            base.Draw(gameTime);
        }

        public void DrawModelo(Model modelo, Matrix mundo, Texture2D textura)
        {
            foreach (ModelMesh mesh in modelo.Meshes)
            {
                foreach (BasicEffect efeito in mesh.Effects)
                {
                    efeito.EnableDefaultLighting();
                    efeito.View = visao;
                    efeito.Projection = projecao;
                    efeito.World = mundo;
                    efeito.TextureEnabled = true;
                    efeito.Texture = textura;
                }
                mesh.Draw();
            }
        }
    }
}
