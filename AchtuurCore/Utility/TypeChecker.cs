using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchtuurCore.Utility
{
    internal static class TypeChecker
    {

        public static bool isType<T>(object obj)
        {
            return obj.GetType() == typeof(T);
        }

    }
}
