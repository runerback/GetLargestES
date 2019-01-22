using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetLargestES
{
    interface IShortcutCommandSource
    {
        void RemoveSelection();
        void SelectAll();
        void Reset();
    }
}
