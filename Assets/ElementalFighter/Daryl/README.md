# My movement and dodging work

My starter is in [PR #1](https://github.com/DarylOkeke/Elemental-VR-Fighter/pull/1), not main yet. It includes:

- `Health`: damage, healing, reset, and health/KO events.
- `Hurtbox`: head/body hits forwarded to health, with damage and hit-position events.
- `DashController`: forward dash, cooldown, and a separate input action per rig.
- `ArenaBounds`: keeps the player root inside the arena.
- `HealthBar`: displays the assigned player's health.
- `BodyHurtboxFollow`: keeps the body collider upright beneath the tracked head.

The shared prefab wires these together with an XR Origin and health HUD. Dash is currently Space or the left controller's primary button.

All 10 Play Mode tests pass, including the prefab's dash binding with synthetic keyboard input. The scene and full health HUD also render in the Editor. Actual keyboard input in the interactive Editor wasn't verified, and I still need headset/controller and Android testing. OpenXR currently reports `XR_ERROR_RUNTIME_UNAVAILABLE` without an active runtime.

Bounds only clamp the rig's origin, not physical room-scale movement. The headset safety boundary still matters.
