using System;
using System.Collections.Generic;

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
