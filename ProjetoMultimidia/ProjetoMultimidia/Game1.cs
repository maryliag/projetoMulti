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
using Coding4Fun.Kinect.Wpf;

namespace ProjetoMultimidia
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KinectSensor sensor;

        Model player;
        Model pista;
        Model box;

        Texture2D cabeca;
        Texture2D maoEsquerda;
        Texture2D maoDireita;
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

        float rotacaoPlayer;
        float posicaoLateral;
        float velocidade;
        float velocidadeMaxima = 10.0f;
        float velocidadeMAximaRe = -0.1f;
        float altura;
        float alturaMaxima = 2.0f;

        Boolean subindo;

        int id = 1;
        int idRetornado;
        int vidas;
        int posicao;
        int cabecaX;
        int cabecaY;
        int maoEsquerdaX;
        int maoEsquerdaY;
        int maoDireitaX;
        int maoDireitaY;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 640;
            graphics.PreferredBackBufferHeight = 480;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            sensor = KinectSensor.KinectSensors[0];
            sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(sensor_SkeletonFrameReady);
            sensor.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(sensor_DepthFrameReady);
            //pode ser esse
            //sensor.SkeletonStream.Enable();
            sensor.SkeletonStream.Enable(new TransformSmoothParameters()
            {
                Correction = 0.5f,
                JitterRadius = 0.05f,
                MaxDeviationRadius = 0.04f,
                Smoothing = 0.5f
            });
            sensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
            sensor.Start();
            valoresIniciais();

            base.Initialize();
        }

        void sensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            throw new NotImplementedException();
        }

        void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            throw new NotImplementedException();
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
            texturaObstaculo = Content.Load<Texture2D>("texturas\\pedra");
            coracao = Content.Load<Texture2D>("texturas\\coracao2");
            gameover = Content.Load<Texture2D>("texturas\\gameover");
            restart = Content.Load<Texture2D>("texturas\\restart");

            cabeca = Content.Load<Texture2D>("texturas\\coracao3");
            maoDireita = Content.Load<Texture2D>("texturas\\coracao3");
            maoEsquerda = Content.Load<Texture2D>("texturas\\coracao3");
         
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
                //rotacaoPlayer += 0.1f;
                if (posicao != 0)
                {
                    posicao--;
                }
            }
            if (teclado.IsKeyDown(Keys.Right))
            {
                //rotacaoPlayer -= 0.1f;
                if (posicao != 2)
                {
                    posicao++;
                }
                
            }
            posicaoLateral = novaPosicaoLateral(posicao, posicaoLateral);



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
                valoresIniciais();
            }



            if (altura > 0)
            {
                if (subindo)
                {
                    altura += 0.05f;
                } else
                {
                    altura -= 0.05f;
                    if (altura < 0)
                    {
                        altura = 0;
                    }
                }

                if (altura >= alturaMaxima)
                {
                    subindo = false;
                }
            }

            if (teclado.IsKeyDown(Keys.Z))
            {
                subindo = true;
                altura = 0.01f;
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

            Vector3 novaPosicaoPlayer = new Vector3(posicaoLateral, 0, velocidade);
            posicaoPlayer.Z += Vector3.Transform(novaPosicaoPlayer, Matrix.CreateRotationY(rotacaoPlayer)).Z;
            posicaoPlayer.X = Vector3.Transform(novaPosicaoPlayer, Matrix.CreateRotationY(rotacaoPlayer)).X;
            posicaoPlayer.Y = altura;


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
            DrawModelo(pista, Matrix.CreateRotationX(MathHelper.ToRadians(-90)) * Matrix.CreateTranslation(new Vector3(0,0,400)), texturaChaoPista);

            addObstaculo(box, new Vector3(0, 2, 70), texturaObstaculo);
            addObstaculo(box, new Vector3(15,-2, 90), texturaObstaculo);
            addObstaculo(box, new Vector3(-15, 3, 140), texturaObstaculo);
            addObstaculo(box, new Vector3(0, 0, 180), texturaObstaculo);
            addObstaculo(box, new Vector3(15, 1, 220), texturaObstaculo);
            addObstaculo(box, new Vector3(0, 2, 225), texturaObstaculo);
            addObstaculo(box, new Vector3(-15, 5, 280), texturaObstaculo);
            addObstaculo(box, new Vector3(15, -3, 305), texturaObstaculo);
            addObstaculo(box, new Vector3(0, 0, 320), texturaObstaculo);
            addObstaculo(box, new Vector3(-15, 4, 360), texturaObstaculo);
            addObstaculo(box, new Vector3(0, 1, 400), texturaObstaculo);
            addObstaculo(box, new Vector3(15, 1, 410), texturaObstaculo);
            addObstaculo(box, new Vector3(0, 3, 460), texturaObstaculo);

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

            spriteBatch.Draw(cabeca, new Rectangle(cabecaX, cabecaY, 100, 150), Color.White);
            spriteBatch.Draw(maoEsquerda, new Rectangle(maoEsquerdaX, maoEsquerdaY, 75, 75), Color.White);
            spriteBatch.Draw(maoDireita, new Rectangle(maoDireitaX, maoDireitaY, 75, 75), Color.White);
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

        public float novaPosicaoLateral(int posicao, float posicaoLateral)
        {
            float velocidade = 0.3f;
            if (posicao == 0)
            {
                if (posicaoLateral < 10.0f)
                {
                    posicaoLateral += velocidade;
                } 
            }
            if (posicao == 1)
            {
                if (posicaoLateral > 0.0f)
                {
                    posicaoLateral -= velocidade;
                }
                else if (posicaoLateral < 0.0f)
                {
                    posicaoLateral += velocidade;
                }
            }
            if (posicao == 2)
            {
                if (posicaoLateral > -10.0f)
                {
                    posicaoLateral -= velocidade;
                }
            }
            return posicaoLateral;
        }

        public void valoresIniciais()
        {
            vidas = 3;
            velocidade = 0.0f;
            rotacaoPlayer = 0.0f;
            posicaoLateral = 0.0f;
            posicao = 1;
            altura = 0.0f;
            subindo = true;
            obstaculos = new List<Area>();
            idsObstaculosAtravessados = new List<int>();
            posicaoPlayer = new Vector3(0, 0, 20);
            posicaoCamera = new Vector3(0, 10, 0);

            projecao = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45),
                graphics.GraphicsDevice.Viewport.AspectRatio, 1.0f, 100.0f);
            visao = Matrix.CreateLookAt(posicaoCamera, posicaoPlayer, Vector3.Up);
            
        }

      
    }
}
