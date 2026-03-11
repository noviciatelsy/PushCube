using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : GridObject
{
    public override bool IsBlocking()
    {
        return true;
    }
}
