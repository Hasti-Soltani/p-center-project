using static p_centerProblem.DistanceMatrix;

namespace p_centerProblem.Solutions;

public static class OptimizedTabuSearch
{
    public static (double, int[]) Solve(double[,] distanceMatrix, int p, int maxIterations, int tabuTenure)
    {
        Random rand = new();
        Dictionary<int, int> tabuAdd = new();
        var n = distanceMatrix.GetLength(0);

        var (bestObjective, currentCenters) = GreedyPCenter.Solve(distanceMatrix, p);
        var bestSolution = currentCenters.ToArray();

        for (var iteration = 0; iteration < maxIterations; iteration++)
        {
            // Remove expired tabu entries
            var expired = tabuAdd.Where(kvp => kvp.Value <= iteration).Select(kvp => kvp.Key).ToList();
            foreach (var center in expired) tabuAdd.Remove(center);

            var (newSolution, newObjective) = FindBestNeighbor(
                currentCenters, n, distanceMatrix, tabuAdd, iteration, bestObjective, rand);

            // Update tabu list with removed center
            var removedCenter = currentCenters.Except(newSolution).Single();
            tabuAdd[removedCenter] = iteration + tabuTenure;

            currentCenters = newSolution;

            if (newObjective < bestObjective)
            {
                bestSolution = newSolution.ToArray();
                bestObjective = newObjective;
            }
        }

        return (bestObjective, bestSolution);
    }

    private static (int[] solution, double objective) FindBestNeighbor(
        int[] currentCenters,
        int n,
        double[,] distances,
        Dictionary<int, int> tabuAdd,
        int currentIteration,
        double bestObjective,
        Random rand)
    {
        var currentSet = new HashSet<int>(currentCenters);
        var bestRegularObj = double.MaxValue;
        var bestTabuObj = double.MaxValue;
        int[] bestRegular = null, bestTabu = null;

        foreach (var oldCenter in currentCenters)
        {
            for (var newCenter = 0; newCenter < n; newCenter++)
            {
                if (currentSet.Contains(newCenter)) continue;

                // Check tabu status
                var isTabu = tabuAdd.TryGetValue(newCenter, out var expire) && expire > currentIteration;

                // Generate neighbor
                var neighbor = currentCenters.Select(c => c == oldCenter ? newCenter : c).ToArray();
                var neighborObj = MaxAssignedDistance(neighbor, distances);

                // Update best candidates
                if (!isTabu)
                {
                    if (neighborObj < bestRegularObj)
                    {
                        bestRegularObj = neighborObj;
                        bestRegular = neighbor;
                    }
                }
                else if (neighborObj < bestTabuObj)
                {
                    bestTabuObj = neighborObj;
                    bestTabu = neighbor;
                }
            }
        }

        // Apply aspiration criteria
        if (bestTabu != null && bestTabuObj < bestObjective && bestTabuObj < bestRegularObj)
        {
            return (bestTabu, bestTabuObj);
        }

        // Return best found solution
        return bestRegular != null
            ? (bestRegular, bestRegularObj)
            : GenerateRandomNeighbor(currentCenters, n, distances, rand);
    }


    private static (int[] solution, double objective) GenerateRandomNeighbor(
        int[] current, int n, double[,] distances, Random rand)
    {
        var neighbor = current.ToArray();
        var index = rand.Next(neighbor.Length);
        int newCenter;

        do newCenter = rand.Next(n);
        while (neighbor.Contains(newCenter));

        neighbor[index] = newCenter;
        return (neighbor, MaxAssignedDistance(neighbor, distances));
    }
}