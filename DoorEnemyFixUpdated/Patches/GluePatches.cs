using HarmonyLib;
using UnityEngine;

namespace DoorEnemyFixUpdated.Patches
{
    [HarmonyPatch]
    internal static class GluePatches
    {
        [HarmonyPatch(typeof(GlueGunProjectile), nameof(GlueGunProjectile.SetLandedStatic))]
        [HarmonyPostfix]
        private static void Post_LandStatic(GlueGunProjectile __instance, bool enableCollider)
        {
            // Only runs if landed on door
            if (!enableCollider)
            {
                // Still want glue to collide with other glue, so set these to prevent touching enemy
                __instance.m_landedOnEnemy = true;
                __instance.m_onEnemyVolumeScale = 1f;
            }
        }

        [HarmonyPatch(typeof(GlueGunProjectile), nameof(GlueGunProjectile.SetLandedOnEnemy))]
        [HarmonyPostfix]
        private static void Post_LandEnemy(GlueGunProjectile __instance)
        {
            __instance.m_collider.enabled = false;
        }

        [HarmonyPatch(typeof(GlueGunProjectile), nameof(GlueGunProjectile.CollisionCheck))]
        [HarmonyPostfix]
        private static void Post_Collision(GlueGunProjectile __instance, ref bool __result)
        {
            if (!__result) return;

            var collider = __instance.m_projLastRayHitCollider;
            if (collider == null || __instance.m_projLastRayHitCollider.gameObject.layer != LayerManager.LAYER_ENEMY_DAMAGABLE) return;

            var agent = collider.GetComponent<IDamageable>().GetBaseAgent();
            if (!DoorUtil.CanTouchNode(__instance.m_projTargetPos, agent.CourseNode))
            {
                // Scan again without enemies
                int mask = __instance.CollisionMask & ~LayerManager.MASK_ENEMY_DAMAGABLE;
                Vector3 move = __instance.m_projVelocity * Time.fixedDeltaTime;
                __result = Physics.SphereCast(__instance.m_projTargetPos, __instance.m_sphereCastRadius, move, out var hitInfo, 1f, mask);
                if (__result)
                {
                    __instance.m_projLastRayHitPoint = hitInfo.point;
                    __instance.m_projLastRayHitNormal = hitInfo.normal;
                    __instance.m_projLastRayHitCollider = hitInfo.collider;
                }
            }
        }
    }
}
