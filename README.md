# Unity Essentials

This module is part of the Unity Essentials ecosystem and follows the same lightweight, editor-first approach.
Unity Essentials is a lightweight, modular set of editor utilities and helpers that streamline Unity development. It focuses on clean, dependency-free tools that work well together.

All utilities are under the `UnityEssentials` namespace.

```csharp
using UnityEssentials;
```

## Installation

Install the Unity Essentials entry package via Unity's Package Manager, then install modules from the Tools menu.

- Add the entry package (via Git URL)
    - Window → Package Manager
    - "+" → "Add package from git URL…"
    - Paste: `https://github.com/CanTalat-Yakan/UnityEssentials.git`

- Install or update Unity Essentials packages
    - Tools → Install & Update UnityEssentials
    - Install all or select individual modules; run again anytime to update

---

# Editor Window Drawer

> Quick overview: A tiny fluent API for building polished EditorWindow UIs fast. Compose header/body/footer and an optional resizable pane, choose a skin, wire pre/post/update hooks, and show as Window/Utility/Popup/DropDown.

EditorWindowDrawer is a thin wrapper over `EditorWindow` that standardizes layout and boilerplate. It gives you a fluent builder to define sections (Header, Body, Footer, optional Pane on any side), optional border, style skins, and helpers to show the window in common modes. It also exposes handy hooks (Initialize, Pre/Post process, Update) and returns `Repaint`/`Close` delegates for your logic.

![screenshot](Documentation/Screenshot.png)

## Features
- Fluent window builder
  - `SetInitialization`, `SetPreProcess`, `SetPostProcess`, `AddUpdate`
  - `SetHeader`, `SetBody`, `SetFooter` with optional skins
  - `SetPane` with side: Left/Top/Right/Bottom, optional GenericMenu, and skin
- Ready-to-show modes
  - `ShowAsWindow`, `ShowAsUtility`, `ShowAsPopup` (auto-close on focus loss), `ShowAsDropDown(Rect,size)`
- Clean layout primitives
  - Standardized Header → Body (with optional split pane) → Footer frame
  - Optional border (`SetDrawBorder`)
  - Built-in skins: None, Margin, BigMargin, Box, Window, HelpBox, Toolbar, Dark, Light
- Handy helpers
  - `GetRepaintEvent(out Action repaint)`, `GetCloseEvent(out Action close)`
  - `BodyScrollPosition` and `PaneScrollPosition` for manual scroll handling
  - Theme colors: `NormalColor`, `HighlightColor`, `LightColor`, `DarkColor`, `BlackColor`, `BorderColor`
- Sensible defaults
  - Positions new windows at mouse (or centered via parameters)
  - Auto-closes if no sections are set (prevents empty windows)

## Requirements
- Unity Editor 6000.0+ (Editor-only; no runtime code)
- No external dependencies

Tip: Use `ShowAsPopup` for lightweight pickers and `ShowAsDropDown` to anchor to a control/rect.

## Usage
Minimal window

```csharp
// Inside an editor script
[MenuItem("Tools/My Sample Window")]
private static void Open()
{
    EditorWindowDrawer
        .CreateInstance("My Sample", new(320, 200), new(420, 320))
        .SetBody(() =>
        {
            GUILayout.Label("Hello from Body", EditorStyles.boldLabel);
            GUILayout.Label("Put your content here.");
        }, EditorWindowStyle.Margin)
        .ShowAsWindow();
}
```

Header + Body + Footer with actions

