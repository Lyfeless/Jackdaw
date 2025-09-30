## FMOD Integration
A loader and controller for using [FMOD](fmod.com) projects in Jackdaw.
This extension is still work-in-progress and may have breaking changes in the future.

### Library Files
This extension makes use of the FMOD and FMOD Studio api, which can't be included in this repo for licensing reasons. The appropriate library files can be found [here](https://www.fmod.com/download). The project will compile without them, but in order to run both api files need to be inside the build directory.

### Usage
Adding a `AudioManager` component into the game's node tree will automatically load all project bank files found at the provided path. Sounds can either be played directly from the manager or using the player component if more control is needed.