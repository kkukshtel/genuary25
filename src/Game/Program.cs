using System.Numerics;
using System.Reflection;
using genuary25;
using Zinc;
using Zinc.Core;

InputSystem.Events.Key.Down += (key,_) =>  {
	if (key == Key.C)
	{
		Engine.Clear = !Engine.Clear;
	}
	if (key == Key.COMMA)
	{
		Engine.ShowMenu = !Engine.ShowMenu;
	}
};


List<SketchInfo> sketches = getSketches().ToList().OrderBy(info => info.Name).ToList();
Engine.Run(new Engine.RunOptions(1280,720,"genuary25", 
	() =>
	{
		var scene = new genuary1();
		scene.Mount(0);
		scene.Load(() => scene.Start());
	}, 
	() =>
	{
		if(Engine.ShowMenu)
		{
			drawDemoOptions();
		}
	}
	));

void drawDemoOptions()
{
	ImGUI.MainMenu(() =>
	{
		ImGUI.Menu("days", () =>
		{
			Scene? scene = null;
			foreach (var entry in sketches)
			{
				if (ImGUI.MenuItem(entry.Name))
				{
					scene = Activator.CreateInstance(entry.Type) as Scene;
					// scene = Activator.CreateInstance(type.Type) as Scene;
					scene.Name = entry.Name;
				}
			}
			if (scene != null)
			{
				Engine.TargetScene.Unmount(() =>
				{
					scene.Mount(0);
					scene.Load(() => scene.Start());
				});
			}
		});
        
		ImGUI.Button("Reload Scene",new Vector2(100,20),() => {
			var targetSceneType = Engine.TargetScene.GetType();
			Engine.TargetScene.Unmount(() => {
				var reloadedScene = Activator.CreateInstance(targetSceneType) as Scene;
				reloadedScene.Mount(0);
				reloadedScene.Load(() => reloadedScene.Start());
			});
		});
	});
}


IEnumerable<SketchInfo> getSketches()
{
    return Assembly.GetExecutingAssembly().GetTypes()
        .Where(type => type.Namespace == "genuary25")
        .Select(type => new
        {
            Type = type,
            Attribute = type.GetCustomAttribute<GenuarySketchAttribute>()
        })
        .Where(t => t.Attribute != null)
        .Select(t => new SketchInfo(t.Type, t.Attribute.Name))
        .OrderBy((info => info.Name));
}
public record SketchInfo(Type Type, string Name);

[AttributeUsage(AttributeTargets.Class)]
public class GenuarySketchAttribute : Attribute
{
    public string Name { get; private set; }
    public GenuarySketchAttribute(string name)
    {
        Name = name;
    }
}