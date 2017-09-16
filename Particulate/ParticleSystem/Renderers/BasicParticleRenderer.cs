using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTKExtensions;
using OpenTKExtensions.Framework;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Particulate.ParticleSystem.Renderers
{
    /// <summary>
    /// A basic renderer for particles
    /// </summary>
    public class BasicParticleRenderer : GameComponentBase, IRenderable, IUpdateable, IReloadable, ITransformable
    {
        public int DrawOrder { get; set; } = 0;
        public bool Visible { get; set; } = true;
        public Matrix4 ViewMatrix { get; set; } = Matrix4.Identity;
        public Matrix4 ModelMatrix { get; set; } = Matrix4.Identity;
        public Matrix4 ProjectionMatrix { get; set; } = Matrix4.Identity;


        protected VBO vertexVBO = new VBO("basicparticles_v");
        protected VBO indexVBO = new VBO("basicparticles_i", BufferTarget.ElementArrayBuffer);
        protected ShaderProgram program = new ShaderProgram("basicparticles_sp");

        protected string VertexShaderName = "particles.glsl|vert";
        protected string FragmentShaderName = "particles.glsl|frag";

        protected int width;
        protected int height;

        public BasicParticleRenderer(int width = 256, int height = 256)
        {
            this.width = width;
            this.height = height;

            Loading += BasicParticleRenderer_Loading;
            Unloading += BasicParticleRenderer_Unloading;
        }

        private void BasicParticleRenderer_Loading(object sender, EventArgs e)
        {
            InitVBOs();
            Reload();
        }

        private void InitVBOs()
        {
            // Create vertex and index buffers for our particles.
            // These point to texels in the textures that store particle position and attributes.

            Vector2[] vertex = new Vector2[width * height];
            uint[] index = new uint[width * height];

            int i = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    vertex[i] = new Vector2((float)x / (float)width, (float)y / (float)height);
                    index[i] = (uint)i;
                    i++;
                }
            }

            vertexVBO.SetData(vertex);
            indexVBO.SetData(index);
        }

        private void BasicParticleRenderer_Unloading(object sender, EventArgs e)
        {
            program?.Unload();
        }


        public void Reload()
        {
            this.ReloadShader(this.LoadShader, this.SetShader, log);
        }

        private ShaderProgram LoadShader()
        {
            var program = new ShaderProgram(this.GetType().Name);
            program.Init(
                VertexShaderName,
                FragmentShaderName,
                new List<Variable>
                {
                    new Variable(0, "vertex")
                });
            return program;
        }

        private void SetShader(ShaderProgram newprogram)
        {
            if (this.program != null)
            {
                this.program.Unload();
            }
            this.program = newprogram;
        }

        public void Update(IFrameUpdateData frameData)
        {
            
        }

        public void Render(IFrameRenderData frameData)
        {
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.ProgramPointSize);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One);


            program.UseProgram()
                .SetUniform("projectionMatrix", ProjectionMatrix)
                .SetUniform("modelMatrix", ModelMatrix)
                .SetUniform("viewMatrix", ViewMatrix);
            vertexVBO.Bind(this.program.VariableLocation("vertex"));
            indexVBO.Bind();
            GL.DrawElements(BeginMode.Points, indexVBO.Length, DrawElementsType.UnsignedInt, 0);

        }
    }
}
