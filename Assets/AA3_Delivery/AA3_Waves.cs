using System;
using System.Runtime.InteropServices;
using static AA3_Waves;

[System.Serializable]
public class AA3_Waves
{
    [System.Serializable]
    public struct Settings
    {
        public float density;
        public float gravity;
    }
    public Settings settings;
    [System.Serializable]
    public struct WavesSettings
    {
        public float amplitude;
        public float frequency;
        public float phase;
        public Vector3C direction;
        public float speed;
    }
    public WavesSettings[] wavesSettings;
    [System.Serializable]
    public struct BuoySettings
    {
        public float buoyancyCoeficient;
        public float buoyVelocity;
        public float mass;
    }
    public BuoySettings buoySettings;

    public SphereC buoy;
    public struct Vertex
    {
        public Vector3C originalposition;
        public Vector3C position;
        public Vertex(Vector3C _position)
        {
            this.position = _position;
            this.originalposition = _position;
        }
    }
    public Vertex[] points;
    private float elapsedTime;

    public AA3_Waves()
    {
        elapsedTime = 0.0f;
    }

    private float CalculateWaveHeightAtBuoy()
    {
        float height = 0.0f;

        foreach(WavesSettings ws in wavesSettings)
        {
            float k = (float)(2.0f * Math.PI / ws.frequency);
            height += (float)(ws.amplitude * k * Math.Sin(k * (Vector3C.Dot(buoy.position, ws.direction) + elapsedTime * ws.speed) + ws.phase));
        }
        return height;
    }

    private float CalculateSubmergedVolume(float waveHeight)
    {
        if(!buoy.IsBelowSphere(new Vector3C(buoy.position.x, waveHeight, buoy.position.z)))
        {
            float height = waveHeight - (buoy.position.y - buoy.radius);
            return 1.0f / 3.0f * (float)Math.PI * height * height * (3.0f * buoy.radius - height);
        }
        return 0.0f;
    }

    private float CalculateBuoyForce(float waveHeight)
    {
        if(CalculateSubmergedVolume(waveHeight) != 0.0f)
        {
            float floatabilityForce = settings.density * settings.gravity * CalculateSubmergedVolume(waveHeight) * buoySettings.buoyancyCoeficient;
            return floatabilityForce - buoySettings.mass * settings.gravity;
        }
        else
            return 0.0f;
    }

    private void BuoyEuler(float waveHeight, float dt)
    {
        buoySettings.buoyVelocity += (CalculateBuoyForce(waveHeight) / buoySettings.mass + settings.gravity) * dt;

        buoy.position.y += buoySettings.buoyVelocity * dt;
    }

    public void Update(float dt)
    {
        elapsedTime += dt;

        for (int i = 0; i < points.Length; i++)
        {
            Vector3C originalPos = points[i].originalposition;

            points[i].position = Vector3C.zero;

            for (int j = 0; j < wavesSettings.Length; j++)
            {
                WavesSettings ws = wavesSettings[j];
                float k = (float)(2.0f * Math.PI / ws.frequency);

                points[i].position += new Vector3C
                (
                    (float)(originalPos.x + ws.amplitude * k * Math.Cos(k * (Vector3C.Dot(originalPos, ws.direction) + elapsedTime * ws.speed) + ws.phase) * ws.direction.x),
                    (float)(ws.amplitude * k * Math.Sin(k * (Vector3C.Dot(originalPos, ws.direction) + elapsedTime * ws.speed) + ws.phase)),
                    (float)(originalPos.z + ws.amplitude * k * Math.Cos(k * (Vector3C.Dot(originalPos, ws.direction) + elapsedTime * ws.speed) + ws.phase) * ws.direction.z)
                );
            }
        }

        BuoyEuler(CalculateWaveHeightAtBuoy(), dt);
    }

    public void Debug()
    {
        if(points != null)
        foreach (var item in points)
        {
            item.position.Print(0.05f);
        }

        buoy.Print(Vector3C.blue);
    }
}
