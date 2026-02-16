#!/usr/bin/env python3
"""
Cosmic Colony Wiki Page Generator

This script generates wiki pages from the template and JSON data.
It can create new pages or update existing ones based on the item schema.

Usage:
    python generate_wiki_page.py --category nebula_organisms --data data.json
    python generate_wiki_page.py --interactive
"""

import json
import os
import sys
import argparse
from datetime import date
from pathlib import Path


class WikiPageGenerator:
    def __init__(self, wiki_root="CosmicWiki"):
        self.wiki_root = Path(wiki_root)
        self.template_path = self.wiki_root / "templates" / "wiki_page_template.md"
        self.schema_path = self.wiki_root / "data" / "schemas" / "item_schema.json"
        self.mapping_path = self.wiki_root / "data" / "acnh_cosmic_mapping.json"

    def load_template(self):
        """Load the wiki page template."""
        with open(self.template_path, 'r') as f:
            return f.read()

    def load_mapping(self):
        """Load the ACNH to Cosmic mapping data."""
        with open(self.mapping_path, 'r') as f:
            return json.load(f)

    def load_schema(self):
        """Load the item schema."""
        with open(self.schema_path, 'r') as f:
            return json.load(f)

    def fill_template(self, template, data):
        """
        Fill the template with data from the JSON.
        Uses {{variable}} syntax for replacement.
        """
        # Extract fields from JSON structure
        acnh = data.get('acnh_data', {})
        cosmic = data.get('cosmic_data', {})
        tech = data.get('technical_implementation', {})
        meta = data.get('wiki_metadata', {})

        # Build replacement dictionary
        replacements = {
            'cosmic_name': cosmic.get('name', 'Unknown Item'),
            'acnh_name': acnh.get('name', 'Unknown'),
            'nookipedia_url': meta.get('nookipedia_url', '#'),
            'acnh_category': acnh.get('category', 'Unknown'),
            'cosmic_category': cosmic.get('category', 'Unknown'),
            'acnh_bells': str(acnh.get('sell_price', 0)),
            'cosmic_credits': str(cosmic.get('sell_price_credits', 0)),
            'acnh_rarity': acnh.get('rarity', 'Unknown'),
            'cosmic_rarity': cosmic.get('rarity', 'Unknown'),
            'acnh_location': acnh.get('location', 'Unknown'),
            'cosmic_location': cosmic.get('location', 'Unknown'),
            'cosmic_lore_description': cosmic.get('lore', ''),
            'cosmic_lore_extended': self._generate_extended_lore(cosmic),
            'item_json_data': json.dumps(data, indent=2),
            'primary_class': tech.get('primary_class', 'Unknown'),
            'implementation_overview': self._generate_implementation_overview(tech),
            'implementation_steps': self._generate_implementation_steps(tech),
            'code_example': self._extract_code_examples(tech),
            'prefab_hierarchy': self._generate_prefab_hierarchy(tech),
            'acnh_description': acnh.get('description', 'No description available'),
            'cosmic_description': cosmic.get('description', cosmic.get('lore', '')),
            'related_items_list': self._generate_related_items(meta),
            'tags_list': self._generate_tags(meta),
            'created_date': meta.get('created_date', str(date.today())),
            'updated_date': meta.get('last_updated', str(date.today())),
            'item_id': data.get('id', 'unknown_id'),
            'category': self._get_category_folder(cosmic.get('category', ''))
        }

        # Replace all {{variable}} instances
        result = template
        for key, value in replacements.items():
            result = result.replace('{{' + key + '}}', str(value))

        return result

    def _generate_extended_lore(self, cosmic_data):
        """Generate extended lore section from cosmic data."""
        lore = cosmic_data.get('lore', '')
        # Add visual theme if available
        visual = cosmic_data.get('visual_theme', {})
        if visual:
            lore += f"\n\n**Visual Characteristics:**\n"
            if 'color_palette' in visual:
                lore += f"- Color Palette: {', '.join(visual['color_palette'])}\n"
            if 'particle_effects' in visual:
                lore += f"- Effects: {', '.join(visual['particle_effects'])}\n"
        return lore

    def _generate_implementation_overview(self, tech_data):
        """Generate implementation overview text."""
        overview = f"This item uses the **{tech_data.get('primary_class', 'Unknown')}** class"
        if 'scriptable_object_type' in tech_data:
            overview += f" with a **{tech_data['scriptable_object_type']}** ScriptableObject"
        overview += "."

        if 'integration_notes' in tech_data:
            overview += f"\n\n{tech_data['integration_notes']}"

        return overview

    def _generate_implementation_steps(self, tech_data):
        """Generate formatted implementation steps."""
        steps = tech_data.get('setup_steps', [])
        if not steps:
            return "No specific setup steps provided."

        result = ""
        for step_data in steps:
            step_num = step_data.get('step', '?')
            desc = step_data.get('description', '')
            code = step_data.get('code_example', '')

            result += f"#### {step_num}. {desc}\n\n"
            if code:
                result += f"```\n{code}\n```\n\n"

        return result

    def _extract_code_examples(self, tech_data):
        """Extract and combine code examples."""
        examples = []
        for step in tech_data.get('setup_steps', []):
            if 'code_example' in step:
                examples.append(step['code_example'])

        return '\n\n'.join(examples) if examples else "// No code example provided"

    def _generate_prefab_hierarchy(self, tech_data):
        """Generate visual prefab hierarchy."""
        prefab = tech_data.get('prefab_structure', {})
        if not prefab:
            return "No prefab structure defined"

        return self._format_hierarchy_recursive(prefab, 0)

    def _format_hierarchy_recursive(self, obj, indent_level):
        """Recursively format prefab hierarchy."""
        result = ""
        indent = "  " * indent_level

        for key, value in obj.items():
            if isinstance(value, dict):
                result += f"{indent}{key}/\n"
                if 'components' in value:
                    for comp in value['components']:
                        result += f"{indent}  - {comp}\n"
                if 'children' in value:
                    for child in value['children']:
                        result += self._format_hierarchy_recursive(
                            {child.get('name', 'Child'): child}, indent_level + 1
                        )
            else:
                result += f"{indent}{key}: {value}\n"

        return result

    def _generate_related_items(self, meta_data):
        """Generate related items list."""
        related = meta_data.get('related_items', [])
        if not related:
            return "None"

        # Convert IDs to markdown links (assuming similar structure)
        return "\n".join([f"- [{item}](../{item}.md)" for item in related])

    def _generate_tags(self, meta_data):
        """Generate tags list."""
        tags = meta_data.get('tags', [])
        if not tags:
            return "None"

        return " ".join([f"`{tag}`" for tag in tags])

    def _get_category_folder(self, category):
        """Map category to folder name."""
        category_map = {
            'nebula_organism': 'nebula_organisms',
            'micro_drone': 'micro_drones',
            'ancient_artifact': 'ancient_artifacts',
            'gathering_tool': 'tools',
            'tool': 'tools',
            'material': 'materials',
            'metallic_compound': 'materials',
            'furniture': 'furniture',
            'building': 'buildings',
            'npc': 'npcs'
        }
        return category_map.get(category, 'misc')

    def generate_page(self, data_file, output_path=None):
        """
        Generate a wiki page from JSON data.

        Args:
            data_file: Path to JSON file with item data
            output_path: Optional custom output path
        """
        # Load data
        with open(data_file, 'r') as f:
            data = json.load(f)

        # Load template
        template = self.load_template()

        # Fill template
        filled = self.fill_template(template, data)

        # Determine output path
        if output_path is None:
            category = self._get_category_folder(
                data.get('cosmic_data', {}).get('category', 'misc')
            )
            item_id = data.get('id', 'unknown')
            filename = f"{item_id}.md"
            output_path = self.wiki_root / "pages" / category / filename

        # Ensure directory exists
        output_path.parent.mkdir(parents=True, exist_ok=True)

        # Write file
        with open(output_path, 'w') as f:
            f.write(filled)

        print(f"✅ Generated wiki page: {output_path}")
        return output_path

    def interactive_mode(self):
        """Run in interactive mode to create a new wiki page."""
        print("🌌 Cosmic Colony Wiki Page Generator - Interactive Mode\n")

        # Load mapping for reference
        mapping = self.load_mapping()

        # Gather basic info
        print("=== Basic Information ===")
        acnh_name = input("ACNH Item Name: ")
        cosmic_name = input("Cosmic Colony Name: ")
        category = input("Category (fish/bug/tool/material/etc): ")

        print("\n=== Pricing ===")
        sell_price = input("Sell Price (Bells/Credits): ")

        print("\n=== Lore ===")
        lore = input("Cosmic Lore Description: ")

        # Build minimal JSON
        data = {
            "id": cosmic_name.lower().replace(" ", "_"),
            "acnh_data": {
                "name": acnh_name,
                "category": category,
                "sell_price": int(sell_price) if sell_price.isdigit() else 0
            },
            "cosmic_data": {
                "name": cosmic_name,
                "category": category,
                "lore": lore,
                "sell_price_credits": int(sell_price) if sell_price.isdigit() else 0
            },
            "technical_implementation": {
                "primary_class": input("Primary TopDown Engine Class (PickableItem/Tool/etc): "),
                "setup_steps": []
            },
            "wiki_metadata": {
                "created_date": str(date.today()),
                "last_updated": str(date.today()),
                "tags": []
            }
        }

        # Save as temp JSON
        temp_file = self.wiki_root / "data" / "temp_interactive.json"
        with open(temp_file, 'w') as f:
            json.dump(data, f, indent=2)

        # Generate page
        self.generate_page(temp_file)

        print("\n✨ Wiki page generated! You can now edit it manually to add more details.")


def main():
    parser = argparse.ArgumentParser(
        description="Generate Cosmic Colony Wiki pages from JSON data"
    )
    parser.add_argument(
        '--category',
        help='Item category (e.g., nebula_organisms, tools)'
    )
    parser.add_argument(
        '--data',
        help='Path to JSON data file'
    )
    parser.add_argument(
        '--output',
        help='Custom output path'
    )
    parser.add_argument(
        '--interactive',
        action='store_true',
        help='Run in interactive mode'
    )

    args = parser.parse_args()

    generator = WikiPageGenerator()

    if args.interactive:
        generator.interactive_mode()
    elif args.data:
        generator.generate_page(args.data, args.output)
    else:
        print("Error: Provide either --data or --interactive")
        parser.print_help()
        sys.exit(1)


if __name__ == "__main__":
    main()
