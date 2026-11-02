import pulp
import math
import time
from itertools import combinations


# Step 1: Parse the input data
def load_points(file_path):
    points = []
    with open(file_path, "r") as f:
        for line in f:
            x, y = map(float, line.strip().split(","))
            points.append((x, y))
    return points


# Function to calculate Euclidean distance
def euclidean_distance(point1, point2):
    return math.sqrt((point1[0] - point2[0]) ** 2 + (point1[1] - point2[1]) ** 2)


# Create a distance matrix for the points
def create_distance_matrix(points):
    n = len(points)
    distances = []
    for i in range(n):
        for j in range(n):
            distances.append((i, j, euclidean_distance(points[i], points[j])))
    return distances


# Binary search-based p-center ILP solver
def optimized_p_center_solver(points, p, solver="CBC"):  # Use Gurobi or CBC
    n = len(points)
    distance_matrix = create_distance_matrix(points)

    # Precompute sorted unique distances for binary search
    sorted_distances = sorted(set(d[2] for d in distance_matrix))

    # Helper function to solve feasibility for a given max distance d
    def is_feasible(d):
        # Create the ILP problem
        problem = pulp.LpProblem("P_Center_Problem", pulp.LpMinimize)

        # Create variables
        y = pulp.LpVariable.dicts(
            "y", range(n), cat="Binary"
        )  # 1 if point j is a center
        x = pulp.LpVariable.dicts(
            "x", [(i, j) for i in range(n) for j in range(n)], cat="Binary"
        )  # if point i is assigned to center j

        # Constraint: Each demand point must be assigned to one center
        for i in range(n):
            problem += pulp.lpSum(x[i, j] for j in range(n)) == 1

        # Constraint: A demand point i can only be assigned to j if j is a center
        for i in range(n):
            for j in range(n):
                problem += x[i, j] <= y[j]

        # Constraint: Limit the number of centers to p
        problem += pulp.lpSum(y[j] for j in range(n)) == p

        # Constraint: Only allow assignments if the distance is within d
        for i, j, dist in distance_matrix:
            if dist > d:
                problem += x[i, j] == 0

        # Solve the problem (no objective, just constraints for feasibility)
        solver_backend = (
            pulp.GUROBI_CMD(msg=False)
            if solver == "GUROBI"
            else (
                pulp.CPLEX_CMD(msg=False)
                if solver == "CPLEX"
                else pulp.PULP_CBC_CMD(msg=False)
            )
        )
        problem.solve(solver_backend)

        # Check feasibility
        return problem.status == pulp.LpStatusOptimal

    # Perform binary search on distances
    low, high = 0, len(sorted_distances) - 1
    best_distance = None

    while low <= high:
        mid = (low + high) // 2
        d = sorted_distances[mid]
        if is_feasible(d):  # Test feasibility
            best_distance = d
            high = mid - 1  # Search lower distances
        else:
            low = mid + 1  # Search higher distances

    # Return the best (minimum feasible) distance
    return best_distance


# Example Usage
if __name__ == "__main__":

    samples = {"1": "small", "2": "medium", "3": "big", "4": "veryBig"}

    for key in samples:
        print(f"{key}. {samples[key]}")

    choice = input("Enter your choice (1-4): ").strip()

    file_path = f"./Samples/{samples.get(choice)}-sample.txt"
    points = load_points(file_path)
    # Number of centers to place
    p = input("Enter Number of Centers: ")

    # Use the optimized solver
    start_time = time.time()
    min_max_distance = optimized_p_center_solver(points, p, solver="CBC")

    print("Minimum Maximum Distance:", min_max_distance)
    print("in ", time.time() - start_time, "ms")
