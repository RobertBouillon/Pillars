# Pillars

Pillars is a collection of common code for applications. Each "Pillar" provides a framework for your 
application.

## Development Status
This library is at various states of stability and is not maintained as a distributable library. 
This library is under MIT license, so feel free to clone this project as a submodule or copy / paste 
any code of interest into your project as you see fit. If appropriate, please credit me, but it is 
not required.

## Usage Instructions
This library is designed to be used as a submodule - ergo you use this as a project reference instead of
a NuGet reference. See it [GIT manual on submodules](https://git-scm.com/book/en/v2/Git-Tools-Submodules) 
for usage.

The parent project for this repo is [Pillars.dev](https://github.com/RobertBouillon/Pillars.Dev), which exists
to facilitate testing, builds, and releases. Most contributions to this repo, however, are done as part of
the development of other projects.

This project also requires [Supergene](https://github.com/RobertBouillon/Supergene) as a submodule.

### Instructions
To use this project, execute the following GIT commands in your GIT repo

1. `git submodule add https://github.com/RobertBouillon/Supergene.git`
2. `git submodule add https://github.com/RobertBouillon/Pillars.git`

After cloning the submodules, add the Supergene, Pillars, and Pillars.v1 projects to your solution


# Pillars

| Pillar | Description |
|-|-|
| Hierarchy | Basic hierarchical class support | 
| Time | Clocks and efficient timers |
| File System | An improved File / Directory API that is not tightly coupled to the local file system|
| Workers | Specialized thread containers |
| Modules | A hierarchical representation of your application's internal structure |
| Modules.Configuration | Configuration support for Modules |
| Modules.Logging | Logging support for Modules |
| Modules.Logging.Alerts | Stateful logs |
| Modules.Meters | Performance metrics for modules |

