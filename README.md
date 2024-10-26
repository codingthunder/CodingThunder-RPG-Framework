# What is the Coding Thunder RPG Framework
The CTRPGF is an RPG Framework with a focus on story-driven game control and data-driven flexibility. It emphasizes the ability to do game development with the tools best suited for the task, rather than doing everything inside the engine.

Right now, the Framework is still in very, very early development. It's going to have plenty of bugs, and it will be some time before it's considered feature-complete. But I wanted to get the ball rolling on it and let people take a crack at it if they want to.

> [!IMPORTANT]
> This project is in early, early development. That means there aren't many railguards for developers. While you do not need to know how to code in order to use the framework, you will need to be able to write precise syntax. Capitalization matters. Attention to detail matters. And obviously, if you wish to add any extra mechanics on top of the engine, you'll need to code that yourself (or pay somebody to do it for you).

# Design Philosophy & Development Goals
This Framework is built with users in mind who:
1. Are not visually inclined.
2. Who want to write story-driven games.

The core tenet of the Framework is "Logic as Data."

By treating incoming data (either from the story, or from other sources) as a logical expression instead of just a hard value, the Framework gives the people defining that data a lot of power. For story-writers, that means gaining the ability to manipulate the game scene directly from story script. For gameplay system designers, that means flexibility in how you define the stats for in-game objects, classes, etc.

Some of that tenet is already apparent in this pre-alpha release, but I want to add more. Some features, such as Unit Tests, are simply necessary. Others are up for discussion. If you have any ideas for how the Framework can improve, let me know.

With that in mind, here are the following features I hope to implement, in roughly the order I hope to implement them. This list can and will change as development continues.

The following are necessary to achieve what I consider to be "version 1.0."

- [x] Parsable Cmd System
- [x] Expression-driven Data Management
- [x] Story-driven Game Flow
- [] Semi-automated Documentation (includes adjusting Cmds to make them easier to work with)
- [] Unit Tests (seriously, though, these will make programming Cmds a million times easier)
- [] Stat-Driven RPG Mechanics
- [] QUESTS!
- [] Basic Stat-Driven Combat
- [] Basic Audio Implementation
- [] Proper Unity Package Support

After that, well, there are a lot of possibilities. Here are a few I have in mind. Feel free to make suggestions!

- [] Quality-of-life Animation & UI improvements
- [] Advanced Combat Mechanics
- [] Cmd & Expression Linting
- [] Multi-platform support & Flexible Input Options

# How to Use
## Try out the Example Project
So, you technically CAN install the framework as a Unity Package, and long-term, I kind of hope to make that the default option. However, at the moment, getting it working is a pain in the ass.

Before putting in too much work to use the Framework, I **strongly** encourage you to look at the Example Project and see how I do things. Much of the Framework at the moment is convention, not code. And while I hope to incorporate more hard-coded elements, certain things are just, "This is how I did it."

### Running the Example Project
Clone the repo or download it as a zip and unpack it. In File Explorer, go to the directory "examples/BasicExample/Packages/com.codingthunder.rpgframework/". In another window, go to the directory "src/CodingThunder-RPG-Framework/". Copy the contents of CodingThunder-RPG-Framework into com.codingthunder.rpgframework. THEN you can open the project in Unity.

You should be able to run the example from the MainMenu scene. Alternatively, I'll probably make a build of the example available on Itch or something. As a developer, you should be most interested in the ExampleStory Ink Script and the ExampleScene in Unity.

If you want to see how Cmds work, feel free to poke around in the Package folder.

### Developing Features
If you want to actually develop the Framework's code, I suggest replacing the com.codingthunder.rpgframework directory with a SymLink junction on windows. For other OS's, you're on your own.

Documentation is kind of scarce at the moment. That's literally the next thing on my list. I explain Cmds and RPGRefs a bit down below, and I tried to do a decent job of including documentation in the code itself, but it is NOT thorough or consistent at the moment. 

## Installing the Framework as a Package
If you don't want to run the example project and instead want to include the Framework in a project of your own, this is probably the easiest way to do it. It's annoying, but Unity really dislikes including Git dependencies inside of package manifests.

So here's what you do first: Either install the following dependencies via the manifest.json for your project, or use the Unity Package Manager. We do these first to avoid dependency problems.

> "com.github-glitchenzo.nugetforunity": "https://github.com/GlitchEnzo/NuGetForUnity.git?path=/src/NuGetForUnity",<br>
> "com.inkle.ink-unity-integration": "https://github.com/inkle/ink-unity-integration.git#upm",<br>

After that's done, do the same thing for the RPG Framework (alternatively, you can always just download the framework as a Zip file and insert it into your Assets, especially if you plan to make changes to the code):

> "com.codingthunder.rpgframework": "https://github.com/codingthunder/CodingThunder-RPG-Framework.git?path=/src/CodingThunder-RPG-Framework",

