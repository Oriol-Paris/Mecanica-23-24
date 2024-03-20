using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class AA2_Cloth
{
    [System.Serializable]
    public struct Settings
    {
        public Vector3C gravity;
        [Min(1)]
        public float width;
        [Min(1)]
        public float height;
        [Min(2)]
        public int xPartSize;
        [Min(2)]
        public int yPartSize;
    }
    public Settings settings;
    [System.Serializable]
    public struct ClothSettings
    {
        public float structuralElasticCoef;
        public float damptCoef;

        public float structuralSpring;
    }
    public ClothSettings clothSettings;

    [System.Serializable]
    public struct SettingsCollision
    {
        public SphereC sphere;
    }
    public SettingsCollision settingsCollision;
    public struct Vertex
    {
        public Vector3C actualPosition;
        public Vector3C velocity;
        public Vertex(Vector3C _position)
        {
            this.actualPosition = _position;
            this.velocity = new Vector3C(0, 0, 0);
        }

        public void Euler(Vector3C force, float dt)
        {
            velocity += force * dt;
            actualPosition += velocity * dt;
        }
    }
    public Vertex[] points;
    public void Update(float dt)
    {
        System.Random rnd = new System.Random();
        int xVertex = settings.xPartSize + 1;

        for (int i = settings.xPartSize + 1; i < points.Length; i++)
        {
            float moduleY = (points[i - xVertex].actualPosition - points[i].actualPosition).magnitude - clothSettings.structuralSpring;
            Vector3C forceVector = (points[i - xVertex].actualPosition - points[i].actualPosition).normalized * moduleY;

            Vector3C dampingForce = (points[i].velocity - points[i - xVertex].velocity) * clothSettings.damptCoef;
            Vector3C structuralSpringForce = forceVector * clothSettings.structuralElasticCoef - dampingForce;

            points[i].Euler(settings.gravity + structuralSpringForce, dt);
        }
    }

    public void Debug()
    {
        settingsCollision.sphere.Print(Vector3C.blue);

        if (points != null)
            foreach (var item in points)
            {
                item.actualPosition.Print(0.05f);
            }
    }
}
