# Daryl: Movement and Dodging

The starter implementation is on this review branch, with ten passing Unity Editor Play Mode tests on Windows. It is not merged to main. BodyHurtboxFollow additionally keeps the body hurtbox upright and under the tracked head. Real-headset/controller validation is still required.

Planned scripts:

- Health: owns hit points. TakeDamage(), Heal(), ResetHealth(). Fires onHealthChanged and onKO events. This is the shared health component the other mechanics call.
- Hurtbox: attach to a trigger collider on the head and body. Projectiles call ApplyHit(damage, hitPoint). Forwards to Health and fires onHit for feedback and combo listeners.
- DashController: quick dash with a cooldown. Wire playerRoot (the XR Origin or player root) and a dash InputAction in the Inspector. Dashes in the camera facing direction, flattened to the ground plane.
- ArenaBounds: clamps the player root inside a rectangle. Set center and size in the Inspector to match the arena floor.
- HealthBar: skeletal UI readout. Wire a Health and a UI Slider. Enough for a first in-engine demo.

Wiring checklist for the shared scene:

1. Player root with Health, DashController, ArenaBounds.
2. Head and body trigger colliders with Hurtbox, Health assigned or found from a parent.
3. A Canvas with a Slider and HealthBar wired to the player's Health.
4. Dash InputAction assigned on DashController. Agree on the shared input map with the team first so a casting or throwing motion never triggers a dash by accident.
5. Room-scale dodging is physical: leaning and ducking move the headset and the hurtbox with it. No extra code needed for that part.
