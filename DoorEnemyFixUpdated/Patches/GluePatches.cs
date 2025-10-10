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
            if (!enableCollider)
                __instance.m_collider.enabled = false;
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
                __result = false;
        }
    }
}
