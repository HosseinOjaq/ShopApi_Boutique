using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Identity
{
    public class ActionData
    {
        public string ActionName { get; set; }
        public string ActionNameLocalized { get; set; }
        public string ActionIcon { get; set; }
        public bool AllowAnonymous { get; set; }
        public bool RequiresAuthorization { get; set; }
        public bool RequiresHttpPost { get; set; }
    }
}
