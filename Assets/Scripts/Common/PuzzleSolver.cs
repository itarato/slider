#nullable enable

using System.Collections.Generic;
using UnityEngine;

namespace Common {
    public class PuzzleSolver {
        public class Move {
            public int sliderIdx;

            public int fromX;
            public int fromY;

            public int toX;
            public int toY;

            public Move(int sliderIdx, int fromX, int fromY, int toX, int toY) {
                this.sliderIdx = sliderIdx;
                this.fromX = fromX;
                this.fromY = fromY;
                this.toX = toX;
                this.toY = toY;
            }

            public override string ToString() {
                return "Move #" + sliderIdx.ToString() + " " + fromX.ToString() + ":" + fromY.ToString() + " -> " + toX.ToString() + ":" + toY.ToString();
            }
        }

        public static Move? FindSolution(Puzzle startState) {
            if (startState.IsEndPosition()) {
                Debug.Log("Already end state.");
                return null;
            }

            Puzzle state = startState.Clone();
            Puzzle originState = state.Clone();

            HashSet<string> knownHashes = new HashSet<string>();
            Dictionary<string, string> stepParentMap = new();
            knownHashes.Add(new string(state.Hash()));

            LinkedList<char[]> worklist = new LinkedList<char[]>();
            worklist.AddLast(state.Hash());

            for (int stateCounter = 0; worklist.Count > 0; stateCounter++) {
                if (stateCounter % 1000 == 0) Debug.Log("Batch iter: " + stateCounter.ToString());

                char[] currentHash = worklist.First!.Value;
                worklist.RemoveFirst();

                //setup state
                state.ResetFromHash(currentHash);

                if (state.IsEndPosition()) {
                    Debug.Log("Found solution!!!");
                    return ExtractNextStep(state, originState, stepParentMap);
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

            Debug.Log("No solution.");
            return null;
        }

        private static Move? ExtractNextStep(Puzzle state, Puzzle originState, Dictionary<string, string> stepParentMap) {
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
                Debug.Log("ERROR. Not enough steps and it did not fail at end-pos detection. Investigate.");
                return null;
            }

            string nextStepHashString = path[path.Count - 2];
            char[] hash = nextStepHashString.ToCharArray();
            state.ResetFromHash(hash);

            for (int i = 0; i < state.GetSliders().Count; i++) {
                if (!state.GetSliders()[i].Equals(originState.GetSliders()[i])) {
                    return new Move(
                        i,
                        originState.GetSliders()[i].x,
                        originState.GetSliders()[i].y,
                        state.GetSliders()[i].x,
                        state.GetSliders()[i].y
                    );
                }
            }

            return null;
        }
    }
}
