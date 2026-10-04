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

Write-Host "Built up to 3 opening slides in $slides"
