using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Test : MonoBehaviour
{
    public List<TestClass> testList = new();

    private void Start()
    {
        Debug.Log(testList[0].testString);
        Debug.Log(testList[1].testString);
    }
}

[System.Serializable]
public class TestClass
{
    public int testInt;
    public float testFloat;
    public string testString;
    public bool testBool;

    [FormerlySerializedAs("testList")]
    public List<TestClass2> insideTestList = new();
}

[System.Serializable]
public class TestClass2
{
    public int testInt;
    public float testFloat;
    public string testString;
    public bool testBool;
}
