# 05 - Configuration Model

## Purpose

This document describes the configuration model used throughout the distributed-bms project.

The objective is to separate building knowledge from software implementation. Buildings should be configured using human-readable configuration files rather than source code.

Configuration is considered part of the project and shall be version controlled.

---

# Design Principles

The configuration model shall be:

* Human-readable
* Version controlled
* Modular
* Extensible
* Independent of PLC manufacturer
* Independent of visualisation

Where practical, adding or modifying a building should require changes to configuration rather than software.

---

# Layers

The configuration model is divided into independent layers.

## Interface

Describes communication with one or more PLCs.

Responsibilities include:

* Driver selection
* Network configuration
* PLC definitions
* Point definitions
* Scan rates
* Communication options

The Interface layer is the only layer that has knowledge of PLC variables or protocol-specific details.

---

## System

Describes the logical building systems.

Examples include:

* Air handling units
* Heating systems
* Chilled water systems
* Domestic hot water
* Lighting
* Power monitoring

A System contains Components.

---

## Component

A Component represents a single physical or logical piece of equipment.

Examples include:

* Supply air fan
* Exhaust air fan
* Mixing valve
* Temperature sensor
* Pressure sensor
* Pump
* Damper

A Component exposes one or more Point Types.

---

## Point

A Point represents a single item of operational data.

Examples include:

* Process value
* Set point
* Command
* Feedback
* Alarm
* Enable
* Running

Points are referenced by Components and mapped to PLC variables by the Interface layer.

---

## Catalogues

Catalogues define reusable object types.

Examples include:

* Component types
* Point types
* Symbol definitions
* Alarm categories

Catalogues encourage consistency across installations.

---

# Relationships

The configuration model follows this hierarchy:

```text
System
    ↓
Component
    ↓
Point
    ↓
PLC Variable
```

Only the Interface layer knows how Points are mapped to PLC variables.

---

# Separation of Responsibilities

Each layer has a single responsibility.

| Layer     | Responsibility        |
| --------- | --------------------- |
| Interface | PLC communication     |
| System    | Building organisation |
| Component | Equipment definition  |
| Point     | Operational data      |
| Catalogue | Reusable definitions  |

---

# Independence

The configuration model is independent of:

* PLC manufacturer
* Communication protocol
* User interface
* SCADA deployment
* Hardware platform

The same configuration should support a local autonomous Node or a centrally supervised deployment.

---

# Versioning

Configuration files are version controlled.

Every deployed Node should display its configuration version.

Changes should be traceable through the project's Git history.

---

# Future Configuration

The model is intended to evolve.

Future configuration layers may include:

* Drawings
* Schedules
* Users
* Alarm routing
* Historian configuration

New configuration should extend the existing model rather than replace it.
