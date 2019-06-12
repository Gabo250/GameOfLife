using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    /// <summary>
    /// Class of BusinessLogic
    /// </summary>
    public class BusinessLogic
    {
        private Table table;       

        private Table copy;

        private Task[] tasks;       

        private bool start;

        private Stopwatch stop;

        private int ProcessorCount; 

        private int n;       

        /// <summary>
        /// BusinessLogic Initalize
        /// </summary>
        public BusinessLogic()
        {
            Initalize();
        }

        /// <summary>
        /// Gets or set the Game Table
        /// </summary>
        public Table Table
        {
            get { return table; }
            set { table = value; }
        }

        /// <summary>
        /// Gets or set Round Start
        /// </summary>
        public bool Start
        {
            get
            {
                return start;
            }

            set
            {
                start = value;
            }
        }

        /// <summary>
        /// Initalize BusinessLogic
        /// </summary>
        private void Initalize()
        {
            this.table = new Table(160);
            this.copy = new Table(table.Grid.GetLength(0));            
            this.start = false;
            this.ProcessorCount = Environment.ProcessorCount;
            this.n = table.Grid.GetLength(0) / ProcessorCount;
            this.tasks = new Task[ProcessorCount];
            this.stop = new Stopwatch();
        }

        /// <summary>
        /// Start round with ProccessorCount task
        /// </summary>
        public void Starts()
        {
            stop.Start();           
            StartPCopy(copy, table);
            StartRoundTasks();
            StartPCopy(table, copy);
            //StartRoundSequential();           
            Console.WriteLine(stop.ElapsedMilliseconds);
            stop.Reset();
        }

        /// <summary>
        /// Start Round
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="end"></param>
        private void StartParallelRound(int startX, int end)
        {
            int Neighbor;
            for (int i = startX; i < end + startX; i++)
            {
                for (int j = 0; j < table.Grid.GetLength(1); j++)
                {
                    Neighbor = 0;
                    for (int k = -1; k < 2; k++)
                    {
                        for (int u = -1; u < 2; u++)
                        {
                            if (k != 0 || u != 0)
                            {
                                if ((i + k >= 0 && i + k < table.Grid.GetLength(0)) && (j + u >= 0 && j + u < table.Grid.GetLength(1)))
                                {
                                    if (table.Grid[i + k, j + u] == 1)
                                    {
                                        Neighbor++;
                                    }
                                }
                            }
                        }
                    }

                    if (table.Grid[i, j] == 1 && !(Neighbor == 2 || Neighbor == 3))
                    {
                        copy.Grid[i, j] = 0;
                    }

                    if (table.Grid[i, j] == 0 && Neighbor == 3)
                    {
                        copy.Grid[i, j] = 1;
                    }
                }
            }
        }

        /// <summary>
        /// for One Task 
        /// </summary>
        private void StartRoundSequential()
        {
            Copy(copy, table);
            for (int i = 0; i < table.Grid.GetLength(0); i++)
            {                
                for (int j = 0; j < table.Grid.GetLength(1); j++)
                {
                    int neighbor = 0;
                    for (int k = -1; k < 2; k++)
                    {
                        for (int u = -1; u < 2; u++)
                        {
                            if (k != 0 || u != 0)
                            {
                                if ((i + k >= 0 && i + k < table.Grid.GetLength(0)) && (j + u >= 0 && j + u < table.Grid.GetLength(1)))
                                {
                                    if (table.Grid[i + k, j + u] == 1)
                                    {
                                        neighbor++;
                                    }                                   
                                }
                            }
                        }
                    }

                    if (table.Grid[i, j] == 1 && !(neighbor == 2 || neighbor == 3))
                    {
                        copy.Grid[i, j] = 0;
                    }

                    if (table.Grid[i, j] == 0 && neighbor == 3)
                    {
                        copy.Grid[i, j] = 1;
                    }
                }
            }

            Copy(table, copy);      
        }

        /// <summary>
        /// start tasks for round
        /// </summary>
        private void StartRoundTasks()
        {
            int startX = 0;
            int end = n;
            for (int i = 0; i < tasks.Length; i++)
            {
                int ii = startX;
                tasks[i] = new Task(() => StartParallelRound(ii, end)/*, TaskCreationOptions.LongRunning*/);
                tasks[i].Start();
                startX += n;
            }

            Task.WaitAll(tasks);
        }

        /// <summary>
        /// Copy Table grid
        /// </summary>
        /// <param name="t1">to</param>
        /// <param name="t2">from</param>
        private void Copy(Table t1, Table t2)
        {
            for (int i = 0; i < t2.Grid.GetLength(0); i++)
            {
                for (int j = 0; j < t2.Grid.GetLength(1); j++)
                {
                    t1.Grid[i, j] = t2.Grid[i, j];
                }
            }
        }

        /// <summary>
        /// start parallelcopy
        /// </summary>
        private void StartPCopy(Table t1, Table t2)
        {
            int startX = 0;
            int end = n;
            for (int i = 0; i < tasks.Length; i++)
            {
                int ii = startX;
                tasks[i] = new Task(() => ParallelCopy(ii, end, t1, t2)/*, TaskCreationOptions.LongRunning*/);
                tasks[i].Start();
                startX += n;
            }

            Task.WaitAll(tasks);
        }

        /// <summary>
        /// ParellelCopy Table Grid
        /// </summary>
        /// <param name="StartX">startindex</param>
        /// <param name="end">endindex</param>
        /// <param name="t1">to</param>
        /// <param name="t2">from</param>
        private void ParallelCopy(int StartX, int end, Table t1, Table t2)
        {
            for (int i = StartX; i < StartX + end; i++)
            {
                for (int j = 0; j < table.Grid.GetLength(1); j++)
                {
                    t1.Grid[i, j] = t2.Grid[i, j];
                }
            }
        }

        /// <summary>
        /// Cleargrid
        /// </summary>
        public void ClearGrid()
        {
            for (int i = 0; i<Table.Grid.GetLength(0); i++)
            {
               for (int j = 0; j<Table.Grid.GetLength(1); j++)
               {
                   table.Grid[i, j] = 0;
               }
            }
        }

        /// <summary>
        /// Generate Random Population
        /// </summary>
        public void Generate()
        {
            double[,] vmi = new double[Table.Grid.GetLength(0), Table.Grid.GetLength(0)];
           
            
            for (int i = 0; i < vmi.GetLength(0); i++)
            {
                for (int j = 0; j < vmi.GetLength(1); j++)
                {
                    vmi[i, j] = PerlinGenerator.Perlin(i, j);                   
                }                
            }

            Table rand = new Table(table.Grid.GetLength(0));
            Parallel.For(0, vmi.GetLength(0),
                i =>
                {
                    Parallel.For(0, vmi.GetLength(1), j =>
                    {
                        if (vmi[i, j] >= 0.9)
                        {
                            rand.Grid[i, j] = 1;
                        }
                    });
                });          

            StartPCopy(table, rand);            
        }        
    }
}
