using System;
using System.Drawing;
using UnityEditor.ShaderGraph;
using UnityEngine.UIElements;

[System.Serializable]
public class AA1_ParticleSystem
{
    [System.Serializable]
    public struct Settings
    {
        public uint particlePool;
        public Vector3C gravity;
        public float bounce;
    }
    public Settings settings;

    [System.Serializable]
    public struct SettingsCascade
    {
        public Vector3C PointA;
        public Vector3C PointB;
        public Vector3C Direction;
        public bool randomDirection;
        public float minForce; //Per particle
        public float maxForce; //Per particle
        public float minPartPerSecond; //Per system
        public float maxPartPerSecond; //Per system
        public float minPartLife; //Per particle
        public float maxPartLife; //Per particle
        public float PartPerSecond 
        { 
            private set { }
            
            get
            {
                Random rng = new Random();
                float num = (float)rng.NextDouble();
                return minPartPerSecond + (num - 0.0f) * (maxPartPerSecond - minPartPerSecond) / (1.0f - 0.0f);
            } 
        }
    }
    public SettingsCascade settingsCascade;

    [System.Serializable]
    public struct SettingsCannon
    {
        public Vector3C Start;
        public Vector3C Direction;
        public float angle;
        public float minForce; //Per particle
        public float maxForce; //Per particle
        public float minPartPerSecond; //Per system
        public float maxPartPerSecond; //Per system
        public float minPartLife; //Per particle
        public float maxPartLife; //Per particle
        public float PartPerSecond
        {
            private set { }

            get
            {
                Random rng = new Random();
                float num = (float)rng.NextDouble();
                return minPartPerSecond + (num - 0.0f) * (maxPartPerSecond - minPartPerSecond) / (1.0f - 0.0f);
            }
        }
    }
    public SettingsCannon settingsCannon;

    [System.Serializable]
    public struct SettingsCollision
    {
        public PlaneC[] planes;
        public SphereC[] spheres;
        public CapsuleC[] capsules;
    }
    public SettingsCollision settingsCollision;



    public struct Particle
    {
        public Vector3C position;
        public Vector3C acceleration;
        public Vector3C velocity;
        public float life;
        public float size;

        public void AddForce(Vector3C force)
        {
            acceleration += force;
        }

        public void UpdatePosition(float dt)
        {
            velocity = velocity + acceleration * dt;
            position = position + velocity * dt;
        }

        public void Collision(PlaneC plane, float bounceFactor)
        {
            Vector3C vn =
                plane.normal * 
                ((velocity.x * plane.normal.x) + (velocity.y * plane.normal.y) + (velocity.z * plane.normal.z))
                / (plane.normal.magnitude * plane.normal.magnitude);

            velocity = velocity - vn * 2;
            velocity *= bounceFactor;
        }
        public void Collision(SphereC sphere, float bounceFactor)
        {
            PlaneC plane = new PlaneC(position, position - sphere.position);
            Collision(plane, bounceFactor);
        }
        public void Collision(CapsuleC capsule, float bounceFactor)
        {
            PlaneC plane = new PlaneC(position, position - LineC.From(capsule.positionA, capsule.positionB).NearestPointToPoint(position));
            Collision(plane, bounceFactor);
        }
    }

    Random rnd = new Random();
    float simTimer = 0.0f;

    public Particle[] Update(float dt)
    {
        Particle[] particles = new Particle[settings.particlePool];

        simTimer += dt;

        //Spawn Particles
        if(simTimer >= settingsCascade.PartPerSecond)
        {

        }

        //Update Particles
        foreach (Particle p in particles)
        {
            p.UpdatePosition(dt);
        }

        //Check Collisions
        foreach (Particle p in particles)
        {
            CheckCollisions(p);
        }

        return particles;
    }

    public Particle SetUpCascadeParticle()
    {
        Particle p = new Particle();

        //Particle Life
        Random rng = new Random();
        p.life = settingsCascade.minPartLife + ((float)rng.NextDouble()) * (settingsCascade.maxPartLife - settingsCascade.minPartLife);

        //Particle Starting Position
        p.position = settingsCascade.PointA + (new Vector3C((float)rng.NextDouble(), (float)rng.NextDouble(), (float)rng.NextDouble())) * (settingsCascade.PointB - settingsCascade.PointA);

        //Particle Starting Acceleration
        p.acceleration = 
            new Vector3C(settingsCascade.minForce, settingsCascade.minForce, settingsCascade.minForce) * settingsCascade.Direction
            + new Vector3C(((float)rng.NextDouble()), ((float)rng.NextDouble()), ((float)rng.NextDouble())) 
            * (settingsCascade.maxForce - settingsCascade.minForce);
        p.AddForce(settings.gravity);

        //Particle Starting Velocity
        p.velocity = settingsCascade.Direction.normalized;

        return p;
    }

    public Particle SetUpConeParticle()
    {
        Particle p = new Particle();

        //Particle Life
        Random rng = new Random();
        p.life = settingsCannon.minPartLife + ((float)rng.NextDouble()) * (settingsCannon.maxPartLife - settingsCannon.minPartLife);

        //Particle Starting Position
        p.position = settingsCannon.Start + (new Vector3C((float)rng.NextDouble(), (float)rng.NextDouble(), (float)rng.NextDouble())) * ((settingsCannon.Start + settingsCannon.Direction) - settingsCannon.Start);

        //Particle Starting Acceleration
        p.acceleration =
            new Vector3C(settingsCannon.minForce, settingsCannon.minForce, settingsCannon.minForce) * settingsCannon.Direction
            + new Vector3C(((float)rng.NextDouble()), ((float)rng.NextDouble()), ((float)rng.NextDouble()))
            * (settingsCannon.maxForce - settingsCannon.minForce);
        p.AddForce(settings.gravity);

        //Particle Starting Velocity


        return p;
    }

    public void CheckCollisions(Particle particle)
    {
        foreach(PlaneC plane in settingsCollision.planes) //
        {

        }
        foreach (SphereC sphere in settingsCollision.spheres)
        {
            if (sphere.IsInside(particle.position))
            {
                particle.position = (particle.position - sphere.position).normalized * sphere.radius;
                particle.Collision(sphere, settings.bounce);
            }
        }
        foreach(CapsuleC capsule in settingsCollision.capsules)
        {
            if(capsule.IsInside(particle.position))
            {
                particle.position = (particle.position - LineC.From(capsule.positionA, capsule.positionB).NearestPointToPoint(particle.position)).normalized * capsule.radius;
                particle.Collision(capsule, settings.bounce);
            }
        }
    }

    public void Debug()
    {
        foreach (var item in settingsCollision.planes)
        {
            item.Print(Vector3C.red);
        }
        foreach (var item in settingsCollision.capsules)
        {
            item.Print(Vector3C.green);
        }
        foreach (var item in settingsCollision.spheres)
        {
            item.Print(Vector3C.blue);
        }
    }
}
