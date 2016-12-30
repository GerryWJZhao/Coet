using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coet.Server.Infrastructure
{
    class CoetArry
    {
        public static List<List<T>> splitList<T>(List<T> list, int partCount)
        {
            int totalCount = list.Count;
            List<List<T>> total = new List<List<T>>();
            int c = totalCount / partCount;
            if (c < 1)
            {
                total.Add(list);
            }
            else
            {
                int y = totalCount % partCount;
                if (y > 0)
                {
                    partCount++;
                }
                for (int i = 0; i < partCount; i++)
                {
                    List<T> r = list.Skip(c * i).Take(c).ToList();
                    total.Add(r);
                }
            }
            return total;
        }
    }
}
