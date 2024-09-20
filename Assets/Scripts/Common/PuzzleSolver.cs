#nullable enable

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Common {
    /**
     * Provides solution to a state of a puzzle.
     */
    public class PuzzleSolver {
        /**
         * Represents a move (target) of a slider.
         */
        public class Move {
            public int sliderIdx;

            // Target virtual (board) coordinate.
            public int x;
            public int y;

            public Move(int sliderIdx, int x, int y) {
                this.sliderIdx = sliderIdx;
                this.x = x;
                this.y = y;
            }

            public override string ToString() {
                return "Move #" + sliderIdx.ToString() + " to " + x.ToString() + ":" + y.ToString();
            }
        }

        async public static Task<Move?> FindSolution(Puzzle startState) {
            return await Task.Run(() => {
                if (startState.IsEndPosition()) {
                    return new Move(
                        startState.specialSliderIdx,
                        5,
                        3
                    );
                }

                Puzzle state = startState.Clone();
                Puzzle originState = state.Clone();

                HashSet<string> knownHashes = new HashSet<string>();
                Dictionary<string, string> stepParentMap = new();
                knownHashes.Add(new string(state.Hash()));

                LinkedList<char[]> worklist = new LinkedList<char[]>();
                worklist.AddLast(state.Hash());

                for (; worklist.Count > 0;) {
                    char[] currentHash = worklist.First!.Value;
                    worklist.RemoveFirst();

                    // Set board to current state.
                    state.ResetFromHash(currentHash);

                    if (state.IsEndPosition()) {
                        return ExtractNextStep(state, originState, stepParentMap);
                    }

                    // Find all possible states of the current board.
                    List<char[]> allPossibleNextStates = state.AllPossibleStates();
                    foreach (var possibleNextState in allPossibleNextStates) {
                        //  filter to not-yet seen ones
                        string newStateString = new string(possibleNextState);
                        if (knownHashes.Contains(newStateString)) continue;

                        // Set up for computing.
                        knownHashes.Add(newStateString);
                        worklist.AddLast(possibleNextState);

                        stepParentMap[newStateString] = new string(currentHash);
                    }
                }

                return null;
            });
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
                        state.GetSliders()[i].x,
                        state.GetSliders()[i].y
                    );
                }
            }

            return null;
        }
    }
}
