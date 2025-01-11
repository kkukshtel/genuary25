using System.Numerics;
using Zinc;
namespace genuary25;

[GenuarySketch("day10")]
public class genuary10 : Scene
{
    static float tau = MathF.Tau;
    static float one = tau / tau;
    static float two = one + one;
    static float zero = tau - tau;
    public override void Preload()
    {
        Engine.showStats = false;
    }
    public override void Create()
    {
        //create rings
        var numRings = (int)(tau * tau * tau);
        for (int ringIndex = (int)zero; ringIndex < (int)(tau * tau); ringIndex+=(int)one)
        {
            var root = new SceneEntity(true, update: (self, dt) => {
                (self as SceneEntity).Rotation += (one/tau)/tau/tau;
            });
            Quick.Center(root);

            var density = tau * tau;
            var inc = tau / density;
            var lineColor = Palettes.GetRandomColor();
            for (float i = zero; i < tau; i+=inc)
            {
                root.AddChild(new Shape(width: inc * ringIndex * ringIndex, height: inc * ringIndex * ringIndex, color: lineColor)
                {
                    LocalX = two * tau * ringIndex,
                    LocalY = zero,
                });
                root.Rotation = root.Rotation + inc;
            }
        }

    }
}
