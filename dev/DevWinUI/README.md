<h3 align="center">
  <a href="https://ghost1372.github.io">Documentation</a>
  <span> · </span>
  <a href="https://ghost1372.github.io/ReleaseNotes">Release notes</a>
  <span> · </span>
  <a href="https://github.com/ghost1372/DevWinUI/tree/main/dev/DevWinUI.Gallery">Samples</a>
  <span> · </span>
  <a href="https://apps.microsoft.com/detail/DevWinUI%20Gallery%20App/9nmx5x5dlsrq?launch=true
	&mode=mini">Gallery App (Store)</a>
</h3>

<center>

<div align="center">
	
|Packages|Download/Installation|Documentation|
|:---|:---|:---:|
|[![NuGet Version](https://img.shields.io/nuget/v/DevWinUI?label=DevWinUI)](https://www.nuget.org/packages/DevWinUI)|[![NuGet Download](https://img.shields.io/nuget/dt/DevWinUI?label=DevWinUI)](https://www.nuget.org/packages/DevWinUI)|[![Document](https://img.shields.io/badge/See%20Here-%20?logo=github&label=Document&color=red)](https://Ghost1372.github.io/DevWinUI/)|

</div>

---

# DevWinUI
 
### Experience WinUI 3 quickly and easily with the help of DevWinUI, Everything you need to develop an application is gathered in one place.


## Install
```
Install-Package DevWinUI
```

After installing, add the following resource to app.xaml

```xml
<ResourceDictionary Source="ms-appx:///DevWinUI/Themes/Generic.xaml" />
```

## ⚠️ Important: Package Renaming (v10.0.0+)

This version introduces **breaking changes** related to package renaming to support meta packages.

### Package Changes

| Version | Controls Library |
|---------|--------------|------------------|
| **v9.9.4 and below** | `DevWinUI.Controls` |
| **v10.0.0+** | `DevWinUI` |

### ResourceDictionary Path Changes

| Version | ResourceDictionary Path |
|---------|------------------------|
| **v9.9.4 and below** | `ms-appx:///DevWinUI.Controls/Themes/Generic.xaml` |
| **v10.0.0+** | `ms-appx:///DevWinUI/Themes/Generic.xaml` |

### Migration Guide

**If you are using v10.0.0 or above:**
- Install `DevWinUI` for the full package (controls + core) — recommended for most users

**If you are using v9.9.4 or below:**
- Install `DevWinUI.Controls` for custom controls, styles, and XAML resources (includes DevWinUI core)

### Note for Upgrading from v9.9.4 → v10.0.0+

Simply replace `DevWinUI.Controls` with `DevWinUI` in your project references and update the ResourceDictionary path as shown above. The `DevWinUI` package now includes everything from the old `DevWinUI.Controls` plus the core library.

## Demo

See the [Gallery](https://github.com/Ghost1372/DevWinUI) app to see how to use it

## Documentation

See Here for Online [Documentation](https://ghost1372.github.io/DevWinUI/)

![GalleryApp](https://raw.githubusercontent.com/ghost1372/DevWinUI-Resources/refs/heads/main/DevWinUI-Docs/GalleryApp.png)
