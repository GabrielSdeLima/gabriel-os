# Gabriel OS — Design System Implementation Prompt

## Context

Gabriel OS is a personal life operating system built as a WPF/.NET 10 desktop app with Clean Architecture and SQLite. The MVP-0 is scaffolded and building. This prompt defines the full visual identity and instructs you to implement it across the application.

The design philosophy is: **calm, intentional, human.** This is not a corporate dashboard. It's a personal system that should feel like a well-designed journal or a quiet workspace — warm without being saccharine, structured without being rigid. Think Obsidian meets Arc Browser meets a Muji notebook.

---

## 1. Color System

Implement a full dual-theme system (Light default, Dark available via toggle). Use WPF ResourceDictionaries for theme switching.

### Light Theme (Default) — "Morning Fog"

```
Background.Primary:       #FAFAF8    (warm off-white, NOT pure white — pure white is harsh)
Background.Secondary:     #F0EFEB    (subtle warm gray for cards/panels)
Background.Tertiary:      #E8E6E0    (borders, dividers, subtle separators)
Background.Elevated:      #FFFFFF    (floating elements, modals — slight contrast against primary)

Text.Primary:             #1A1A1A    (near-black, NOT pure black — softer on eyes)
Text.Secondary:           #6B6860    (warm gray for secondary info, timestamps, labels)
Text.Tertiary:            #9C9890    (placeholders, disabled states)
Text.Inverse:             #FAFAF8    (text on accent-colored backgrounds)

Accent.Primary:           #7C6F64    (warm taupe — main interactive elements, links, active states)
Accent.PrimaryHover:      #6A5D52    (darker on hover)
Accent.PrimaryPressed:    #584E44    (darker still on press)
Accent.Subtle:            #E8E2DC    (accent tinted backgrounds for tags, badges, highlights)

Accent.Secondary:         #B8A9A0    (rose-taupe — secondary actions, decorative elements)
Accent.Warm:              #D4A98C    (warm terracotta — sparingly, for emphasis or notifications)
Accent.Cool:              #8BA4A8    (muted sage-teal — for success states, completed items)

Semantic.Success:         #7A9E7E    (muted green)
Semantic.Warning:         #C4A265    (muted gold)
Semantic.Error:           #B5706A    (muted coral-red)
Semantic.Info:            #7E95A8    (muted steel blue)

Border.Default:           #DDD9D3    (subtle, warm)
Border.Focused:           #7C6F64    (matches accent)
Border.Subtle:            #ECEAE6    (barely visible, structural only)

Shadow.Small:             0 1px 3px rgba(26,26,26,0.06)
Shadow.Medium:            0 4px 12px rgba(26,26,26,0.08)
Shadow.Large:             0 8px 24px rgba(26,26,26,0.10)
```

### Dark Theme — "Deep Water"

```
Background.Primary:       #1C1B1A    (warm near-black, NOT pure black — avoids OLED harshness)
Background.Secondary:     #242320    (slightly elevated cards/panels)
Background.Tertiary:      #2E2D29    (borders, dividers)
Background.Elevated:      #302F2B    (floating elements, modals)

Text.Primary:             #E8E6E0    (warm off-white)
Text.Secondary:           #9C9890    (warm gray)
Text.Tertiary:            #6B6860    (placeholders, disabled)
Text.Inverse:             #1C1B1A    (text on light accent backgrounds)

Accent.Primary:           #C4B5A8    (light warm taupe)
Accent.PrimaryHover:      #D4C5B8    (lighter on hover)
Accent.PrimaryPressed:    #B0A194    (slightly muted on press)
Accent.Subtle:            #33302B    (dark accent tint for tags/badges)

Accent.Secondary:         #A0908A    (muted rose)
Accent.Warm:              #D4A98C    (same terracotta — works in both themes)
Accent.Cool:              #8BA4A8    (same sage-teal)

Semantic.Success:         #8AAE8E    (slightly brighter for dark bg)
Semantic.Warning:         #D4B275    (slightly brighter)
Semantic.Error:           #C58078    (slightly brighter)
Semantic.Info:            #8EA5B8    (slightly brighter)

Border.Default:           #3A3835    (subtle warm border)
Border.Focused:           #C4B5A8    (matches dark accent)
Border.Subtle:            #2A2926    (barely visible)

Shadow.Small:             0 1px 3px rgba(0,0,0,0.20)
Shadow.Medium:            0 4px 12px rgba(0,0,0,0.25)
Shadow.Large:             0 8px 24px rgba(0,0,0,0.30)
```

