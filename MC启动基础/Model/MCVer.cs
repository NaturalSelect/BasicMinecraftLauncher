using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC启动基础.Model
{
    public class MCVer
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