```csharp
EditorWindowDrawer.CreateInstance("Processor", new(360, 240), new(520, 360))
    .SetInitialization(() => Debug.Log("Init once"))
    .SetPreProcess(() => { /* runs at start of OnGUI */ })
    .SetPostProcess(() => { /* runs at end of OnGUI */ })
    .SetHeader(() =>
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Processor", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Help", GUILayout.Width(64))) Application.OpenURL("https://…");
        GUILayout.EndHorizontal();
    }, EditorWindowStyle.Toolbar)
    .SetBody(() =>
    {
        GUILayout.Label("Main content area");
    }, EditorWindowStyle.Margin)
    .SetFooter(() =>
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Status: Ready", EditorStyles.miniLabel);
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Close", GUILayout.Width(80))) GUIUtility.ExitGUI();
        GUILayout.EndHorizontal();
    }, EditorWindowStyle.HelpBox)
    .GetRepaintEvent(out var Repaint)
    .GetCloseEvent(out var Close)
    .ShowAsUtility();
```

Add a resizable pane (left, right, top, or bottom)

```csharp
EditorWindowDrawer.CreateInstance("With Pane", new(420, 300), new(720, 420))
    .SetPane(() =>
    {
        // Use PaneScrollPosition for long lists
        GUILayout.Label("Pane");
        // e.g., filters, tree, selection list …
    }, EditorPaneStyle.Left, EditorWindowStyle.Box)
    .SetBody(() =>
    {
        // Use BodyScrollPosition if you draw scrolling content
        GUILayout.Label("Main Body");
    }, EditorWindowStyle.Margin)
    .SetFooter(() => GUILayout.Label("Footer", EditorStyles.centeredGreyMiniLabel))
    .SetDrawBorder()
    .ShowAsWindow();
```

Show as a dropdown anchored to a control

```csharp
// In an inspector/editor OnGUI method
var rect = GUILayoutUtility.GetLastRect();
if (GUILayout.Button("Pick…", GUILayout.Width(80)))
{
    EditorWindowDrawer.CreateInstance()
        .SetBody(() => GUILayout.Label("Dropdown content"), EditorWindowStyle.Margin)
        .ShowAsDropDown(rect, new Vector2(240, 280));
}
```

## How It Works
- Lifecycle
  - `CreateInstance(title?, minSize?, size?, position?, centerX?, centerY?)` configures desired size/title/position
  - Section actions (`SetHeader/SetPane/SetBody/SetFooter`) and hooks (`SetInitialization/SetPreProcess/SetPostProcess`) are stored
  - `ShowAs…` presents the window and runs `Initialization` once; `PreProcess`/`PostProcess` wrap each OnGUI
- Layout
  - OnGUI draws: optional border → window frame → Header → Body with optional split Pane → Footer
  - Skins are applied per-section to wrap content with Editor built-in styles (HelpBox/Toolbar/etc.)
- Behavior
  - `ShowAsPopup` and `ShowAsDropDown` set a flag to close on lost focus
  - If all section actions are null, a background update closes the window automatically
  - Colors adjust to Pro/Light skin at enable time and are available to your content code

## Notes and Limitations
- Provide at least one section (`Header`, `Body`, `Pane`, or `Footer`); empty windows auto-close
- Manage scroll for large content using `BodyScrollPosition`/`PaneScrollPosition`
- `GenericMenu` parameters on `SetBody/SetPane` are optional; use if you need context menus
- Always call one of the `ShowAs…` methods to present the window; simply creating the instance won’t display it
- `ShowAsDropDown` uses screen coordinates of the provided rect

## Files in This Package
- `Editor/EditorWindowDrawer.cs` – Core window subclass, colors, frame, and lifecycle hooks
- `Editor/EditorWindowDrawerBuilder.cs` – Fluent API: show modes, sections, hooks, border, events
- `Editor/EditorWindowDrawerLayout*.cs` – Layout helpers for header/body/pane/footer/window/border/splitter
- `Editor/EditorWindowDrawerUtilities.cs` – Small utilities
- `Editor/Examples/` – Minimal samples (if present)
- `Editor/UnityEssentials.EditorWindowDrawer.Editor.asmdef` – Editor assembly definition

## Tags
unity, unity-editor, editorwindow, ui, layout, dockless, popup, dropdown, utility, pane, toolbar, helpbox, tools, workflow
