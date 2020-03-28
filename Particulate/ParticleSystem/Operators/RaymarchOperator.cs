using OpenTKExtensions.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Particulate.ParticleSystem.Operators
{
    public class RaymarchOperator : OperatorComponentBase
    {
        public RaymarchOperator() : base("raymarch_operator.glsl|vert", "raymarch_operator.glsl|frag")
        {

        }

    }
}
