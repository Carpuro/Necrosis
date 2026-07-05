using UnityEngine;

/// <summary>
/// NECROSIS://PROTOCOLO — Firma del jugador (GDD pilar 2 + v0.5 §3.2 "Eres el faro").
/// Expone dos radios de detección que la IA consulta:
///  - NoiseRadius: ruido por movimiento (agachado &lt; caminar &lt; correr)
///  - EnergyRadius: firma energética (linterna). De noche se multiplica.
///
/// Setup: mismo GameObject que PlayerController. Asignar un Spot Light hijo
/// en 'flashlight' (la linterna). Tecla F para alternar.
/// </summary>
[RequireComponent(typeof(PlayerController))]
public class PlayerSignature : MonoBehaviour
{
    [Header("Ruido por movimiento (metros)")]
    public float idleNoise = 0.5f;
    public float crouchNoise = 1.5f;
    public float walkNoise = 6f;
    public float runNoise = 14f;

    [Header("Firma energética (metros)")]
    public float flashlightEnergyRadius = 8f;
    [Tooltip("De noche, la ciudad calla y tú brillas: multiplicador nocturno")]
    public float nightEnergyMultiplier = 10f;

    [Header("Visibilidad (× al alcance visual del Cazador)")]
    [Tooltip("Estilo Project Zomboid: agacharse y quedarse quieto te hace más difícil de VER.")]
    public float idleVisibility = 0.7f;
    public float crouchVisibility = 0.45f;
    public float walkVisibility = 1f;
    public float runVisibility = 1.25f;

    [Header("Referencias")]
    public Light flashlight;

    public float NoiseRadius { get; private set; }
    public float EnergyRadius { get; private set; }
    /// <summary>Multiplicador de "qué tan fácil es verte" según tu postura/movimiento.</summary>
    public float VisibilityScale { get; private set; }
    public bool FlashlightOn { get; private set; }

    PlayerController player;

    void Awake()
    {
        player = GetComponent<PlayerController>();
        if (flashlight != null) flashlight.enabled = false;
    }

    void Update()
    {
        // --- Linterna (F) ---
        if (Input.GetKeyDown(KeyCode.F))
        {
            FlashlightOn = !FlashlightOn;
            if (flashlight != null) flashlight.enabled = FlashlightOn;
        }

        // --- Ruido según estado de movimiento ---
        NoiseRadius = player.CurrentState switch
        {
            PlayerController.MoveState.Idle => idleNoise,
            PlayerController.MoveState.Crouch => crouchNoise,
            PlayerController.MoveState.Walk => walkNoise,
            PlayerController.MoveState.Run => runNoise,
            _ => idleNoise
        };

        // --- Visibilidad según postura/movimiento (el Cazador la usa para su alcance visual) ---
        VisibilityScale = player.CurrentState switch
        {
            PlayerController.MoveState.Idle => idleVisibility,
            PlayerController.MoveState.Crouch => crouchVisibility,
            PlayerController.MoveState.Walk => walkVisibility,
            PlayerController.MoveState.Run => runVisibility,
            _ => walkVisibility
        };

        // --- Firma energética ---
        float energy = FlashlightOn ? flashlightEnergyRadius : 0f;
        bool night = DayNightCycle.Instance != null && DayNightCycle.Instance.IsNight;
        EnergyRadius = night ? energy * nightEnergyMultiplier : energy;
    }

    // Visualización en el editor para tunear a ojo
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, NoiseRadius);
        Gizmos.color = new Color(0f, 1f, 1f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, EnergyRadius);
    }
}
