---
name: schoolnet-style
description: "Create or refine Schoolnet-branded web pages, MVC/Razor screens, and HTML guides using the supplied Schoolnet visual reference: light surfaces, blue accents, green calls to action, and generous spacing. Use for Schoolnet styling requests; preserve an explicitly requested OLED or other document theme."
---

# Schoolnet style

Use the supplied Schoolnet website screenshot as the visual direction. Adapt its hierarchy and branding to the requested page rather than copying its marketing content or carousel clipping.

## Resources

- [Visual reference](assets/schoolnet-reference.png): inspect when matching composition, header, hero, section spacing, or media placement.
- [Design guidance](references/design.md): read for palette, typography, component choices, and the distinction between marketing and LMS layouts.
- [Starter CSS](assets/schoolnet-style.css): opt-in `.sn-*` classes for standalone pages and prototypes. Adapt into existing project styles instead of introducing a second global reset.
- [Official local logo](assets/schoolnet-logo.png): use when the target project has no existing Schoolnet logo. In SampleLMS prefer `Schoolnet.WEBUI/wwwroot/images/schoolnet-logo.png`.

## Working approach

1. Identify the requested deliverable and inspect its existing layout/styles. Reuse the target application's framework and components. A style request does not authorize publishing, deployment, or changes to unrelated screens.
2. Match the reference's recognizable features: Schoolnet logo on a light header, restrained dark headings, blue structural accents, green primary actions, pale gray hero surfaces, white content sections, and substantial whitespace.
3. Choose the appropriate composition:
   - Marketing page: compact horizontal navigation, a blue divider below the header, a two-column text/media hero, impact metrics, and alternating product sections.
   - LMS screen: preserve the existing sidebar, permissions, metric cards, filter bar, and readable tables. Apply the visual vocabulary to the requested screen without replacing the application shell.
   - HTML guide: use a light document layout with blue section accents, green controls, and readable diagrams when appropriate. If the user explicitly requests OLED, honor its black/monochrome theme and use only compatible branding.
4. Use the existing official logo with its aspect ratio intact. Use real approved product imagery when provided. Do not extract clipped product images from the screenshot or invent product claims, statistics, testimonials, or partner affiliations.
5. Preserve field names, form actions, validation, navigation targets, and server permissions when restyling working forms. Make controls labelled and keyboard accessible; show errors in words rather than relying on color.
6. Check the result at desktop and narrow mobile widths. Ensure text wraps, the hero stacks, actions remain reachable, media fits, and tables scroll within their container. Verify behavior relevant to changed controls and inspect rendered screenshots.

## Style decisions

- Colors in the bundled guidance are reference-based approximations, not an official brand standard. Existing approved project tokens take precedence for maintenance work.
- Use green to emphasize an action or short heading phrase, blue for structural accents and metrics, and dark ink for normal text. Keep large reading surfaces light.
- The screenshot's visual density comes partly from its capture scale. Use readable text sizes instead of reproducing its small rendered text literally.
- Do not introduce gradients, ornamental dashboard widgets, stock statistics, or mandatory carousels just because they can fit the page. Add only what serves the requested content.

## Example requests

- “Use $schoolnet-style to design a Create Batch help page.”
- “Use $schoolnet-style to refine the trainee edit form while preserving its behavior.”
- “Use $schoolnet-style to create a Schoolnet product landing page with my supplied content.”
