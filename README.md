![Jackdaw Logo](Logo.png)

## Jackdaw
A simple game library/engine designed to reduce the overhead of making 2D projects.

**Note**: This project is unfinished and likely to have breaking changes as features get added/adjusted. Use at your own risk!

### Getting started
Creating a basic project with Jackdaw is as simple as creating a game instance and adding actors to the game root. All children of the game's root actor with be updated and rendered automatically.
```cs
// Create the game instance with a basic configuration
Game game = new(new GameConfig() {
    ApplicationName = "JackdawExample",
    WindowTitle = "My First Jackdaw Game",
    Window = new() {
        WindowWidth = 320,
        WindowHeight = 180,
        ClearColor = "0x6495ED"
    }
});

// Create a new actor to use as the root
Actor rootActor = new(game);
rootActor.Position.LocalPosition = new(100, 50);

// Add a component to the root
rootActor.Components.Add(new SpriteComponent(game, "fallback-man"));

// Assign the root actor to the game;
game.Root = rootActor;

// Run the game
game.Start();
```

#### Samples
Example projects are in progress and will be available soon.

#### Extensions
Jackdaw is designed to not require any specific programs to run, so extensions have been made to interface with some existing utilities. All current Jackdaw extensions are located in [this-repo].

#### Dependancies
This project is built off of the framework [foster]. Anything requiring other external libraries or programs should be implemented through extensions.