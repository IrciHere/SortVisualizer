using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SortVisualizer
{
    class SortBlocks
    {
        public List<int> blocks = new List<int>();

        //constructor
        public SortBlocks(int howMany, MainWindow mainWindow)
        {
            Random rnd = new Random();
            for (int i = 0; i < howMany; i++)
            {
                blocks.Add(rnd.Next(1, 100));
            }

            mainWindow.Visualize(blocks);
        }
    }
}
