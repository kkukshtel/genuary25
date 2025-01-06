using System.Numerics;
using Zinc;
using Zinc.Core;
using System.Collections;

using static Zinc.Core.ImGUI;

namespace genuary25;

[GenuarySketch("day4")]
public class genuary4 : Scene
{
    int gridDim = 42;
    int squareSize = 57;
    int gridCellDim = 64;
    int outlineWidth = 4;
    float numSlices = 10;
    float columnSpacing = 20;
    int gridDimInCells = 3;
    List<Shape> allShapes = new();
    List<Anchor> allSlices = new();
    float baseOffset = 100;
    OctaviaNoise noise = new OctaviaNoise(seed: 42);
    public record BaseYPos(float y, int index) : Tag($"BASEYPOS:{y},INDEX:{index}");
    public override void Create()
    {
        for (int sliceIndex = 0; sliceIndex < numSlices; sliceIndex++)
        {
            var slice = new Grid(cellWidth:gridCellDim, cellHeight:gridCellDim, numHorizontalCells:gridDimInCells, numVerticalCells:gridDimInCells){
                RotationBehavior = GridComponent.ChildRotationBehavior.Match
            };
            Quick.Center(slice);
            slice.Y -= sliceIndex * columnSpacing - baseOffset;

            double noiseSample = noise.CellNoiseXY(xsize: 100, ysize:100, x: 0, y: sliceIndex * 0.2);

            var renderOffset = sliceIndex * -2;

            for(int i = 0; i < slice.NumHorizonalCells * slice.NumVerticalCells; i++)
            {
                var outlinedSquare = slice.AddChild(
                    new SceneEntity(true,children:[
                        new Shape(width:squareSize,height:squareSize){Tags = ["bg"],RenderOrder=renderOffset + 1,Renderer_Color=Palettes.ONE_BIT_MONITOR_GLOW[1]},
                        new Shape(width:squareSize-outlineWidth,height:squareSize-outlineWidth){Tags = ["fg"],RenderOrder=renderOffset + 0,Renderer_Color=Palettes.ONE_BIT_MONITOR_GLOW[0]},
                    ]));           
                allShapes.AddRange(outlinedSquare.GetChildren().Select(c => c as Shape));
                outlinedSquare.Rotation = MathF.PI;
            }
            allSlices.Add(slice);
            slice.Tag(new BaseYPos(slice.Y,sliceIndex));
            // slice.PushGridPositions();
            slice.Rotation = MathF.PI/5f;
            new Coroutine(RotateSlice(slice,noiseSample),$"rotate{sliceIndex}");
        }
    }

    public IEnumerator RotateSlice(Anchor a, double wait)
    {
        yield return new WaitForSeconds((float)wait);
        while(true)
        {
            yield return new FloatTween(MathF.PI/5f,MathF.PI/5f + MathF.PI / 2F,Easing.EaseOutBounce)
            {
                Duration = 2.5f,
                ValueUpdated = (v) => {
                    a.Rotation = v;
                },
            };
        }
    }

    public override void Update(double dt)
    {
        if(Engine.ShowMenu)
        {
            Window("params", () => {
                SliderInt("square size",ref squareSize,1,1000,"",SliderFlags.None);
                SliderInt("outline width",ref outlineWidth,0,squareSize,"",SliderFlags.None);
                SliderFloat("column spacing",ref columnSpacing,0,1000,"",SliderFlags.None);
            });

            foreach (var shape in allShapes)
            {
                if(shape.Tagged("bg"))
                {
                    shape.Renderer_Width = squareSize;
                    shape.Renderer_Height = squareSize;
                }
                if(shape.Tagged("fg"))
                {
                    shape.Renderer_Width = squareSize - outlineWidth;
                    shape.Renderer_Height = squareSize - outlineWidth;
                }
            }

            // foreach (var slice in allSlices)
            // {
            //     slice.GetTag<BaseYPos>(out var tag);
            //     slice.Y = tag.y + columnSpacing * -tag.index;
            // }
        }
    }
}