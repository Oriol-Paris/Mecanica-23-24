using System;

[System.Serializable]
public class AA2_Rigidbody
{
    [System.Serializable]
    public struct Settings
    {
        public Vector3C gravity;
        public float bounce;
    }
    public Settings settings;

    [System.Serializable]
    public struct SettingsCollision
    {
        public PlaneC[] planes;
    }
    public SettingsCollision settingsCollision;


    [System.Serializable]
    public struct CubeRigidbody
    {
        public Vector3C position;
        public Vector3C size;
        public Vector3C euler;

        public Vector3C lastPosition { private set; get; }

        public Vector3C[] rotatedVertices { private set; get; }
        private Vector3C[] originalVertices;

        public Vector3C linearVelocity { private set; get; }
        public Vector3C angularVelocity { private set; get; }
        public Vector3C linearAcceleration { private set; get; }

        [NonSerialized]
        public bool accelerationInit;

        public CubeRigidbody(Vector3C _position, Vector3C _size, Vector3C _euler)
        {
            position = _position;
            size = _size;
            euler = _euler;

            lastPosition = _position;

            rotatedVertices = new Vector3C[8];
            originalVertices = new Vector3C[8]
            {
                new Vector3C(-size.x, -size.y, size.z),
                new Vector3C(-size.x, size.y, size.z),
                new Vector3C(-size.x, -size.y, -size.z),
                new Vector3C(-size.x, size.y, -size.z),
                new Vector3C(size.x, -size.y, size.z),
                new Vector3C(size.x, size.y, size.z),
                new Vector3C(size.x, -size.y, -size.z),
                new Vector3C(size.x, size.y, -size.z),
            };

            linearVelocity = Vector3C.zero;
            angularVelocity = new Vector3C(0.0f, 15.0f, 0.0f);
            linearAcceleration = Vector3C.zero;

            accelerationInit = false;
        }

        public void AddLinearForce(Vector3C force)
        {
            linearAcceleration += force;
        }

        public void Euler(float dt)
        {
            linearVelocity += linearAcceleration * dt;

            lastPosition = position;

            position += linearVelocity * dt;
            euler += angularVelocity * dt;
        }

        public void RotateVertices()
        {
            for (int i = 0; i < originalVertices.Length; ++i)
            {
                rotatedVertices[i] = MatrixC.RotationToVector(euler, originalVertices[i]);
            }
        }

        public void CollidedWithPlane(Vector3C collisionPoint, PlaneC collisionPlane, Settings _settings)
        {
            if (collisionPlane.DistanceToPoint(collisionPoint) < 0.0f)
            {
                position = collisionPlane.NearestPoint(lastPosition) + collisionPlane.normal 
                * Vector3C.Dot(collisionPlane.normal, lastPosition - collisionPlane.position);

                Vector3C vn = collisionPlane.normal.normalized * Vector3C.Dot(linearVelocity, collisionPlane.normal);
                Vector3C vt = linearVelocity - vn;
                linearVelocity = (vt - vn) * _settings.bounce;
            }
        }
    }
    public CubeRigidbody crb = new CubeRigidbody(Vector3C.zero, new(.1f, .1f, .1f), Vector3C.zero);

    public void Update(float dt)
    {
        if (!crb.accelerationInit)
        {
            crb.AddLinearForce(settings.gravity);
            crb.accelerationInit = true;
        }

        crb.Euler(dt);
        crb.RotateVertices();

        CheckCollisionWithPlanes();
    }

    public void CheckCollisionWithPlanes()
    {
        foreach (PlaneC plane in settingsCollision.planes)
        {
            for (int i = 0; i < crb.rotatedVertices.Length; i++)
            {
                crb.CollidedWithPlane(crb.rotatedVertices[i] + crb.position, plane, settings);
            }
        }
    }

    public void Debug()
    {
        foreach (var vertex in crb.rotatedVertices)
        {
            SphereC sphere = new SphereC(vertex + crb.position, 0.01f);
            sphere.Print(Vector3C.green);
        }
    }

}
