using System;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using static AA1_ParticleSystem;

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
        int xVertices = settings.xPartSize + 1;

        Vector3C[] structuralForces = CalculateStructuralForces();
        Vector3C[] shearForces = CalculateShearForces();
        Vector3C[] bendingForces = CalculateBendingForces();

        for (int i = 0; i < points.Length; i++)
        {
            if (i > 0 && i < xVertices - 1)
            {
                points[i].Euler(settings.gravity + structuralForces[i] + shearForces[i] + bendingForces[i], dt);
                continue;
            }
            if (i < xVertices) continue;
            points[i].Euler(settings.gravity + structuralForces[i] + shearForces[i] + bendingForces[i], dt);
        }

        CalculateColisions();
    }

    Vector3C[] CalculateStructuralForces()
    {
        int xVertices = settings.xPartSize + 1;
        Vector3C[] structuralForces = new Vector3C[points.Length];

        //Structural Forces
        for (int i = 0; i < points.Length; i++)
        {
            //Structural Ver
            if (i >= xVertices)
            {
                float structuralMagnitudeY = (points[i - xVertices].actualPosition - points[i].actualPosition).magnitude
                - clothSettings.structuralSpringL;

                //Damping Force
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

        return structuralForces;
    }


    Vector3C[] CalculateShearForces()
    {
        int xVertices = settings.xPartSize + 1;
        Vector3C[] shearForces = new Vector3C[points.Length];

        //Shear Forces
        for (int i = 0; i < points.Length - xVertices; i++)
        {
            //Left to Right
            if (i % xVertices != xVertices - 1)
            {
                float shearMagnitudeR = (points[i].actualPosition - points[i + xVertices + 1].actualPosition).magnitude
                - clothSettings.shearSpringL;

                //Damping Force
                Vector3C damptingForce = (points[i + xVertices + 1].velocity - points[i].velocity)
                    * clothSettings.shearDamptCoef;

                Vector3C structuralForceR = (points[i].actualPosition - points[i + xVertices + 1].actualPosition).normalized
                    * (shearMagnitudeR * clothSettings.shearElasticCoef) - damptingForce;

                shearForces[i + xVertices + 1] += structuralForceR;
                shearForces[i] -= structuralForceR;
            }

            //Right to Left
            if (i % xVertices != 0)
            {
                float shearMagnitudeL = (points[i].actualPosition - points[i + xVertices - 1].actualPosition).magnitude
                - clothSettings.shearSpringL;

                //Damping Force
                Vector3C damptingForce = (points[i + xVertices - 1].velocity - points[i].velocity)
                    * clothSettings.shearDamptCoef;

                Vector3C structuralForceL = (points[i].actualPosition - points[i + xVertices - 1].actualPosition).normalized
                    * (shearMagnitudeL * clothSettings.shearElasticCoef) - damptingForce;

                shearForces[i + xVertices - 1] += structuralForceL;
                shearForces[i] -= structuralForceL;
            }
        }

        return shearForces;
    }


    Vector3C[] CalculateBendingForces()
    {
        int xVertices = settings.xPartSize + 1;
        Vector3C[] bendingForces = new Vector3C[points.Length];

        //Bending Forces
        for (int i = 0; i < points.Length; i++)
        {
            //Vertical
            if (i >= xVertices * 2)
            {
                float bendingMagnitudeY = (points[i - (2 * xVertices)].actualPosition - points[i].actualPosition).magnitude
                - clothSettings.bendingSpringL;

                Vector3C damptingForce = (points[i].velocity - points[i - (2 * xVertices)].velocity)
                    * clothSettings.bendingDamptCoef;

                Vector3C bendingForceVectorY = (points[i - (2 * xVertices)].actualPosition - points[i].actualPosition).normalized
                    * (bendingMagnitudeY * clothSettings.bendingElasticCoef) - damptingForce;

                bendingForces[i] += bendingForceVectorY;
                bendingForces[i - xVertices] -= bendingForceVectorY;
            }

            //Horizontal
            if (i % xVertices > 2)
            {
                float bendingMagnitudeX = (points[i - 2].actualPosition - points[i].actualPosition).magnitude
                    - clothSettings.bendingSpringL;

                //Fuerza de amortiguamiento
                Vector3C damptingForce = (points[i].velocity - points[i - 2].velocity)
                * clothSettings.bendingDamptCoef;

                Vector3C bendingForceVectorX = (points[i - 2].actualPosition - points[i].actualPosition).normalized
                    * bendingMagnitudeX * clothSettings.bendingElasticCoef - damptingForce;

                bendingForces[i] += bendingForceVectorX;
                bendingForces[i - 2] -= bendingForceVectorX;
            }
        }

        return bendingForces;
    }


    void CalculateColisions()
    {
        for (int i = 0; i < points.Length; i++)
        {
            if (settingsCollision.sphere.IsInside(points[i].actualPosition))
            {
                points[i].actualPosition = (points[i].actualPosition - settingsCollision.sphere.position).normalized * settingsCollision.sphere.radius + settingsCollision.sphere.position;

                points[i].velocity = Vector3C.zero;
            }
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
