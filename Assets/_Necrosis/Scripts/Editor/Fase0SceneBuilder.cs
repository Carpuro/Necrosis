using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// NECROSIS://PROTOCOLO — Constructor de la escena greybox de fase 0.
/// Automatiza los pasos manuales de README_FASE0.md para que la escena sea
/// reproducible desde cero (menú: Necrosis > Construir escena Fase 0).
///
/// Contenido: jugador + cámara al hombro + ciclo día/noche + NavMesh horneado
/// + 4 Cazadores + clip de estática para el Coro. Fase 0 completa.
/// </summary>
public static class Fase0SceneBuilder
{
    const string ScenePath = "Assets/_Necrosis/Scenes/Fase0.unity";
    const string NavMeshPath = "Assets/_Necrosis/Scenes/Fase0_NavMesh.asset";
    const string HunterMatPath = "Assets/_Necrosis/Materials/Hunter_Red.mat";
    const string StaticClipPath = "Assets/_Necrosis/Audio/static_noise.wav";

    [MenuItem("Necrosis/Construir escena Fase 0")]
    public static void BuildScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        // --- Iluminación ambiente casi negra: la noche debe ser NOCHE ---
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.02f, 0.02f, 0.03f);

        // --- Suelo ---
        var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.localScale = new Vector3(10f, 1f, 10f); // 100x100 m
        ground.isStatic = true;

        // --- Greybox: cubos como edificios/muros, con esquinas y pasillos ---
        var blocks = new List<(Vector3 pos, Vector3 scale)>
        {
            (new Vector3( 10f, 2.5f,  8f), new Vector3( 6f, 5f, 10f)),
            (new Vector3( -9f, 2.0f, 14f), new Vector3( 8f, 4f,  4f)),
            (new Vector3(-16f, 3.0f, -6f), new Vector3( 5f, 6f, 12f)),
            (new Vector3(  4f, 1.5f, -14f), new Vector3(12f, 3f,  3f)),
            (new Vector3( 20f, 2.5f, -10f), new Vector3( 4f, 5f,  8f)),
            (new Vector3( -4f, 2.0f, 26f), new Vector3(14f, 4f,  5f)),
            (new Vector3( 24f, 3.5f, 18f), new Vector3( 7f, 7f,  7f)),
            (new Vector3(-24f, 2.0f, 20f), new Vector3( 4f, 4f, 14f)),
            (new Vector3(-28f, 2.5f, -20f), new Vector3(10f, 5f,  4f)),
            (new Vector3( 12f, 2.0f, 30f), new Vector3( 3f, 4f, 10f)),
            (new Vector3( 32f, 2.0f,  2f), new Vector3( 3f, 4f, 16f)),
            (new Vector3( -2f, 1.0f,  6f), new Vector3( 3f, 2f,  3f)), // cobertura baja
        };
        var greybox = new GameObject("Greybox");
        foreach (var (pos, scale) in blocks)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "Block";
            cube.transform.SetParent(greybox.transform);
            cube.transform.position = pos;
            cube.transform.localScale = scale;
            cube.isStatic = true;
        }

        // --- NavMesh: hornear ANTES de crear cápsulas (el bake usa render meshes;
        //     si jugador/Cazadores ya existieran, dejarían agujeros en el mesh) ---
        var surface = ground.AddComponent<NavMeshSurface>();
        surface.BuildNavMesh();
        AssetDatabase.CreateAsset(surface.navMeshData, NavMeshPath);

        // --- Ciclo día/noche ---
        var sun = GameObject.Find("Directional Light").GetComponent<Light>();
        sun.shadows = LightShadows.Soft;
        var cycleGo = new GameObject("DayNightCycle");
        var cycle = cycleGo.AddComponent<DayNightCycle>();
        cycle.sunLight = sun;

        // --- Jugador (cápsula + todos los componentes) ---
        var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.tag = "Player";
        Object.DestroyImmediate(player.GetComponent<CapsuleCollider>()); // lo sustituye el CharacterController
        player.transform.position = new Vector3(0f, 1.1f, 0f);

        var controller = player.AddComponent<CharacterController>();
        controller.height = 2f;
        controller.radius = 0.4f;
        controller.center = Vector3.zero;

        var movement = player.AddComponent<PlayerController>();
        var signature = player.AddComponent<PlayerSignature>();
        player.AddComponent<PlayerHealth>();

        var audio = player.AddComponent<AudioSource>();
        audio.loop = true;
        audio.playOnAwake = true;
        // Clip de estática placeholder (ruido blanco generado); sustituir por uno
        // con más carácter (freesound.org) cuando toque pulir audio
        audio.clip = AssetDatabase.LoadAssetAtPath<AudioClip>(StaticClipPath);
        player.AddComponent<ChorusAudio>();
        player.AddComponent<DebugHUD>();

        // Pivote de cámara a la altura del hombro
        var pivot = new GameObject("CameraPivot");
        pivot.transform.SetParent(player.transform);
        pivot.transform.localPosition = new Vector3(0f, 0.6f, 0f); // 1.1 + 0.6 ≈ 1.7 m del suelo

        // Linterna (Spot) apuntando al frente
        var flashGo = new GameObject("Flashlight");
        flashGo.transform.SetParent(player.transform);
        flashGo.transform.localPosition = new Vector3(0f, 0.4f, 0.3f);
        flashGo.transform.localRotation = Quaternion.identity;
        var flash = flashGo.AddComponent<Light>();
        flash.type = LightType.Spot;
        flash.range = 15f;
        flash.spotAngle = 50f;
        flash.intensity = 2.5f;
        signature.flashlight = flash;

        // --- Cámara al hombro ---
        var cam = GameObject.Find("Main Camera");
        var shoulder = cam.AddComponent<ShoulderCamera>();
        shoulder.target = pivot.transform;
        movement.cameraTransform = cam.transform;

        // --- Cazadores: 4 cápsulas rojas repartidas, deambulan (sin puntos de patrulla) ---
        var hunterMat = AssetDatabase.LoadAssetAtPath<Material>(HunterMatPath);
        if (hunterMat == null)
        {
            hunterMat = new Material(Shader.Find("Standard")) { color = new Color(0.7f, 0.08f, 0.08f) };
            AssetDatabase.CreateAsset(hunterMat, HunterMatPath);
        }
        var hunters = new GameObject("Hunters");
        var spawnPoints = new[]
        {
            new Vector3( 15f, 1.1f,  15f),
            new Vector3(-14f, 1.1f, -14f),
            new Vector3(  2f, 1.1f,  22f),
            new Vector3(-20f, 1.1f,   8f),
        };
        foreach (var pos in spawnPoints)
        {
            var hunter = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            hunter.name = "Hunter";
            hunter.transform.SetParent(hunters.transform);
            hunter.transform.position = pos;
            hunter.GetComponent<MeshRenderer>().sharedMaterial = hunterMat;
            hunter.AddComponent<NavMeshAgent>();
            var ai = hunter.AddComponent<HunterAI>();
            ai.obstacleMask = 1 << 0; // capa Default: los muros bloquean su visión
        }

        // --- Guardar y registrar en Build Settings (PlayerHealth recarga por buildIndex) ---
        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };

        Debug.Log($"[NECROSIS] Escena de fase 0 construida y guardada en {ScenePath}");
    }
}
