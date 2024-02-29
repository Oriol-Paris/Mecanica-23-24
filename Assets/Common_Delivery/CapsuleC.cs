using System;
using UnityEngine.UIElements;

[System.Serializable]
public struct CapsuleC
{
    #region FIELDS
    public Vector3C positionA;
    public Vector3C positionB;
    public float radius;
    #endregion

    #region PROPIERTIES
    #endregion

    #region CONSTRUCTORS
    public CapsuleC(Vector3C positionA, Vector3C positionB, float radius)
    {
        this.positionA = positionA;
        this.positionB = positionB;
        this.radius = radius;
    }
    //public CapsuleC(Vector3C position, float radius, float rotation)
    #endregion

    #region OPERATORS
    #endregion

    #region METHODS
    public bool IsInside(Vector3C point)
    {
        LineC l = LineC.From(positionA, positionB);
        Vector3C v = l.NearestPointToPoint(point);
        float d = v.magnitude;

        return d < radius;
    }
    public override bool Equals(object obj)
    {
        if (obj is CapsuleC)
        {
            CapsuleC other = (CapsuleC)obj;
            return other.positionA == positionA && other.positionB == positionB && other.radius == radius;
        }

        return false;
    }
    #endregion

    #region FUNCTIONS
    #endregion

}