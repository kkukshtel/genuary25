using System.Collections;
using System.Numerics;
using Zinc;
using Zinc.Core;
using static Zinc.Core.ImGUI;

namespace genuary25;
[GenuarySketch("day2")]
public class genuary2 : Scene
{
    int gridDim = 16;
    int cellDim = 32;
    public record GridPos(int x, int y) : Tag($"GRID:{x},{y}");
    OctaviaNoise noise;
    Grid g;
    public override void Create()
    {
        noise = new OctaviaNoise(seed: 88883);
        g = new Grid(cellWidth:cellDim, cellHeight:cellDim, numHorizontalCells:gridDim, numVerticalCells:gridDim);
        
        for (int i = 0; i < gridDim * gridDim; i++)
        {
            int x = i % gridDim;
            int y = i / gridDim;
            g.GetLocalGridPosition(i, out float gx, out float gy);
            var s = new Shape(width:cellDim,height:cellDim){
                X = g.X + gx, 
                Y = g.Y + gy, 
                Renderer_Color=Palettes.ENDESGA[2],
                Tags = [new GridPos(x,y)]
            };
            new Coroutine(MoveToAdjacentGridPos(s),$"move{i}");
        }
    }

    public IEnumerator MoveToAdjacentGridPos(Shape s)
    {
        var colorChange = new ColorTween(Palettes.ONE_BIT_MONITOR_GLOW[0],Palettes.ONE_BIT_MONITOR_GLOW[1],Easing.EaseInOutQuad);
        while(true)
        {
            s.GetTags<GridPos>(out var tags);
            Console.WriteLine(tags.Count);
            var gridTag = tags[0];
            double noiseSample = noise.CellNoiseXY(xsize: 100, ysize:100, x: gridTag.x, y: gridTag.y);
            yield return new WaitForSeconds((float)noiseSample);
            var f = MathF.Floor(Quick.RandFloat() * 4);
            // 0 = up, 1 = right, 2 = down, 3 = left
            //clamp to grid dim
            if(gridTag.x == 0 && f == 3) f = 1;
            if(gridTag.x == gridDim - 1 && f == 1) f = 3;
            if(gridTag.y == 0 && f == 0) f = 2;
            if(gridTag.y == gridDim - 1 && f == 2) f = 0;
            (int x, int y) nextGridPos = f switch
            {
                0 => (gridTag.x,gridTag.y - 1),
                1 => (gridTag.x + 1,gridTag.y),
                2 => (gridTag.x,gridTag.y + 1),
                3 => (gridTag.x - 1,gridTag.y),
            };
            
            g.GetLocalGridPosition(nextGridPos.y * gridDim + nextGridPos.x, out float gx, out float gy);
            var nextPos = new Vector2(g.X + gx, g.Y + gy);
            s.Untag(gridTag);
            s.Tag(new GridPos(nextGridPos.x,nextGridPos.y));
            yield return new Vector2Tween(new Vector2(s.X,s.Y),nextPos,Easing.EaseInOutCirc)
            {
                Duration = 0.5f,
                ValueUpdated = (v) => {s.X = v.X; s.Y = v.Y;},
                ProgressUpdated = (p) => {
                    s.Renderer_Color = colorChange.Sample(p);
                }
            };
            yield return new ColorTween(Palettes.ONE_BIT_MONITOR_GLOW[1],Palettes.ONE_BIT_MONITOR_GLOW[0],Easing.EaseInOutQuad)
            {
                Duration = 0.5f,
                ValueUpdated = (v) => s.Renderer_Color = v
            };
        }
    }
}


