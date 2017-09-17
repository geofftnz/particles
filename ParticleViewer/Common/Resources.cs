using OpenTKExtensions.Camera;
using OpenTKExtensions.Components;
using OpenTKExtensions.Framework;
using OpenTKExtensions.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParticleViewer.Common
{
    /// <summary>
    /// Wrapper for common resources, such as fonts
    /// </summary>
    public class Resources : CompositeGameComponent
    {
        public Font fontLucidaConsole { get; private set; }
        public FrameCounter frameCounter { get; private set; }
        public TextManager text { get; private set; }
        //public ICamera camera { get; private set; }


        public Resources()
        {
            Components.Add(fontLucidaConsole = new Font(@"Resources\Font\lucon.ttf_sdf.1024.png", @"Resources\Font\lucon.ttf_sdf.1024.txt"), 1);
            Components.Add(frameCounter = new FrameCounter(fontLucidaConsole) { TextSize = 0.0006f}, 2);
            Components.Add(text = new TextManager("tm", fontLucidaConsole), 2);
            //Components.Add(camera = new WalkCamera())
        }

    }
}
