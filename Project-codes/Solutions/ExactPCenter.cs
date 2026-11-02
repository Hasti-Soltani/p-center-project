namespace p_centerProblem.Solutions;

public static class ExactPCenter
{
    public static (double cost, int[] centers) Solve(double[,] distanceMatrix
        , int p)
    {
        var n = distanceMatrix.GetLength(0);
        var allCombinations = GetCombinations(Enumerable.Range(0, n).ToList(), p);
        var bestCost = double.MaxValue;
        int[] bestCenters = [];

        foreach (var combination in allCombinations)
        {
            var centers = combination.ToArray();
            var cost = DistanceMatrix.MaxAssignedDistance(centers, distanceMatrix);

            if (cost > bestCost) continue;

            bestCost = cost;
            bestCenters = centers;
        }

        return (bestCost, bestCenters);
    }

    private static IEnumerable<List<int>> GetCombinations(List<int> list, int combinationSize)
    {
        while (true)
        {
            if (combinationSize == 0)
            {
                yield return new List<int>();
                yield break;
            }
            if (list.Count < combinationSize)
            {
                yield break;
            }

            var head = list[0];
            var tail = list.Skip(1).ToList();
            
            foreach (var subCombination in GetCombinations(tail, combinationSize - 1))
            {
                var newCombination = new List<int> { head };
                newCombination.AddRange(subCombination);
                yield return newCombination;
            }
            
            list = tail;
        }
    }
}