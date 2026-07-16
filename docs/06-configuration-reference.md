# 06 - Configuration Reference

## Purpose

This document defines the configuration files used by the distributed-bms platform.

Each configuration file has a clearly defined responsibility.

Configuration files should remain human-readable, easy to maintain and suitable for version control.

This document describes the purpose and meaning of each configuration field. It does not describe runtime behaviour.


---

# Configuration File Types

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
config/

  interfaces/
      cabinet-1.yaml
      cabinet-2.yaml
      chiller-plant.yaml

  systems/
      ahu-1.yaml
      ahu-2.yaml
      heating.yaml
      chilled-water.yaml
---

# Interface configuration file (interface-name.yaml)

## Purpose

Interface configuration files define communication between a Node and one or more PLCs.

It contains communication settings and point mappings.

It is the only configuration file that has knowledge of PLC-specific variables.

---

## name

**Required:** Yes

**Type:** Text

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

**Type:** Text

**Description**
Version number of the file

**Example:**
```yaml
version: 1
```

---

## description

**Required:** No

**Type:** Text

**Description**

Human-readable description of the interface.

**Example:**

```yaml
description: Control Cabinet main office building
```

---

## driver

**Required:** Yes

**Type:** Text

**Description**

Communication driver used by the interface.

**Examples include:**

* beckhoff-ads
* saia-sbus
* simulator

---

## address

**Required:** Depends on driver.

**Type:** Text

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

**Type:** Object

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

## poll-frequency

**Required:** No

**Type:** Integer

**Description**

Polling interval used by the communication driver.

**Example:**

```yaml
poll-frequency: 1000
```

---

## variables

**Required:** Yes

**Type:** List

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

# System configuration file (system-name.yaml)

(Documentation to be completed.)

---

# Component types configuration file (component-types.yaml)

(Documentation to be completed.)

---

# Point types configuration file (point-types.yaml)

(Documentation to be completed.)

---

# Future Configuration

Additional configuration files may include:

* drawings.yaml
* schedules.yaml
* users.yaml
* historian.yaml

These files will follow the same documentation structure used throughout this reference.
