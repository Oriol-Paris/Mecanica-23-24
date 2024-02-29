using System;

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
        public Vector3C direction;
        public bool randomDirection;
        public float minForce; //Per particle
        public float maxForce; //Per particle
        public float minPartPerSecond; //Per system
        public float maxPartPerSecond; //Per system
        public float minPartLife; //Per particle
        public float maxPartLife; //Per particle
        public float PartPerSecond { private set { } get
        {
            Random rng = new Random();
            float num = (float)rng.NextDouble();
            return minPartPerSecond + (num - 0.0f) * (maxPartPerSecond - minPartPerSecond) / (1 - 0);
        } }
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
        public Vector3C lastPosition;
        public Vector3C acceleration;
        public Vector3C velocity;
        public float size;

        public void AddForce(Vector3C force)
        {
            acceleration += force;
        }
    }

    Random rnd = new Random();

    public Particle[] Update(float dt)
    {
        Particle[] particles = new Particle[settings.particlePool];
        for (int i = 0; i < particles.Length; ++i)
        {
            particles[i].position = new Vector3C((float)rnd.NextDouble(), 0.0f, 0);
            particles[i].lastPosition = new Vector3C((float)rnd.NextDouble(), 0.0f, 0);
            particles[i].size = 0.1f;

            
        }
        return particles;
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
