using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MyTypeGem { ROMB, RECT, OVAL, CIRCLE, OCTAGON, TRIANGLE, NONE }

public class my_gem_type : MonoBehaviour
{
    public MyTypeGem myType = MyTypeGem.NONE;
}
