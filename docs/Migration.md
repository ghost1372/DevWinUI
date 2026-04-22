<h2 align="center">⚠️ Important: Package Renaming (v10.0.0+) ⚠️</h2>

This version introduces **breaking changes** related to package renaming to support meta packages.

### Package Changes

| Version | Core Library | Controls Library |
|---------|--------------|------------------|
| **v9.9.4 and below** | `DevWinUI` | `DevWinUI.Controls` |
| **v10.0.0+** | `DevWinUI.Base` | `DevWinUI` |

### ResourceDictionary Path Changes

| Version | ResourceDictionary Path |
|---------|------------------------|
| **v9.9.4 and below** | `ms-appx:///DevWinUI.Controls/Themes/Generic.xaml` |
| **v10.0.0+** | `ms-appx:///DevWinUI/Themes/Generic.xaml` |

### Migration Guide

**If you are using v10.0.0 or above:**
- Install `DevWinUI.Base` for core utilities only
- Install `DevWinUI` for the full package (controls + core) — recommended for most users

**If you are using v9.9.4 or below:**
- Install `DevWinUI` for core utilities (services, helpers, extensions, managers)
- Install `DevWinUI.Controls` for custom controls, styles, and XAML resources (includes DevWinUI core)

### Note for Upgrading from v9.9.4 → v10.0.0+

Simply replace `DevWinUI.Controls` with `DevWinUI` in your project references and update the ResourceDictionary path as shown above. The `DevWinUI` package now includes everything from the old `DevWinUI.Controls` plus the core library.

