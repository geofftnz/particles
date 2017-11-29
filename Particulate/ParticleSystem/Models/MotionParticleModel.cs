using OpenTKExtensions.Framework;
using OpenTKExtensions.Resources;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Particulate.ParticleSystem.Models
{
    /// <summary>
    /// Exists to hold the double-buffered textures for a particle system
    /// </summary>
    public class MotionParticleModel : GameComponentBase, IRenderable
    {
        public int Width { get; protected set; }
        public int Height { get; protected set; }

        public bool Visible { get; set; } = true;
        public int DrawOrder { get; set; } = int.MaxValue;

        public Texture ParticlePositionRead { get { return particlePosition.ReadTexture; } }
        public Texture ParticlePositionWrite { get { return particlePosition.WriteTexture; } }

        public Texture ParticleVelocityRead { get {  return particleVelocity.ReadTexture; } }
        public Texture ParticleVelocityWrite { get { return particleVelocity.WriteTexture; } }

        public Texture ParticleColourRead { get {  return particleColour.ReadTexture; } }
        public Texture ParticleColourWrite { get { return particleColour.WriteTexture; } }

        private DoubleBufferedTexture particlePosition;
        private DoubleBufferedTexture particleVelocity;
        private DoubleBufferedTexture particleColour;

        public MotionParticleModel(int width, int height) : base()
        {
            Width = width;
            Height = height;

            Resources.Add(particlePosition = new DoubleBufferedTexture("particlepos", Width, Height, TextureTarget.Texture2D, PixelInternalFormat.Rgba32f, PixelFormat.Rgba, PixelType.Float, Texture.Params().FilterNearest().ClampToEdge().ToArray()));

            Resources.Add(particleVelocity = new DoubleBufferedTexture("particlevel", Width, Height, TextureTarget.Texture2D, PixelInternalFormat.Rgba32f, PixelFormat.Rgba, PixelType.Float, Texture.Params().FilterNearest().ClampToEdge().ToArray()));

            Resources.Add(particleColour = new DoubleBufferedTexture("particlecol", Width, Height, TextureTarget.Texture2D, PixelInternalFormat.Rgba32f, PixelFormat.Rgba, PixelType.Float, Texture.Params().FilterNearest().ClampToEdge().ToArray()));
        }

        public void Render(IFrameRenderData frameData)
        {
            // we're not actually drawing anything, but we'll swap the double-buffered textures
            particlePosition.Swap();
            particleVelocity.Swap();
            particleColour.Swap();
        }
    }
}
