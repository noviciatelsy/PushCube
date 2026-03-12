using System.Collections.Generic;

public class GridCell
{
    public Ground ground;
    public List<GridObject> objects = new List<GridObject>();

    public bool HasGround()
    {
        return ground != null;
    }

    public bool IsBlocked()
    {
        foreach (var obj in objects)
        {
            if (obj.IsBlocking())
                return true;
        }
        return false;
    }

    public T GetObject<T>() where T : GridObject
    {
        foreach (var obj in objects)
        {
            if (obj is T t)
                return t;
        }
        return null;
    }
}