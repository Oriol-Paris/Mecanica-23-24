using System;

[System.Serializable]
public struct PlaneC
{
    #region FIELDS
    public Vector3C position;
    public Vector3C normal;
    #endregion

    #region PROPIERTIES
    public Vector3C right { get { return new Vector3C(); } } //
    public Vector3C up { get { return new Vector3C(); } } //
    public Vector3C forward { get { return new Vector3C(); } } //
    public float distance { get { return 0f - Vector3C.Dot(normal, position); } set { } }
    #endregion

    #region CONSTRUCTORS
    public PlaneC(Vector3C position, Vector3C normal)
    {
        this.position = position;
        this.normal = normal;
    }
    public PlaneC(float a, float b, float c, float d) //
    {
        this.position = new Vector3C();
        this.normal = new Vector3C(a, b, c);
    }
    public PlaneC(Vector3C normal, float distance) //
    {
        this.normal = normal;
        this.position = new Vector3C();
    }
    #endregion

    #region OPERATORS
    #endregion

    #region METHODS
    public (float a, float b, float c, float d) ToEquation()
    {
        return (normal.x, normal.y, normal.z, distance);
    }
    public Vector3C NearestPoint(Vector3C point)
    {
        float num = Vector3C.Dot(normal, point) + distance;
        return point - normal * num;
    }

    public float DistanceToPoint(Vector3C point)
    {
        float num = normal.x * point.x + normal.y * point.y + normal.z * point.z + distance;
        float denom = MathF.Sqrt(normal.x * normal.x + normal.y * normal.y + normal.z * normal.z);
        return num / denom;
    }
    public override bool Equals(object obj)
    {
        if(obj is PlaneC)
        {
            PlaneC p = (PlaneC)obj;
            return p.position == position && p.normal == normal && p.right == right && p.up == up && p.forward == forward;
        }

        return false;
    }
    #endregion

    #region FUNCTIONS

    #endregion

}