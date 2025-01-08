using System.Numerics;
using Zinc;
using Zinc.Core;
using System.Collections;

using static Zinc.Core.ImGUI;
using System.Diagnostics;

namespace genuary25;

[GenuarySketch("day8")]
public class genuary8 : Scene
{
    OctaviaNoise noise = new OctaviaNoise(seed: 42);
    int shapeTick = 0;
    Shape bg;
    bool flipflop;
    public override void Create()
    {
        Engine.Clear = false;
        Engine.showStats = false;
        bg = new Shape(width: Engine.Width, height: Engine.Height, color: Palettes.ONE_BIT_MONITOR_GLOW[0]){
            Renderer_Pivot = new Vector2(0,0),
            RenderOrder = 2
        };
        var colorChange = new ColorTween(Palettes.ONE_BIT_MONITOR_GLOW[0],Palettes.ONE_BIT_MONITOR_GLOW[1],Easing.Linear);
        var horSampler = new FloatTween(0,Engine.Width,Easing.Linear);
        for (int i = 0; i < 1000; i++)
        {
            var stamp = new Shape(width: 3, height: 3, update: (self, dt) => {
                if(shapeTick < 1000000)
                {
                    self.Rotation += 0.01f;
                    self.RenderOrder = -shapeTick;
                    self.Renderer_Color = colorChange.Sample(flipflop ? shapeTick/1000000f : 1 - shapeTick/1000000f);
                    var next = new Vector2(horSampler.Sample(shapeTick/1000000f),Quick.RandFloat() * Engine.Height);
                    self.X = next.X;
                    self.Y = next.Y;
                    shapeTick++;
                }
            });
        }
    }

    public override void Update(double dt)
    {
        if(bg != null)
        {
            bg.Destroy();
            bg = null;
        }
        if(shapeTick >= 1000000)
        {
            shapeTick = 0;
            flipflop = !flipflop;
        }
    }
    public override void Cleanup()
    {
        Engine.Clear = true;
    }
}
