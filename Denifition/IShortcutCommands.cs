using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GetLargestES
{
    interface IShortcutCommands
    {
        ICommand RemoveSelectionCommand { get; }
        ICommand SelectAllCommand { get; }
        ICommand ResetCommand { get; }
    }
}
