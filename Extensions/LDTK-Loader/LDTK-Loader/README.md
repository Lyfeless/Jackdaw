## LDTK Loader
An extension for loading levels made in the [LDTK](ldtk.io) editor as nodes for use in Jackdaw games.
This extension is still work-in-progress and may be missing integration with some LDTK features.

### Usage
The extension loads levels using the `LDTKLoader` class, which expects an a .ldtk file in the project. In order to work correctly, the setting `Save levels as seperate files` needs to be enabled in the ldtk project's EXTRA FILES options.

The extension uses conversion functions to load entities into corresponding actor instances. To set up an actor for use with level loading references need to be defined with the `RegisterActor` function.

Tile grids are loaded without needing registration but can make use of additional loading features to automatically set up some systems. Custom data can be loaded using the tileset's `custom data` field, which the loader expects in lines of key-value pairs with a colon seperator (ex; `custom: my custom data goes here`). The loader has some built-in custom data fields that can be used to set up some common systems.
    - Using the `collider` data key, a tile can be given collsion information on load. Syntax: `collider: [TYPE] [DATA]`
      - FULL: A rectangle collider the exact size of the tile. Syntax: `collider: FULL`
      - RECT: A rectangle collider of any size. Syntax: `collider: RECT [x] [y] [width] [height]` ex: `collider: RECT 4 4 8 8`
      - CIRCLE: A circular collider centered on its x/y position. Syntax: `collider: CIRCLE [x] [y] [radius]` ex `collider: CIRCLE 8 8 4`
      - POLY: A convex polygon collider made of an abritrary bumber of points. Syntax: `collider: OLY [x1] [y1] [x2] [y2] ...` ex `collider: POLY 0 8 16 8 8 16 0 8
    - Using the `anim` data key, a tile can run a sprite animation instead of showing its normal texture. Syntax `anim: [NAME]` ex `anim: testAnim`