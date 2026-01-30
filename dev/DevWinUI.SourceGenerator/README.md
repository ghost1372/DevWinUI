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
|[![NuGet Version](https://img.shields.io/nuget/v/DevWinUI.SourceGenerator?label=DevWinUI.SourceGenerator)](https://www.nuget.org/packages/DevWinUI)|[![NuGet Download](https://img.shields.io/nuget/dt/DevWinUI.SourceGenerator?label=DevWinUI.SourceGenerator)](https://www.nuget.org/packages/DevWinUI.SourceGenerator)|[![Document](https://img.shields.io/badge/See%20Here-%20?logo=github&label=Document&color=red)](https://Ghost1372.github.io/DevWinUISourceGenerator)|

</div>

---

# DevWinUI.SourceGenerator
 
## Install
```
Install-Package DevWinUI.SourceGenerator
```

## Example
For generating `BreadcrumbPageMappings` you need to define `<AdditionalFiles Include="**\*.xaml" Link="%(RecursiveDir)%(Filename)%(Extension)"/>`, for `NavigationPageMappings` you need to define `<AdditionalFiles Include="Assets\NavViewMenu\AppData.json" Link="%(RecursiveDir)%(Filename)%(Extension)"/>` and for `StringsPropertyGenerator` you need to define `<AdditionalFiles Include="Strings\en-US\Resources.resw" Link="%(RecursiveDir)%(Filename)%(Extension)"/>`

```xml
<ItemGroup>
  <AdditionalFiles Include="Assets\NavViewMenu\AppData.json" Link="%(RecursiveDir)%(Filename)%(Extension)"/>
  <AdditionalFiles Include="**\*.xaml" Link="%(RecursiveDir)%(Filename)%(Extension)"/>
  <AdditionalFiles Include="Strings\en-US\Resources.resw" Link="%(RecursiveDir)%(Filename)%(Extension)"/>
</ItemGroup>
```

you can define namespace for each file in `csproj` and `PropertyGroup` section:

```xml
<StringsNamespace>myStringsNamespace</StringsNamespace>
<NavigationMappingsNamespace>MyNavigationMappingsNamespace</NavigationMappingsNamespace>
<BreadcrumbMappingsNamespace>MyBreadcrumbMappingsNamespace</BreadcrumbMappingsNamespace>
```

## Demo

See the [Gallery](https://github.com/Ghost1372/DevWinUI) app to see how to use it

## Documentation

See Here for Online [Documentation](https://ghost1372.github.io/devWinUI/)

![GalleryApp](https://raw.githubusercontent.com/ghost1372/DevWinUI-Resources/refs/heads/main/DevWinUI-Docs/GalleryApp.png)
