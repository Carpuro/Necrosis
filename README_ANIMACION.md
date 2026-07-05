# NECROSIS://PROTOCOLO — Modelos y animaciones de prueba (Mixamo)

Guía para meter modelos rigged de prueba (Humano y Cazador) y ver animaciones,
SIN tocar la lógica del juego. Los scripts ya alimentan un `Animator` opcional:
si no asignas modelo, todo sigue funcionando con las cápsulas de siempre.

> Esto es una PRUEBA de animación (fase 1 temprana). No sustituye el greybox.
> El cableado es MANUAL por ahora; si reconstruyes la escena con
> `Fase0SceneBuilder` se pierde. Cuando elijamos modelos definitivos, los
> integramos en el builder.

---

## 1. Descargar de Mixamo (gratis, requiere cuenta Adobe)

En https://www.mixamo.com:

**Cazador (zombi):**
1. Characters → busca "Zombie" → elige uno (p. ej. "Zombie" / "Mutant").
2. Animations → descarga, para ese personaje:
   - `Zombie Idle` (o `Idle`)
   - `Zombie Walk`
   - `Zombie Running` (para la persecución / frenesí)
   - `Zombie Attack`
3. Descarga cada anim como **FBX Binary, 30 fps, Without Skin** (la malla va una
   sola vez con el personaje; las anims van "sin piel").
   El personaje: **With Skin**.

**Humano (jugador):**
1. Characters → cualquiera (p. ej. "X Bot" / "Y Bot" para prueba neutra).
2. Animations: `Idle`, `Walking`, `Running`, `Crouch Walk` (o `Crouching`).
3. Mismo esquema: personaje With Skin, anims Without Skin.

---

## 2. Importar a Unity

1. Crea carpetas: `Assets/_Necrosis/Characters/Hunter/` y `.../Player/`.
2. Arrastra los FBX ahí (el personaje + sus anims).
3. Selecciona cada FBX → Inspector → pestaña **Rig**:
   - Animation Type = **Humanoid**, Avatar Definition = **Create From This Model**
     (para el personaje) / **Copy From Other Avatar** (para los FBX de solo anim,
     apuntando al avatar del personaje). → **Apply**.
4. En cada FBX de animación → pestaña **Animation**: marca **Loop Time** en
   idle/walk/run (no en attack). Renombra el clip si quieres.

---

## 3. Animator Controller

Crea un Animator Controller por tipo (`HunterAnimator`, `PlayerAnimator`) en
la carpeta correspondiente. Parámetros EXACTOS que los scripts ya envían:

### Jugador (`PlayerController.animator`)
| Parámetro | Tipo  | Lo usa para |
|-----------|-------|-------------|
| `Speed`   | float | velocidad planar en m/s (0 = quieto, ~6.5 = corriendo) |
| `Crouch`  | bool  | true mientras te agachas |

- Blend Tree 1D con `Speed`: Idle (0) → Walk (~3.5) → Run (~6.5).
- Estado/capa de crouch conmutado por el bool `Crouch` (o un blend aparte).

### Cazador (`HunterAI.animator`)
| Parámetro   | Tipo  | Lo usa para |
|-------------|-------|-------------|
| `Speed`     | float | `NavMeshAgent.velocity.magnitude` (locomoción) |
| `Statue`    | bool  | true de noche congelado (pose de estatua) |
| `Attacking` | bool  | true en Attack/Frenzy (ataque en loop) |

- Blend Tree 1D con `Speed`: Idle (0) → Walk → Run.
- `Statue == true` → clip idle/quieto (transición desde Any State).
- `Attacking == true` → clip de ataque.

---

## 4. Cablear el modelo a la cápsula

Por cada cápsula (Player y cada Hunter) en la escena:

1. Arrastra el **personaje FBX** como HIJO de la cápsula.
2. Posiciónalo en `localPosition = (0, -1, 0)` aprox. (los pies del modelo al
   fondo de la cápsula; ajusta a ojo).
3. En el **Animator** del modelo hijo: asigna el Controller del paso 3 y
   **DESMARCA "Apply Root Motion"** (el movimiento lo mandan
   `CharacterController`/`NavMeshAgent`, no la animación).
4. Selecciona la cápsula raíz → en `PlayerController` / `HunterAI`, arrastra ese
   Animator al campo **animator**.
5. Desactiva el **MeshRenderer** de la cápsula raíz (para ver solo el modelo;
   el collider y la lógica siguen en la raíz).

Play. El Humano debería caminar/correr/agacharse y el Cazador
patrullar/perseguir/atacar/congelarse con sus clips.

---

## 5. Notas

- Rig **Humanoid** en todo = las animaciones se comparten/retargetean entre
  modelos; puedes probar una anim humana en el zombi y viceversa.
- Si ves warnings "Parameter 'Speed' does not exist": el Controller asignado no
  tiene ese parámetro con ese nombre EXACTO (respeta mayúsculas).
- El ataque real (daño) lo sigue haciendo `HunterAI`; `Attacking` es solo visual.
