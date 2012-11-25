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
using Microsoft.Kinect;

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
        Model box;

        Texture2D texturaChaoPista;
        Texture2D texturaObstaculo;
        Texture2D coracao;
        Texture2D gameover;
        Texture2D restart;

        Matrix visao;
        Matrix projecao;

        Vector3 posicaoPlayer;
        Vector3 posicaoCamera;

        List<Area> obstaculos;
        List<int> idsObstaculosAtravessados;

        float rotacaoPlayer = 0.0f;
        float velocidade;
        float velocidadeMaxima = 10.0f;
        float velocidadeMAximaRe = -0.1f;

        int id = 1;
        int idRetornado;
        int vidas = 3;
        int vidasMaximas = 3;

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
            obstaculos = new List<Area>();
            idsObstaculosAtravessados = new List<int>();
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
            box = Content.Load<Model>("modelos\\box");

            texturaChaoPista = Content.Load<Texture2D>("texturas\\chao2");
            texturaObstaculo = Content.Load<Texture2D>("texturas\\x");
            coracao = Content.Load<Texture2D>("texturas\\coracao2");
            gameover = Content.Load<Texture2D>("texturas\\gameover");
            restart = Content.Load<Texture2D>("texturas\\restart");
            

         
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


            if (teclado.IsKeyDown(Keys.R))
            {
                vidas = 3;
                Initialize();
            }
           

            idRetornado = atravessouObstaculo(posicaoPlayer.X, posicaoPlayer.Z);
            if (!idRetornado.Equals(0))
            {
                if (!idsObstaculosAtravessados.Contains(idRetornado))
                {
                    idsObstaculosAtravessados.Add(idRetornado);
                    vidas -= 1;
                }
                
                if (vidas <= 0)
                {
                    velocidade = 0;
                }
                else
                {
                    velocidade = 0.1f;
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
            int posicao = 10;
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            DrawModelo(player,  Matrix.CreateRotationY(rotacaoPlayer) * Matrix.CreateTranslation(posicaoPlayer),null);
            DrawModelo(pista, Matrix.CreateRotationX(MathHelper.ToRadians(-90)) * Matrix.CreateTranslation(new Vector3(0,-1,0)), texturaChaoPista);

            addObstaculo(box, new Vector3(0, 0, 70), texturaObstaculo);
            addObstaculo(box, new Vector3(15, 0, 60), texturaObstaculo);
            addObstaculo(box, new Vector3(-15, 0, 100), texturaObstaculo);
            addObstaculo(box, new Vector3(0, 0, 110), texturaObstaculo);
            addObstaculo(box, new Vector3(15, 0, 160), texturaObstaculo);
            addObstaculo(box, new Vector3(0, 0, 165), texturaObstaculo);
            addObstaculo(box, new Vector3(-15, 0, 190), texturaObstaculo);
            addObstaculo(box, new Vector3(15, 0, 205), texturaObstaculo);
            addObstaculo(box, new Vector3(0, 0, 220), texturaObstaculo);
            addObstaculo(box, new Vector3(-15, 0, 240), texturaObstaculo);
            addObstaculo(box, new Vector3(0, 0, 250), texturaObstaculo);
            addObstaculo(box, new Vector3(15, 0, 270), texturaObstaculo);
            addObstaculo(box, new Vector3(0, 0, 300), texturaObstaculo);

            spriteBatch.Begin();
            for (int i = 0; i < vidas; i++)
            {
                spriteBatch.Draw(coracao, new Vector2(posicao, 10), Color.White);
                posicao += 30;
            }

            if (vidas.Equals(0))
            {
                spriteBatch.Draw(gameover, new Vector2(200, 300), Color.White);
                spriteBatch.Draw(restart, new Vector2(310, 400), Color.White);
            }
            spriteBatch.End();

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

        public void addObstaculo(Model obstaculo, Vector3 position, Texture2D textura)
        {
            Area area = new Area(id, position.X - 3.5f, position.X + 6f, position.Z - 1f, position.Z + 5f);
            DrawModelo(obstaculo, Matrix.CreateTranslation(position), textura);
            obstaculos.Add(area);
            id++;
        }

        public int atravessouObstaculo(float x, float z)
        {
            foreach (Area a in obstaculos)
            {
                if (a.isInArea(x, z))
                {
                    return a.getId();
                }
            }
            return 0;
        }
    }
}
