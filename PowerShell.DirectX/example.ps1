Add-PSSnapin "DirectXSnapIn"

Clear-D2DVisuals

Create-D2DRadialGradientBrush -BrushId 1001 -OffsetY -0.25
Create-D2DGradientStopForBrush -BrushId 1001 -Color "#fff" -Offset 0
Create-D2DGradientStopForBrush -BrushId 1001 -Color "#00f" -Offset 0.15
Create-D2DGradientStopForBrush -BrushId 1001 -Color "#004" -Offset 1

$visual_rootSphere = 1001
$sphereCount = 10
Create-D2DEllipseVisual -VisualId $visual_rootSphere -FillBrushId 1001 -Width 300 -Height 300
for ($iter = 1; $iter -lt $sphereCount; ++$iter) {
    Clone-D2DVisual -VisualId ($visual_rootSphere + $iter) -CloneVisualId 1001
}

$frame = 0

while ($frame -lt 250) {
    $frame = $frame + 1
    $a = $frame * 0.01
    $p = 0.5

    for ($iter = 0; $iter -lt $sphereCount; ++$iter) {
        $x = 500 + 300 * [System.Math]::Sin($a + $p*$iter)
        $y = 500 + 300 * [System.Math]::Cos($a + $p*$iter)

        Move-D2DVisual -VisualId ($visual_rootSphere + $iter) -Y $y -X $x
    }


    Wait-D2DForRefresh
}