# NECROSIS://PROTOCOLO — Convención de nombres de assets

Objetivo: que con solo leer el nombre sepamos EXACTO qué es cada archivo, y
detectar huecos de un vistazo. Todo en `snake_case`, minúsculas.

## Modelos de personaje — `Characters/<Player|Hunter>/`
Solo el nombre del personaje (la carpeta ya dice que es un modelo). Sin
`model_` ni `_tpose` (todos son bind pose en T).

| Actual | Nuevo |
|---|---|
| model_ybot_tpose.fbx | `y_bot.fbx` |
| model_xbot_tpose.fbx | `x_bot.fbx` |
| model_zombie_tpose.fbx | `zombie.fbx` |
| model_zombiegirl_tpose.fbx | `zombie_girl.fbx` |
| model_parasite_tpose.fbx | `parasite.fbx` |

## Animaciones — `Characters/<Player|Hunter>/Animations/`
Sin prefijo de personaje (la carpeta lo implica). Formato:

```
<categoria>_<direccion>[_<variante>]
```

- **categoria**: idle, walk, run, crouch, jump, turn, melee, death, hit
- **direccion**: straight (al frente), back, left, right, turn_left, turn_right
- **variante**: slow / fast (velocidad); normal se omite

Transiciones: `<desde>_to_<hasta>` (p. ej. `crouch_to_stand_180`).

### Mapa de renombrado (jugador actual)
| Actual | Nuevo |
|---|---|
| idle.fbx | `idle.fbx` |
| walk.fbx | `walk_straight.fbx` |
| run.fbx | `run_straight.fbx` |
| run_fast.fbx | `run_straight_fast.fbx` |
| crouch_idle.fbx | `crouch_idle.fbx` |
| crouch_walking.fbx | `crouch_straight.fbx` |
| crouch_turntostand_180.fbx | `crouch_to_stand_180.fbx` |
| melee_kick.fbx | `melee_kick.fbx` |
| melee_swing.fbx | `melee_swing.fbx` |
| death.fbx | `death_1.fbx`  (variante de muerte 1) |
| dying.fbx | `death_2.fbx`  (variante de muerte 2) |

> death_1 y death_2 son DOS animaciones de muerte distintas (Carlos). Renómbralas
> a algo descriptivo (p. ej. `death_front` / `death_backward`) cuando decidamos
> cuál es cuál en Play.

### Cazador (ya casi encaja; folder = Hunter)
`zombie_*` → sin prefijo: `idle`, `walk`, `run_straight`, `attack`,
`scream`, `death_front`.

## Huecos visibles con esta convención (lo que falta)
- `run_turn_left`, `run_turn_right`  (solo hay versión *injured*)
- `walk_turn_right`  (hay left + 180; se puede espejar)
- `crouch_turn_left/right`
