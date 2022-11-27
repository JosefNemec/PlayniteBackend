using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PlayniteServices
{
    public class ServicePaths
    {
        public static string ExecutingDirectory { get; }

        static ServicePaths()
        {
            ExecutingDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }
    }
}
