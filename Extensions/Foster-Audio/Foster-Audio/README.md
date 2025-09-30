## Foster.Audio Integration
A wrapper for the [Foster.Audio](https://github.com/MrBrixican/Foster.Audio) library. A simple solution for adding audio support to Jackdaw, but not as feature-rich as other options.
This extension is still work-in-progress and may have breaking changes in the future.

### Usage
Adding a `AudioManager` component into the game's node tree will automatically load all audio files based on the given config data. Sounds can either be played directly from the manager or using any of the player components if more options or control are needed.