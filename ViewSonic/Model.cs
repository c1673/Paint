using Microsoft.CSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ViewSonic
{
    public class CanvasShape
    {
        public CanvasShape(Shape s)
        {
            Shape = s;
        }

        public Shape Shape { get; set; }
    }
}
