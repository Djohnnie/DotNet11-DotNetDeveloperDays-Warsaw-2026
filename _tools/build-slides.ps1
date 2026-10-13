[CmdletBinding()]
param(
    [int[]]$OnlySlides
)

$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$slides = Join-Path $root '_slides'
$resources = Join-Path $root '_resources'
[IO.Directory]::CreateDirectory($slides) | Out-Null

function DataUri([string]$path, [string]$mime) {
    "data:$mime;base64,$([Convert]::ToBase64String([IO.File]::ReadAllBytes($path)))"
}

$regular = DataUri (Join-Path $resources 'fonts\OpenSans-Regular.ttf') 'font/ttf'
$bold = DataUri (Join-Path $resources 'fonts\OpenSans-Bold.ttf') 'font/ttf'
$heading = DataUri (Join-Path $resources 'fonts\Raleway-Bold.ttf') 'font/ttf'
$logo = DataUri (Join-Path $resources 'developerdays-logo.png') 'image/png'
$speakerPhoto = DataUri (Join-Path $resources 'speaker-photo.jpg') 'image/jpeg'
$qrCode = DataUri (Join-Path $resources 'qrcode.svg') 'image/svg+xml'
$slideThree = [IO.File]::ReadAllText((Join-Path $slides 'slide-03.svg'))
$qrMatch = [regex]::Match($slideThree, "<a href='[^']+'><image [^>]*href='(data:image/svg\+xml;base64,[^']+)'")
if (-not $qrMatch.Success) {
    throw 'Could not find the repository QR code in slide-03.svg.'
}
$repositoryQr = $qrMatch.Groups[1].Value
$zureSvg = [IO.File]::ReadAllText((Join-Path $resources 'zure.svg')).Replace('currentColor', '#ffffff')
$zureLogo = "data:image/svg+xml;base64,$([Convert]::ToBase64String([Text.Encoding]::UTF8.GetBytes($zureSvg)))"
$repoUrl = 'https://github.com/Djohnnie/DotNet11-DotNetDeveloperDays-Warsaw-2026'
$defs = @"
<defs>
  <clipPath id="speaker-photo-clip"><circle cx="360" cy="560" r="215"/></clipPath>
  <style>
    @font-face{font-family:'Open Sans';font-weight:400;src:url('$regular') format('truetype')}
    @font-face{font-family:'Open Sans';font-weight:700;src:url('$bold') format('truetype')}
    @font-face{font-family:Raleway;font-weight:700;src:url('$heading') format('truetype')}
    text{font-family:'Open Sans',sans-serif;fill:#cccccc}
    .heading{font-family:Raleway,sans-serif;font-weight:700;fill:#ffffff;letter-spacing:-2px}
    .label{font-size:20px;letter-spacing:3px;font-weight:700;fill:#ff6a3f}
  </style>
  <linearGradient id="accent" x1="0" y1="1" x2="1" y2="0">
    <stop stop-color="#bd0926"/><stop offset="1" stop-color="#ff6a3f"/>
  </linearGradient>
</defs>
"@

function Text([int]$x, [int]$y, [string]$value, [int]$size = 32, [string]$class = '') {
    '<text x="{0}" y="{1}" font-size="{2}" class="{3}">{4}</text>' -f $x, $y, $size, $class, [Security.SecurityElement]::Escape($value)
}

function Lines([int]$x, [int]$y, [string[]]$values, [int]$size = 32, [int]$gap = 52, [string]$class = '') {
    for ($i = 0; $i -lt $values.Count; $i++) {
        Text $x ($y + $i * $gap) $values[$i] $size $class
    }
}

function SaveSlide([string]$name, [string]$title, [string]$description, [string]$body, [string]$number, [string]$section = 'WHAT''S NEW IN .NET 11') {
    if ($OnlySlides.Count -gt 0 -and [int]($name -replace '\D', '') -notin $OnlySlides) {
        return
    }
    $safeTitle = [Security.SecurityElement]::Escape($title)
    $safeDescription = [Security.SecurityElement]::Escape($description)
    if ($number -eq '01') {
        $slideHeader = "<image x='1170' y='405' width='600' height='273' href='$logo'/>"
        $slideFooter = ''
    } else {
        $slideHeader = @(
            (Text 100 100 $section 20 'label')
            "<image x='1580' y='55' width='240' height='109' href='$logo'/>"
        ) -join "`n"
        $slideFooter = @(
            '<path d="M100 985H1820" stroke="#444444"/>'
            "<a href='$repoUrl'>$(Text 100 1030 'github.com/Djohnnie/DotNet11-DotNetDeveloperDays-Warsaw-2026' 24)</a>"
            "<text x='1820' y='1032' text-anchor='end' font-size='24' fill='#ffffff'>$number</text>"
            "<image x='1526' y='880' width='294' height='75' href='$zureLogo'/>"
        ) -join "`n"
    }
    $svg = @"
<?xml version="1.0" encoding="utf-8"?>
<svg xmlns="http://www.w3.org/2000/svg" width="3840" height="2160" viewBox="0 0 1920 1080" role="img" aria-labelledby="title description">
<title id="title">$safeTitle</title>
<desc id="description">$safeDescription</desc>
$defs
<rect width="1920" height="1080" fill="#161616"/>
<rect width="1920" height="8" fill="url(#accent)"/>
$slideHeader
$body
$slideFooter
</svg>
"@
    [IO.File]::WriteAllText((Join-Path $slides $name), $svg, [Text.UTF8Encoding]::new($false))
}

$body = @(
    (Text 100 325 'What''s new in' 86 'heading')
    (Text 100 435 '.NET 11 & C# 15' 80 'heading')
    '<rect x="100" y="495" width="180" height="8" fill="url(#accent)"/>'
    (Lines 100 575 @('A tour of the latest .NET platform', 'and C# language updates') 38 56)
    (Text 100 805 'Johnny Hooyberghs' 34 'heading')
    (Text 100 860 '.NET DeveloperDays / Warsaw / 2026' 26)
) -join "`n"
SaveSlide 'slide-01.svg' 'What''s new in .NET 11 and C# 15?' 'Opening slide for a tour of .NET 11 and C# 15 at .NET DeveloperDays Warsaw 2026.' $body '01' 'WHAT''S NEW IN .NET 11'

$body = @(
    (Text 100 245 'Johnny Hooyberghs' 72 'heading')
    "<image x='145' y='345' width='430' height='430' preserveAspectRatio='xMidYMid slice' clip-path='url(#speaker-photo-clip)' href='$speakerPhoto'/>"
    '<circle cx="360" cy="560" r="215" fill="none" stroke="#bd0926" stroke-width="3"/>'
    '<circle cx="750" cy="380" r="7" fill="#ff6a3f"/>'
    (Lines 780 390 @('Focus on .NET application development,', 'Azure cloud, and generative AI') 34 44)
    '<circle cx="750" cy="500" r="7" fill="#ff6a3f"/>'
    (Lines 780 510 @('18 years of professional experience', 'as a software enthusiast') 34 44)
    '<circle cx="750" cy="620" r="7" fill="#ff6a3f"/>'
    (Lines 780 630 @('Microsoft MVP in Developer Technologies', 'and Azure since 2020') 34 44)
    '<circle cx="750" cy="780" r="7" fill="#ff6a3f"/>'
    (Text 780 790 'Public speaker and trainer' 34)
) -join "`n"
SaveSlide 'slide-02.svg' 'Johnny Hooyberghs' 'Speaker introduction with photo, professional background, and interests.' $body '02' 'ABOUT THE SPEAKER'

$body = @(
    (Text 100 245 'Explore the code and examples.' 68 'heading')
    (Text 100 315 'Scan to open the .NET 11 and C# 15 repository' 30)
    "<a href='$repoUrl'><image x='690' y='340' width='540' height='540' href='$qrCode'/></a>"
) -join "`n"
SaveSlide 'slide-03.svg' 'Explore the code and examples' 'QR code linking to the .NET 11 and C# 15 examples repository.' $body '03' 'FOLLOW ALONG'

function SaveFeatureSlide([int]$number, [string]$title, [string[]]$features) {
    $columnCount = if ($features.Count -gt 6) { 2 } else { 1 }
    $rowCount = [Math]::Ceiling($features.Count / [double]$columnCount)
    $rowGap = if ($rowCount -gt 8) { 48 } elseif ($rowCount -gt 6) { 58 } else { 70 }
    $bodyParts = [Collections.Generic.List[string]]::new()
    $bodyParts.Add((Text 100 245 $title 68 'heading'))
    $bodyParts.Add('<rect x="100" y="280" width="180" height="8" fill="url(#accent)"/>')

    for ($i = 0; $i -lt $features.Count; $i++) {
        $column = if ($columnCount -eq 2) { $i % 2 } else { 0 }
        $row = if ($columnCount -eq 2) { [Math]::Floor($i / 2) } else { $i }
        $x = if ($column -eq 0) { 100 } else { 990 }
        $y = 350 + $row * $rowGap
        $bodyParts.Add("<circle cx='$($x + 5)' cy='$($y - 9)' r='6' fill='#ff6a3f'/>")
        $bodyParts.Add((Text ($x + 26) $y $features[$i] 26))
    }

    $body = $bodyParts -join "`n"
    $slideNumber = $number.ToString('00')
    SaveSlide "slide-$slideNumber.svg" $title "Overview of .NET 11 and C# 15 features: $title." $body $slideNumber 'FEATURE OVERVIEW'
}

SaveFeatureSlide 4 'C# 15' @(
    'Collection expression arguments'
    'Union types'
    'Closed hierarchies'
    'Extension indexers'
    'Labeled break and continue'
    'Memory-safety updates'
)

SaveFeatureSlide 5 '.NET libraries and runtime' @(
    'Run processes and capture output'
    'Control process lifetime and inspect exit status'
    'Stream memory and text without copying'
    'Compress and decompress spans'
    'Protect ZIP entries with passwords'
    'Add Zstandard compression'
    'Validate ZIP checksums when reading'
    'Create hard links'
    'Serialize asynchronous streams as JSON Lines'
    'Expand JSON contracts and numeric support'
    'Resolve DNS record types'
    'Work with Unicode scalars and validate text'
    'Use ordinal casing and floating-point hex'
    'Construct bit arrays from spans'
    'Build frozen dictionaries from collection expressions'
    'Add generic numeric APIs'
    'Parse numbers from delimited input'
    'Join both sides of two LINQ sequences'
    'Validate data asynchronously'
    'Use newer cryptography and faster Half operations'
    'Runtime-managed async (preview)'
)

SaveFeatureSlide 6 'SDK and build' @(
    'Run the CLI and file-based apps more efficiently'
    'Pass environment variables and use improved watch workflows'
    'Improve test runs'
    'Publish reproducible containers'
    'Add tar support and expose MSBuild globs'
)

SaveFeatureSlide 7 'ASP.NET Core and Blazor' @(
    'Protect endpoints from cross-origin requests by default'
    'Use async validation in Minimal APIs'
    'Short-circuit selected endpoints'
    'Describe server-sent events in OpenAPI'
    'Navigate relatively and carry temporary data in Blazor'
    'Pause inactive Blazor Server circuits'
    'Cache server-rendered component subtrees'
    'Improve QuickGrid interaction'
    'Add environment and metadata-aware UI'
)

SaveFeatureSlide 8 'Entity Framework Core 11' @(
    'Use complex types with more mappings'
    'Translate more LINQ and generate leaner SQL'
    'Query JSON and create JSON indexes'
    'Search SQL Server with vectors and full text'
    'Simplify migration workflows'
    'Inspect tracked entries without extra change detection'
    'Warn when async queries run synchronously'
)

SaveFeatureSlide 9 'Windows Forms' @(
    'Opt into modern visual styles and avoid startup flashes'
    'Render check boxes as toggle switches'
    'Suspend painting during bulk updates'
)

SaveFeatureSlide 10 'WPF' @(
    'Reliability and compatibility fixes'
)

 $body = @(
    (Text 100 245 'Any questions?' 72 'heading')
    (Text 100 315 'Continue exploring .NET 11 and C# 15' 30)
     "<a href='$repoUrl'><image x='690' y='340' width='540' height='540' href='$repositoryQr'/></a>"
) -join "`n"
SaveSlide 'slide-11.svg' 'Thank you' 'Thank you for joining this overview of .NET 11 and C# 15. Scan the QR code to open the repository.' $body '11' 'THANK YOU'

Write-Host "Built selected slides in $slides"
