using System.Numerics;
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

Dictionary<int,Type> sceneDays = new Dictionary<int,Type>()
{
    {1,typeof(genuary1)},
    // {2,typeof(genuary2)},
    // {3,typeof(genuary3)},
    // {4,typeof(genuary4)},
    // {5,typeof(genuary5)},
    // {6,typeof(genuary6)},
    // {7,typeof(genuary7)},
    // {8,typeof(genuary8)},
    // {9,typeof(genuary9)},
    // {10,typeof(genuary10)},
    // {11,typeof(genuary11)},
    // {12,typeof(genuary12)},
    // {13,typeof(genuary13)},
    // {14,typeof(genuary14)},
    // {15,typeof(genuary15)},
    // {16,typeof(genuary16)},
    // {17,typeof(genuary17)},
    // {18,typeof(genuary18)},
    // {19,typeof(genuary19)},
    // {20,typeof(genuary20)},
    // {21,typeof(genuary21)},
    // {22,typeof(genuary22)},
    // {23,typeof(genuary23)},
    // {24,typeof(genuary24)},
    // {25,typeof(genuary25)},
    // {26,typeof(genuary26)},
    // {27,typeof(genuary27)},
    // {28,typeof(genuary28)},
    // {29,typeof(genuary29)},
    // {30,typeof(genuary30)},
    // {31,typeof(genuary31)},
};

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
			foreach (var entry in sceneDays)
			{
				if (ImGUI.MenuItem(entry.Key.ToString()))
				{
					scene = Activator.CreateInstance(entry.Value) as Scene;
					// scene = Activator.CreateInstance(type.Type) as Scene;
					scene.Name = entry.Key.ToString();
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