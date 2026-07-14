# 04 - Coding Standards

## Purpose

This document defines the coding standards used throughout the distributed-bms project.

Consistency is preferred over personal coding style.

These standards apply to all project contributors.

---

# General

## Readability

Code should be written for humans rather than computers.

Clear code is preferred over clever code.

If a section of code requires a lengthy explanation, consider rewriting it.

---

## Simplicity

Prefer straightforward solutions.

Avoid unnecessary abstraction.

Avoid premature optimisation.

---

## Comments

Comments should explain **why**, not **what**.

Obvious code should not be commented.

Keep comments accurate and update them when the code changes.

---

## Naming Conventions

The project uses consistent naming conventions across source code and configuration.

### Python

Python identifiers use `snake_case`.

Examples:

* `current_values`
* `publish_event()`
* `system_configuration`

### JavaScript

JavaScript identifiers also use `snake_case`.

Although `camelCase` is common in JavaScript, `snake_case` is used throughout this project to maintain consistency with Python and the configuration model.

### YAML

YAML keys use `kebab-case`.

Examples:

```yaml
poll-frequency: 1000
system-category: air-handling
component-type: supply-air-fan
```

Component names, point names and other user-defined identifiers should follow the conventions described in the configuration model.

---

## Naming

Choose descriptive names.

Avoid abbreviations unless they are standard within the building automation industry.

Examples:

* `supply_air_temperature`
* `mixing_valve`
* `alarm_state`

Avoid meaningless names such as:

* `temp1`
* `data2`
* `obj`

---

# Python

## Formatting

Indentation uses **4 spaces**.

Use spaces rather than tabs.

Lines should remain reasonably short.

Blank lines should separate logical sections.

---

## Imports

Imports are grouped as:

1. Standard library
2. Third-party packages
3. Project modules

---

## Functions

Functions should have a single responsibility.

Functions exceeding approximately one page should be reviewed for possible decomposition.

---

## Classes

Use classes only where they provide a clear advantage.

Simple modules and functions are preferred.

---

# JavaScript

## Formatting

Indentation uses **2 spaces**.

Opening braces remain on the same line.

Spacing and brackets as in regular English.

Example:

```javascript
if (value == true) {
  updatePoint();
}
```

---

## Style

Prefer standard browser APIs.

Avoid unnecessary frameworks.

Use `const` wherever practical.

Use `let` when reassignment is required.

Avoid `var`.

---

# HTML

HTML describes document structure.

Avoid embedding JavaScript within HTML.

---

# CSS

CSS describes presentation only.

Avoid inline styles.

Keep selectors simple.

---

# YAML

Indentation uses **2 spaces**.

Configuration files should remain easy to read.

Avoid unnecessary nesting.

Comments should explain intent where appropriate.

---

# Markdown

Documentation should use Markdown.

Keep documents concise.

Use headings consistently.

Examples should be complete and easy to copy.

---

# Configuration

Configuration files are part of the project source.

Configuration changes should be committed and version controlled in the same manner as source code.

---

# Git

Commit messages should be short and descriptive.

Examples:

* Add Beckhoff ADS driver
* Document scheduling architecture
* Introduce component catalogue

Avoid generic messages such as:

* Updates
* Fixes
* Miscellaneous

---

# Future Changes

These coding standards are intended to evolve.

Where a new convention improves readability or maintainability, this document should be updated before applying the convention throughout the project.
