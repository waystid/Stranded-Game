# {{cosmic_name}}

> **ACNH Equivalent:** [{{acnh_name}}]({{nookipedia_url}})

---

## Quick Reference

| Property | ACNH Value | Cosmic Colony Value |
|----------|-----------|---------------------|
| **Name** | {{acnh_name}} | {{cosmic_name}} |
| **Category** | {{acnh_category}} | {{cosmic_category}} |
| **Sell Price** | {{acnh_bells}} Bells | {{cosmic_credits}} Credits |
| **Rarity** | {{acnh_rarity}} | {{cosmic_rarity}} |
| **Location** | {{acnh_location}} | {{cosmic_location}} |

---

## 🌌 Cosmic Lore

{{cosmic_lore_description}}

### In-Universe Context
{{cosmic_lore_extended}}

---

## 📊 Data Fields (LLM-Friendly)

```json
{{item_json_data}}
```

---

## 🎮 Technical Implementation (Unity + TopDown Engine)

### Primary Class: `{{primary_class}}`

**Setup Overview:**
{{implementation_overview}}

### Step-by-Step Configuration

{{implementation_steps}}

### Code Example

```csharp
{{code_example}}
```

### Prefab Structure
```
{{prefab_hierarchy}}
```

---

## 🔄 Translation Actions

### 🌟 Translate Lore
**Original ACNH Description:**
> {{acnh_description}}

**Cosmic Translation:**
> {{cosmic_description}}

**Translation Guide:**
- Replace "island" terminology with "planet/colony"
- Swap natural elements (water → plasma, trees → xeno-flora)
- Add sci-fi flavor (bioluminescent, energy-based, crystalline)
- Maintain the cozy, exploration-focused tone

### 📐 Define Additional Data Fields

Need to add more data? Use this template:

```json
{
  "custom_field_name": "value",
  "gameplay_mechanic": "description",
  "spawn_rate": 0.0,
  "required_tool": "tool_id"
}
```

### 🔧 Extend Unity Implementation

**Additional Components to Consider:**
- Particle System (for cosmic effects)
- Audio Source (space-themed sounds)
- Light Component (bioluminescence)
- MMFeedbacks (collection feedback)

---

## 📎 Related Items

{{related_items_list}}

---

## 🏷️ Tags

{{tags_list}}

---

<sub>**Wiki Metadata:**</sub>
<sub>Created: {{created_date}} | Last Updated: {{updated_date}} | ID: `{{item_id}}`</sub>

---

## 🤖 LLM Agent Integration

### Query Examples for Claude Code:

```
# Finding this item data
Read the file: CosmicWiki/pages/{{category}}/{{item_id}}.md

# Getting JSON data only
Grep for "```json" in CosmicWiki/pages/{{category}}/{{item_id}}.md

# Implementation reference
Search for "Technical Implementation" section in {{item_id}}.md
```

### Agent Usage Pattern:
1. **Research Phase**: Read this wiki page to understand the item
2. **Planning Phase**: Extract JSON data and implementation notes
3. **Implementation Phase**: Follow step-by-step technical guide
4. **Validation Phase**: Check against data fields for completeness

---

