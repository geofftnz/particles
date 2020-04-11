using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTKExtensions;
using OpenTKExtensions.Framework;
using OpenTKExtensions.Resources;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Particulate.ParticleSystem.Renderers
{
    /// <summary>
    /// A basic renderer for particles
    /// </summary>
    public class ColourParticleRenderer : GameComponentBase, IRenderable, IUpdateable, IReloadable, ITransformable, IResizeable
    {
        public int DrawOrder { get; set; } = 0;
        public bool Visible { get; set; } = true;
        public Matrix4 ViewMatrix { get; set; } = Matrix4.Identity;
        public Matrix4 ModelMatrix { get; set; } = Matrix4.Identity;
        public Matrix4 ProjectionMatrix { get; set; } = Matrix4.Identity;

        public Func<Texture> ParticlePositionTextureFunc { get; set; } = null;
        public Func<Texture> ParticleColourTextureFunc { get; set; } = null;

        protected BufferObject<Vector2> vertexVBO;
        protected BufferObject<uint> indexVBO;
        protected ReloadableResource<ShaderProgram> program;

        protected string VertexShaderName = "particles_col.glsl|vert";
        protected string FragmentShaderName = "particles_col.glsl|frag";

        protected int width;
        protected int height;

        protected float screenWidth = 800.0f;

        public ColourParticleRenderer(int width = 256, int height = 256)
        {
            this.width = width;
            this.height = height;

            Loading += ColourParticleRenderer_Loading;
            Unloading += ColourParticleRenderer_Unloading;
        }

        private void ColourParticleRenderer_Loading(object sender, EventArgs e)
        {
            InitVBOs();
            program = new ReloadableResource<ShaderProgram>("proghost", () => { return new ShaderProgram("prog", "vertex", "", true, VertexShaderName, FragmentShaderName); }, (sp) => new ShaderProgram(sp));
            Resources.Add(program);

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

            vertexVBO = new BufferObject<Vector2>("vbuf", BufferTarget.ArrayBuffer, BufferUsageHint.StaticDraw, vertex);
            Resources.Add(vertexVBO);

            indexVBO = new BufferObject<uint>("ibuf", BufferTarget.ElementArrayBuffer, BufferUsageHint.StaticDraw, index);
            Resources.Add(indexVBO);
        }

        private void ColourParticleRenderer_Unloading(object sender, EventArgs e)
        {
            program?.Unload();
        }

        public void Update(IFrameUpdateData frameData)
        {

        }

        public void Render(IFrameRenderData frameData)
        {
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.ProgramPointSize);
            GL.PointParameter(PointParameterName.PointSpriteCoordOrigin, (int)PointSpriteCoordOriginParameter.UpperLeft);
            //GL.Enable(EnableCap.VertexProgramPointSize);
            GL.Enable(EnableCap.PointSprite);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);

            ParticlePositionTextureFunc?.Invoke()?.Bind(TextureUnit.Texture0);
            ParticleColourTextureFunc?.Invoke()?.Bind(TextureUnit.Texture1);

            program.Resource.Use()
                .SetUniform("screenFactor", (float)Math.Sqrt(screenWidth / 1280.0))
                .SetUniform("projectionMatrix", ProjectionMatrix)
                .SetUniform("modelMatrix", ModelMatrix)
                .SetUniform("viewMatrix", ViewMatrix)
                .SetUniform("particlePositionTexture", 0)
                .SetUniform("particleColourTexture", 1);
            vertexVBO.Bind(program.Resource.VariableLocations["vertex"]);
            indexVBO.Bind();
            GL.DrawElements(BeginMode.Points, indexVBO.Length, DrawElementsType.UnsignedInt, 0);

        }

        public void Resize(int width, int height)
        {
            this.screenWidth = width;
        }

        public void Reload()
        {
            Resources.Reload();
        }
    }
}
