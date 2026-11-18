# Giovanni: Elemental Bending

Giovanni's part: gesture recognition, elemental projectiles, damage, and effects. The shared prefab is a placeholder for now.

Projectiles should call `Hurtbox.ApplyHit(damage, hitPoint)` rather than changing health fields directly. Keep combat separate from networking so Ethan can sync it.
