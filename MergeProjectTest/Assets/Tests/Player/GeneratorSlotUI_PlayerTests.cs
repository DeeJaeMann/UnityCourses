using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GeneratorSlotUI_PlayerTests
{
    
    [UnityTest]
    public IEnumerator Player_SanityCheck()
    {
        yield return null;
        Assert.IsTrue(true);
    }
    // A Test behaves as an ordinary method
    [Test]
    public void GeneratorSlotUI_PlayerTestsSimplePasses()
    {
        // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator GeneratorSlotUI_PlayerTestsWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
