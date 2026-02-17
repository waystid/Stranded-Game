#!/bin/bash
# CosmicWiki Query Helper Functions
# Usage: source scripts/helpers/wiki_query.sh

WIKI_ROOT="CosmicWiki"

# Find wiki page by name
wiki_find() {
    local item_name="$1"
    find "$WIKI_ROOT/pages" -type f -name "*${item_name}*.md" 2>/dev/null
}

# Search wiki by ACNH name
wiki_find_acnh() {
    local acnh_name="$1"
    grep -r "ACNH Equivalent.*${acnh_name}" "$WIKI_ROOT/pages" 2>/dev/null | cut -d: -f1
}

# Extract JSON data from wiki page
wiki_extract_json() {
    local wiki_file="$1"
    if [ -f "$wiki_file" ]; then
        # Extract content between ```json and ```
        sed -n '/```json/,/```/p' "$wiki_file" | sed '1d;$d'
    else
        echo "Error: File not found: $wiki_file" >&2
        return 1
    fi
}

# Search by rarity
wiki_find_rarity() {
    local rarity="$1"
    grep -r "\"rarity\": \"${rarity}\"" "$WIKI_ROOT/pages" 2>/dev/null | cut -d: -f1 | sort -u
}

# Search by class
wiki_find_class() {
    local class="$1"
    grep -r "\"primary_class\": \"${class}\"" "$WIKI_ROOT/pages" 2>/dev/null | cut -d: -f1 | sort -u
}

# Search by minimum price
wiki_find_price_min() {
    local min_price="$1"
    grep -r "\"sell_price_credits\":" "$WIKI_ROOT/pages" 2>/dev/null | \
        awk -F: -v min="$min_price" '$NF+0 >= min {print $1}' | sort -u
}

# Get item summary (quick overview)
wiki_summary() {
    local wiki_file="$1"
    if [ ! -f "$wiki_file" ]; then
        echo "Error: File not found: $wiki_file"
        return 1
    fi

    echo "=== Wiki Summary: $(basename "$wiki_file" .md) ==="
    echo

    # Extract key fields from JSON
    local json=$(wiki_extract_json "$wiki_file")

    echo "Name: $(echo "$json" | grep -o '"cosmic_name": *"[^"]*"' | cut -d'"' -f4)"
    echo "ACNH: $(echo "$json" | grep -o '"acnh_equivalent": *"[^"]*"' | cut -d'"' -f4)"
    echo "Rarity: $(echo "$json" | grep -o '"rarity": *"[^"]*"' | cut -d'"' -f4)"
    echo "Class: $(echo "$json" | grep -o '"primary_class": *"[^"]*"' | cut -d'"' -f4)"
    echo "Price: $(echo "$json" | grep -o '"sell_price_credits": *[0-9]*' | grep -o '[0-9]*') credits"
    echo
    echo "File: $wiki_file"
}

# List all items in category
wiki_list_category() {
    local category="$1"
    find "$WIKI_ROOT/pages/${category}" -type f -name "*.md" 2>/dev/null | sort
}

# Search with multiple criteria
wiki_search() {
    local results="$WIKI_ROOT/pages/**/*.md"

    while [[ $# -gt 0 ]]; do
        case $1 in
            --rarity)
                results=$(echo "$results" | xargs grep -l "\"rarity\": \"$2\"" 2>/dev/null)
                shift 2
                ;;
            --class)
                results=$(echo "$results" | xargs grep -l "\"primary_class\": \"$2\"" 2>/dev/null)
                shift 2
                ;;
            --acnh)
                results=$(echo "$results" | xargs grep -l "ACNH Equivalent.*$2" 2>/dev/null)
                shift 2
                ;;
            --price-min)
                results=$(echo "$results" | xargs grep -l "\"sell_price_credits\":" 2>/dev/null | \
                    xargs grep "\"sell_price_credits\":" | \
                    awk -F: -v min="$2" '$NF+0 >= min {print $1}')
                shift 2
                ;;
            *)
                echo "Unknown option: $1"
                return 1
                ;;
        esac
    done

    echo "$results" | sort -u
}

# Generate new wiki page using Python script
wiki_create() {
    if [ ! -f "$WIKI_ROOT/scripts/generate_wiki_page.py" ]; then
        echo "Error: Wiki generator script not found"
        return 1
    fi

    python3 "$WIKI_ROOT/scripts/generate_wiki_page.py" "$@"
}

# Print usage information
wiki_help() {
    cat <<EOF
CosmicWiki Helper Functions

Available functions:
  wiki_find <name>              - Find wiki page by item name
  wiki_find_acnh <acnh_name>    - Find wiki page by ACNH equivalent
  wiki_find_rarity <rarity>     - Find all items of specified rarity
  wiki_find_class <class>       - Find all items of specified class
  wiki_find_price_min <credits> - Find items worth at least X credits
  wiki_extract_json <file>      - Extract JSON data block from wiki page
  wiki_summary <file>           - Show quick summary of wiki page
  wiki_list_category <category> - List all pages in category
  wiki_search [options]         - Advanced search with multiple criteria
    Options: --rarity, --class, --acnh, --price-min
  wiki_create [options]         - Generate new wiki page (interactive)
  wiki_help                     - Show this help message

Examples:
  wiki_find plasma_eel
  wiki_find_acnh "Sea Bass"
  wiki_search --rarity Rare --class PickableItem
  wiki_summary CosmicWiki/pages/nebula_organisms/plasma_eel.md

To use these functions:
  source scripts/helpers/wiki_query.sh
EOF
}

echo "CosmicWiki helper functions loaded. Type 'wiki_help' for usage."
