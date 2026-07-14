# 02 - Architecture

TODO Terminology to be finalised before v0.1

## Purpose

This document describes the architectural principles of **distributed-bms**.

It defines the responsibilities of the major parts of the system and the relationships between them.

The purpose of this document is to ensure that architectural decisions remain consistent throughout the lifetime of the project.

---

# Overview

The platform is based on autonomous distributed nodes.

Each node is responsible for monitoring and controlling one or more local building systems.

A central Building Management System (BMS), where present, supervises the nodes but is not required for normal operation.

```
                 Operator

                    │

             HTML5 Browser

                    │

          HTTP / REST / SSE

                    │

            Distributed Node
      (Python / Flask Application)

          │                  │

     Configuration        PLC Driver
     (YAML files)      (Beckhoff, etc.)

          │                  │

          └──────────┬───────┘

                     │

                   PLC(s)

                     │

             Building Systems
```

---

# Architectural Principles

## Nodes are autonomous

Each node shall continue to operate correctly without communication to a central BMS.

Loss of network connectivity shall not prevent local monitoring or control.

---

## Sources of Truth

### PLC

The PLC is the authoritative source for:

* Process values
* Equipment status
* Control state
* Local alarms
* Control logic

### Node

The Node is the authoritative source for:

* Interface metadata
* System definitions
* Component definitions
* Visualisation metadata
* Configuration versions

### Central BMS

The Central BMS, where present, is the authoritative source for:

* User accounts
* Long-term historian
* Multi-site reporting
* Enterprise alarm

---

## The central BMS is a client

The central BMS (if present) communicates with nodes through published APIs.

It supervises the distributed nodes but does not own their configuration.

Typical responsibilities include:

* Multi-building overview
* Long-term historian
* Alarm routing
* Reporting
* User management

---

## PLCs control plant

PLCs remain responsible for deterministic control.

Typical responsibilities include:

* Control loops
* Interlocks
* Safety functions
* Equipment sequencing
* Local plant logic

The distributed node can be seen as a gateway to the PLC, it can not replace it.

---

## Metadata-driven

Building systems are described using metadata rather than hard-coded software.

Configuration is intended to be human-readable and version controlled.

The primary configuration layers are:

* Interface
* System
* Component Catalogue
* Symbol Catalogue

Each layer has a single responsibility.

---

# Configuration Layers

## Interface

Describes communication with PLCs.

Examples include:

* Driver
* Network address
* Point definitions
* Mapping between PLC variables and Node points
* Point descriptions
* Scan rates

---

## System

Describes the öogocal structure of a building system.

Examples include:

* Air Handling Unit
* Heating Circuit
* Pump Station

Systems are composed of reusable components.

---

## Component Catalogue

Defines standard component types.

Examples include:

* Supply air fan
* Mixing valve
* Temperature sensor
* Pressure sensor

Each component type defines expected behaviour and available properties.

---

## Symbol Catalogue

Defines how component types are represented graphically.

Changing the graphical representation of a component shall not require changes to the yaml config or the node's program.

---

# Network Architecture

The node shall not depend on routed network traffic to communicate with its associated PLC(s).

The node should ideally exist in the same cabinet & on the same physical switch as its PLC, and on the same local subnet.

---

# Communications

The platform uses open protocols.

Current interfaces include:

* HTTP
* REST
* Server-Sent Events (SSE)

Additional interfaces may be added provided they remain consistent with the architectural principles described in this document.

---

# Failure Modes

## WAN unavailable

Nodes continue operating normally.

Local HMIs remain available.

Central supervision and historian functions are unavailable until communication is restored.

---

## Central BMS unavailable

Nodes continue operating normally.

Schedules already deployed continue executing.

Alarm forwarding and long-term historian functions are temporarily unavailable.

---

## Node replacement

A node may be replaced without changing the architectural model.

Configuration is restored from version-controlled metadata (e.g. YAML files).

The replacement node immediately becomes the new source of truth.

---

# Scope

This document intentionally describes the architecture rather than implementation details.

Programming languages, directory structures, APIs and coding standards are described in separate documents.
