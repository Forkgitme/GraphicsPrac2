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

namespace GraphicsPractical2
{

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        // Often used XNA objects
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private FrameRateCounter frameRateCounter;

        // Game objects and variables
        private Camera camera;
        
        // Model
        private Model model;
        private Material modelMaterial;

        // Quad
        private VertexPositionNormalTexture[] quadVertices;
        private short[] quadIndices;
        private Matrix quadTransform;

        // The gamma correction post-processing shader
        Effect gammaCorrection;
        RenderTarget2D renderTarget;

        // Keyboard state
        private KeyboardState previousKS, currentKS;

        // Bool which determines whether Phong or Blinn-Phon shading is used
        private bool phongShading = false;

        //Worldmatrix
        private Matrix world;

        public Game1()
        {
            this.graphics = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "Content";
            // Create and add a frame rate counter
            this.frameRateCounter = new FrameRateCounter(this);
            this.Components.Add(this.frameRateCounter);
        }

        protected override void Initialize()
        {
            // Copy over the device's rasterizer state to change the current fillMode
            this.GraphicsDevice.RasterizerState = new RasterizerState() { CullMode = CullMode.None };
            // Set up the window
            this.graphics.PreferredBackBufferWidth = 800;
            this.graphics.PreferredBackBufferHeight = 600;
            this.graphics.IsFullScreen = false;
            // Let the renderer draw and update as often as possible
            this.graphics.SynchronizeWithVerticalRetrace = false;
            this.IsFixedTimeStep = false;
            // Flush the changes to the device parameters to the graphics card
            this.graphics.ApplyChanges();
            // Initialize the camera
            this.camera = new Camera(new Vector3(0, 50, 100), new Vector3(0, 0, 0), new Vector3(0, 1, 0));

            this.IsMouseVisible = true;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            //Create WorldMatrix
            //world = new Matrix(10.0f, 0, 0, 0, 0, 10.0f, 0, 0, 0, 0, 10.0f, 0, 0, 0, 0, 1f);
            world = new Matrix(10.0f, 0, 0, 0, 0, 6.5f, 0, 0, 0, 0, 2.5f, 0, 0, 0, 0, 1f);
            // Create a SpriteBatch object
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
            // Load the "Simple" effect
            Effect effect = this.Content.Load<Effect>("Effects/Simple");
            // Load the model and let it use the "Simple" effect
            this.model = this.Content.Load<Model>("Models/Teapot");
            this.model.Meshes[0].MeshParts[0].Effect = effect;
            // Setup the quad
            this.setupQuad();

            gammaCorrection = this.Content.Load<Effect>("Effects/PostProcessing");
            renderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight, true, GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);

            modelMaterial = new Material();
            effect.Parameters["LightingPosition"].SetValue(new Vector4(50, 50, 50, 1));
            effect.Parameters["PhongBool"].SetValue(0f);
            modelMaterial.DiffuseColor = Color.Red;
            modelMaterial.AmbientColor = Color.Red;
            modelMaterial.AmbientIntensity = 0.2f;
            modelMaterial.SpecularColor = Color.White;
            modelMaterial.SpecularIntensity = 2.0f;
            modelMaterial.SpecularPower = 25.0f;
            modelMaterial.SetEffectParameters(effect);
        }

        /// <summary>
        /// Sets up a 2 by 2 quad around the origin.
        /// </summary>
        private void setupQuad()
        {
            float scale = 50.0f;

            // Normal points up
            Vector3 quadNormal = new Vector3(0, 1, 0);

            this.quadVertices = new VertexPositionNormalTexture[4];
            // Top left
            this.quadVertices[0].Position = new Vector3(-1, 0, -1);
            this.quadVertices[0].Normal = quadNormal;
            // Top right
            this.quadVertices[1].Position = new Vector3(1, 0, -1);
            this.quadVertices[1].Normal = quadNormal;
            // Bottom left
            this.quadVertices[2].Position = new Vector3(-1, 0, 1);
            this.quadVertices[2].Normal = quadNormal;
            // Bottom right
            this.quadVertices[3].Position = new Vector3(1, 0, 1);
            this.quadVertices[3].Normal = quadNormal;

            this.quadIndices = new short[] { 0, 1, 2, 1, 2, 3 };
            this.quadTransform = Matrix.CreateScale(scale);
        }

        protected override void Update(GameTime gameTime)
        {
            previousKS = currentKS;
            currentKS = Keyboard.GetState();

            if (currentKS.IsKeyDown(Keys.P) && previousKS.IsKeyUp(Keys.P))
            {
                phongShading = !phongShading;
                this.model.Meshes[0].MeshParts[0].Effect.Parameters["PhongBool"].SetValue(phongShading ? 1f : 0f);
            }

            float timeStep = (float)gameTime.ElapsedGameTime.TotalSeconds * 60.0f;

            // Update the window title
            this.Window.Title = "XNA Renderer | FPS: " + this.frameRateCounter.FrameRate + " | Phong: " + phongShading;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Clear the screen in a predetermined color and clear the depth buffer
            this.GraphicsDevice.SetRenderTarget(renderTarget);
            this.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DeepSkyBlue, 1.0f, 0);
            
            // Get the model's only mesh
            ModelMesh mesh = this.model.Meshes[0];
            Effect effect = mesh.Effects[0];

            // Set the effect parameters
            effect.CurrentTechnique = effect.Techniques["Simple"];
            // Matrices for 3D perspective projection
            this.camera.SetEffectParameters(effect);
            effect.Parameters["World"].SetValue(world);
            effect.Parameters["InverseTransposed"].SetValue(Matrix.Transpose(Matrix.Invert(world)));
            // Draw the model
            mesh.Draw();

            this.GraphicsDevice.SetRenderTarget(null);

            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.LinearClamp, DepthStencilState.Default,
                RasterizerState.CullNone, gammaCorrection);
            spriteBatch.Draw(renderTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
