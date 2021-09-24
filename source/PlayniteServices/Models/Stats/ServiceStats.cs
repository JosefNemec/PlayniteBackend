using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayniteServices.Models.Stats
{
    public class ServiceStats
    {
        public int UserCount;
        public int LastWeekUserCount;
        public SortedDictionary<string, int> UsersByVersion = new SortedDictionary<string, int>();
        public SortedDictionary<string, int> UsersByWinVersion = new SortedDictionary<string, int>();
        public int X86Count;
        public int X64Count;
    }
}
