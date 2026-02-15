# Curved World Shader Creation Guide

## Overview
This guide walks you through creating the **Curved World Shader** using Unity's Shader Graph. This shader creates the "Rolling Log" effect that curves the world away from the camera.

---

## Step-by-Step Instructions

### 1. Create the Shader Graph Asset

1. In Unity Project window, navigate to **Assets/Shaders/** (create folder if needed)
2. Right-click → **Create > Shader Graph > URP > Lit Shader Graph**
3. Name it: `CurvedWorld`

### 2. Open Shader Graph

1. Double-click `CurvedWorld.shadergraph` to open the Shader Graph editor
2. You'll see a blank graph with a Master Stack on the right

### 3. Add Properties

Click the **Blackboard** (+) icon to add a property:

**Property 1: Curve Strength**
- Type: Float
- Name: `CurveStrength`
- Default Value: `0.005`
- Mode: Default
- Range: Min `0`, Max `0.02`

### 4. Build the Node Graph

This shader works by offsetting the vertex Y position based on distance from camera.

**Add these nodes (right-click in graph → Create Node):**

#### Input Nodes:
1. **Position** node
   - Set Space: `World`
   - This gives us the vertex world position

2. **Camera Position** node
   - Set Space: `World`
   - This gives us the camera's world position

#### Calculation Nodes:

3. **Subtract** node
   - Connect: Position (Output) → A (Input)
   - Connect: Camera Position (Output) → B (Input)
   - This calculates the distance vector from camera to vertex

4. **Split** node
   - Connect: Subtract (Output) → In (Input)
   - We need to extract just the Z component (distance along camera forward axis)

5. **Multiply** node (for squaring distance)
   - Connect: Split (Z output) → A (Input)
   - Connect: Split (Z output) → B (Input)
   - This gives us distance²

6. **Multiply** node (apply curve strength)
   - Connect: Previous Multiply (Output) → A (Input)
   - Drag `CurveStrength` property from Blackboard → B (Input)

7. **Multiply** node (invert)
   - Connect: Previous Multiply (Output) → A (Input)
   - Create a Float node, set value to `-1`, connect to B (Input)
   - This inverts the curve to bend downward

8. **Add** node (apply offset to Y position)
   - Connect: Position node (Y output) → A (Input)
   - Connect: Previous Multiply (Output) → B (Input)
   - This adds the curve offset to the original Y position

9. **Combine** node (Vector3)
   - Connect: Position (X output) → R (Input)
   - Connect: Add node (Output) → G (Input)
   - Connect: Position (Z output) → B (Input)
   - This rebuilds the position vector with curved Y

10. **Transform** node
    - Connect: Combine (Output) → In (Input)
    - Set Conversion: `World` to `Object`
    - This converts back to object space for the vertex shader

#### Output:

11. Connect to Master Stack:
    - Connect: Transform (Output) → Vertex (Position) on Master Stack

### 5. Configure Master Stack

In the Master Stack (right side):
- **Surface Type**: Opaque
- **Workflow**: Metallic
- **Two Sided**: Unchecked (unless you need it)

### 6. Save the Shader

- Press **Ctrl+S** (Windows) or **Cmd+S** (Mac)
- Close the Shader Graph window

---

## Create Material from Shader

1. In Project window, navigate to **Assets/Materials/** (create if needed)
2. Right-click → **Create > Material**
3. Name it: `CurvedWorld_Material`
4. In Inspector:
   - Click the **Shader** dropdown at the top
   - Select: **Shader Graphs > CurvedWorld**
5. Adjust **Curve Strength** slider:
   - Start with `0.005` (subtle curve)
   - Increase to `0.01` for more dramatic effect
   - Max `0.02` for extreme curvature

---

## Apply to Scene Objects

**CRITICAL**: Every 3D object in the scene must use this shader, or non-curved objects will appear to float!

### Automated Application (Recommended):

Use the Editor script we created:

1. Go to **Tools > Galactic Crossing > Materials > Apply Curved Shader to All Objects**
2. This will find all MeshRenderers and apply the material

### Manual Application:

1. Select all environment objects in Hierarchy
2. In Inspector, find **Materials** section
3. Drag `CurvedWorld_Material` to replace existing materials

**Objects to apply to:**
- Terrain/Ground plane
- All furniture (30+ prefabs in Loft3D)
- Walls, floors, ceilings
- Player character model
- NPC models
- Trees
- Props and decorations

---

## Testing the Shader

1. Press **Play** in Unity
2. Move the camera/player around
3. You should see:
   - World bends away from camera (horizon curves downward)
   - Objects further from camera appear lower
   - Everything curves together smoothly

### Troubleshooting:

**No curve visible:**
- Increase `CurveStrength` to 0.01 or higher
- Make sure material is applied to ground/terrain
- Check camera is far enough from objects (5-10 units minimum)

**Objects floating:**
- Some objects still have old materials
- Run the automation script again to apply shader to all objects

**Curve too extreme:**
- Reduce `CurveStrength` to 0.003 or lower
- Adjust based on your camera distance

**Performance issues:**
- The shader is vertex-shader only (very fast)
- If issues occur, reduce polygon count on distant objects
- Use LOD (Level of Detail) system for far objects

---

## Next Steps After Shader

Once the shader is working:

1. **Test movement** - Grid-based character movement should work
2. **Place environment objects** - Add trees, items, NPC to scene
3. **Test interactions** - Tree shaking, item pickup, dialogue
4. **Adjust camera** - Position camera for best view of curved world
5. **Polish lighting** - Adjust lights to complement the curved aesthetic

---

## Reference Values

**Recommended Settings:**

| Setting | Value | Notes |
|---------|-------|-------|
| CurveStrength | 0.005 - 0.008 | Good for standard gameplay |
| Camera Distance | 7-10 units | Best viewing distance |
| Camera Angle | 30-45° downward | Top-down view |
| Field of View | 35-40° | Narrower FOV enhances curve |

**Extreme Settings (for testing):**

| Setting | Value | Notes |
|---------|-------|-------|
| CurveStrength | 0.015 - 0.02 | Very pronounced curve |
| Camera Distance | 15+ units | Far view shows curve dramatically |

---

## Visual Reference

The final shader graph should look like this flow:

```
Position (World) ──┬─> Subtract ──> Split ──> Multiply (Z×Z) ──> Multiply (×Strength) ──> Multiply (×-1) ──> Add ──┐
                   │                  │                                                                              │
Camera Pos ────────┘                  │                                                                              │
                                      │                                                                              │
Position (X) ─────────────────────────┴───────────────────────────────────────────────────────────────> Combine ─> Transform ─> Vertex Position
Position (Y) ──────────────────────────────────────────────────────────────────────────────────────────┘               (World→Object)
Position (Z) ─────────────────────────────────────────────────────────────────────────────────────────┘
```

---

## Shader Math Explanation

The shader calculates:

```
distanceFromCamera = vertexPosition.z - cameraPosition.z
curveOffset = -(distanceFromCamera²) × curveStrength
newY = vertexPosition.y + curveOffset
```

This creates a parabolic curve where objects further from the camera (larger Z distance) are pushed down more dramatically (due to the square function).

The negative sign makes the curve bend away from the camera (downward), creating the "Rolling Log" or "curved planet" effect.

---

## Done!

Once you've created and applied this shader, the world will visibly curve. Combined with the grid movement and cozy interactions, this creates the unique Animal Crossing-style feel of the Galactic Crossing MVP.
