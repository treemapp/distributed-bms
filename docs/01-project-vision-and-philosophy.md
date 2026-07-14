# 01 - Project Vision and Philosophy

## Vision

**distributed-bms** is an open, distributed Building Management System designed around autonomous nodes. Nodes communicate with PLCs, IOT and other devices, to visualise & record device data, alert alarm conditions and schedule operations.

A node can connect to one, several or all devices on a site. A node operates independently of other nodes, and, using the same software, a central node can interface with all other nodes on a network and function as a SCADA.

Nodes contain point and alarm states, and can contain flow diagrams (HMI), record events (historian) and scheduling.

Nodes' interfaces to PLCs, description of PLC points, flow diagrams & other drawings & presentation are all configured in human-readable YAML files, facilitating quick and live updates of HMI, SCADA, scheduling and alarm notification, by non-IT staff - e.g. HVAC technicians, HVAC engineers.

Robust site management can be assured by placing the simple, low-cost nodes in control cabinets hosting PLCs and/or other devices.

Operators connect to nodes using any web browser on any device type - phone, tablet, pc - over LAN or direct via Ethernet.

The project aims to simplify both commissioning and operation by describing building systems using open, human-readable metadata rather than proprietary SCADA databases.

---

## Objectives

The project has the following objectives:

* Distributed HMI accessible over LAN, WAN, also locally available for equipment rooms and in case of network failure.
* Run efficiently on low-cost Mini-PCs and other modest hardware.
* Provide a modern HTML5 user interface.
* Remove the need for panel PCs in equipment rooms.
* Generate user interfaces from yaml definitions.
* All nodes run the same software, only the yaml-based configuration differs from node to node.
* Flexible software means a node can funtion as a fully fledged SCADA for a site or organisation.
* Use of widely supported software and conventions enable development and maintenance by regular software development staff, as opposed to vendor-specific specialists.
* Build a platform with an expected service life measured in decades.

---

## Design Philosophy

The project is based on a small number of guiding principles.

### Nodes are autonomous

A node must continue operating correctly without a SCADA connection.

### Metadata is configuration, no config in software

Building systems are described using declarative configuration files rather than hard-coded logic.

### Supported technologies

The project will aim to support common technlogies in Building Management, including:

* Modbus
* Beckhoff
* SAIA
* Fidelix
* M-bus
* MQTT

### Small software footprint

TODO

### Open technologies

Where practical, only open and widely supported technologies are used.

Examples include:

* Python
* Flask
* HTML5
* JavaScript
* CSS
* SVG
* YAML
* HTTP
* REST APIs
* Server-Sent Events (SSE)

### Simplicity

Complexity should be removed through architecture rather than hidden inside software.

Every configuration file, API endpoint and software module should have a single, well-defined responsibility.

### Long-term maintainability

The platform should remain understandable and maintainable many years after it is deployed.

Readability is preferred over cleverness.

Simple solutions are preferred over sophisticated ones.

---

## Intended Applications

The platform is intended for distributed building automation systems including:

* Air handling units (AHUs)
* Heating systems
* Cooling systems
* Pump stations
* Plant rooms
* Energy monitoring
* Technical alarms

The architecture should remain sufficiently general that additional building systems can be added without redesigning the platform.

---

## Project Vision

The long-term goal is to provide an open, metadata-driven Building Management System that can be deployed from a single autonomous node through to a complete multi-building installation using exactly the same software.

The architecture should scale by adding nodes rather than increasing central complexity.