---

## 2. Typography

Use these font stacks. Download and embed as application resources — do NOT rely on system fonts.

```
Font.Display:       "Fraunces"         — for titles, headers, hero text
                    Optical size: 9-144, Weight: 100-900
                    Use weight 600 for section headers, 700 for page titles
                    Fallback: "Georgia", serif

Font.Body:          "Inter"            — for body text, UI labels, inputs, buttons
                    Weight: 400 (regular), 500 (medium), 600 (semibold)
                    Fallback: "Segoe UI Variable", "Segoe UI", sans-serif

Font.Mono:          "JetBrains Mono"   — for code, data, timestamps, IDs
                    Weight: 400, 500
                    Fallback: "Cascadia Code", "Consolas", monospace
```

### Type Scale (base: 14px)

```
Display.Large:      Fraunces 700,    32px,  line-height 1.2,   letter-spacing -0.02em
Display.Medium:     Fraunces 600,    24px,  line-height 1.25,  letter-spacing -0.01em
Display.Small:      Fraunces 600,    20px,  line-height 1.3,   letter-spacing -0.01em

Heading.Large:      Inter 600,       18px,  line-height 1.35
Heading.Medium:     Inter 600,       16px,  line-height 1.4
Heading.Small:      Inter 500,       14px,  line-height 1.4,   letter-spacing 0.01em, UPPERCASE

Body.Large:         Inter 400,       16px,  line-height 1.6
Body.Medium:        Inter 400,       14px,  line-height 1.6
Body.Small:         Inter 400,       12px,  line-height 1.5

Label.Medium:       Inter 500,       13px,  line-height 1.4
Label.Small:        Inter 500,       11px,  line-height 1.35,  letter-spacing 0.02em

Mono.Medium:        JetBrains Mono 400,  13px,  line-height 1.5
Mono.Small:         JetBrains Mono 400,  11px,  line-height 1.45
```

---

## 3. Spacing & Layout

Use a 4px base unit. All spacing should be multiples of 4.

```
Space.XXS:     2px
Space.XS:      4px
Space.SM:      8px
Space.MD:      12px
Space.LG:      16px
Space.XL:      24px
Space.XXL:     32px
Space.XXXL:    48px
Space.Huge:    64px
```

### Layout Principles

- **Generous whitespace everywhere.** When in doubt, add more space, not less.
- Content max-width: 720px for text-heavy areas (readability).
- Card/panel padding: minimum 20px (Space.XL - 4px), prefer 24px.
- Margin between sections: 32px minimum, 48px preferred.
- Sidebar width: 240px collapsed icon-only: 56px.
- Never let elements feel "cramped." If two elements feel too close, they are.

### Border Radius

```
Radius.SM:      4px     (small elements: tags, badges, chips)
Radius.MD:      8px     (buttons, inputs, small cards)
Radius.LG:      12px    (cards, panels, modals)
Radius.XL:      16px    (large containers, hero sections)
Radius.Full:    9999px  (pills, avatars, circular elements)
```

---

## 4. Component Styles

### Buttons

```xaml
<!-- Primary Button -->
Background: Accent.Primary
Foreground: Text.Inverse
FontFamily: Inter
FontWeight: 500
FontSize: 14px
Padding: 10px 20px
BorderRadius: Radius.MD (8px)
Border: none
Hover: Accent.PrimaryHover with 150ms ease-out transition
Pressed: Accent.PrimaryPressed
Disabled: opacity 0.4, no pointer events

<!-- Secondary Button -->
Background: transparent
Foreground: Accent.Primary
Border: 1px solid Border.Default
Hover: Background.Secondary
Pressed: Background.Tertiary

<!-- Ghost Button -->
Background: transparent
Foreground: Text.Secondary
Border: none
Hover: Background.Secondary, Foreground → Text.Primary
```

