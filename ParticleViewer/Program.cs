using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParticleViewer
{
    static class Program
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        static void Main()
        {
            log.Info("Particles START");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Startup());

            try
            {
                using (var v = new ParticleTestBench())
                {
                    v.Run(60);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "uncaught exception");
                throw;
            }


            log.Info("Particles END");
        }
    }
}

