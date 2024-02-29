using System;

[System.Serializable]
public struct LineC
{
    #region FIELDS
    public Vector3C origin;
    public Vector3C direction;
    #endregion

    #region PROPIERTIES
    #endregion

    #region CONSTRUCTORS
    public LineC(Vector3C origin, Vector3C direction)
    {
        this.origin = origin;
        this.direction = direction;
    }
    #endregion

    #region OPERATORS
    #endregion

    #region METHODS
    public Vector3C NearestPointToPoint(Vector3C point)
    {
        Vector3C v = point - origin;
        Vector3C nearest = direction.normalized * Vector3C.Dot(v, direction.normalized);
        return nearest + origin;
    }
    #endregion

    #region FUNCTIONS
    public static LineC From(Vector3C pointA, Vector3C pointB)
    {
        Vector3C d = new Vector3C(pointB - pointA);
        d.Normalize();

        return new LineC(pointA, d);
    }
    #endregion

}