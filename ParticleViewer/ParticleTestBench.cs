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

namespace ParticleViewer
{
    public class ParticleTestBench : GameWindow
    {
        private GameComponentCollection components = new GameComponentCollection();

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

            components.Add(new BasicParticleRenderer());

        }

        private void ParticleTestBench_Resize(object sender, EventArgs e)
        {
            components.Resize(ClientRectangle.Width, ClientRectangle.Height);
        }

        private void ParticleTestBench_Unload(object sender, EventArgs e)
        {
            components.Unload();
        }

        private void ParticleTestBench_Load(object sender, EventArgs e)
        {
            components.Load();
        }

        private void ParticleTestBench_RenderFrame(object sender, FrameEventArgs e)
        {

            GL.ClearColor(0.0f, 0.1f, 0.4f, 1.0f);
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

        }
    }
}
