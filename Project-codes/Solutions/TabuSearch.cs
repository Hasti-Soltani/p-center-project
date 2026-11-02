namespace p_centerProblem.Solutions;

public static class TabuSearch
{
    public static (double, int[]) Solve(double[,] distanceMatrix, int p, int maxIterations
        , int tabuTenure)
    {
        Random rand = new();
        List<int[]> tabuList = [];
        var n = distanceMatrix.GetLength(0);
        var (bestObjective, currentCenters) = GreedyPCenter.Solve(distanceMatrix, p);
        var bestSolution = currentCenters.ToArray();
        

        for (var iteration = 0; iteration < maxIterations; iteration++)
        {
            (currentCenters, var currentObjective) =
                FindBestNeighbor(currentCenters, n, distanceMatrix, tabuList, rand);

            tabuList.Add(currentCenters.ToArray());
            if (tabuList.Count > tabuTenure)
            {
                tabuList.RemoveAt(0);
            }
            
            if (!(currentObjective < bestObjective)) continue;

            bestSolution = currentCenters.ToArray();
            bestObjective = currentObjective;
        }

        return (bestObjective, bestSolution);
    }

    private static Tuple<int[], double> FindBestNeighbor(
        int[] currentCenters,
        int n,
        double[,] distanceMatrix,
        List<int[]> tabuList,
        Random random)
    {
        var p = currentCenters.Length;
        var bestNeighborObjective = double.MaxValue;
        int[] bestNeighbor = null;

        foreach (var centerToReplace in currentCenters)
        {
            for (var newCenter = 0; newCenter < n; newCenter++)
            {
                if (currentCenters.Contains(newCenter)) continue;

                var candidateSolution = currentCenters.ToArray();
                for (var i = 0; i < p; i++)
                {
                    if (candidateSolution[i] == centerToReplace)
                    {
                        candidateSolution[i] = newCenter;
                        break;
                    }
                }

                var candidateObjective =
                    DistanceMatrix.MaxAssignedDistance(candidateSolution, distanceMatrix);

                if (candidateObjective < bestNeighborObjective && !IsTabu(candidateSolution, tabuList))
                {
                    bestNeighbor = candidateSolution;
                    bestNeighborObjective = candidateObjective;
                }
            }
        }

        if (bestNeighbor != null) return new Tuple<int[], double>(bestNeighbor, bestNeighborObjective);

        bestNeighbor = currentCenters.ToArray();
        bestNeighbor[random.Next(p)] = random.Next(n);
        bestNeighborObjective = DistanceMatrix.MaxAssignedDistance(bestNeighbor, distanceMatrix);

        return new Tuple<int[], double>(bestNeighbor, bestNeighborObjective);
    }

    private static bool IsTabu(int[] candidateSolution, List<int[]> tabuList)
    {
        return tabuList.Any(tabuSolution =>
            tabuSolution.OrderBy(x => x).SequenceEqual(candidateSolution.OrderBy(x => x)));
    }
}