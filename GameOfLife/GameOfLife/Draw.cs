using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace GameOfLife
{
    /// <summary>
    /// Display Class
    /// </summary>
    public class Draw : FrameworkElement
    {
        private BusinessLogic Logic;

        private RectangleGeometry[,] LifeSpace;       

        private RectangleGeometry AutoGenerate;

        private RectangleGeometry Start;

        private RectangleGeometry Stop;

        private RectangleGeometry Clear;

        private DispatcherTimer Timer;

        private int velocx = 0;

        private int velocy = 0;

        private const int SIZE = 5;

        /// <summary>
        /// Display Initalize
        /// </summary>
        public Draw()
        {
            Logic = new BusinessLogic();            
            Initalize();
            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            Timer.Tick += Timer_Tick;
            this.MouseLeftButtonDown += Draw_MouseLeftButtonDown;
            this.MouseRightButtonDown += Draw_MouseRightButtonDown;            
        }        

        /// <summary>
        /// Render
        /// </summary>
        /// <param name="drawingContext">draw</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
             //Living space draw
             if (LifeSpace != null)
             {
               
                for (int i = 0; i < LifeSpace.GetLength(0); i++)
                {
                    for (int j = 0; j < LifeSpace.GetLength(1); j++)
                    {
                       if (Logic != null && Logic.Table.Grid[i,j] == 0)
                       {
                           drawingContext.DrawGeometry(Brushes.White, new Pen(Brushes.Gray, 1), LifeSpace[i, j]);
                       }
                       else if (Logic != null && Logic.Table.Grid[i, j] == 1)
                       {
                           drawingContext.DrawGeometry(Brushes.Black, new Pen(Brushes.Gray, 1), LifeSpace[i, j]);
                       }
                    }
                }               
                //Autogenerate button draw
                drawingContext.DrawGeometry(Brushes.Gray, new Pen(Brushes.DimGray, 2), AutoGenerate);
                drawingContext.DrawText(new FormattedText("AutoGenerate", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Tahoma"), 15, Brushes.Black), new Point(velocx + 255, 95));
                //Start button draw
                drawingContext.DrawGeometry(Brushes.Gray, new Pen(Brushes.DimGray, 2), Start);
                drawingContext.DrawText(new FormattedText("Start", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Tahoma"), 15, Brushes.Black), new Point(velocx + 283, 165));
                //Stop button draw
                drawingContext.DrawGeometry(Brushes.Gray, new Pen(Brushes.DimGray, 2), Stop);
                drawingContext.DrawText(new FormattedText("Stop", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Tahoma"), 15, Brushes.Black), new Point(velocx + 283, 235));
                //Clear button draw
                drawingContext.DrawGeometry(Brushes.Gray, new Pen(Brushes.DimGray, 2), Clear);
                drawingContext.DrawText(new FormattedText("Clear", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Tahoma"), 15, Brushes.Black), new Point(velocx + 283, 305));
            }            
        }

        /// <summary>
        /// Initalize varriables
        /// </summary>
        private void Initalize()
        {
            LifeSpace = new RectangleGeometry[Logic.Table.Grid.GetLength(0), Logic.Table.Grid.GetLength(0)];            
            for (int i = 0; i < LifeSpace.GetLength(0); i++)
            {
                velocy = 0;
                for (int j = 0; j < LifeSpace.GetLength(1); j++)
                {
                    LifeSpace[i, j] = new RectangleGeometry(new Rect(i + velocx, j + velocy, SIZE, SIZE));
                    velocy += SIZE;
                }

                velocx += SIZE;
            }
           
            AutoGenerate = new RectangleGeometry(new Rect(velocx + 250, 80, 100, 50));
            Start = new RectangleGeometry(new Rect(velocx + 250, 150, 100, 50));
            Stop = new RectangleGeometry(new Rect(velocx + 250, 220, 100, 50));
            Clear = new RectangleGeometry(new Rect(velocx + 250, 290, 100, 50));
        }

        private void Draw_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Logic.Start)
            {
                Point mpoint = e.GetPosition(this);
                for (int i = 0; i < LifeSpace.GetLength(0); i++)
                {
                    for (int j = 0; j < LifeSpace.GetLength(1); j++)
                    {
                        if (LifeSpace[i, j].Bounds.Contains(mpoint))
                        {
                            Logic.Table.Grid[i, j] = 0;
                        }
                    }
                }

                InvalidateVisual();
            }               
        }

        private void Draw_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Point mpoint = e.GetPosition(this);
            if (!Logic.Start)
            {                
                for (int i = 0; i < LifeSpace.GetLength(0); i++)
                {
                    for (int j = 0; j < LifeSpace.GetLength(1); j++)
                    {
                        if (LifeSpace[i, j].Bounds.Contains(mpoint))
                        {
                            Logic.Table.Grid[i, j] = 1;
                        }
                    }
                }

                if (Start.Bounds.Contains(mpoint))
                {
                    Logic.Start = true;
                    Timer.Start();
                    Logic.Starts();
                }

                if (AutoGenerate.Bounds.Contains(mpoint))
                {
                    Logic.Generate();
                }

                if (Clear.Bounds.Contains(mpoint))
                {
                    Logic.ClearGrid();
                }
            }

            if (Stop.Bounds.Contains(mpoint) && Logic.Start)
            {
                Logic.Start = false;
                Timer.Stop();
            }

            InvalidateVisual();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {           
            Logic.Starts();
            InvalidateVisual();
        }        
    }
}
