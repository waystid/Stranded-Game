using Synty.SidekickCharacters.API;
using Synty.SidekickCharacters.Database;
using Synty.SidekickCharacters.Database.DTO;
using Synty.SidekickCharacters.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Feature 007 Phase D+E — Swaps the baked HumanCustomMesh with a SidekickRuntime-built
/// character assembled from the player's saved preset indices (head / upper / lower body).
///
/// Phase D: Runs in Awake() — before TDE's CharacterAnimator.Start() — swap is invisible to TDE.
/// Phase E: Exposes public SwapMesh(PlayerCharacterData) so WardrobeUI can hot-swap mid-game.
///
/// Avatar: uses SK_BaseModel avatar (loaded from Resources prefab asset) which exactly
/// matches the Synty skeleton bone names. Human-Custom-avatar bone names differ from
/// the raw Synty rig and caused silent Rebind() failure (T-pose).
///
/// DestroyImmediate: old mesh must be destroyed immediately (not deferred)
/// so Rebind() does not bind to the old skeleton bones before they disappear.
///
/// Attach to HumanCustomPlayer root alongside CharacterCustomizer.
/// </summary>
public class SyntyCharacterLoader : MonoBehaviour
{
    // ── Lifecycle ─────────────────────────────────────────────────────────────

    void Awake()
    {
        if (!PlayerCharacterData.HasSavedData()) return;

        var data = ScriptableObject.CreateInstance<PlayerCharacterData>();
        data.Load();
        SwapMesh(data);
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Hot-swap the character mesh to match the given PlayerCharacterData.
    /// Safe to call at runtime (mid-game). Re-swaps destroy the previous SidekickMesh first.
    /// </summary>
    public void SwapMesh(PlayerCharacterData data)
    {
        // ── Init Synty runtime ────────────────────────────────────────────────
        var dbManager = new DatabaseManager();

        var baseModel = Resources.Load<GameObject>("Meshes/SK_BaseModel");
        var baseMat   = Resources.Load<Material>("Materials/M_BaseMaterial");

        if (baseModel == null || baseMat == null)
        {
            Debug.LogWarning("[SyntyCharacterLoader] SK_BaseModel or M_BaseMaterial not found in Resources.");
            return;
        }

        // SK_BaseModel avatar exactly describes the Synty bone hierarchy.
        var skBaseAvatar = baseModel.GetComponent<Animator>()?.avatar;

        var runtime = new SidekickRuntime(baseModel, baseMat, null, dbManager);
        SidekickRuntime.PopulateToolData(runtime);
        var partLibrary = runtime.MappedPartDictionary;

        // ── Load preset lists ─────────────────────────────────────────────────
        var headPresets  = SidekickPartPreset.GetAllByGroup(dbManager, PartGroup.Head);
        var upperPresets = SidekickPartPreset.GetAllByGroup(dbManager, PartGroup.UpperBody);
        var lowerPresets = SidekickPartPreset.GetAllByGroup(dbManager, PartGroup.LowerBody);

        // ── Collect parts from saved preset indices ───────────────────────────
        var parts = new List<SkinnedMeshRenderer>();
        CollectPresetParts(dbManager, partLibrary, headPresets,  data.headPresetIndex,      parts);
        CollectPresetParts(dbManager, partLibrary, upperPresets, data.upperBodyPresetIndex,  parts);
        CollectPresetParts(dbManager, partLibrary, lowerPresets, data.lowerBodyPresetIndex,  parts);

        if (parts.Count == 0)
        {
            Debug.LogWarning("[SyntyCharacterLoader] No parts resolved — aborting swap.");
            return;
        }

        // ── Spawn temp character (off-screen) ─────────────────────────────────
        var tempGO = runtime.CreateCharacter("_SidekickTemp", parts, false, true);
        if (tempGO == null) return;
        tempGO.transform.position = new Vector3(9999f, 9999f, 9999f);

        // ── Find SuitModel + Animator ─────────────────────────────────────────
        var suitModel = transform.Find("SuitModel");
        if (suitModel == null)
        {
            Debug.LogWarning("[SyntyCharacterLoader] SuitModel child not found.");
            Destroy(tempGO);
            return;
        }
        var animator = suitModel.GetComponent<Animator>();

        // ── Extract skeleton root + all SMRs before reparenting ──────────────
        var skeletonRoot = tempGO.transform.Find("root");
        var smrs = tempGO.GetComponentsInChildren<SkinnedMeshRenderer>(true);

        // ── Remove previous SidekickMesh (re-swap) AND old baked mesh ────────
        // DestroyImmediate so bones are gone before Rebind().
        var existingSidekick = suitModel.Find("SidekickMesh");
        if (existingSidekick != null) DestroyImmediate(existingSidekick.gameObject);

        var oldMesh = suitModel.Find("HumanCustomMesh");
        if (oldMesh != null) DestroyImmediate(oldMesh.gameObject);

        // ── Scale container (mirrors the old 0.6667 scale to keep net scale 1.0) ──
        var container = new GameObject("SidekickMesh");
        container.transform.SetParent(suitModel, false);
        container.transform.localScale = Vector3.one * (1f / 1.5f); // 0.6667

        // ── Reparent skeleton + SMRs into container ───────────────────────────
        if (skeletonRoot != null)
            skeletonRoot.SetParent(container.transform, false);

        foreach (var smr in smrs)
            smr.transform.SetParent(container.transform, false);

        // ── Rebind Animator with SK_BaseModel avatar so bone names match Synty rig ─
        if (animator != null)
        {
            if (skBaseAvatar != null) animator.avatar = skBaseAvatar;
            animator.Rebind();
            animator.Update(0f);
            Debug.Log($"[SyntyCharacterLoader] Rebind complete. avatar={animator.avatar?.name}, isHuman={animator.isHuman}");
        }

        // ── Cleanup (tempGO is now empty — skeleton + SMRs were reparented out) ─
        Destroy(tempGO);

        Debug.Log("[SyntyCharacterLoader] Visual swap complete.");
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private static void CollectPresetParts(
        DatabaseManager dbManager,
        Dictionary<CharacterPartType, Dictionary<string, SidekickPart>> partLibrary,
        List<SidekickPartPreset> presets,
        int idx,
        List<SkinnedMeshRenderer> result)
    {
        if (presets == null || presets.Count == 0) return;
        idx = Mathf.Clamp(idx, 0, presets.Count - 1);

        var rows = SidekickPartPresetRow.GetAllByPreset(dbManager, presets[idx]);
        foreach (var row in rows)
        {
            if (string.IsNullOrEmpty(row.PartName)) continue;
            try
            {
                var typeName = Synty.SidekickCharacters.Utils.CharacterPartTypeUtils.GetTypeNameFromShortcode(row.PartType);
                var type     = Enum.Parse<CharacterPartType>(typeName);
                if (!partLibrary.ContainsKey(type)) continue;
                var dict = partLibrary[type];
                if (!dict.ContainsKey(row.PartName)) continue;
                var partGO = dict[row.PartName].GetPartModel();
                if (partGO == null) continue;
                var smr = partGO.GetComponentInChildren<SkinnedMeshRenderer>();
                if (smr != null) result.Add(smr);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[SyntyCharacterLoader] Skipping '{row.PartName}': {e.Message}");
            }
        }
    }
}