You're going to see some errors. That's normal. NuGetForUnity integrates NuGet (C# .NET's Package Manager) into your Unity project (making libraries such as NewtonSoft.Json available for use, yay).

Go to Packages/CodingThunder-RPG-Framework/Config (inside the Unity editor), and copy the two config files in there.

![Config Files screenshot](/docs/images/nuget_config_files.png)

Then go to your Assets folder (again, in Unity). Delete the two matching NuGet config files and paste in the ones you copied.

![Screenshot of Config Files in Assets](/docs/images/nuget_configs_in_Assets.png)

Finally, select NuGet from the top menu in Unity, and then select "Restore Packages." There may be a few curl errors, but after a moment, it should successfully install the appropriate NuGet packages.

![Screenshot of menu option to resture nuget packages](/docs/images/restore_nuget_packages.png)

Once all of that is finished, your project should be ready to start working.

## Starting a New Project
After that, well, I'm working on getting a how-to guide up and running. I made an example, but it uses some assets I don't have the rights to, so I'm going to try making a different example soon.

# Core Features
The Framework is built around two pillars: Cmds, and RPGRefs

## Cmds
> [!IMPORTANT]
> Putting this RIGHT AT THE TOP so it can't be missed. If you're going to define custom Cmds, **MAKE SURE YOU INVOKE YOUR completionCallback!!!** If your Cmd isn't ending when it's supposed to, and your Sequence isn't moving to the next Cmd when it's supposed to, you probably forgot to invoke your completionCallback.

Cmds exist so that non-coders can easily insert gameplay logic into narrative designs, triggers, etc. It is possible to use Cmds directly from code, but if at any point the Dev actually writes the Cmd out to be parsed by the Framework, at some point that string is going to be fed into a CmdExpression, which will then spit out the actual Cmd.

I'm personally not a fan of node-based systems (I'm not a super visual person), and so I wanted something where I could just plug commands into my story script to make things happen during cutscenes. One of the Framework's dependencies is the [Ink framework](https://github.com/inkle/ink), a powerful tool for writing stories for games. The CTRPGF supports inserting Cmds directly into your Ink story script to control the game.

However, that is not the only place you can write Cmds. Multiple built-in components support using Cmds inside of Sequences. For example, you can place a trigger on a Key object that adds the Key to the player's inventory and makes itself disappear. You can also define certain Cmds to occur on a scene's startup using the SceneInitalizer component.

Finally, you are able to define your own Cmds by extending the ICmd Interface. At time of writing, you also need to be sure your class is inside the "CodingThunder.RPGUtilities.Cmds" namespace, but I plan to change that soon.

> [!CAUTION]
> Because CmdExpressions are manually written, they can be and often are messed up by typos, by forgetting to uppercase, and so on. I don't have a syntax checker in place at the moment, so if a Cmd doesn't work, make sure you're typing everything correctly.

## RPGRef
The RPGRef was built out of a desire to have fine-tuned control over the data in my RPGs. It works best with primitives and strings, but it can be used for just about any data type. The idea is that, without writing any extra lines of code, I can have one sword's damage be based on the attacker's strength minus the defender's defense, have another sword's damage be set to always be 1, and a third sword's damage be based on the time of day (assuming I'm already tracking the time of day).

> [!CAUTION]
> RPGRef's are powerful and DANGEROUS. You'll see why in a second, but **at no point should players be able to set the values of RPGRefs.** If they do, they will have the ability to arbitrarily call methods and set important variables. They can and will be able to hack and break your game. As I continue development, I will add more guardrails to the Framework, but consider yourself warned.

Under the hood, RPGRefs implement [Dynamic Expresso](https://github.com/dynamicexpresso/DynamicExpresso) to evaluate most expressions. So, any of the following are valid inputs for the RPGRef's ReferenceId:

- "1"
- "3.5 + 2"
- "(1.5 + 2) / 5.7"
- "ClassName.staticFloatVariable - 42.0"
- "ClassName.staticFloatVariable1 * Classname.staticFloatVariable2"

However, it can be a pain in the ass to have to write GameDataManager.Instance.Lookup("PlayerPosition.x") every time you want to get the player's position. And if you try to parse a Vector2, DynamicExpresso will throw an error because Unity itself doesn't support parsing Vector2's (for some stupid reason). Enter Root Keywords and CustomParsers.

### Root Keywords
> [!NOTE]
> Earlier into development, I referred to Root Keywords as labels, and there are still places in the code where I do so. I changed them to be called Root Keywords to better describe their purpose. I'm also considering calling them Shortcuts. Planning to update for uniformity soon.

Root Keywords are useful for adding custom data lookup locations into your expression by using the '$$' symbols.

When the Resolver encounters the $$ symbols, it will check a Root Keyword dictionary for Function delegates it can invoke. If it finds one that matches the next set of characters before a '.' symbol, it will invoke that matching Function delegate.

> In layman's terms, you can turn this: GameDataManager.Instance.Lookup("PlayerPosition").x
> Into this: $$GameData.PlayerPosition.x

Because you are invoking a Function delegate, you can do a lot more than just replace a lookup expression, but that's the gist of it. Whatever your action returns, the Resolver will substitute your Root Keyword for the result's actual value, and then continue accessing properties/methods/etc. down its chain.

So now, if you want to access story variables, all you need is:
> $$Story.PlayerName

This setup is particularly useful because you may not want to provide public access to data structures in certain classes. By registering a Function delegate with a Root Keyword, you can expose important data to the RPGRef's LookupResolver without the LookupResolver needing to be aware of that class whatsoever.

### Custom Parsers
So, you use Custom Parsers for data types that aren't supported by DynamicExpresso natively. For example, I use it to parse Vector2 objects from strings. However, not only is the current Vector 2 parser broken if you include Root Keywords for the values, but I don't currently like the way that the Resolver uses Parsers in its processing logic, so I'm going to need to fix that sooner than later. So, yeah, at the moment, use custom parsers at your own risk.

### Final Notes on RPGRefs
It's worth noting that RPGRefs will almost always be slower to resolve than using a Data Type's built-in parser (if it has one), especially if you have a complex expression for your ReferenceId. Always keep that in mind when you're balancing your game code's speed. It may be that you DON'T want to support expressions for certain data fields in your logic or stats, especially if you're running this logic every frame. Be judicious in how you use RPGRefs.

