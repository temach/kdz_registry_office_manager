using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kdz_manager
{
    interface IFromMapDataRow<T>
    {
        T FromMapDataRow(MapDataRow input);
    }
}
