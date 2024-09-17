using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PuzzleTest {
    [Test]
    public void PuzzleTestSimplePasses() {
        Common.Puzzle puzzle = new Common.Puzzle();
        Assert.AreEqual(0, puzzle.GetSliders().Count);
    }
}
