using OpenTK.Graphics.OpenGL4;
using OpenTKExtensions.Framework;
using OpenTKExtensions.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Particulate.ParticleSystem.Targets
{
    public class PosVelColRenderTarget : RenderTargetBase
    {

        public PosVelColRenderTarget(int width, int height) : base(false, false, width, height)
        {
            SetOutput(0,
                new TextureSlotParam(TextureTarget.Texture2D, PixelInternalFormat.Rgba32f, PixelFormat.Rgba, PixelType.Float, false,
                    TextureParameter.Create(TextureParameterName.TextureMagFilter, TextureMagFilter.Nearest),
                    TextureParameter.Create(TextureParameterName.TextureMinFilter, TextureMinFilter.Nearest),
                    TextureParameter.Create(TextureParameterName.TextureWrapS, TextureWrapMode.Repeat),
                    TextureParameter.Create(TextureParameterName.TextureWrapT, TextureWrapMode.Repeat)
                ));
            SetOutput(1,
                new TextureSlotParam(TextureTarget.Texture2D, PixelInternalFormat.Rgba32f, PixelFormat.Rgba, PixelType.Float, false,
                    TextureParameter.Create(TextureParameterName.TextureMagFilter, TextureMagFilter.Nearest),
                    TextureParameter.Create(TextureParameterName.TextureMinFilter, TextureMinFilter.Nearest),
                    TextureParameter.Create(TextureParameterName.TextureWrapS, TextureWrapMode.Repeat),
                    TextureParameter.Create(TextureParameterName.TextureWrapT, TextureWrapMode.Repeat)
                ));
            SetOutput(2,
                new TextureSlotParam(TextureTarget.Texture2D, PixelInternalFormat.Rgba32f, PixelFormat.Rgba, PixelType.Float, false,
                    TextureParameter.Create(TextureParameterName.TextureMagFilter, TextureMagFilter.Nearest),
                    TextureParameter.Create(TextureParameterName.TextureMinFilter, TextureMinFilter.Nearest),
                    TextureParameter.Create(TextureParameterName.TextureWrapS, TextureWrapMode.Repeat),
                    TextureParameter.Create(TextureParameterName.TextureWrapT, TextureWrapMode.Repeat)
                ));
        }

    }
}