### Cards / Panels

```
Background: Background.Secondary (or Background.Elevated for floating)
Border: 1px solid Border.Subtle
BorderRadius: Radius.LG (12px)
Padding: 24px
Shadow: Shadow.Small (optional, only for elevated)
Hover (if interactive): Border → Border.Default, Shadow → Shadow.Medium
Transition: all 200ms ease-out
```

### Input Fields

```
Background: Background.Primary
Border: 1px solid Border.Default
BorderRadius: Radius.MD (8px)
Padding: 10px 14px
FontFamily: Inter
FontSize: 14px
Foreground: Text.Primary
Placeholder: Text.Tertiary
Focus: Border → Border.Focused, subtle box-shadow with Accent.Primary at 15% opacity
Transition: border-color 150ms ease-out, box-shadow 150ms ease-out
```

### Sidebar / Navigation

```
Background: Background.Secondary
Width: 240px (expanded), 56px (collapsed)
Transition: width 250ms cubic-bezier(0.4, 0, 0.2, 1)
Active item: Background → Accent.Subtle, Foreground → Accent.Primary, left border 3px Accent.Primary
Hover: Background → Background.Tertiary
Icons: 20px, stroke-width 1.5px, color Text.Secondary (active: Accent.Primary)
```

### Theme Toggle Button

```
Place in sidebar footer or top-right settings area.
Style: icon-only button (sun ☀ for light, moon 🌙 for dark)
Use a smooth crossfade animation (200ms) when switching themes.
Toggle should persist user preference to SQLite (or app settings).
On click: swap the merged ResourceDictionary from LightTheme.xaml to DarkTheme.xaml (or vice versa).
```

---

## 5. Animation & Motion

**Core principle: everything breathes, nothing startles.** For an HSP user, abrupt transitions are jarring. All motion should feel organic.

```
Easing.Standard:        cubic-bezier(0.4, 0, 0.2, 1)     — general purpose
Easing.Decelerate:      cubic-bezier(0.0, 0, 0.2, 1)     — elements entering
Easing.Accelerate:      cubic-bezier(0.4, 0, 1, 1)       — elements leaving

Duration.Instant:       100ms    — micro-interactions (hover color changes)
Duration.Fast:          150ms    — button state changes, focus rings
Duration.Normal:        250ms    — panel transitions, theme switch
Duration.Slow:          400ms    — page transitions, modal open/close
Duration.Gentle:        600ms    — large layout shifts, onboarding animations
```

### Specific animations

- **Page/view transitions:** Fade in (opacity 0→1) + slight upward translate (8px→0) over Duration.Slow with Easing.Decelerate.
- **Cards appearing in lists:** Stagger each by 50ms, same fade+translate pattern.
- **Sidebar expand/collapse:** Width transition over Duration.Normal, content crossfades.
- **Hover states:** Color transitions over Duration.Fast.
- **Theme switch:** Crossfade entire window content over Duration.Normal. Do NOT do an abrupt swap.
- **Loading states:** Subtle pulsing opacity animation (0.4→0.7→0.4) over 1.5s, infinite, ease-in-out.
- **No bouncing, no overshooting, no elastic effects.** Keep everything grounded and calm.

---

## 6. Iconography

Use **Lucide Icons** (or Phosphor Icons as alternative) — they have a consistent, slightly rounded, humanist style that matches the design philosophy.

```
Icon size: 20px (standard), 16px (small/inline), 24px (emphasis)
Stroke width: 1.5px
Color: inherits from text color of parent (Text.Secondary default, Accent.Primary when active)
```

If bundling icons as XAML Path resources, ensure consistent viewbox (0 0 24 24) and stroke properties.

---

## 7. Implementation Architecture

### File Structure for Themes

