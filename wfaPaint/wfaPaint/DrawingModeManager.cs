using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wfaPaint
{
    internal class DrawingModeManager
    {
        public enum MyDrawMode
        {
            Pencil,
            Line,
            Ellipse,
            Rectangle,
            Triangle,
            Arrow,
            Star,
            Hexagon,
            Select
        }

        public MyDrawMode CurrentMode { get; private set; } = MyDrawMode.Pencil;

        public void SetMode(MyDrawMode mode)
        {
            CurrentMode = mode;
        }
    }
}
