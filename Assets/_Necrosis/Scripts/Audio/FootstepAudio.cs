using UnityEngine;

/// <summary>
/// NECROSIS://PROTOCOLO — Pasos del jugador.
/// Reproduce un bucle de pisadas mientras te mueves, según la postura
/// (agachado &lt; caminar &lt; correr), en volumen y ritmo. Encaja con el sigilo:
/// correr suena fuerte, agacharse casi calla — lo mismo que ya delata tu
/// <see cref="PlayerSignature.NoiseRadius"/> a los Cazadores.
///
/// Setup: mismo GameObject que PlayerController. Crea su propio AudioSource
/// (no toca el del Coro). El builder le asigna los clips por superficie.
/// TODO fase 1: elegir clip por superficie con un raycast al suelo.
/// </summary>
public class FootstepAudio : MonoBehaviour
{
    [Header("Clips por postura (superficie por defecto)")]
    public AudioClip walkLoop;
    public AudioClip runLoop;    // si es null, usa walkLoop acelerado
    public AudioClip crouchLoop;

    [Header("Volumen")]
    [Range(0f, 1f)] public float walkVolume = 0.5f;
    [Range(0f, 1f)] public float runVolume = 0.8f;
    [Range(0f, 1f)] public float crouchVolume = 0.2f;

    [Header("Pitch (ritmo del paso)")]
    public float walkPitch = 1f;
    public float runPitch = 1.25f;
    public float crouchPitch = 0.85f;

    [Tooltip("Velocidad mínima para considerar que te mueves (m/s).")]
    public float moveThreshold = 0.15f;

    PlayerController player;
    AudioSource src;

    void Awake()
    {
        player = GetComponent<PlayerController>();
        src = gameObject.AddComponent<AudioSource>(); // propio, aparte del Coro
        src.loop = true;
        src.playOnAwake = false;
        src.spatialBlend = 0f; // tus propios pasos: 2D
    }

    void Update()
    {
        if (player == null) return;

        // Quieto (incluye agacharse sin moverse): silencio
        if (player.PlanarSpeed < moveThreshold)
        {
            if (src.isPlaying) src.Pause();
            return;
        }

        AudioClip clip;
        float vol, pitch;
        switch (player.CurrentState)
        {
            case PlayerController.MoveState.Sprint:
                clip = runLoop != null ? runLoop : walkLoop;
                vol = runVolume; pitch = runPitch * 1.15f; // esprint: pasos más rápidos
                break;
            case PlayerController.MoveState.Run:
                clip = runLoop != null ? runLoop : walkLoop;
                vol = runVolume; pitch = runPitch;
                break;
            case PlayerController.MoveState.Crouch:
                clip = crouchLoop != null ? crouchLoop : walkLoop;
                vol = crouchVolume; pitch = crouchPitch;
                break;
            default: // Walk
                clip = walkLoop;
                vol = walkVolume; pitch = walkPitch;
                break;
        }
        if (clip == null) return;

        if (src.clip != clip) { src.clip = clip; src.Play(); }
        else if (!src.isPlaying) src.Play(); // reanudar tras Pause
        src.volume = vol;
        src.pitch = pitch;
    }
}
