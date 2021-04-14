using System;
using System.Collections.Generic;
using System.Text;

namespace SMCISD.Student360.Resources.Infrastructure.ExtensionMethods
{
    public static class ArrayExtensionMethods
    {
        public static bool IsNullOrEmpty(this Array array)
        {
            return (array == null || array.Length == 0);
        }
    }
}
