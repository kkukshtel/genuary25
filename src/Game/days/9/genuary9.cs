using System.Numerics;
using Zinc;
namespace genuary25;

[GenuarySketch("day9")]
public class genuary9 : Scene
{
    static float stampSize = 32;
    static float innerChildSize = 24;
    public class Stamp : Shape
    {
        public Stamp(Color c) : base(color: c)
        {
            AddChildren([
                new Shape(width:24,height:24){ Renderer_Pivot = new Vector2(0f), RenderOrder = -1, X = innerChildSize - stampSize, Y = innerChildSize - stampSize},
            ]);
        }
    }
    public override void Create()
    {
        List<uint> myPal = [
            Palettes.ENDESGA[0],
            Palettes.ENDESGA[1],
            Palettes.ENDESGA[2],
            Palettes.ENDESGA[3],
            Palettes.ENDESGA[4],
            Palettes.ENDESGA[5]
        ];
        Palettes.SetActivePalette(myPal);

        Engine.showStats = false;
        var baseColor = Palettes.GetRandomColor();
        myPal.Remove(baseColor);
        var start = new Vector2(stampSize / 2f, stampSize);
        var lastRot = 0f;
        for(int i = 0; i < MathF.Ceiling(Engine.Width / stampSize); i++)
        {
            for(int j = 0; j < MathF.Ceiling(Engine.Height / stampSize); j++)
            {
                new Stamp(baseColor){
                    X = start.X + i * stampSize,
                    Y = start.Y + j * stampSize,
                    Rotation = lastRot
                };
            }
            lastRot += MathF.PI / 2f;
        }
    }
}
