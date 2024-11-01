using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class BurstSum
{
    [Test]
    public void BurstSumTest()
    {
        var sum = 0;
        for (var i = 0; i < 1000000; i++)
        {
            sum += i;
        }
        Debug.Log(sum);
    }
}
