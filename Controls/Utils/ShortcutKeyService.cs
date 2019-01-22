using Runerback.Utils.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace GetLargestES
{
    sealed class ShortcutKeyService : Behavior<PointCanvas>
    {
        public ShortcutKeyService()
        {
        }

        private readonly List<InputBinding> inputBindings = new List<InputBinding>();

        protected override void OnAttached()
        {
            var target = AssociatedObject;
            var window = Window.GetWindow(target);
            if (window == null)
            {
                Console.WriteLine("Window no found");
                return;
            }

            var commands = new Commands(target);
            inputBindings.Add(new KeyBinding(
                commands.RemoveSelectionCommand,
                new KeyGesture(Key.Delete)));
            inputBindings.Add(new KeyBinding(
                commands.SelectAllCommand,
                new KeyGesture(Key.A, ModifierKeys.Control)));
            inputBindings.Add(new KeyBinding(
                commands.ResetCommand,
                new KeyGesture(Key.R, ModifierKeys.Alt)));

            window.InputBindings.AddRange(inputBindings);
        }

        protected override void OnDetaching()
        {
            var target = AssociatedObject;
            var window = Window.GetWindow(target);
            if (window == null)
                return;

            var activeInputBindings = window.InputBindings;
            foreach (var item in inputBindings)
                activeInputBindings.Remove(item);
            inputBindings.Clear();
        }

        sealed class Commands : IShortcutCommands
        {
            private readonly IShortcutCommandSource source;

            private Commands()
            {
                removeSelectionCommand = new SimpleCommand(RemoveSelection);
                selectAllCommand = new SimpleCommand(SelectAll);
                resetCommand = new SimpleCommand(Reset);
            }

            public Commands(IShortcutCommandSource source) : this()
            {
                this.source = source ?? throw new ArgumentNullException(nameof(source));
            }

            private readonly SimpleCommand removeSelectionCommand;
            public ICommand RemoveSelectionCommand => removeSelectionCommand;

            private void RemoveSelection(object obj)
            {
                source.RemoveSelection();
            }

            private readonly SimpleCommand selectAllCommand;
            public ICommand SelectAllCommand => selectAllCommand;

            private void SelectAll(object obj)
            {
                source.SelectAll();
            }

            private readonly SimpleCommand resetCommand;
            public ICommand ResetCommand => resetCommand;

            private void Reset(object obj)
            {
                source.Reset();
            }
        }
    }
}
