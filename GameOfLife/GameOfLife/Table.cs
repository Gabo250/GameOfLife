using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    /// <summary>
    /// Class of LifeSpace
    /// </summary>
    public class Table
    {
        private int[,] grid;       

        /// <summary>
        /// Table cons
        /// </summary>
        /// <param name="width">width of table</param>
        public Table(int width)
        {
            grid = new int[width, width];
        }

        /// <summary>
        /// Gets or set the grid
        /// </summary>
        public int[,] Grid
        {
            get { return grid; }
            set { grid = value; }
        }
    }
}
