---
title: Intro
---

# A technical report on an AI for solving a sliding puzzle independently

research statement/arguement
A* is the best way of solving sliding puzzles with AI.

## Research

### Defence of Arguements

Methods chosen to solve it (A-Star)
Comparison
The A* Sorting algorithm is the most efficent way of solving a sliding puzzle, as it always going to find the shortest path to the goal state.

Normally, an A* algorithm is found using the sum of an estimate of the distance between the initial and goal states, as well as the actual distance, calculated using Djikstra if it is required.
In the case of a sliding puzzle, the estimate is the number of misplaced tiles and the number of moves from its initial state.

sum up alternatives

A* will always find the shortest route to the goal.

A* does have a complexity problem.

If A* returns a solution, the solution is always going to be optimal.
"Among all optimal algorithms that start from the same start node and use the same heuristic h, A* expands the minimal number of paths.
problem: A* could be unlucky about how it breaks ties.
So let’s define optimal efficiency as expanding the minimal number of paths p for which f(p) =/= f*, where f* is the cost of the shortest path." Search: A* Optimal Efficency, Standford.

LOcal Serarch would be prone to going towards the wrong solutions.

A* is a priority first algorithm, using a priority queue and not using breadth first or depth first. (BFS/DFS)/.

A* has heuristic, whereas Dijkstra on its own doesn't.  A* would never explore extra nodes because it is more focused on the correct path.

If the graph wasen't weighted by the number of tiles that would be misplaced, then BFS would be the best. If they were weighted but did there was no heuristic (in this case, the distance from the root node), then Dijkstra on its own would be better.


### Appropriateness of Practice-Based Research Methods
Coming soon...

### Application of Academic Conventions
Coming soon...


advantages and disadvantages
A*:
+: always finds the shortest path to solving a puzzle
+: much faster at finding a solution than alternatives.

-: very complicated.
-: algorithm is complete if the branching factor is finite and every action has a fixed cost.


## Artifact

### Reflection
how the project went

https://stackoverflow.com/questions/34244452/whats-the-difference-between-best-first-search-and-a-search
https://ai.stackexchange.com/questions/8902/what-are-the-differences-between-a-and-greedy-best-first-search
https://www.quora.com/What-are-the-advantages-and-disadvantages-of-A*-search-and-Dijkstras-algorithm-When-should-each-be-used
https://www.quora.com/What-are-the-advantages-and-disadvantages-of-local-search-compared-to-A*-search
https://www.quora.com/Why-does-A*-algorithm-outperform-Dijkstra-shortest-path-algorithm
https://www.quora.com/What-is-the-best-algorithm-for-shortest-path-problems-with-constraints-Dijkstra-BFS-A*