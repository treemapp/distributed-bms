# 03 - Software Principles

## Purpose

This document defines the software principles used throughout the distributed-bms project.

The objective is to encourage software that is simple, understandable and maintainable throughout the lifetime of the project.

Technology choices shall always support the architectural principles described in **01 - Project Vision and Philosophy** and **02 - Architecture**.

---

# General Principles

## Simplicity

Simple solutions are preferred over clever solutions.

Software should be understandable by a software developer unfamiliar with the project.

Code readability takes precedence over brevity.

---

## Stability

Well-established technologies are preferred over fashionable technologies.

The project values long-term stability over rapid adoption of new frameworks.

---

## Dependencies

External dependencies should be kept to a minimum.

Every dependency introduces maintenance, security and deployment costs.

Before adding a dependency, consider whether the functionality can reasonably be implemented within the project.

---

## Open Standards

Open standards should be used wherever practical.

Examples include:

* HTTP
* REST
* Server-Sent Events (SSE)
* YAML
* SVG
* JSON

Where proprietary protocols are required (for example PLC communications), they should be isolated behind a well-defined driver interface.

---

# Programming Languages

The primary languages used by the project are:

* Python
* JavaScript
* HTML5
* CSS
* YAML
* Markdown

Each language has a clearly defined role.

---

## Python

Python implements the server application.

Responsibilities include:

* Driver management
* Configuration
* State management
* API implementation
* Event distribution

Python code should remain portable and platform-independent wherever practical.

---

## JavaScript

JavaScript implements the browser user interface.

The project deliberately avoids large JavaScript frameworks.

Browser code should rely on standard Web APIs wherever practical.

---

## HTML

HTML describes application structure.

Presentation logic should not be embedded within HTML.

---

## CSS

CSS is responsible for presentation.

Application behaviour belongs in JavaScript.

---

## YAML

YAML defines engineering metadata.

Configuration should remain human-readable and suitable for version control.

---

# Project Structure

The software should be organised into small modules with clearly defined responsibilities.

Large monolithic source files should be avoided.

Modules communicate through well-defined interfaces.

---

# Driver Model

Communication with external devices shall be implemented using drivers.

The remainder of the application shall not depend upon protocol-specific implementation details.

Drivers may be open source or proprietary.

---

# Configuration Before Code

Where practical, engineering behaviour should be defined by configuration rather than source code.

New equipment types should ideally be introduced by extending catalogues and configuration files rather than modifying software.

---

# Testing

Software should be designed to permit simulation.

A simulator driver is considered a first-class component of the project and should remain available throughout development.

---

# Documentation

Documentation is considered part of the software.

Architectural decisions shall be documented before implementation.

Documentation should evolve alongside the codebase.

---

# Evolution

The software architecture should evolve cautiously.

Backward compatibility should be maintained where practical.

Changes affecting architecture or software principles should be discussed and documented before implementation.
