using System;

[System.Serializable]
public struct SphereC
{
    #region FIELDS
    public Vector3C position;
    public float radius;
    #endregion

    #region PROPIERTIES
    public static SphereC unitary { get { return new SphereC(new Vector3C(), 1f); } } //
    #endregion

    #region CONSTRUCTORS
    public SphereC(Vector3C position, float radius)
    {
        this.position = position;
        this.radius = radius;
    }
    #endregion

    #region OPERATORS
    #endregion

    #region METHODS
    public bool IsInside(Vector3C point) //
    {
        Vector3C v = new Vector3C(position, point);
        float d = v.magnitude;

        return d < radius;
    }
    public override bool Equals(object obj)
    {
        if(obj is SphereC)
        {
            SphereC other = (SphereC)obj;
            return other.position == position && other.radius == radius;
        }

        return false;
    }
    #endregion

    #region FUNCTIONS
    #endregion

}