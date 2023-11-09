using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class TitleAttribute : Attribute
    {
        private string v;

        public TitleAttribute(string v)
        {
            this.v = v;
        }
    }
}
