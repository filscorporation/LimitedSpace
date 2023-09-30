using System;
using System.Collections.Generic;

namespace SteelCustom.MapSystem
{
    public static class PathFinder<T> where T : class, IMapElement
    {
        public static List<T> FindPath(T inElement, T outElement, T[,] grid, bool ignoreOutNotFree = false)
        {
            T target = inElement;
            int size = grid.GetLength(0);
            int stopper = 0;
            
            List<T> openList = new List<T>();
            List<T> closeList = new List<T>();

            openList.Add(inElement);
            inElement.List = 1;
            grid[inElement.X, inElement.Y].List = 1;

            while (!target.Equals(outElement) && stopper < size * size
                && openList.Count > 0 && closeList.Count < size * size)
            {
                stopper++;
                //Refresh(OpenList);
                //Refresh(CloseList);
                target = openList[0];
                
                foreach (T cl in openList)
                {
                    if (target.FValue > cl.FValue)
                    {
                        target = grid[cl.X, cl.Y];
                    }
                }
                
                openList.Remove(target);
                closeList.Add(target);
                target.List = 2;
                grid[target.X, target.Y].List = 2;

                for (int i = 1; i >= -1; i--)
                {
                    for (int j = 1; j >= -1; j--)
                    {
                        if (/*(i == 0 && j != 0) || (i != 0 && j == 0)*/// No diagonals
                            i != 0 || j != 0)
                        {
                            if (target.X + i >= size || target.X + i < 0
                            || target.Y + j >= size || target.Y + j < 0)
                            {
                                
                            }
                            else
                            {
                                if ((grid[target.X + i, target.Y + j].Passable
                                     || ignoreOutNotFree
                                     && grid[target.X + i, target.Y + j].Equals(outElement))
                                && grid[target.X + i, target.Y + j].List != 2)
                                {
                                    if (grid[target.X + i, target.Y + j].List == 1)
                                    {
                                        if (grid[target.X + i, target.Y + j].GValue
                                        > target.GValue + 10)
                                        {
                                            grid[target.X + i, target.Y + j].ParentX = target.X;
                                            grid[target.X + i, target.Y + j].ParentZ = target.Y;
                                            
                                            int zero = i * j;
                                            FindValue(grid[target.X + i, target.Y + j], target, outElement, zero);
                                        }
                                    }
                                    else
                                    {
                                        openList.Add(grid[target.X + i, target.Y + j]);
                                        grid[target.X + i, target.Y + j].List = 1;

                                        int zero = i * j;
                                        FindValue(grid[target.X + i, target.Y + j], target, outElement, zero);
                                        
                                        grid[target.X + i, target.Y + j].ParentX = target.X;
                                        grid[target.X + i, target.Y + j].ParentZ = target.Y;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (stopper >= size * size || openList.Count < 1 || closeList.Count >= size * 5)
            {
                // Dead end
                CleanList(closeList);
                CleanList(openList);
                return new List<T>();
            }

            List<T> path = new List<T>();
            int m = 0;

            outElement = grid[outElement.X, outElement.Y];

            path.Add(outElement);
            while (!inElement.Equals(outElement) && m < size)
            {
                m++;

                outElement = grid[outElement.ParentX, outElement.ParentZ];
                path.Add(outElement);
            }

            path.Reverse();

            CleanList(closeList);
            CleanList(openList);

            return path;
        }

        private static void CleanList(IEnumerable<T> list)
        {
            foreach (T element in list)
            {
                element.List = 0;
                element.FValue = 0;
            }
        }

        private static void FindValue(T element, T inT, T outT)
        {
            element.HValue = (Math.Abs(outT.X - element.X) + Math.Abs(outT.Y - element.Y)) * 10;
            element.GValue = inT.GValue + 10;
            element.FValue = element.HValue + element.GValue;
        }

        private static void FindValue(T element, T inT, T outT, int zero)
        {
            element.HValue = (Math.Abs(outT.X - element.X) + Math.Abs(outT.Y - element.Y)) * 10;
            if (zero == 0)
            {
                element.GValue = inT.GValue + 10; //straight
            }
            else
            {
                element.GValue = inT.GValue + 14; //diagonally
            }
            element.FValue = element.HValue + element.GValue;
        }
    }
}