using OpenTKExtensions.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Particulate.ParticleSystem.Operators
{
    public class PosVelColOperator : OperatorComponentBase
    {
        public PosVelColOperator() : base("posvelcol_operator.glsl|vert", "posvelcol_operator.glsl|frag")
        {

        }

    }
}
