# Presentation tools

The numbered SVG sources in `_slides` include the opening slides and feature
overviews, and can be exported as a full-bleed image-based PowerPoint. The
tools use PowerShell and Microsoft Edge, with no Python, Node.js, or package
installation required.

## Build the SVG sources

From the repository root:

```powershell
.\_tools\build-slides.ps1
```

To regenerate only selected slides without touching the others:

```powershell
.\_tools\build-slides.ps1 -OnlySlides 1
```

Edit the content and layouts in `build-slides.ps1` and rerun it. It generates
`slide-01.svg` through `slide-13.svg`, embedding the fonts, conference logo,
Zure icon, speaker photo, and repository QR code from `_resources`. Use
`-OnlySlides` with slide numbers to regenerate selected slides without
touching the others.

The slides have a 3840 x 2160 canvas and a 1920 x 1080 coordinate system. Keep
important content within the 100-unit horizontal margins and above the footer
at y=985. Use Raleway 700 for headings and Open Sans for body text. XML-escape
text when editing the generator.

Slides 2 through 13 include the repository link, Zure icon, and slide number
in the footer; the title slide has no footer. Slide 4 shows the annual release
cadence and LTS/STS support durations from the
[Microsoft support policy](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core).
Slide 5 shows all .NET 11 previews and release candidates, using release dates
from [Microsoft's release metadata](https://github.com/dotnet/core/blob/main/release-notes/11.0/releases.json),
verified on 20 October 2026. These dates are embedded in the generator so
building slides does not require network access.

Slides 6 through 12 list the
numbered example folders from `DotNet11AndCSharp15.slnx`, grouped and ordered
as in the solution, without feature details. Rebuild these slides after adding
or renaming examples in the solution.

## Export PowerPoint

```powershell
.\_tools\svg_to_pptx.ps1
```

The exporter collects numbered `slide-NN.svg` files, sorts them by name,
renders each at 3840 x 2160 using headless Edge, and writes
`DotNet11-DotNetDeveloperDays-Warsaw-2026.pptx` at the repository root.
Rendering failures stop generation without replacing an existing deck.

**The exported PowerPoint slides are images, not editable text or shapes.**
Edit the SVG source and export again.