```
/Presentation
  /Themes
    /Fonts
      Fraunces-VariableFont.ttf
      Inter-VariableFont.ttf
      JetBrainsMono-Regular.ttf
      JetBrainsMono-Medium.ttf
    SharedResources.xaml         ← spacing, typography, radius, animations (theme-independent)
    LightTheme.xaml              ← light color tokens as SolidColorBrush resources
    DarkTheme.xaml               ← dark color tokens as SolidColorBrush resources
    ButtonStyles.xaml            ← button templates referencing DynamicResource brushes
    CardStyles.xaml              ← card/panel templates
    InputStyles.xaml             ← textbox, combobox, etc.
    NavigationStyles.xaml        ← sidebar, nav items
    ScrollBarStyles.xaml         ← custom scrollbar (thin, rounded, semi-transparent)
    ToggleStyles.xaml            ← theme toggle and other toggles
  /Converters
    BoolToVisibilityConverter.cs
    ThemeIconConverter.cs        ← returns sun/moon icon based on current theme
```

### Theme Switching Mechanism

```csharp
// In App.xaml.cs or a ThemeService
public class ThemeService : IThemeService
{
    private const string ThemeKey = "app_theme";
    private readonly ISettingsRepository _settings;
    
    public ThemeMode CurrentTheme { get; private set; } = ThemeMode.Light;
    
    public void SetTheme(ThemeMode mode)
    {
        var app = Application.Current;
        var themeDictionary = app.Resources.MergedDictionaries
            .FirstOrDefault(d => d.Source?.ToString().Contains("Theme") == true);
        
        if (themeDictionary != null)
            app.Resources.MergedDictionaries.Remove(themeDictionary);
        
        var newTheme = mode switch
        {
            ThemeMode.Light => new ResourceDictionary { Source = new Uri("Themes/LightTheme.xaml", UriKind.Relative) },
            ThemeMode.Dark => new ResourceDictionary { Source = new Uri("Themes/DarkTheme.xaml", UriKind.Relative) },
            _ => throw new ArgumentOutOfRangeException()
        };
        
        app.Resources.MergedDictionaries.Add(newTheme);
        CurrentTheme = mode;
        _settings.Set(ThemeKey, mode.ToString());
    }
    
    public void Toggle() => SetTheme(CurrentTheme == ThemeMode.Light ? ThemeMode.Dark : ThemeMode.Light);
    
    public void LoadSavedTheme()
    {
        var saved = _settings.Get(ThemeKey);
        if (Enum.TryParse<ThemeMode>(saved, out var mode))
            SetTheme(mode);
    }
}

public enum ThemeMode { Light, Dark }
```

### Critical XAML Pattern

**All color references in styles/templates MUST use `{DynamicResource BrushName}` — NOT `{StaticResource}`.**
StaticResource won't update when the theme dictionary swaps at runtime. This is the #1 mistake in WPF theming.

```xaml
<!-- CORRECT -->
<Setter Property="Background" Value="{DynamicResource BackgroundPrimaryBrush}" />

<!-- WRONG — will not update on theme switch -->
<Setter Property="Background" Value="{StaticResource BackgroundPrimaryBrush}" />
```

### Example Resource Definition (LightTheme.xaml)

