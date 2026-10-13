# Schoolnet visual reference

## What the supplied screenshot establishes

The screenshot shows a Schoolnet education website with a light horizontal header, official wordmark on the left, compact links on the right, and a green Partner With Us action. A full-width blue band separates the header from a pale hero. The hero places text and impact metrics on the left and education/product imagery on the right. The next white section has a centered heading and a text/media product layout.

The screenshot is a reference for style and proportions. Its existing marketing numbers and product claims are not default content for new pages. Product imagery appears clipped within the captured carousel; do not reproduce clipping as a general layout rule.

## Palette

These are practical approximations based on the screenshot. They are not official sampled brand values. The SampleLMS application already has approved local conventions; reuse those where consistency matters.

| Role | Reference-inspired value | SampleLMS existing value |
| --- | --- | --- |
| Structural blue | `#007CDC` | `#007CDC` |
| Action green | `#00A650` | `#0AA34F` |
| Headings / body ink | `#161B22` | `#263339` |
| Supporting dark blue | `#024B6C` | `#024B6C` |
| Pale surface | `#F7F8FA` | `#F3F6F8` |
| Main surface | `#FFFFFF` | `#FFFFFF` |
| Border | `#E1E8EC` | `#E1E8EC` |
| Secondary text | `#61727D` | `#697E89` |

White text on the reference's bright green can be too weak for small button labels. The starter CSS uses deeper `#00833F` for primary buttons and `#0066B8` for text links to support readable contrast. Use approved project colors where required, checking contrast and adjusting treatment rather than claiming the screenshot is an accessibility specification.

## Typography and rhythm

- Use the project's existing sans-serif font. Without one, use `"Segoe UI", Arial, sans-serif`; do not invent an exact font match from the image.
- Body copy: 16px with roughly 1.6 line height for marketing/docs. Existing dense LMS tables can retain established 14px text when readable.
- Main heading: 32–40px desktop, 28–32px mobile, weight 700, tight line height. Section heading: 24–28px. Product subheading: 20–24px.
- Short green heading emphasis is a distinctive feature. Keep long explanatory text dark, not green.
- Use a spacing scale of 8, 12, 16, 24, 32, 48, and 64px. Use generous section padding and clear separation between headline, explanation, and action.
- Center content in a container around 1120–1200px wide; allow full-width section backgrounds and the blue header band.

## Components

**Header:** logo at natural aspect ratio, around 130–160px wide on a standalone marketing header. Use the existing larger sidebar logo in an LMS. Do not recreate the wordmark as typed text.

**Primary action:** green fill, readable white label, modest radius around 4–6px, 44px minimum height, semibold text, clear focus ring. Use meaningful labels: Save draft batch, Assign selected trainees, or Learn more as appropriate.

**Secondary action:** white/light surface with an outline or restrained blue link. Distinguish Save, Cancel, and destructive actions without making all buttons green.

**Hero:** text takes roughly 55–60% of the width, media takes the remainder. Use a pale background, left-aligned headline/body/action, and imagery that fits without distortion. Stack on mobile with text first. Avoid fixed heights that crop content.

**Impact metrics:** blue numerals with secondary labels; thin dotted blue dividers can echo the screenshot. Use only actual supplied numbers. This component is optional and belongs mainly to marketing pages.

**Content sections:** white/light backgrounds, centered section title when appropriate, text/media columns with generous whitespace. Simple bordered cards are useful for LMS/docs, but the reference marketing page does not require everything to become a card.

**Forms and tables:** visible labels above inputs, a light bordered panel, clear required-field indicators, and explicit validation messages. Keep filter rows and tables readable; contain horizontal scrolling within table wrappers on narrow screens.

## Responsive and interaction checks

At about 820px, stack hero and product columns. Let header links wrap or use an accessible navigation disclosure. At about 520px, make forms single-column and stack metrics when necessary. Check a 390px mobile viewport and a desktop viewport around 1440px.

Provide visible keyboard focus. Ensure text/link/button contrast, do not rely solely on blue/green status colors, and respect reduced-motion preferences. A screenshot alone does not establish carousel behavior; add controls and accessible behavior only if a carousel is actually requested.
