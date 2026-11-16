// Copyright (c) Files Community
// Licensed under the MIT License.

namespace DevWinUI;

/// <summary>
/// How expandable items present their expander in the <see cref="SidebarView"/>.
/// </summary>
public enum SidebarExpanderStyle
{
    /// <summary>
    /// Chevron on the left of the row, rows indented per level (tree view).
    /// </summary>
    Tree,
    /// <summary>
    /// Section headings with the chevron on the right and children flush left (expander groups).
    /// </summary>
    Group
}
