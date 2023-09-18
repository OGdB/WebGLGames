using UnityEngine;

/// <summary>
/// Responsible for playing various particles at provided positions.
/// </summary>
public class ParticleEffectPlayer : MonoBehaviour
{
    private static ParticleSystem[] brickParticleSystems;
    private static int currentBrickPs = 0;
    private static ParticleSystem[] woodParticleSystems;
    private static int currentWoodPs = 0;
    private static ParticleSystem[] metalParticleSystems;
    private static int currentMetalPs = 0;

    private void Awake()
    {
        brickParticleSystems = transform.GetChild(0).GetComponentsInChildren<ParticleSystem>();
        woodParticleSystems = transform.GetChild(1).GetComponentsInChildren<ParticleSystem>();
        metalParticleSystems = transform.GetChild(2).GetComponentsInChildren<ParticleSystem>();
    }

    private static void PlayParticles(ParticleSystem[] particleSystems, ref int currentIndex, Vector3 worldPosition)
    {
        particleSystems[currentIndex].transform.position = worldPosition;
        particleSystems[currentIndex].Play();

        currentIndex++;
        currentIndex %= particleSystems.Length;
    }

    public static void PlayBrickParticles(Vector3 worldPosition)
    {
        PlayParticles(brickParticleSystems, ref currentBrickPs, worldPosition);
    }

    public static void PlayWoodParticles(Vector3 worldPosition)
    {
        PlayParticles(woodParticleSystems, ref currentWoodPs, worldPosition);
    }

    public static void PlayMetalParticles(Vector3 worldPosition)
    {
        PlayParticles(metalParticleSystems, ref currentMetalPs, worldPosition);
    }

}
