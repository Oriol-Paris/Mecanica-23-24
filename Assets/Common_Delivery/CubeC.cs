using System;

[System.Serializable]
public struct CubeC
{
    #region FIELDS
    Vector3C position;
    Vector3C scale;
    Vector3C rotation;
    #endregion

    #region PROPIERTIES
    #endregion

    #region CONSTRUCTORS
    public CubeC(Vector3C position = new Vector3C(), Vector3C scale = new Vector3C(), Vector3C rotation = new Vector3C())
    {
        this.position = position;
        this.scale = scale;
        this.rotation = rotation;
    }
    #endregion

    #region OPERATORS
    #endregion

    #region METHODS
    public bool IsInside(Vector3C point)
    {
        if(point.x <= (position.x + scale.x) && point.y <= (position.y + scale.y) 
        && point.z <= (position.z + scale.z))
            return true;
        return false;
    }
    public Vector3C NearestPoint(Vector3C point)
    {
        return new Vector3C();
    }
    public override bool Equals(Object obj)
    {
        return false;
    }
    #endregion

    #region FUNCTIONS
    #endregion

}