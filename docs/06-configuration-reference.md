# 06 - Configuration Reference

## Purpose

This document defines the configuration files used by the distributed-bms platform.

Each configuration file has a clearly defined responsibility.

Configuration files should remain human-readable, easy to maintain and suitable for version control.

This document describes the purpose and meaning of each configuration field. It does not describe runtime behaviour.

---

# Configuration Files

The platform currently defines the following configuration files:

| File                 | Purpose                          |
| -------------------- | -------------------------------- |
| interface.yaml       | Defines communication with PLCs  |
| system.yaml          | Defines building systems         |
| component-types.yaml | Defines reusable component types |
| point-types.yaml     | Defines reusable point types     |

Additional configuration files may be introduced in future releases.

---

# interface.yaml

## Purpose

`interface.yaml` defines communication between a Node and one or more PLCs.

It contains communication settings and point mappings.

It is the only configuration file that has knowledge of PLC-specific variables.

---

## name

**Required**

Yes

**Type**

Text

**Description**

Unique name of the interface.

The name shall be unique within the site.

Examples:

```yaml
name: 1411-AS01
```

---

## description

**Required**

No

**Type**

Text

**Description**

Human-readable description of the interface.

Example:

```yaml
description: Apparatskåp Hus 11 Plan 5
```

---

## driver

**Required**

Yes

**Type**

Text

**Description**

Communication driver used by the interface.

Examples include:

* beckhoff-ads
* saia-sbus
* simulator

---

## address

**Required**

Depends on driver.

**Type**

Text

**Description**

Network address or connection string required by the selected driver.

Examples:

```yaml
address: 10.0.19.1
```

or

```yaml
address: 192.168.10.5
```

---

## poll-frequency

**Required**

No

**Type**

Integer

**Description**

Polling interval used by the communication driver.

Example:

```yaml
poll-frequency: 1000
```

---

## variables

**Required**

Yes

**Type**

List

**Description**

Defines the PLC variables that are made available to the Node.

Each variable requires a unique identifier.

Example:

```yaml
variables:

  - id: 1411_LB11_GT21_PV
    description: Supply air temperature

  - id: 1411_LB11_GT21_CSP
    description: Supply air temperature set point
```

The mapping between PLC variables and distributed-bms Points is defined by the System configuration.

---

# system.yaml

(Documentation to be completed.)

---

# component-types.yaml

(Documentation to be completed.)

---

# point-types.yaml

(Documentation to be completed.)

---

# Future Configuration

Additional configuration files may include:

* drawings.yaml
* schedules.yaml
* users.yaml
* historian.yaml

These files will follow the same documentation structure used throughout this reference.
