# 06 - Configuration Reference

## Purpose

This document defines the configuration files used by the distributed-bms platform.

Each configuration file has a clearly defined responsibility.

Configuration files should remain human-readable, easy to maintain and suitable for version control.

This document describes the purpose and meaning of each configuration field. It does not describe runtime behaviour.

---

### Configuration File Types

- [interface-name.yaml](#interface-name)
- [system-name.yaml](#system-name)
- [component-types.yaml](#component-types)
- [point-types.yaml](#point-types)

The platform currently defines the following configuration files:

| File                 | Purpose                          |
| -------------------- | -------------------------------- |
| interface-name.yaml  | Defines communication with PLCs  |
| system-name.yaml     | Defines building systems         |
| component-types.yaml | Defines reusable component types |
| point-types.yaml     | Defines reusable point types     |

interface.yaml & one or several system.yaml are unique to a distributed node.

component-types.yaml and point-types.yaml are the same on every node.

Additional configuration files may be introduced in future releases.

A Node may contain:

```text
config/
├── interfaces/
│   ├── cabinet-1.yaml
│   ├── cabinet-2.yaml
│   └── chiller-plant.yaml
└── systems/
    ├── ahu-1.yaml
    ├── ahu-2.yaml
    ├── heating.yaml
    └── chilled-water.yaml
```
---

<a id="interface-name"></a>

# Interface configuration file (interface-name.yaml)

## Contents

- [Purpose](#purpose)
- [version](#version)
- [name](#name)
- [description](#description)
- [driver](#driver)
- [address](#address)
- [authentication](#authentication)
- [scan-interval-ms](#scan-interval-ms)
- [variables](#variables)

---

## Purpose

Interface configuration files define communication between a Node and one or more PLCs.

It contains communication settings and point mappings.

It is the only configuration file that has knowledge of PLC-specific variables.

---

## name

**Required:** Yes

**Data Type:** Text

**Description**

Unique name of the interface.

The name shall be unique within the site.

**Examples:**

```yaml
name: cabinet-1
```

---

## version

**Required:** Yes

**Data Type:** Text

**Description**
Version number of the file

**Example:**
```yaml
version: 1
```

---

## description

**Required:** No

**Data Type:** Text

**Description**

Human-readable description of the interface.

**Example:**

```yaml
description: Control Cabinet main office building
```

---

## driver

**Required:** Yes

**Data Type:** Text

**Description**

Communication driver used by the interface.

**Examples include:**

* beckhoff-ads
* saia-sbus
* simulator

---

## address

**Required:** Depends on driver.

**Data Type:** Text

**Description**

Network address or connection string required by the selected driver.

**Examples:**

```yaml
address: 10.0.19.1
```

or

```yaml
address: 192.168.10.5
```

---

## authentication

**Required:** No

**Data Type:** Object

**Description**

Authentication settings used by the selected driver.

Many industrial protocols, such as Beckhoff ADS and SAIA S-Bus, do not require authentication. Other drivers, such as REST-based APIs, may require credentials or tokens.

The contents of the `authentication` section are driver-specific.

Where practical, sensitive information such as passwords or API tokens should not be stored directly in configuration files committed to version control. Instead, drivers should support loading secrets from environment variables or external files.

### Example (Username and Password)

```yaml
authentication:
  username: operator
  password-env: EMS_PASSWORD
```

### Example (Bearer Token)

```yaml
authentication:
  bearer-token-env: EMS_TOKEN
```

### Example (No Authentication)

```yaml
# authentication section omitted
```

---

## scan-interval-ms

**Required:** No

**Data Type:** Integer

**Description**

The interval, in milliseconds, between successive reads of the PLC by the communication driver.

All time intervals within the distributed-bms project are expressed in milliseconds unless explicitly stated otherwise.

Different drivers may impose practical minimum polling intervals depending on the communication protocol and controller performance.

**Example:**

```yaml
scan-interval-ms: 1000
```

---

## variables

**Required:** Yes

**Data Type:** List

**Description**

Defines the PLC variables that are made available to the Node.

Each variable requires a unique identifier.

**Example:**

```yaml
variables:

  - id: 1411_LB11_GT21_PV
    description: Supply air temperature

  - id: 1411_LB11_GT21_CSP
    description: Supply air temperature set point
```

The mapping between PLC variables and Points is defined by the System configuration.

---

<a id="system-name"></a>

# System configuration file (system-name.yaml)

## Contents

- [Purpose](#purpose-1)
- [version](#version-1)
- [name](#name-1)
- [description](#description-1)
- [category](#category)
- [interface](#interface)
- [components](#components)
- [status](#status)
- [schedule](#schedule)
- [alarms](#alarms)

---

## Purpose

A System configuration file defines a single logical building system.

Examples include an air handling unit (AHU), heating system, chilled water system or domestic hot water system.

A Node may contain one or more System configuration files.

Each System references one or more Components and the Interface through which their Points are obtained.

---

## version

**Required:** Yes

**Data Type:** Integer

**Description**

Configuration schema version.

**Example**

```yaml
version: 1
```

---

## name

**Required:** Yes

**Data Type:** Text

**Description**

Unique name of the System.

The name shall be unique within the Node.

**Example**

```yaml
name: ahu-1
```

---

## description

**Required:** No

**Data Type:** Text

**Description**

Human-readable description of the System.

**Example**

```yaml
description: Operating Theatre 8 Air Handling Unit
```

---

## category

**Required:** Yes

**Data Type:** Text

**Description**

The category of building system.

Examples include:

* air-handling
* heating
* chilled-water
* domestic-hot-water
* lighting

**Example**

```yaml
category: air-handling
```

---

## interface

**Required:** Yes

**Data Type:** Text

**Description**

Name of the Interface configuration that provides communication with this System.

**Example**

```yaml
interface: cabinet-1
```

---

## components

**Required:** Yes

**Data Type:** List

**Description**

Defines the Components that make up the System.

Each Component references a Component Type defined in the component catalogue.

The mapping between Component Points and PLC Variables is defined within each Component.

**Example**

```yaml
components:

  - name: tf1
    type: supply-air-fan

  - name: ff1
    type: extract-air-fan

  - name: gt21
    type: supply-air-temperature-sensor
```

---

## status

**Required:** No

**Data Type:** Object

**Description**

Defines how the operational state of the System is determined.

This may reference one or more Component Points.

The exact structure is under development.

**Example:**

```yaml
status:

  running:
    point: tf1.running

  enabled:
    point: tf1.command

  summary:
    point: gp11.process-value
```

---

## schedule

**Required:** No

**Data Type:** Object

**Description**

Defines scheduling behaviour for the System.

This section will be documented when the scheduling model has been finalised.

---

## alarms

**Required:** No

**Data Type:** Object

**Description**

Defines optional System-level alarm behaviour.

This section will be documented in a future revision.


---

<a id="component-types"></a> 

# Component types configuration file (component-types.yaml)

(Documentation to be completed.)

## Contents

- [Purpose](#purpose-2)
- [version](#version-2)
- [description](#description-2)
- [point-types](#point-types)

## Purpose

The Component Type Catalogue defines the reusable physical components available to System configuration files.

Each Component Type represents a physical device or piece of equipment and defines the Point Types that it may expose.

System configuration files instantiate Component Types and map their Points to PLC Variables.

Control strategies, such as weather compensation, PID control and frost protection, are not Component Types. These are modelled separately.

---

## circulation-pump

**Description**

A pump used to circulate water within a heating or cooling circuit.

### Typical Points

* running

### Optional Points

* command
* fault
* overload
* hours-run
* start-count
* speed
* power


---

## two-position-valve

**Description**

A valve with two discrete positions, typically open or closed.

### Typical Points

* position

### Optional Points

* command
* fault

---

## co2-sensor

**Description**

Measures carbon dioxide concentration.

### Typical Points

* process-value

### Optional Points

* set-point
* high-alarm
* sensor-fault

---

## cooling-coil

**Description**

A cooling coil used to reduce air temperature.

### Typical Points

None.

### Optional Points

* inlet-temperature
* outlet-temperature
* valve-position
* condensation-alarm

---

## damper

**Description**

A motorised air damper.

### Typical Points

* position

### Optional Points

* command
* open
* closed
* fault

---

## fan

**Description**

A fan supplying conditioned air to a ventilation system.

### Typical Points

* running

### Optional Points

* command
* speed
* fault
* hours-run
* start-count

---

## filter

**Description**

An air filter used within a ventilation system.

### Typical Points

* dirty

### Optional Points

* pressure-drop
* warning
* alarm

---

## heat-exchanger

**Description**

Transfers heat between two separate air or water streams.

### Typical Points

None.

### Optional Points

* efficiency
* bypass-position
* frost-alarm

---

## heating-coil

**Description**

A heating coil used to raise air temperature.

### Typical Points

None.

### Optional Points

* inlet-temperature
* outlet-temperature
* valve-position
* frost-alarm

---

## humidity-sensor

**Description**

Measures relative humidity.

### Typical Points

* process-value

### Optional Points

* set-point
* high-alarm
* low-alarm
* sensor-fault

---

## mixing-valve

**Description**

A modulating valve used to control the temperature of a water circuit.

### Typical Points

* position

### Optional Points

* command
* fault
* manual-mode

---

## pressure-sensor

**Description**

Measures the pressure of air or water.

### Typical Points

* process-value

### Optional Points

* set-point
* high-alarm
* low-alarm
* sensor-fault

---

## temperature-sensor

**Description**

Measures the temperature of air, water or another medium.

### Typical Points

* process-value

### Optional Points

* set-point
* high-alarm
* low-alarm
* sensor-fault

---

<a id="point-types"></a>

# Point types configuration file (point-types.yaml)

(Documentation to be completed.)

## Contents

- [Purpose](#purpose-3)
- [version](#version-3)
- [point-type](#point-type)
- [description](#description-3)
- [data-type](#data-type)
- [units](#units)
- [display-format](#display-format)
- [read-only](#read-only)

---

# Status definitions configuration file (status-definitions.yaml)

(Documentation to be completed.)

---

# Future Configuration

Additional configuration files may include:

* drawings.yaml
* schedules.yaml
* users.yaml
* historian.yaml

These files will follow the same documentation structure used throughout this reference.
