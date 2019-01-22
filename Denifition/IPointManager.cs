using System.Collections.Generic;
using System.Windows;

namespace GetLargestES
{
    interface IPointManager : IShortcutCommandSource
    {
        IEnumerable<PointData> Points { get; }
        UIElement FindContainer(PointData data);
    }
}
