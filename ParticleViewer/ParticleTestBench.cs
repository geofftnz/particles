using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTKExtensions.Framework;
using Particulate.ParticleSystem.Renderers;
using System.Threading;
using OpenTKExtensions;
using OpenTKExtensions.Filesystem;
using System.Diagnostics;
using ParticleViewer.Common;
using OpenTKExtensions.Camera;
using OpenTKExtensions.Resources;
using Particulate.ParticleSystem.Targets;
using Particulate.ParticleSystem.Operators;
using Particulate.ParticleSystem.Models;

namespace ParticleViewer
{
    public class ParticleTestBench : GameWindow
    {
        private const string SHADERPATH = @"../../Resources/Shaders;../../../Particulate/Resources/Shaders;Resources/Shaders";
        private GameComponentCollection components = new GameComponentCollection();
        private MultiPathFileSystemPoller shaderUpdatePoller = new MultiPathFileSystemPoller(SHADERPATH.Split(';'));
        private double lastShaderPollTime = 0.0;
        private Stopwatch timer = new Stopwatch();
        private CommonResources resources;
        private ICamera camera;

        private int particleArrayWidth = 1024;
        private int particleArrayHeight = 1024;

        ColourParticleRenderer particleRenderer;
        IParticleRenderTarget particleRenderTarget;
        RaymarchOperator particleOperator;
        MotionParticleModel model;

        public class RenderData : IFrameRenderData, IFrameUpdateData
        {
            public double Time { get; set; }
        }
        public RenderData frameData = new RenderData();


        public ParticleTestBench() : base(800, 600, GraphicsMode.Default, "Particles or summin or nuttin")
        {
            VSync = VSyncMode.Off;

            Load += ParticleTestBench_Load;
            Unload += ParticleTestBench_Unload;
            UpdateFrame += ParticleTestBench_UpdateFrame;
            RenderFrame += ParticleTestBench_RenderFrame;
            Resize += ParticleTestBench_Resize;

            // set default shader loader
            ShaderProgram.DefaultLoader = new OpenTKExtensions.Loaders.MultiPathFileSystemLoader(SHADERPATH);

            //OpenTK.Input.Keyboard.GetState()
            components.Add(camera = new WalkCamera()
            {
                FOV = 75.0f,
                ZFar = 10.0f,
                ZNear = 0.001f,
                MovementSpeed = 0.0001f,
                LookMode = WalkCamera.LookModeEnum.Mouse1,
                Position = new Vector3(0f, 0f, 0f),
                EyeHeight = 0f
            }, 1);
            components.Add(resources = new CommonResources());

            // Particle model
            components.Add(model = new MotionParticleModel(particleArrayWidth, particleArrayHeight));

            // Particle render target
            components.Add(particleRenderTarget = new PosVelColRenderTarget(particleArrayWidth, particleArrayHeight)
            {
                DrawOrder = 1,
                SetBuffers = (rt) =>
                {
                    rt.SetOutput(0, model.ParticlePositionWrite);
                    rt.SetOutput(1, model.ParticleVelocityWrite);
                    rt.SetOutput(2, model.ParticleColourWrite);
                }
            });

            // particle operator
            particleRenderTarget.Add(new OperatorTest());
            /*
            particleRenderTarget.Add(particleOperator = new RaymarchOperator()
            {
                TextureBinds = () =>
                {
                    model.ParticlePositionRead.Bind(TextureUnit.Texture0);
                    model.ParticleVelocityRead.Bind(TextureUnit.Texture1);
                    model.ParticleColourRead.Bind(TextureUnit.Texture2);
                },
                SetShaderUniforms = (sp) =>
                {
                    sp.SetUniform("time", (float)timer.Elapsed.TotalSeconds);
                    sp.SetUniform("particlePositionTexture", 0);
                    sp.SetUniform("particleVelocityTexture", 1);
                    sp.SetUniform("particleColourTexture", 2);
                }
            });*/

            // Render particles
            components.Add(particleRenderer = new ColourParticleRenderer(particleArrayWidth, particleArrayHeight)
            {
                DrawOrder = 2,
                ParticlePositionTextureFunc = () => model.ParticlePositionWrite,
                ParticleColourTextureFunc = () => model.ParticleColourWrite
            });



        }

        private void ParticleTestBench_Resize(object sender, EventArgs e)
        {
            GL.Viewport(ClientRectangle);
            components.Resize(ClientRectangle.Width, ClientRectangle.Height);
        }

        private void ParticleTestBench_Unload(object sender, EventArgs e)
        {
            components.Unload();
        }

        private void ParticleTestBench_Load(object sender, EventArgs e)
        {
            components.Load();
            timer.Start();
        }

        private void ParticleTestBench_RenderFrame(object sender, FrameEventArgs e)
        {
            if (shaderUpdatePoller.HasChanges)
            {
                components.Reload();
                shaderUpdatePoller.Reset();
            }

            components.ProjectionMatrix = camera.Projection;
            components.ViewMatrix = camera.View;


            GL.ClearColor(0.0f, 0.1f, 0.2f, 1.0f);
            GL.ClearDepth(1.0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            components.Render(frameData);



            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);

            //text.Render();

            SwapBuffers();
            model.SwapBuffers();
            Thread.Sleep(0);



        }

        private void ParticleTestBench_UpdateFrame(object sender, FrameEventArgs e)
        {
            frameData.Time = timer.Elapsed.TotalSeconds;

            if (frameData.Time - lastShaderPollTime > 2.0)
            {
                shaderUpdatePoller.Poll();
                lastShaderPollTime = frameData.Time;
            }
            components.Update(frameData);
        }
    }
}
