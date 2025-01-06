using System.Numerics;
using Zinc;
using Zinc.Core;

namespace genuary25;

[GenuarySketch("day3")]
public class genuary3 : Scene
{
    int gridDim = 42;
    int cellDim = 16;
    Grid g;
    public override void Create()
    {
        g = new Grid(cellWidth:cellDim, cellHeight:cellDim, numHorizontalCells:gridDim, numVerticalCells:gridDim);
        Quick.Center(g);
        var colorChange = new ColorTween(Palettes.ONE_BIT_MONITOR_GLOW[1],Palettes.ONE_BIT_MONITOR_GLOW[0],Easing.EaseOutQuint);
        var attractor = new SceneEntity(true,update:(self,dt) => {
            (self as Anchor).X = (float)(MathF.Cos((float)Engine.Time/1.04f) * 7.542f + Engine.Width/2);
            (self as Anchor).Y = (float)(MathF.Sin((float)Engine.Time/1.04f) * 7.542f + Engine.Height/2);
        });
        for (int i = 0; i < gridDim * gridDim; i++)
        {
            int x = i % gridDim;
            int y = i / gridDim;
            var a = g.AddChild(new Shape(width:cellDim,height:cellDim/4f,update:(self,dt) => {
                var dir = new Vector2(attractor.X,attractor.Y) - new Vector2(self.X,self.Y);
                self.Rotation = MathF.Atan2(dir.Y,dir.X);
                var dist = Vector2.Distance(new Vector2(attractor.X,attractor.Y),new Vector2(self.X,self.Y));
                self.Renderer_Color = colorChange.Sample(Quick.Map(dist,0,165,1,0));
                if(dist < 165 - 72)
                {
                    self.Renderer_Color = colorChange.Sample(Quick.Map(dist,0,165 - 72,0,1));
                    self.Rotation = -self.Rotation;
                }
                if(dist > 165)
                {
                    self.Renderer_Color = Palettes.ONE_BIT_MONITOR_GLOW[0];
                }
            }
            ));
            Quick.Center(a);
        }
    }
}