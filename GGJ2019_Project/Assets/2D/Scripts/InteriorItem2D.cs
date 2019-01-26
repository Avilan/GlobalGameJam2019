using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

public class InteriorItem2D : MonoBehaviour
{
    public float PlacementOffsetX;

    public float PlacementOffsetY;

    public bool Glowing;

    public void EnableGlow()
    {
        Glowing = true;
        // TODO
    }

    public void DisableGlow()
    {
        Glowing = false;
        // TODO
    }
}
