using System;
using System.Runtime.InteropServices;
using static AA3_Waves;
using static UnityEditor.Progress;

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

    private float CalculateWaveHeightAtBuoy(WavesSettings ws)
    {
        float k = (float)(2.0f * Math.PI / ws.frequency);
        return (float)(ws.amplitude * k * Math.Sin(k * (Vector3C.Dot(buoy.position, ws.direction) + elapsedTime * ws.speed) + ws.phase));
    }

    private float CalculateSubmergedVolume(WavesSettings ws)
    {
        float bottomHeight = buoy.position.y - buoy.radius;
        float height = CalculateWaveHeightAtBuoy(ws) - bottomHeight;
        return (((float)Math.PI * height * height) * 0.3333f) * 3.0f * (buoy.radius - height);
    }

    private float CalculateBuoyForce(WavesSettings ws)
    {
        float floatabilityForce = settings.density * settings.gravity * CalculateSubmergedVolume(ws) * buoySettings.buoyancyCoeficient;
        return floatabilityForce - buoySettings.mass * settings.gravity;
    }

    private void BuyoEuler(WavesSettings ws, float dt)
    {
        buoySettings.buoyVelocity += CalculateBuoyForce(ws)/buoySettings.mass - settings.gravity * dt;
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

                BuyoEuler(ws, dt);
            }
        }
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
