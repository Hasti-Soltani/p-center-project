using System.Diagnostics;
using p_centerProblem.Solutions;
using static p_centerProblem.DistanceMatrix;

namespace p_centerProblem;

internal static class Program
{
    public static void Main()
    {

        var points = new List<DemandPoint>();
        System.Console.Write("Enter number of Centers: ");
        var p = int.Parse(Console.ReadLine());
        Console.WriteLine("1. Small");
        Console.WriteLine("2. Medium");
        Console.WriteLine("3. Big");
        Console.WriteLine("4. VeryBig");
        Console.Write("Enter your choice (1-4): ");
        var choice = Console.ReadLine();

        try
        {
            var sr = new StreamReader($"./Samples/{GetFileName(choice ?? "1")}-sample.txt");
            var line = sr.ReadLine();
            while (!string.IsNullOrEmpty(line))
            {
                var x = double.Parse(line.Split(',')[0]);
                var y = double.Parse(line.Split(',')[1]);
                points.Add(new DemandPoint(x, y));
                line = sr.ReadLine();
            }

            sr.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        // var points = GenerateClusteredPoints(1000, 1000);
        var distanceMatrix = GenerateMatrix(points);

        Console.WriteLine($"for {points.Count} points and {p} centers:");
        var stopwatch = Stopwatch.StartNew();

        stopwatch.Start();
        var (greedyCost, greedyCenters) =
            GreedyPCenter.Solve(distanceMatrix, p);
        stopwatch.Stop();
        var elapsed = stopwatch.Elapsed;
        Console.WriteLine(
            $"Greedy Algorithm: Cost = {greedyCost}\n" +
            $"Centers = {string.Join(", ", greedyCenters)} \n" +
            $"in {elapsed.TotalMilliseconds} ms\n\n");


        stopwatch.Restart();
        var (saCost, saCenters) =
            SimulatedAnnealingPCenter.Solve(distanceMatrix, p);
        stopwatch.Stop();
        elapsed = stopwatch.Elapsed;
        Console.WriteLine(
            $"Simulated Annealing: Cost = {saCost}\n" +
            $"Centers = {string.Join(", ", saCenters)}\n" +
            $"in {elapsed.TotalMilliseconds} ms\n\n");



        stopwatch.Restart();
        var (otsCost, otsCenters) = OptimizedTabuSearch.Solve(distanceMatrix, p, 1000, 40);
        stopwatch.Stop();
        elapsed = stopwatch.Elapsed;
        Console.WriteLine(
            $"Optimized Tabu Search: Cost = {otsCost}\n" +
            $"Centers = {string.Join(", ", otsCenters)}\n" +
            $"in {elapsed.TotalMilliseconds} ms\n\n");

        stopwatch.Restart();
        var (exactCost, exactCenters) =
            ExactPCenter.Solve(distanceMatrix, p);
        stopwatch.Stop();

        elapsed = stopwatch.Elapsed;
        Console.WriteLine(
            $"Exact Algorithm: Cost = {exactCost}\n" +
            $"Centers = {string.Join(", ", exactCenters)}" +
            $"\nin {elapsed.TotalMilliseconds} ms\n\n");

    }

    private static List<DemandPoint> GenerateClusteredPoints(int numPoints, int numClusters)
    {
        var rand = new Random();
        var points = new List<DemandPoint>();

        // Generate cluster centers
        var clusterCenters = new List<DemandPoint>();
        for (var i = 0; i < numClusters; i++)
        {
            clusterCenters.Add(new DemandPoint(rand.NextDouble() * 10000, rand.NextDouble() * 10000));
        }


        for (var i = 0; i < numPoints; i++)
        {
            var clusterCenter = clusterCenters[rand.Next(numClusters)];
            var offsetX = rand.NextDouble() * 100 - 50;
            var offsetY = rand.NextDouble() * 100 - 50;
            points.Add(new DemandPoint(clusterCenter.X + offsetX, clusterCenter.Y + offsetY));
        }

        return points;
    }

    private static string GetFileName(string choice)
    {
        return choice switch
        {
            "1" => "small"
            ,
            "2" => "medium"
            ,
            "3" => "big"
            ,
            "4" => "veryBig"
            ,
            _ => throw new ArgumentOutOfRangeException(nameof(choice), choice, null)
        };
    }
}