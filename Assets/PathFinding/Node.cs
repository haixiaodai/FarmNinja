using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node: IComparable
{
    public int x;
    public int y;
    public bool Isobstacle;
    public float Gcost;
    public Node parent;
    public Vector3 position;
    public float Hcost;
    public float Fcost;
    bool havepath = false;

    public Node(int x,int y,Vector3 position, bool Isobstacle)
    {
        this.x = x;
        this.y = y;
        //Actual position in world
        this.position = position;
        this.Isobstacle = Isobstacle;
        //this.parent = null;
    }

    public Node()
    {
        this.x = 0;
        this.y = 0;
    }

    public int getx()
    {
        return this.x;
    }

    public int gety()
    {
        return this.y;
    }

    public Node getparent()
    {
        return this.parent;
    }

    public void setparent(Node newparent)
    {
        this.parent = newparent;
    }

    public bool getIsobstacle() { return this.Isobstacle; }

    public void setGcost(float newdis)
    {
        this.Gcost = newdis;
    }

    public float getGcost()
    {
        return this.Gcost;
    }

    public Vector3 getWorldPosition()
    {
        return this.position;
    }

    public float getHcost()
    {
        return this.Hcost;
    }

    public float getFcost()
    {
        return this.Fcost;
    }

    public void setFcost(float cost)
    {
        this.Fcost = this.Hcost+cost;
    }

    public void setHcost(float Hcost)
    {
        this.Hcost = Hcost;
        //this.Hcost = Mathf.Abs(endnode.getx() - this.x) + Mathf.Abs(endnode.gety() - this.y);
    }

    public bool gethavepath()
    {
        return this.havepath;
    }

    public void sethavepath( bool newhave)
    {
        havepath = newhave;
    }

    public override bool Equals(object obj)
    {
        var item = obj as Node;

        if (item == null)
        {
            return false;
        }
        if (x == item.x && y == item.y)
            return true;

        return false ;
    }

    public override int GetHashCode()
    {
        return x ^ y;
    }

    public int CompareTo(object obj)
    {
        return (Mathf.Abs(Fcost - ((Node)obj).Fcost) < 0.0001f) ? 0 : (Fcost - ((Node)obj).Fcost) < 0 ? -1 : 1;
    }
}
