using System;
using System.Threading.Tasks;
using System.Windows;

namespace G19LCDTest_WPF
{
    class LifeGame
    {
        private int[,] grid;

        private bool[,] currentGen;
        private bool[,] nextGen;

        public Point GridSize { get; private set; }
        public int Generation { get; private set; }

        private Task processTask;

        public bool this[int x, int y]
        {
            get { return this.currentGen[x, y]; }
            set { this.currentGen[x, y] = value; }
        }

        public bool ToggleCell(int x, int y)
        {
            bool currentValue = this.currentGen[x, y];
            return this.currentGen[x, y] = !currentValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cellSize"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public LifeGame(int cellSize, int width, int height)
        {
            if (cellSize <= 0) throw new DivideByZeroException("Cell size must be larger than 0");



            int X = width / cellSize;
            int Y = height / cellSize;

            Point size = new Point(X, Y);
            GridSize = size;
            grid = new int[X, Y];

            currentGen = new bool[X, Y];
            nextGen = new bool[X, Y];

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        public void SetAllState(bool state)
        {
            for (int x = 0; x < GridSize.X; x++)
            {
                for (int y = 0; y < GridSize.Y; y++)
                {
                    currentGen[x, y] = state;
                }
            }
            Generation = 0;
        }


        /// <summary>
        /// 
        /// </summary>
        public void ProcessNextGen()
        {
            if (this.processTask != null && this.processTask.IsCompleted)
            {
                // when a generation has completed
                // now flip the back buffer so we can start processing on the next generation
                var flip = this.nextGen;
                this.nextGen = this.currentGen;
                this.currentGen = flip;
                Generation++;

                // begin the next generation's processing asynchronously
                this.processTask = this.ProcessGeneration();
                waitForTask();

            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void ProcessFirstGen()
        {
            if (this.processTask == null || (this.processTask != null && this.processTask.IsCompleted))
            {
                // only begin the generation if the previous process was completed
                this.processTask = this.ProcessGeneration();
                waitForTask();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool isBusy
        {
            get
            {
                //If there is a task and it's has not completed yet, then the simulator is busy.
                return this.processTask != null && !this.processTask.IsCompleted;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="gen"></param>
        /// <param name="size"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int getNeighborCount(bool[,] gen, Point size, int x, int y)
        {
            int result = 0;

            for (int ix = -1; ix < 2; ix++)
            {
                int xi = x + ix;
                for (int iy = -1; iy < 2; iy++)
                {
                    if (!(ix == 0 && iy == 0))
                    {
                        int yi = y + iy;
                        if ((xi >= 0 && yi >= 0) && (xi < size.X && yi < size.Y))
                        {
                            //xi is not less than 0 or more than GridSize.X and dito on yi
                            result += gen[xi, yi] ? 1 : 0;
                        }
                    }

                }
            }
            return result;
        }


        /// <summary>
        /// 
        /// </summary>

        private Task ProcessGeneration()
        {
            return Task.Factory.StartNew(() =>
            {
                Parallel.For(0, (int)GridSize.X, x =>
                {
                    Parallel.For(0, (int)GridSize.Y, y =>
                    {
                        int numberOfNeighbors = getNeighborCount(currentGen, GridSize, x, y);

                        bool shouldLive = false;
                        bool isAlive = currentGen[x, y];

                        if (isAlive && (numberOfNeighbors == 2 || numberOfNeighbors == 3)) //Should stay alive
                        {
                            shouldLive = true;
                        }
                        else if (!isAlive && numberOfNeighbors == 3) // New cell is born
                        {
                            shouldLive = true;
                        }

                        nextGen[x, y] = shouldLive;

                    });
                });
            });
        }


        /// <summary>
        /// 
        /// </summary>
        private void waitForTask()
        {
            if (this.processTask != null) //New task as successfully created!
            {
                this.processTask.Wait(); //Do not return until the task is complete.
            }
        }



    }
}
