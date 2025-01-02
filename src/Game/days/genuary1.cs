using System.Collections;
using Zinc;
using Zinc.Core;
using static Zinc.Core.ImGUI;

namespace genuary25;

public class genuary1 : Scene
{
    int gridDim = 16;
    int cellDim = 32;
    public override void Create()
    {
        var noise = new OctaviaNoise(seed: 88883);

        var g = new Grid(cellWidth:cellDim, cellHeight:cellDim, numHorizontalCells:gridDim, numVerticalCells:gridDim, update: (self, dt) =>{
            // self.Rotation += (float)dt;
        });

        for (int i = 0; i < gridDim * gridDim; i++)
        {
            int x = i % gridDim;
            int y = i / gridDim;
            double noiseSample = noise.CellNoiseXY(xsize: 100, ysize:100, x: x, y: y);


            var a = g.AddChild(
                new SceneEntity(true,children:[
                    new Shape(width:cellDim,height:8){X=0,Y=0,Name="hor",Renderer_Color=Palettes.ONE_BIT_MONITOR_GLOW[0]},
                    new Shape(height:cellDim,width:8){X=0,Y=0,Name="ver",Renderer_Color=Palettes.ONE_BIT_MONITOR_GLOW[0]},
                ]){
                    Name = $"{noiseSample}"
                });
            new Coroutine(RotateTween(a,noiseSample),$"rotate{i}");
        }
    }

    public IEnumerator RotateTween(Anchor a, double wait)
    {
        yield return new WaitForSeconds((float)wait);
        var colorChange = new ColorTween(Palettes.ONE_BIT_MONITOR_GLOW[1],Palettes.ONE_BIT_MONITOR_GLOW[0],Easing.EaseInOutQuad);
        List<Shape> shapes = a.GetChildren().Select(c => c as Shape).ToList();
        bool flipflop = false;
        while(true)
        {
            yield return new FloatTween(a.Rotation,a.Rotation + MathF.PI,Easing.EaseInOutCirc)
            {
                Duration = 2.5f,
                ValueUpdated = (v) => {a.Rotation = v;},
                ProgressUpdated = (p) => {
                    p = flipflop ? 1 - p : p;
                    var c = colorChange.Sample(p);
                    foreach (var s in shapes)
                    {
                        s.Renderer_Color = c;
                    }
                }
            };
            flipflop = !flipflop;
        }
    }
}


