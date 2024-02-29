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
    public float distance { get { return 0f - Vector3C.Dot(normal, position); } }
    #endregion

    #region CONSTRUCTORS
    public PlaneC(Vector3C position, Vector3C normal)
    {
        this.position = position;
        this.normal = normal;
    }
    public PlaneC(Vector3C a, Vector3C b, Vector3C c) //
    {
        this.position = new Vector3C();
        this.normal = new Vector3C();
    }
    public PlaneC(float a, float b, float c, float d) //
    {
        this.position = new Vector3C();
        this.normal = new Vector3C();
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
    public Vector3C Intersection(LineC line) //
    {
        return new Vector3C();
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