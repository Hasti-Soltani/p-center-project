using static p_centerProblem.DistanceMatrix;

namespace p_centerProblem.Solutions;

public static class SimulatedAnnealingPCenter
{
    public static (double cost, int[] centers) Solve(
        double[,] distanceMatrix,
        int p,
        double initialTemperature = 200,
        double coolingRate = 0.99,
        int maxIterations = 2000
    )

    {
        var n = distanceMatrix.GetLength(0);
        var maxNoImprovementIterations = maxIterations / 3;
        var rand = new Random();
        
        var currentCenters = new int[p];
        var usedCenters = new HashSet<int>();
        for (var i = 0; i < p; i++)
        {
            int randomCenter;
            do
            {
                randomCenter = rand.Next(n);
            } while (usedCenters.Contains(randomCenter));

            currentCenters[i] = randomCenter;
            usedCenters.Add(randomCenter);
        }

        var currentCost = MaxAssignedDistance(currentCenters, distanceMatrix);
        var bestCost = currentCost;
        var bestCenters = (int[])currentCenters.Clone();

        var temperature = initialTemperature;
        var iterationsWithoutImprovement = 0;

        for (var iter = 0; iter < maxIterations; iter++)
        {
            var newCenters = (int[])currentCenters.Clone();
            var centerToChange = rand.Next(p);
            int newCenter;
            do
            {
                newCenter = rand.Next(n);
            } while (usedCenters.Contains(newCenter));

            newCenters[centerToChange] = newCenter;

            var newCost = MaxAssignedDistance(newCenters, distanceMatrix);
            

            if (newCost < currentCost || rand.NextDouble() < Math.Exp((currentCost - newCost) / temperature))
            {
                currentCenters = newCenters;
                currentCost = newCost;
                
                if (currentCost < bestCost)
                {
                    bestCenters = (int[])currentCenters.Clone();
                    bestCost = currentCost;
                    iterationsWithoutImprovement = 0;
                }
                else
                {
                    iterationsWithoutImprovement++;
                }
            }
            else
            {
                iterationsWithoutImprovement++;
            }

            temperature *= coolingRate;

            if (iterationsWithoutImprovement <= maxNoImprovementIterations &&
                !(temperature < 1e-10)) continue;
            break;
        }

        return (bestCost, bestCenters);
    }
}