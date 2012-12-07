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

        Texture2D texturaChaoPista;
        Texture2D texturaObstaculo;
        Texture2D texturaParedes;
        Texture2D coracao;
        Texture2D gameover;
        Texture2D restart;
        Texture2D congrats;

        Matrix visao;
        Matrix projecao;

        Vector3 posicaoPlayer;
        Vector3 posicaoCamera;

        List<Area> obstaculos;
        List<int> idsObstaculosAtravessados;

        float rotacaoPlayer;
        float posicaoLateral;
        float posicaoLateralMax = 18.0f;
        float posicaoLateralMin = -18.0f;
        float velocidade;
        float velocidadeMaxima = 1.0f;

        int id = 1;
        int idRetornado;
        int vidas;
        int cinturaX;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 640;
            graphics.PreferredBackBufferHeight = 500;
        }

        /// <summary>
        /// ows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            sensor = KinectSensor.KinectSensors[0];
            sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(sensor_SkeletonFrameReady);
            sensor.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(sensor_DepthFrameReady);
            var parameters = new TransformSmoothParameters {
                Smoothing = 0.3f,
                Correction = 0.0f,
                Prediction=0.0f,
                JitterRadius=1.0f,
                MaxDeviationRadius=0.5f
            };
            sensor.SkeletonStream.Enable(parameters);
            sensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
            sensor.Start();
            valoresIniciais();

            base.Initialize();
        }

        void sensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (var depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame != null)
                {
                    var bits = new short[depthFrame.PixelDataLength];
                    depthFrame.CopyPixelDataTo(bits);
                }
            }
        }

        void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = new Skeleton[0];

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }

            foreach (Skeleton skeleton in skeletons)
            {
                if (skeleton.TrackingState.Equals(SkeletonTrackingState.Tracked))
                {
                    if (skeleton != null)
                    {
                        Joint cint = skeleton.Joints[JointType.HipCenter].ScaleTo(640, 480, 0.5f, 0.5f);
                        cinturaX = 20 -(int)cint.Position.X/10;
                    }
                }
            }
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
            texturaParedes = Content.Load<Texture2D>("texturas\\paredes");
            coracao = Content.Load<Texture2D>("texturas\\coracao2");
            gameover = Content.Load<Texture2D>("texturas\\gameover");
            restart = Content.Load<Texture2D>("texturas\\restart");
            congrats = Content.Load<Texture2D>("texturas\\congrats");     
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
            posicaoLateral = cinturaX;
            if (posicaoLateral > posicaoLateralMax)
            {
                posicaoLateral = posicaoLateralMax;
            }
            else if (posicaoLateral < posicaoLateralMin)
            {
                posicaoLateral = posicaoLateralMin;
            }

            if (velocidade < velocidadeMaxima && vidas > 0)
            {
                velocidade += 0.01f;
            }

            if (teclado.IsKeyDown(Keys.R))
            {
                valoresIniciais();
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

            if (posicaoPlayer.Z > 785)
            {
                velocidade=0;
            }

            Vector3 novaPosicaoPlayer = new Vector3(posicaoLateral, 0, velocidade);
            posicaoPlayer.Z += Vector3.Transform(novaPosicaoPlayer, Matrix.CreateRotationY(rotacaoPlayer)).Z;
            posicaoPlayer.X = Vector3.Transform(novaPosicaoPlayer, Matrix.CreateRotationY(rotacaoPlayer)).X;



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

            spriteBatch.Begin();
            spriteBatch.Draw(texturaParedes, new Vector2(0, 0), Color.White);
            spriteBatch.End();

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            DrawModelo(player,  Matrix.CreateRotationY(rotacaoPlayer) * Matrix.CreateTranslation(posicaoPlayer),null);
            DrawModelo(pista, Matrix.CreateRotationX(MathHelper.ToRadians(-90)) * Matrix.CreateTranslation(new Vector3(0,0,400)), texturaChaoPista);

            addObstaculo(box, new Vector3(12, 2, 70), texturaObstaculo);
            addObstaculo(box, new Vector3(15,-2, 90), texturaObstaculo);
            addObstaculo(box, new Vector3(1, 0, 180), texturaObstaculo);
            addObstaculo(box, new Vector3(10, 1, 220), texturaObstaculo);
            addObstaculo(box, new Vector3(0, 2, 225), texturaObstaculo);
            addObstaculo(box, new Vector3(-15, 5, 280), texturaObstaculo);
            addObstaculo(box, new Vector3(11, -3, 305), texturaObstaculo);
            addObstaculo(box, new Vector3(0, 0, 320), texturaObstaculo);
            addObstaculo(box, new Vector3(-7, 4, 360), texturaObstaculo);
            addObstaculo(box, new Vector3(1, 1, 400), texturaObstaculo);
            addObstaculo(box, new Vector3(10, 1, 410), texturaObstaculo);
            addObstaculo(box, new Vector3(0, 3, 460), texturaObstaculo);
            addObstaculo(box, new Vector3(-13, 4, 460), texturaObstaculo);
            addObstaculo(box, new Vector3(0, 1, 500), texturaObstaculo);
            addObstaculo(box, new Vector3(15, 1, 510), texturaObstaculo);
            addObstaculo(box, new Vector3(0, 3, 580), texturaObstaculo);
            addObstaculo(box, new Vector3(3, 3, 610), texturaObstaculo);
            addObstaculo(box, new Vector3(-7, 4, 650), texturaObstaculo);
            addObstaculo(box, new Vector3(5, 3, 700), texturaObstaculo);
            addObstaculo(box, new Vector3(13, 1, 705), texturaObstaculo);
            addObstaculo(box, new Vector3(-11, 3, 730), texturaObstaculo);
            addObstaculo(box, new Vector3(-6, 3, 760), texturaObstaculo);
            addObstaculo(box, new Vector3(-15, -1, 783), texturaObstaculo);
            addObstaculo(box, new Vector3(15, 0, 784), texturaObstaculo);

            spriteBatch.Begin();
            for (int i = 0; i < vidas; i++)
            {
                spriteBatch.Draw(coracao, new Vector2(posicao, 10), Color.White);
                posicao += 30;
            }

            if (vidas.Equals(0))
            {
                spriteBatch.Draw(gameover, new Vector2(140, 300), Color.White);
                spriteBatch.Draw(restart, new Vector2(250, 400), Color.White);
            }
            if (posicaoPlayer.Z > 785)
            {
                spriteBatch.Draw(congrats, new Vector2(140, 300), Color.White);
                spriteBatch.Draw(restart, new Vector2(250, 400), Color.White);
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

        public void valoresIniciais()
        {
            vidas = 3;
            velocidade = 1.0f;
            rotacaoPlayer = 0.0f;
            posicaoLateral = 0.0f;
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
