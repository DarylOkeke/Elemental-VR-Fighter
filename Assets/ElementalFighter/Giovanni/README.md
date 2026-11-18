# Giovanni: Elemental Bending

Owner folder. Gesture recognition, element projectiles, damage on contact, element visuals.

Contract: projectiles deal damage through Hurtbox.ApplyHit(damage, hitPoint) or Health.TakeDamage(). Do not write to Health fields directly. Keep combat logic in separate components so Ethan can sync it.
