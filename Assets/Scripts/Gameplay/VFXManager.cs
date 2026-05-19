using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public void PlayJumpPuff(Vector3 position)
    {
        SpawnBurst(position, count: 6, speed: 1.5f, size: 0.12f, lifetime: 0.25f, color: new Color(0.8f, 0.8f, 0.8f, 1f));
    }

    public void PlayDeathBurst(Vector3 position)
    {
        SpawnBurst(position, count: 14, speed: 3.5f, size: 0.2f, lifetime: 0.5f, color: Color.red);
    }

    private void SpawnBurst(Vector3 position, int count, float speed, float size, float lifetime, Color color)
    {
        GameObject go = new GameObject("VFX_Burst");
        go.transform.position = position;

        ParticleSystem ps = go.AddComponent<ParticleSystem>();

        var main = ps.main;
        main.startLifetime = lifetime;
        main.startSpeed = speed;
        main.startSize = size;
        main.startColor = color;
        main.maxParticles = count * 2;
        main.playOnAwake = false;
        main.loop = false;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.enabled = false;

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.05f;

        ps.Emit(count);

        Destroy(go, lifetime + 0.1f);
    }
}
