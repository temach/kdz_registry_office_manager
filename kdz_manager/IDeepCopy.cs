using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kdz_manager
{
    /// <summary>
    /// Class can create a deep copy of itself.
    /// </summary>
    public interface IDeepCopy<T>
    {
        T DeepCopy();
    }
}
