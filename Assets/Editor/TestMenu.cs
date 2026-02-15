
using UnityEngine;
using UnityEditor;

public class TestMenu
{
    [MenuItem("Tools/Test Menu")]
    public static void Test()
    {
        Debug.Log("Test Menu Works!");
    }
}
