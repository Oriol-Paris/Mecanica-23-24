using System;
using System.Data;
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
        [Header("Structural Spring")]
        public float structuralElasticCoef;
        public float structuralDamptCoef;
        public float structuralSpringL;

        [Header("Shear Spring")]
        public float shearElasticCoef;
        public float shearDamptCoef;
        public float shearSpringL;

        [Header("Bending Spring")]
        public float bendingElasticCoef;
        public float bendingDamptCoef;
        public float bendingSpringL;
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
        public Vector3C lastPosition;
        public Vector3C actualPosition;
        public Vector3C velocity;
        public Vertex(Vector3C _position)
        {
            this.actualPosition = _position;
            this.lastPosition = _position;
            this.velocity = new Vector3C(0, 0, 0);
        }

        public void Euler(Vector3C force, float dt) {
            lastPosition = actualPosition;
            velocity += force * dt;
            actualPosition += velocity * dt;
        }
    }
    public Vertex[] points;

    public void Update(float dt)
    {
        //System.Random rnd = new System.Random();
        int xVertices = settings.xPartSize + 1;
        int yVertices = settings.yPartSize + 1;

        Vector3C[] structuralForces = new Vector3C[points.Length];
        
        for (int i = 0; i < points.Length; i++)
        {
            //if (i < xVertices) continue;

            //Structural Ver
            if (i >= xVertices)
            {
                float structuralMagnitudeY = (points[i - xVertices].actualPosition - points[i].actualPosition).magnitude
                - clothSettings.structuralSpringL;

                //Fuerza de amortiguamiento
                Vector3C damptingForce = (points[i].velocity - points[i - xVertices].velocity)
                    * clothSettings.structuralDamptCoef;

                Vector3C structuralForceVectorY = (points[i - xVertices].actualPosition - points[i].actualPosition).normalized
                    * (structuralMagnitudeY * clothSettings.structuralElasticCoef) - damptingForce;

                structuralForces[i] += structuralForceVectorY;
                structuralForces[i - xVertices] -= structuralForceVectorY;
            }

            //Structural Hor
            if (i % xVertices != 0)
            {
                float structuralMagnitudeX = (points[i - 1].actualPosition - points[i].actualPosition).magnitude
                    - clothSettings.structuralSpringL;

                //Fuerza de amortiguamiento
                Vector3C damptingForce = (points[i].velocity - points[i - 1].velocity)
                * clothSettings.structuralDamptCoef;

                Vector3C structuralForceVectorX = (points[i - 1].actualPosition - points[i].actualPosition).normalized
                    * structuralMagnitudeX * clothSettings.structuralElasticCoef - damptingForce;

                structuralForces[i] += structuralForceVectorX;
                structuralForces[i - 1] -= structuralForceVectorX;
            }


        }
        for (int i = 0; i < points.Length; i++)
        {
            if (i > 0 && i < xVertices - 1)
            {
                points[i].Euler(settings.gravity + structuralForces[i], dt);
                continue;
            }
            if (i < xVertices) continue;
            points[i].Euler(settings.gravity + structuralForces[i], dt);
        }
    }

    public void Debug()
    {
        settingsCollision.sphere.Print(Vector3C.blue);

        if (points != null)
            foreach (var item in points)
            {
                item.lastPosition.Print(0.05f);
                item.actualPosition.Print(0.05f);
            }
    }
}
