using Synty.SidekickCharacters.API;
using Synty.SidekickCharacters.Database;
using Synty.SidekickCharacters.Database.DTO;
using Synty.SidekickCharacters.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Feature 007 Phase D — Swaps the baked HumanCustomMesh with a SidekickRuntime-built
/// character assembled from the player's saved preset indices (head / upper / lower body).
///
/// Runs in Awake() — before TDE's CharacterAnimator.Start() initialises — so the swap
/// is invisible to TDE and all gameplay components work unchanged.
///
/// After Awake() returns:
///   CharacterCustomizer.Start()  → finds new SMRs → applies saved colors. ✅
///   CharacterAnimator.Start()    → finds Animator on SuitModel with rebound skeleton. ✅
///
/// Attach to HumanCustomPlayer root alongside CharacterCustomizer.
/// </summary>
public class SyntyCharacterLoader : MonoBehaviour
{
    // ── Synty runtime state ───────────────────────────────────────────────────

    private DatabaseManager _dbManager;
    private SidekickRuntime _runtime;
    private Dictionary<CharacterPartType, Dictionary<string, SidekickPart>> _partLibrary;

    private List<SidekickPartPreset> _headPresets  = new List<SidekickPartPreset>();
    private List<SidekickPartPreset> _upperPresets = new List<SidekickPartPreset>();
    private List<SidekickPartPreset> _lowerPresets = new List<SidekickPartPreset>();

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    void Awake()
    {
        if (!PlayerCharacterData.HasSavedData()) return;

        var data = ScriptableObject.CreateInstance<PlayerCharacterData>();
        data.Load();

        // ── Init Synty runtime ────────────────────────────────────────────────
        _dbManager = new DatabaseManager();

        var baseModel = Resources.Load<GameObject>("Meshes/SK_BaseModel");
        var baseMat   = Resources.Load<Material>("Materials/M_BaseMaterial");

        if (baseModel == null || baseMat == null)
        {
            Debug.LogWarning("[SyntyCharacterLoader] SK_BaseModel or M_BaseMaterial not found in Resources.");
            return;
        }

        _runtime    = new SidekickRuntime(baseModel, baseMat, null, _dbManager);
        SidekickRuntime.PopulateToolData(_runtime);
        _partLibrary = _runtime.MappedPartDictionary;

        // ── Load preset lists ─────────────────────────────────────────────────
        _headPresets  = SidekickPartPreset.GetAllByGroup(_dbManager, PartGroup.Head);
        _upperPresets = SidekickPartPreset.GetAllByGroup(_dbManager, PartGroup.UpperBody);
        _lowerPresets = SidekickPartPreset.GetAllByGroup(_dbManager, PartGroup.LowerBody);

        // ── Collect parts from saved preset indices ───────────────────────────
        var parts = new List<SkinnedMeshRenderer>();
        CollectPresetParts(_headPresets,  data.headPresetIndex,       parts);
        CollectPresetParts(_upperPresets, data.upperBodyPresetIndex,   parts);
        CollectPresetParts(_lowerPresets, data.lowerBodyPresetIndex,   parts);

        if (parts.Count == 0)
        {
            Debug.LogWarning("[SyntyCharacterLoader] No parts resolved — aborting swap.");
            return;
        }

        // ── Spawn temp character (off-screen) ─────────────────────────────────
        var tempGO = _runtime.CreateCharacter("_SidekickTemp", parts, false, true);
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

        // ── Extract avatar + skeleton root + all SMRs before reparenting ──────
        var tempAnim    = tempGO.GetComponent<Animator>();
        var syntyAvatar = tempAnim != null ? tempAnim.avatar : null;
        var skeletonRoot = tempGO.transform.Find("root");
        var smrs = tempGO.GetComponentsInChildren<SkinnedMeshRenderer>(true);

        // ── Remove old baked mesh ─────────────────────────────────────────────
        var oldMesh = suitModel.Find("HumanCustomMesh");
        if (oldMesh != null) Destroy(oldMesh.gameObject);

        // ── Scale container (mirrors the old 0.6667 scale to keep net scale 1.0) ──
        var container = new GameObject("SidekickMesh");
        container.transform.SetParent(suitModel, false);
        container.transform.localScale = Vector3.one * (1f / 1.5f); // 0.6667

        // ── Reparent skeleton + SMRs into container ───────────────────────────
        if (skeletonRoot != null)
            skeletonRoot.SetParent(container.transform, false);

        foreach (var smr in smrs)
            smr.transform.SetParent(container.transform, false);

        // ── Rebind Animator to new skeleton ───────────────────────────────────
        if (animator != null)
        {
            if (syntyAvatar != null) animator.avatar = syntyAvatar;
            animator.Rebind();
            animator.Update(0f);
        }

        // ── Cleanup (tempGO is now empty — skeleton + SMRs were reparented out) ─
        Destroy(tempGO);

        Debug.Log("[SyntyCharacterLoader] Visual swap complete.");
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void CollectPresetParts(List<SidekickPartPreset> presets, int idx, List<SkinnedMeshRenderer> result)
    {
        if (presets == null || presets.Count == 0) return;
        idx = Mathf.Clamp(idx, 0, presets.Count - 1);

        var rows = SidekickPartPresetRow.GetAllByPreset(_dbManager, presets[idx]);
        foreach (var row in rows)
        {
            if (string.IsNullOrEmpty(row.PartName)) continue;
            try
            {
                var typeName = Synty.SidekickCharacters.Utils.CharacterPartTypeUtils.GetTypeNameFromShortcode(row.PartType);
                var type     = Enum.Parse<CharacterPartType>(typeName);
                if (!_partLibrary.ContainsKey(type)) continue;
                var dict = _partLibrary[type];
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
