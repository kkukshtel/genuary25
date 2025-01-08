using System.Numerics;
using Zinc;
using Zinc.Core;
using System.Collections;

using static Zinc.Core.ImGUI;
using System.Diagnostics;
using Volatile;

namespace genuary25;

[GenuarySketch("day5")]
public class genuary5 : Scene
{
    OctaviaNoise noise = new OctaviaNoise(seed: 42);
    public record ShapeInfo(float sway, SceneEntity column) : Tag($"column");
    public float kernelDim = 25f;
    public float fieldDim = 100f;
    float sliceHeight = 10;
    float cellDim = 30;
    Grid root;
    float maxColumnHeight = 400; //this is based on noise sample of 1 + 1 * 200
    float maxSway = 100;
    float attractorMax = 300;
    public override void Create()
    {
        // int gridDim = MathF.Floor(fieldDim / kernelDim);
        int gridDim = 10;
        int noiseMapSize = 100;
        float kernelSize = noiseMapSize / (float)gridDim;
        root = new Grid(cellWidth:cellDim, cellHeight:cellDim, numHorizontalCells:gridDim, numVerticalCells:gridDim,update:(self,dt) => {
            // root.Rotation += 0.01f;
        }){
            RotationBehavior = GridComponent.ChildRotationBehavior.Invert
        };
        Quick.Center(root);
        root.Y += 150f;

        var attractor = new SceneEntity(true){
            Name = "attractor",
        };
        Quick.Center(attractor);
        new Coroutine(MoveAttractor(attractor));

        
        for (int y = 0; y < gridDim; y++)
        {
            for (int x = 0; x < gridDim; x++)
            {
                var renderColumnIndex = x + y;
                //sample four times around the kernel nsew from the xy point and average the values:
                double noiseSample = 0;
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        noiseSample += noise.CellNoiseXY(xsize: noiseMapSize, ysize:noiseMapSize, x: x * kernelSize, y: y * kernelSize);
                    }
                }
                noiseSample /= 9f;
                noiseSample += 1;
                var columnHeight = noiseSample * 200;
                var numColumnSlices = MathF.Floor((float)columnHeight / sliceHeight);

                var column = new SceneEntity(true){
                    Name = $"column{x},{y}",
                    X = x * cellDim,
                    Y = y * cellDim,
                };
                var rendStart = 2000 * -renderColumnIndex;
                var colorChange = new ColorTween(Palettes.ONE_BIT_MONITOR_GLOW[0],Palettes.ONE_BIT_MONITOR_GLOW[1],Easing.EaseInCirc);
                var maxSwayTween = new FloatTween(0,maxSway,Easing.EaseInCubic);
                for (int i = 0; i < numColumnSlices; i++)
                {
                    var a = new Shape(width:cellDim,height:cellDim,update:(self,dt) => {
                        self.GetTag<ShapeInfo>(out var tag);
                        var sway = new FloatTween(0,tag.sway,Easing.EaseInOutQuad);
                        var mouseDist = attractor.X - tag.column.X;
                        var sign = MathF.Sign(mouseDist);
                        var sample = sway.Sample(Mathf.Clamp(MathF.Abs(mouseDist),0,attractorMax)/attractorMax) * sign;
                        self.X = column.X + sample;
                    }){
                        RenderOrder = rendStart - i,
                        Y = i * -sliceHeight,
                        Renderer_Color = colorChange.Sample(Quick.Map(i,0,maxColumnHeight/sliceHeight,0,1)),
                        Tags = [new ShapeInfo(maxSwayTween.Sample(i / numColumnSlices),column)],
                        Rotation = MathF.PI/4f, // 45 degrees will make it look like a diamond
                    };
                    // new Coroutine(MoveTowardsMouse(a));
                    column.AddChild(a);
                }
                root.AddChild(column);

            }
        }

        root.Rotation = MathF.PI / 8f; // Set desired rotation
        // root.PushGridPositions(); // Immediately apply the rotation to children

    }

    public IEnumerator MoveAttractor(SceneEntity e)
    {
        var flipflop = false;
        while(true)
        {
            yield return new FloatTween(-attractorMax, attractorMax, Easing.EaseInOutQuad)
            {
                Duration = (Quick.RandFloat() + 1f) * 3,
                ValueUpdated = (v) => {
                    e.X = (Engine.Width / 2f) + (v * (flipflop ? 1 : -1));
                },
            };
            flipflop = !flipflop;
        }
    }
}
