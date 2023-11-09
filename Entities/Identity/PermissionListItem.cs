﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Identity
{
    public class PermissionListItem
    {
        public string PermissiongGroupName { get; set; }
        public string PermissiongGroupNamespace { get; set; }
        public bool PermissiongGroupRequiresAuthorization { get; set; }
        public string PersmissionName { get; set; }
        public bool PermissiongRequiresAuthorization { get; set; }
        public bool PermissionAllowAnonymous { get; set; }
        public string ActionFullName { get; set; }
    }
}
