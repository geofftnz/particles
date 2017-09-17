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

namespace ParticleViewer
{
    public class ParticleTestBench : GameWindow
    {
        private const string SHADERPATH = @"../../Resources/Shaders;../../../Particulate/Resources/Shaders;Resources/Shaders";
        private GameComponentCollection components = new GameComponentCollection();
        private MultiPathFileSystemPoller shaderUpdatePoller = new MultiPathFileSystemPoller(SHADERPATH.Split(';'));
        private double lastShaderPollTime = 0.0;
        private Stopwatch timer = new Stopwatch();
        private Resources resources;
        private ICamera camera;

        public class RenderData : IFrameRenderData, IFrameUpdateData
        {
            public double Time { get; set; }
        }
        public RenderData frameData = new RenderData();


        public ParticleTestBench() : base(800, 600, GraphicsMode.Default, "Particles or summin or nuttin", GameWindowFlags.Default, DisplayDevice.Default, 4, 3, GraphicsContextFlags.ForwardCompatible)
        {
            this.VSync = VSyncMode.Off;

            this.Load += ParticleTestBench_Load;
            this.Unload += ParticleTestBench_Unload;
            this.UpdateFrame += ParticleTestBench_UpdateFrame;
            this.RenderFrame += ParticleTestBench_RenderFrame;
            this.Resize += ParticleTestBench_Resize;

            components.Add(camera = new WalkCamera(this.Keyboard, this.Mouse)
            {
                FOV = 75.0f,
                ZFar = 10.0f,
                ZNear = 0.001f,
                MovementSpeed = 0.0001f,
                LookMode = WalkCamera.LookModeEnum.Mouse1                
            }, 1);
            components.Add(resources = new Resources());
            components.Add(new BasicParticleRenderer(1024, 1024));

            // set default shader loader
            ShaderProgram.DefaultLoader = new OpenTKExtensions.Loaders.MultiPathFileSystemLoader(SHADERPATH);

        }

        private void ParticleTestBench_Resize(object sender, EventArgs e)
        {
            GL.Viewport(this.ClientRectangle);
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
