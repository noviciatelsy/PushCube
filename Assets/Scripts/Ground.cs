using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : GridObject
{
    public override bool IsBlocking()
    {
        return false;
    }
}
