using System;

namespace Entities.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class IgnorePermissionCheckAttribute : Attribute
    {
    }
}
