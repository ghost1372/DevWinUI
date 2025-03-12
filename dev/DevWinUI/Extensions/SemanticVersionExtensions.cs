namespace DevWinUI;
public static class SemanticVersionExtensions
{
    /// <summary>
    /// Calculates the next patch version based on the provided semantic version.
    /// </summary>
    /// <param name="semanticVersion">Represents the current version from which the next patch version will be derived.</param>
    /// <returns>Returns a new semantic version that increments the patch version.</returns>
    public static SemanticVersion NextPatchVersion(this SemanticVersion semanticVersion)
    {
        if (semanticVersion.IsPrerelease)
        {
            return new SemanticVersion(semanticVersion.Major, semanticVersion.Minor, semanticVersion.Patch);
        }

        return new SemanticVersion(semanticVersion.Major, semanticVersion.Minor, semanticVersion.Patch + 1);
    }

    /// <summary>
    /// Calculates the next minor version based on the provided semantic version.
    /// </summary>
    /// <param name="semanticVersion">Represents the current version from which the next minor version will be derived.</param>
    /// <returns>Returns a new semantic version that increments the minor version while resetting the patch version.</returns>
    public static SemanticVersion NextMinorVersion(this SemanticVersion semanticVersion)
    {
        if (semanticVersion.IsPrerelease)
        {
            return new SemanticVersion(semanticVersion.Major, semanticVersion.Minor, 0);
        }

        return new SemanticVersion(semanticVersion.Major, semanticVersion.Minor + 1, 0);
    }

    /// <summary>
    /// Calculates the next major version based on the current semantic version.
    /// </summary>
    /// <param name="semanticVersion">Represents the current version from which the next major version is derived.</param>
    /// <returns>Returns a new semantic version that represents the next major version.</returns>
    public static SemanticVersion NextMajorVersion(this SemanticVersion semanticVersion)
    {
        if (semanticVersion.IsPrerelease)
        {
            return new SemanticVersion(semanticVersion.Major, 0, 0);
        }

        return new SemanticVersion(semanticVersion.Major + 1, 0, 0);
    }
}
