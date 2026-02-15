# Material Automation - Quick Start Guide

## 5-Minute Setup

### Step 1: Open the Conversion Tool
In Unity Editor:
```
Tools > Galactic Crossing > Convert Materials to Curved World
```

### Step 2: Convert Materials (One Click)
In the Material Conversion window:
1. Verify the material list loaded (should show ~38 materials)
2. Click **"Convert All Pending Materials"**
3. Wait 30-60 seconds for conversion
4. Close window when complete

**Result:** 31 new `_CurvedWorld` material variants created

### Step 3: Assign to Scene (One Click)
```
Tools > Galactic Crossing > Batch Assign Materials to Scene
```
Click "Proceed" to replace all scene materials with curved variants.

### Step 4: Validate (Optional)
```
Tools > Galactic Crossing > Validate Scene Materials
```
Verify conversion was successful.

---

## Menu Reference

| Menu Item | What It Does | When to Use |
|-----------|-------------|-------------|
| **Convert Materials to Curved World** | Opens GUI tool to create `_CurvedWorld` material duplicates | Once per project setup |
| **Batch Assign Materials to Scene** | Replaces scene materials with curved variants | After converting materials; once per scene |
| **Validate Scene Materials** | Generates report of material status | To verify conversion success |
| **List All Material Conversions** | Logs detailed material info to console | For debugging or reference |

---

## Common Commands

### Convert All Materials
```
Tools > Galactic Crossing > Convert Materials to Curved World
→ Click "Convert All Pending Materials" button
```

### Assign to Scene
```
Tools > Galactic Crossing > Batch Assign Materials to Scene
→ Click "Proceed"
```

### Check Status
```
Tools > Galactic Crossing > Validate Scene Materials
```

---

## Troubleshooting

**Materials not showing?**
- Click "Refresh List" in the conversion window
- Enable "Include Subdirectories" toggle

**Shader not found warning?**
- This is normal if curved world shader doesn't exist yet
- Materials will use fallback URP/Lit shader
- Re-assign shader after creating it

**Scene assignment didn't work?**
- Make sure you ran material conversion first
- Verify scene is saved
- Check console for error messages

---

## What Gets Created

### Before
```
/Materials/
  LoftGroundMaterial.mat
  LoftWallMaterialBlue.mat
  ... (38 total)
```

### After
```
/Materials/
  LoftGroundMaterial.mat
  LoftGroundMaterial_CurvedWorld.mat ← NEW
  LoftWallMaterialBlue.mat
  LoftWallMaterialBlue_CurvedWorld.mat ← NEW
  ... (69 total: 38 original + 31 curved variants)
```

### Excluded (Not Converted)
- Particle materials (smoke, blood, explosions)
- VFX materials (explosions, remnants)
- UI materials (reticle)

**Total:** 8 materials excluded, 31 converted

---

## Next Steps

After running material automation:

1. ✅ Materials converted (31 `_CurvedWorld` variants created)
2. ⬜ Create Curved World Shader Graph (see GDD Section 5.2)
3. ⬜ Test shader on ground material
4. ⬜ Tune curvature strength parameter
5. ⬜ Apply to all scenes

---

## Support

- See `MATERIAL_AUTOMATION_README.md` for detailed documentation
- See `MATERIAL_CONVERSION_LIST.md` for complete material list
- Check Unity Console for detailed logs during conversion

---

**Quick Reference Card**

```
┌─────────────────────────────────────────────────────────┐
│ MATERIAL AUTOMATION - QUICK COMMANDS                    │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  CONVERT:  Tools > Convert Materials to Curved World   │
│            → "Convert All Pending Materials"           │
│                                                         │
│  ASSIGN:   Tools > Batch Assign Materials to Scene     │
│            → "Proceed"                                 │
│                                                         │
│  VERIFY:   Tools > Validate Scene Materials            │
│                                                         │
├─────────────────────────────────────────────────────────┤
│  Expected Results:                                      │
│  - 31 materials converted                              │
│  - 8 materials excluded (particles/VFX)                │
│  - Total time: ~2 minutes                              │
└─────────────────────────────────────────────────────────┘
```
