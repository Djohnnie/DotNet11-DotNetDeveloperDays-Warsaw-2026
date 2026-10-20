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

$supportPolicyUrl = 'https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core'
$body = @(
    (Text 100 245 '.NET ships every November' 68 'heading')
    (Text 100 310 'Even versions: Long Term Support (3 years)  /  Odd versions: Standard Term Support (2 years)' 28)
    for ($year = 2023; $year -le 2030; $year++) {
        $x = 420 + ($year - 2023) * 190
        "<path d='M$x 395V820' stroke='#363636' stroke-dasharray='4 8'/>"
        (Text ($x - 20) 350 'NOV' 18)
        (Text ($x - 30) 382 "$year" 26)
    }
    for ($version = 8; $version -le 12; $version++) {
        $isLts = $version % 2 -eq 0
        $duration = if ($isLts) { 3 } else { 2 }
        $track = if ($isLts) { 'LTS' } else { 'STS' }
        $color = if ($isLts) { '#ff6a3f' } else { '#66c7db' }
        $x = 420 + ($version - 8) * 190
        $y = 410 + ($version - 8) * 84
        $width = $duration * 190
        $label = if ($version -ge 11) { ".NET $version *" } else { ".NET $version" }
        $outline = if ($version -ge 11) { "stroke-dasharray='8 6'" } else { '' }
        (Text 100 ($y + 37) $label 30 'heading')
        "<rect x='$x' y='$y' width='$width' height='56' rx='12' fill='$color' fill-opacity='0.18' stroke='$color' stroke-width='2' $outline/>"
        (Text ($x + 22) ($y + 36) "$track / $duration years" 24)
        "<circle cx='$x' cy='$($y + 28)' r='6' fill='$color'/>"
        "<path d='M$($x + $width) $($y + 16)v24' stroke='$color' stroke-width='3'/>"
    }
    (Text 100 865 '* .NET 11 and .NET 12: planned releases. Bars show support duration; exact end dates follow Patch Tuesday.' 22)
    "<a href='$supportPolicyUrl'>$(Text 100 925 'Source: Microsoft .NET support policy' 20)</a>"
) -join "`n"
SaveSlide 'slide-04.svg' '.NET release cadence' 'Annual November releases, with three years of LTS support for even versions and two years of STS support for odd versions. Timeline from November 2023 to November 2030.' $body '04' 'RELEASE CADENCE'

# Release-date snapshot from Microsoft's release metadata, verified on 20 October 2026.
$releaseNotesUrl = 'https://github.com/dotnet/core/blob/main/release-notes/11.0/releases.json'
$previewReleases = @(
    @{ Name = 'Preview 1'; Date = '2026-02-10' }
    @{ Name = 'Preview 2'; Date = '2026-03-10' }
    @{ Name = 'Preview 3'; Date = '2026-04-14' }
    @{ Name = 'Preview 4'; Date = '2026-05-12' }
    @{ Name = 'Preview 5'; Date = '2026-06-09' }
    @{ Name = 'Preview 6'; Date = '2026-07-14' }
    @{ Name = 'Preview 7'; Date = '2026-08-11' }
    @{ Name = 'RC 1'; Date = '2026-09-08' }
    @{ Name = 'RC 2'; Date = '2026-10-13' }
)
$body = @(
    (Text 100 245 'The road to .NET 11' 68 'heading')
    (Text 100 315 '2026 / Seven previews, followed by two release candidates' 30)
    '<path d="M140 590H1790l-16 -10m16 10l-16 10" fill="none" stroke="#777777" stroke-width="4"/>'
    for ($i = 0; $i -lt $previewReleases.Count; $i++) {
        $release = $previewReleases[$i]
        $x = 180 + $i * 195
        $above = $i % 2 -eq 0
        $labelY = if ($above) { 455 } else { 715 }
        $connectorY = if ($above) { 520 } else { 660 }
        $isRc = $release.Name.StartsWith('RC')
        $color = if ($isRc) { '#66c7db' } else { '#ff6a3f' }
        $date = [datetime]::ParseExact($release.Date, 'yyyy-MM-dd', [Globalization.CultureInfo]::InvariantCulture)
        $dateLabel = $date.ToString('dd MMM yyyy', [Globalization.CultureInfo]::InvariantCulture)
        "<path d='M$x 590V$connectorY' stroke='$color' stroke-width='2'/>"
        "<circle cx='$x' cy='590' r='11' fill='$color' stroke='#161616' stroke-width='4'/>"
        "<text x='$x' y='$labelY' text-anchor='middle' font-size='30' class='heading'>$($release.Name)</text>"
        "<text x='$x' y='$($labelY + 44)' text-anchor='middle' font-size='26'>$dateLabel</text>"
    }
    '<circle cx="110" cy="840" r="6" fill="#ff6a3f"/>'
    (Text 130 848 'Previews: public testing, not supported' 24)
    '<circle cx="820" cy="840" r="6" fill="#66c7db"/>'
    (Text 840 848 'Release candidates: go-live support' 24)
    "<a href='$releaseNotesUrl'>$(Text 100 925 'Source: Microsoft .NET 11 release metadata / Dates verified 20 Oct 2026' 20)</a>"
) -join "`n"
SaveSlide 'slide-05.svg' '.NET 11 previews and release candidates' 'Left-to-right timeline of all seven .NET 11 previews and both release candidates, with their release dates from February to October 2026.' $body '05' '.NET 11 RELEASE TIMELINE'

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

[xml]$solution = [IO.File]::ReadAllText((Join-Path $root 'DotNet11AndCSharp15.slnx'))
$sections = @($solution.Solution.Folder | Where-Object { $_.Name -match '^/\d+\. [^/]+/$' })
$featureSlideNumber = 6
foreach ($section in $sections) {
    $title = $section.Name.Trim('/') -replace '^\d+\.\s+', ''
    $features = @(
        $solution.Solution.Folder |
            Where-Object { $_.Name.StartsWith($section.Name) -and $_.Name -ne $section.Name } |
            ForEach-Object { ($_.Name.TrimEnd('/') -split '/')[-1] -replace '^\d+\.\s+', '' }
    )
    SaveFeatureSlide $featureSlideNumber $title $features
    $featureSlideNumber++
}

 $body = @(
    (Text 100 245 'Any questions?' 72 'heading')
    (Text 100 315 'Continue exploring .NET 11 and C# 15' 30)
     "<a href='$repoUrl'><image x='690' y='340' width='540' height='540' href='$repositoryQr'/></a>"
) -join "`n"
SaveSlide 'slide-13.svg' 'Thank you' 'Thank you for joining this overview of .NET 11 and C# 15. Scan the QR code to open the repository.' $body '13' 'THANK YOU'

Write-Host "Built selected slides in $slides"
