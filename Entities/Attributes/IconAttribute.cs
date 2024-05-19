using System;

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
