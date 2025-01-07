using System.Numerics;
using Zinc;
using Zinc.Core;
using System.Collections;

using static Zinc.Core.ImGUI;
using System.Diagnostics;

namespace genuary25;

[GenuarySketch("day5")]
public class genuary5 : Scene
{
    //make a grid compose of towers of squares
    //towers are placed randomly on the grid based off noise map
    //height is based off noise map
    //towers sway towards mouse position, with slices of the towers moving based off 
    //a delay and interpolated point based on their height in the tower

    int gridDim = 42;
    int squareSize = 57;
    int gridCellDim = 64;
    int outlineWidth = 4;
    float numSlices = 10;
    int gridDimInCells = 3;
    List<Shape> allShapes = new();
    List<Anchor> allSlices = new();
    float baseOffset = 100;
    OctaviaNoise noise = new OctaviaNoise(seed: 42);
    public record BaseYPos(float y, int index) : Tag($"BASEYPOS:{y},INDEX:{index}");

    public float kernelDim = 25f;
    public float fieldDim = 100f;
    float sliceHeight = 10;
    float cellDim = 30;
    Grid root;
    public override void Create()
    {
        // int gridDim = MathF.Floor(fieldDim / kernelDim);
        int gridDim = 5;
        int noiseMapSize = 100;
        float kernelSize = noiseMapSize / (float)gridDim;
        root = new Grid(cellWidth:cellDim, cellHeight:cellDim, numHorizontalCells:gridDim, numVerticalCells:gridDim,update:(self,dt) => {
            // root.Rotation += 0.01f;
        }){
            RotationBehavior = GridComponent.ChildRotationBehavior.Invert
        };
        Quick.Center(root);
        
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

                for (int i = 0; i < numColumnSlices; i++)
                {
                    var a = new Shape(width:cellDim,height:cellDim){
                        RenderOrder = rendStart - i,
                        Y = i * -sliceHeight,
                        Renderer_Color = colorChange.Sample(Quick.Map(i,0,numColumnSlices,0,1)),
                        Rotation = MathF.PI/4f, // 45 degrees will make it look like a diamond
                    };
                    new Coroutine(MoveTowardsMouse(a));
                    column.AddChild(a);
                }
                root.AddChild(column);

            }
        }

        root.Rotation = MathF.PI / 8f; // Set desired rotation
        // root.PushGridPositions(); // Immediately apply the rotation to children

    }

    public IEnumerator MoveTowardsMouse(Shape shape)
    {
        var basePos = new Vector2(shape.X, shape.Y);
        while(true)
        {
            yield return new FloatTween(shape.X, InputSystem.MouseX, Easing.EaseOutQuad)
            {
                Duration = 0.5f,
                ValueUpdated = (v) => {
                    shape.X = v;
                },
            };
            
            yield return new WaitForSeconds(0.1f);
        }
    }

    public override void Update(double dt)
    {
        if(Engine.ShowMenu)
        {
            var rootX = root.X;
            var rootY = root.Y;
            Window("params", () => {
                SliderFloat("rootX",ref rootX,0,1000,"",SliderFlags.None);
                SliderFloat("rootY",ref rootY,0,1000,"",SliderFlags.None);
            });
            root.X = rootX;
            root.Y = rootY;
        }
    }
}
