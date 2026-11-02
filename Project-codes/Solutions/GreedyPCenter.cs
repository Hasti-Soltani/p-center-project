using static p_centerProblem.DistanceMatrix;

namespace p_centerProblem.Solutions;

public static class GreedyPCenter
{
    public static (double cost, int[] centers) Solve(double[,] distanceMatrix, int p)
    {
        var n = distanceMatrix.GetLength(0);
        var centers = new List<int>();
        var used = new HashSet<int>();

        for (var step = 0; step < p; step++)
        {
            var bestMaxDist = double.MaxValue;
            var bestCandidate = -1;

            for (var candidate = 0; candidate < n; candidate++)
            {
                if (used.Contains(candidate)) continue;

                centers.Add(candidate);
                var maxDist = MaxAssignedDistance(centers.ToArray(), distanceMatrix);

                if (maxDist < bestMaxDist)
                {
                    bestMaxDist = maxDist;
                    bestCandidate = candidate;
                }

                centers.Remove(candidate);
            }

            centers.Add(bestCandidate);
            used.Add(bestCandidate);
        }
        
        var totalCost = MaxAssignedDistance(centers.ToArray(), distanceMatrix);
        return (totalCost, centers.ToArray());
    }
}