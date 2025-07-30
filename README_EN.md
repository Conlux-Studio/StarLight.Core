<div align="center">

<img Height="300" Width="300" src="Assets/logo.png"/>

# StarLight.Core

[中文](/README.md) / English

#### An efficient, modular, and versatile core for Minecraft launcher

![Star](https://img.shields.io/github/stars/Conlux-Studio/StarLight.Core?logo=github&label=Star&style=for-the-badge)
![Forks](https://img.shields.io/github/forks/Conlux-Studio/StarLight.Core?logo=github&label=Forks&style=for-the-badge)
![NugetVersion](https://img.shields.io/nuget/v/StarLight_Core?logo=nuget&label=Nuget%20Version&style=for-the-badge)
![NugetDownload](https://img.shields.io/nuget/dt/StarLight_Core?logo=nuget&label=Nuget%20Downloads&style=for-the-badge)
![Issues](https://img.shields.io/github/issues-closed/Conlux-Studio/StarLight.Core?logo=github&label=Issues&style=for-the-badge)
![PR](https://img.shields.io/github/issues-pr-closed/Conlux-Studio/StarLight.Core?logo=github&label=Pull%20Requests&style=for-the-badge)
![License](https://img.shields.io/github/license/Conlux-Studio/StarLight.Core?logo=github&label=License&style=for-the-badge&color=ff7a35)

![Alt](https://repobeats.axiom.co/api/embed/ba6e9977d1c23baebac22caa8629dc6f2ae14dd9.svg "Repobeats analytics image")

</div>

<br></br>

## ✨ Features

- 🚀 **Full-Featured**: Beyond launching, it supports various login methods, installation of vanilla and other loaders,
  modpack parsing, and many encapsulated tools.

- 📦 **Modular**: The core is modular with components for launching, installation, verification, and tools, making it
  easier to use.

- 📖 **Fully Open Source**: You can view the completely open source code on GitHub, and it's licensed under the MIT
  license, allowing for reference and learning.

## 📜 Supported Features

> [!TIP]
> StarLight.Core is still under active development. However, due to the developers' academic commitments, we welcome
> contributions to the project.

✅: Supported features

☑️: Completed features requiring developer testing to identify and report issues

🧪: Functional features under continuous optimization

⚠️: Unstable features, not recommended for use

🎨: Being designed, soliciting feedback.

❌: Features not yet started, planned for future support

🧱: Not listed features, either not planned or will not be supported

| Feature                   | Status |
|---------------------------|--------|
| Launch Game               | ✅      |
| Game Finder               | ✅      |
| Java Finder               | ✅      |
| Offline Authenticator     | ✅      |
| Microsoft Authenticator   | ✅      |
| External Authenticator    | ✅      |
| Unified Pass              | ✅      |
| Vanilla Game Installer    | ✅      |
| Fabric Installer          | ✅      |
| Forge Installer           | ☑️     |
| Quilt Installer           | ❌      |
| NeoForge Installer        | 🎨     |
| Optifine Installer        | ❌      |
| Multi-threaded Downloader | ✅      |
| Skin Processor            | ✅      |
| Mod Processor             | 🧪     |
| Modpack Processor         | ❌      |
| CurseForge Downloader     | 🎨     |
| Modrinth Downloader       | 🧪     |
| Error Analyzer            | 🎨     |

## 📘 Documentation and Usage Guide

Documentation: [StarLight_Core Documentation and Help](https://mohen.wiki/)

> [!TIP]
> This also serves as a reference guide for documentation contributors.

- **Namespaces**: Each section of the documentation includes namespaces in the section titles. If you cannot find the
  corresponding methods, try manually adding the namespace.

- **Constructors**: For methods requiring instantiation, we provide complete constructor references. Choose the
  constructor that suits your needs if there are multiple.

- **Method Reference**: Provides the purpose of methods, along with parameters and return values. The code provided is
  not a direct usage example, so avoid copying it directly.

- **Parameter Details**: For complex custom structures, refer to the parameter details in the table of parameters or
  return values.

- **Console Reference**: Offers a complete usage flow and method, which helps in understanding the usage of methods.
  Direct copying is not recommended.

## 🗒️ Quick Start

### 1. Prerequisites

> [!IMPORTANT]
> If you need to use Native AOT publishing, you must use C# .NET 8.0.

> [!TIP]
> Using a newer C# .NET version can provide better JSON parsing performance.

1. Your project must use C# .NET 6.0 or later.

2. Your project targets development on Windows; cross-platform support will be added in the future.

### 2. Download

a. Install via any package manager by searching for `StarLight_Core`.

b. Install via the command line:

```shell
dotnet add package StarLight_Core
```

### 2-2. Manual Download

a. Download from [Nuget](https://www.nuget.org/packages/StarLight_Core).

b. Download from [Github Packages](https://github.com/orgs/Conlux-Studio/packages?repo_name=StarLight.Core).

### 3. Add Required References

```csharp
using StarLight_Core.Utilities;
using StarLight_Core.Authentication;
using StarLight_Core.Launch;
using StarLight_Core.Models.Launch;
```

> [!TIP]
> Some IDEs support automatic addition of references.

### 4. Get Installed Games

```csharp
var gameCore = GameCoreUtil.GetGameCores();
```

### 5. Add an Account

```csharp
var account = new OfflineAuthentication("Steve").OfflineAuth();
```

> [!NOTE]
> For more authenticators, refer to the [Documentation](https://mohen.wiki/) - Authenticators section.

### 6. Launch Game

```csharp
LaunchConfig args = new() // Configure launch parameters
{
    Account = new()
    {
        BaseAccount = account // Account
    },
    GameCoreConfig = new()
    {
        Root = ".minecraft", // Game root directory
        Version = "1.18.2" // Version to launch
    },
    JavaConfig = new()
    {
        JavaPath = "C:\\Program Files\\Java\\jdk-18.0.2.1\\bin\\javaw.exe", // Java path
        MaxMemory = 4096 // Maximum memory
    }
};
var launch = new MinecraftLauncher(args); // Instantiate launcher
var la = await launch.LaunchAsync(ReportProgress); // Launch
```

> [!NOTE]
> For more launch configurations and error handling, refer to the [Documentation](https://mohen.wiki/) - Launcher
> section.

### 7. More Components

Refer to the [StarLight_Core Documentation and Help](https://mohen.wiki/) for tutorials on using other components.

## 🌐 Discussion

Join the QQ Group: [971192670](https://qm.qq.com/q/FcmJDYRoDQ)

[StarLight Launcher Development Group](https://qm.qq.com/q/FcmJDYRoDQ)

For questions or feature suggestions, please submit
an [Issue](https://github.com/Conlux-Studio/StarLight.Core/issues).

For other needs, join the QQ group or email [Official@Conlux.Studio](mailto:official@conlux.studio).
