#nullable enable

using System.Collections.Generic;
using System;

public class PuzzleSolver {
    public static Puzzle? FindSolution(Puzzle state) {
        if (state.IsEndPosition()) {
            Console.WriteLine("Already end state.");
            return null;
        }

        HashSet<string> knownHashes = new HashSet<string>();
        Dictionary<string, string> stepParentMap = new();
        knownHashes.Add(new string(state.Hash()));

        LinkedList<char[]> worklist = new LinkedList<char[]>();
        worklist.AddLast(state.Hash());

        for (int stateCounter = 0; worklist.Count > 0; stateCounter++) {
            if (stateCounter % 1000 == 0) Console.WriteLine("Batch iter: #{0}", stateCounter);

            char[] currentHash = worklist.First!.Value;
            worklist.RemoveFirst();

            //setup state
            state.ResetFromHash(currentHash);

            if (state.IsEndPosition()) {
                Console.WriteLine("Found solution!!!");
                return ExtractNextStep(state, stepParentMap);
            }

            //find all possible next states
            List<char[]> allPossibleNextStates = state.AllPossibleStates();
            foreach (var possibleNextState in allPossibleNextStates) {
                //  filter to not-yet seen ones
                string newStateString = new string(possibleNextState);
                if (knownHashes.Contains(newStateString)) continue;

                //  push it to the end of worklist
                knownHashes.Add(newStateString);
                worklist.AddLast(possibleNextState);

                stepParentMap[newStateString] = new string(currentHash);
            }
        }

        Console.WriteLine("No solution.");
        return null;
    }

    private static Puzzle? ExtractNextStep(Puzzle state, Dictionary<string, string> stepParentMap) {
        string currentStep = new string(state.Hash());
        // Head: solution / tail: current state. Tail-1: next step.
        List<string> path = new List<string> { currentStep };

        for (; ; ) {
            if (stepParentMap.ContainsKey(currentStep)) {
                currentStep = stepParentMap[currentStep];
                path.Add(currentStep);
            } else {
                break;
            }

            //char[] hash = currentStep.ToCharArray();
            //state.ResetFromHash(hash);
        }

        if (path.Count < 2) {
            Console.WriteLine("ERROR. Not enough steps and it did not fail at end-pos detection. Investigate.");
            return null;
        }

        string nextStepHashString = path[path.Count - 2];
        char[] hash = nextStepHashString.ToCharArray();
        state.ResetFromHash(hash);

        return null;
    }
}
