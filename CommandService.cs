using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace GetLargestES
{
    sealed class CommandService : Behavior<PointSetPresenter>
    {
        #region RemoveSelectionCommand

        public ICommand RemoveSelectionCommand
        {
            get { return (ICommand)GetValue(RemoveSelectionCommandProperty); }
        }

        static readonly DependencyPropertyKey RemoveSelectionCommandPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "RemoveSelectionCommand",
                typeof(ICommand),
                typeof(CommandService),
                new PropertyMetadata());

        public static readonly DependencyProperty RemoveSelectionCommandProperty =
            RemoveSelectionCommandPropertyKey.DependencyProperty;

        private void SetRemoveSelectionCommand(ICommand value)
        {
            SetValue(RemoveSelectionCommandPropertyKey, value);
        }

        #endregion RemoveSelectionCommand


    }
}
