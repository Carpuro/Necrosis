# NECROSIS://PROTOCOLO — Convención de nombres de assets

Nombres autoexplicativos que CONSERVAN la info de origen (modelo, tpose) y añaden
descripción. Todo `snake_case`, minúsculas. Carpetas por categoría.

## Modelos — `Characters/<Player|Hunter>/Models/`
`model_<personaje>_tpose.fbx`

| Personaje | Archivo |
|---|---|
| Y Bot (jugador) | `model_y_bot_tpose.fbx` |
| X Bot (jugador, repuesto) | `model_x_bot_tpose.fbx` |
| Zombi normal (Ch10) | `model_zombie_tpose.fbx`  (105MB, gitignored/local) |
| Zombi chica | `model_zombie_girl_tpose.fbx` |
| Parásito (nivel superior) | `model_parasite_tpose.fbx` |

## Animaciones — `Characters/<Player|Hunter>/Animations/<categoria>/`
`animation_<modelo>_<estado>_<detalle>.fbx`

- **modelo**: `ybot` (jugador) · `zombie` (cazador)
- **categoría/carpeta**: `locomotion` · `melee` · `death`
- **estado**: `idle`, `movement`, `crouch`, `melee`, `death`
- **detalle**: dirección/variante (`walk_straight`, `run_straight_fast`, `tostand180`, …)

### Jugador (ybot)
| Carpeta | Archivo |
|---|---|
| locomotion | `animation_ybot_idle` |
| locomotion | `animation_ybot_movement_walk_straight` |
| locomotion | `animation_ybot_movement_run_straight` |
| locomotion | `animation_ybot_movement_run_straight_fast` |
| locomotion | `animation_ybot_crouch_idle` |
| locomotion | `animation_ybot_crouch_movement_straight` |
| locomotion | `animation_ybot_crouch_movement_tostand180` |
| melee | `animation_ybot_melee_kick` |
| melee | `animation_ybot_melee_swing` |
| death | `animation_ybot_death_1`  (dos muertes distintas) |
| death | `animation_ybot_death_2` |

### Cazador (zombie)
`animation_zombie_idle`, `animation_zombie_movement_walk_straight`,
`animation_zombie_movement_run_straight`, `animation_zombie_scream` (locomotion);
`animation_zombie_melee_attack` (melee); `animation_zombie_death_1` (death).

## Regla al descargar de Mixamo
Renombrar SIEMPRE a este esquema y meter en la carpeta de categoría correcta,
aunque todavía no se use. Mover `.fbx` **y** su `.fbx.meta` juntos (preserva el
GUID). El `.meta` de un `.fbx` se llama `<nombre>.fbx.meta` (con el `.fbx`).

## Huecos visibles (lo que falta para turnos)
`animation_ybot_movement_run_turn_left/right`, `..._walk_turn_right`,
`animation_ybot_crouch_turn_left/right`.
