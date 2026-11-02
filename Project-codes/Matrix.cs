namespace p_centerProblem
{
    public class DemandPoint(double x, double y)
    {
        public double X { get; } = x;
        public double Y { get; } = y;

        public double GetDistance(DemandPoint other)
        {
            var dx = X - other.X;
            var dy = Y - other.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }

    public static class DistanceMatrix
    {
        public static double[,] GenerateMatrix(List<DemandPoint> points)
        {
            var n = points.Count;
            var matrix = new double[n, n];
            for (var i = 0; i < n; i++)
            {
                for (var j = i; j < n; j++)
                {
                    if (i == j)
                    {
                        matrix[i, j] = 0;
                    }
                    else
                    {
                        matrix[i, j] = matrix[j, i] = points[i].GetDistance(points[j]);
                    }
                }
            }

            return matrix;
        }

        public static double MaxAssignedDistance(int[] centers, double[,] distanceMatrix)
        {
            var n = distanceMatrix.GetLength(0);
            double maxDistance = 0;
            var demandPointToNearestCenter = AssignPointsToCenters(centers, distanceMatrix);

            for (var i = 0; i < n; i++)
            {
                maxDistance = Math.Max(maxDistance, distanceMatrix[demandPointToNearestCenter[i], i]);
            }

            return maxDistance;
        }

        private static int[] AssignPointsToCenters(int[] centers, double[,] distanceMatrix)
        {
            var n = distanceMatrix.GetLength(0);
            var assignments = new int[n];

            for (var i = 0; i < n; i++)
            {
                var closestCenter = -1;
                var minDistance = double.MaxValue;


                foreach (var center in centers)
                {
                    if (!(distanceMatrix[i, center] < minDistance)) continue;

                    minDistance = distanceMatrix[i, center];
                    closestCenter = center;
                }

                assignments[i] = closestCenter;
            }

            return assignments;
        }
    }
}