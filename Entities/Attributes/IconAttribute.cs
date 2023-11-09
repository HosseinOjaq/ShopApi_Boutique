﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class IconAttribute : Attribute
    {
        public IconAttribute(string Icon)
        {
            this.DisplayIcon = Icon;
        }
        public string DisplayIcon { get; set; }
    }
}
