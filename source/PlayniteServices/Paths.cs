using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PlayniteServices
{
    public class Paths
    {
        private static string executingDirectory;
        public static string ExecutingDirectory
        {
            get
            {
                if (executingDirectory == null)
                {
                    executingDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                }

                return executingDirectory;
            }
        }
    }
}
