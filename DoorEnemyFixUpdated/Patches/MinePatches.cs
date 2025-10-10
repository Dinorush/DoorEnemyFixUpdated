using HarmonyLib;
using UnityEngine;

namespace DoorEnemyFixUpdated.Patches
{
    [HarmonyPatch]
    internal static class MinePatches
    {
        [HarmonyPatch(typeof(MineDeployerInstance), nameof(MineDeployerInstance.OnSpawnData))]
        [HarmonyPostfix]
        private static void Post_Spawn(MineDeployerInstance __instance)
        {
            __instance.m_courseNode = DoorUtil.GetCorrectNode(__instance.transform.position, __instance.m_courseNode);
        }

        // Copy/pasted vanilla logic, with an additional check before OnTargetDetected
        [HarmonyPatch(typeof(MineDeployerInstance_Detect_Laser), nameof(MineDeployerInstance_Detect_Laser.UpdateDetection))]
        [HarmonyPrefix]
        private static bool Pre_Detection(MineDeployerInstance_Detect_Laser __instance)
        {
            if (__instance.m_core.Mode == eStickyMineMode.Disabled || __instance.m_maxLineDistance == 0) return true;

            float scanDist;
            if (Physics.SphereCast(__instance.m_lineRendererAlign.position, 0.1f, __instance.m_lineRendererAlign.forward, out var hitInfo, __instance.m_maxLineDistance, __instance.m_scanMask))
            {
                scanDist = hitInfo.distance;
                if (__instance.m_enemyMask.IsInLayerMask(hitInfo.collider.gameObject))
                {
                    var damageable = hitInfo.collider.GetComponent<IDamageable>();
                    var agent = damageable.GetBaseAgent();
                    if (!DoorUtil.CanTouchNode(agent.Position, agent.CourseNode, __instance.m_core.CourseNode))
                        return false;

                    __instance.OnTargetDetected?.Invoke();
                    return false;
                }
            }
            else
            {
                scanDist = __instance.m_maxLineDistance;
            }

            if (scanDist != __instance.DetectionRange)
            {
                __instance.UpdateDetectionRange(scanDist);
            }
            return false;
        }
    }
}