```xaml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    
    <!-- Backgrounds -->
    <SolidColorBrush x:Key="BackgroundPrimaryBrush" Color="#FAFAF8" />
    <SolidColorBrush x:Key="BackgroundSecondaryBrush" Color="#F0EFEB" />
    <SolidColorBrush x:Key="BackgroundTertiaryBrush" Color="#E8E6E0" />
    <SolidColorBrush x:Key="BackgroundElevatedBrush" Color="#FFFFFF" />
    
    <!-- Text -->
    <SolidColorBrush x:Key="TextPrimaryBrush" Color="#1A1A1A" />
    <SolidColorBrush x:Key="TextSecondaryBrush" Color="#6B6860" />
    <SolidColorBrush x:Key="TextTertiaryBrush" Color="#9C9890" />
    <SolidColorBrush x:Key="TextInverseBrush" Color="#FAFAF8" />
    
    <!-- Accents -->
    <SolidColorBrush x:Key="AccentPrimaryBrush" Color="#7C6F64" />
    <SolidColorBrush x:Key="AccentPrimaryHoverBrush" Color="#6A5D52" />
    <SolidColorBrush x:Key="AccentPrimaryPressedBrush" Color="#584E44" />
    <SolidColorBrush x:Key="AccentSubtleBrush" Color="#E8E2DC" />
    <SolidColorBrush x:Key="AccentSecondaryBrush" Color="#B8A9A0" />
    <SolidColorBrush x:Key="AccentWarmBrush" Color="#D4A98C" />
    <SolidColorBrush x:Key="AccentCoolBrush" Color="#8BA4A8" />
    
    <!-- Semantic -->
    <SolidColorBrush x:Key="SemanticSuccessBrush" Color="#7A9E7E" />
    <SolidColorBrush x:Key="SemanticWarningBrush" Color="#C4A265" />
    <SolidColorBrush x:Key="SemanticErrorBrush" Color="#B5706A" />
    <SolidColorBrush x:Key="SemanticInfoBrush" Color="#7E95A8" />
    
    <!-- Borders -->
    <SolidColorBrush x:Key="BorderDefaultBrush" Color="#DDD9D3" />
    <SolidColorBrush x:Key="BorderFocusedBrush" Color="#7C6F64" />
    <SolidColorBrush x:Key="BorderSubtleBrush" Color="#ECEAE6" />
    
</ResourceDictionary>
```

---

## 8. Scrollbar Styling

Custom scrollbar — thin, unobtrusive, warm-toned:

```
Width: 6px (expands to 8px on hover)
Track: transparent
Thumb: Text.Tertiary at 40% opacity
Thumb hover: Text.Secondary at 60% opacity
Thumb border-radius: Radius.Full (pill shape)
Appear on scroll, fade out after 1.5s of inactivity (opacity animation)
```

---

## 9. Window Chrome

Use custom window chrome (WindowChrome class in WPF) for a clean, borderless look:

```
CaptionHeight: 36px
CornerRadius: 8px (on Windows 11, respect system rounding)
GlassFrameThickness: 0 (no Aero glass)
Background: Background.Primary
Title bar: integrated into app layout — app title left-aligned (Fraunces 600, 14px),
           window controls (minimize, maximize, close) right-aligned, custom-styled to match theme.
Close button hover: Semantic.Error background with Text.Inverse foreground.
```

---

## 10. Checklist for Implementation

1. [ ] Create `/Presentation/Themes/` folder structure
2. [ ] Embed fonts (Fraunces, Inter, JetBrains Mono) as application resources
3. [ ] Create `SharedResources.xaml` with spacing, typography, radius constants
4. [ ] Create `LightTheme.xaml` with all color brush resources
5. [ ] Create `DarkTheme.xaml` with all color brush resources
6. [ ] Implement `ThemeService` with toggle and persistence
7. [ ] Create `ButtonStyles.xaml` with Primary, Secondary, Ghost button styles
8. [ ] Create `CardStyles.xaml` with standard card template
9. [ ] Create `InputStyles.xaml` with styled TextBox, ComboBox
10. [ ] Create `NavigationStyles.xaml` with sidebar styles
11. [ ] Create `ScrollBarStyles.xaml` with custom thin scrollbar
12. [ ] Style the window chrome (borderless, custom title bar)
13. [ ] Add theme toggle button to sidebar footer
14. [ ] Wire up `App.xaml` to merge SharedResources + default LightTheme on startup
15. [ ] Test theme switching at runtime — verify all colors update (no StaticResource leaks)
16. [ ] Verify all animations are smooth and not jarring
17. [ ] Test at different DPI scales (100%, 125%, 150%) — ensure nothing breaks

---

## Design Philosophy Summary

> **This application should feel like you're sitting in a quiet room with natural light, a warm cup of tea, and a notebook that already knows how you think.** Every pixel should serve clarity or warmth — never decoration for its own sake. The interface should disappear when you're focused and gently guide you when you're lost. No harsh contrasts, no cold blues, no aggressive animations. Calm. Intentional. Yours.
