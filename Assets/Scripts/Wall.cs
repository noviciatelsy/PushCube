using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : GridObject
{
    public override bool IsBlocking()
    {
        return true;
    }
}
