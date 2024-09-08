using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class AnimationData {
    public readonly Point2[] Frames;
    public readonly Point2 FrameSize;

    public float DefaultFrametime;
    public float DefaultStartDelay;
    public bool DefaultLooping;
    public Vector2 DefaultPositionOffset;

    public AnimationData(Point2[] frames, Point2 frameSize) {
        Frames = frames;
        FrameSize = frameSize;
    }

    public AnimationData(int horizontalFrames, int verticalFrames, Point2 frameSize) {
        Frames = new Point2[horizontalFrames * verticalFrames];
        FrameSize = frameSize;

        for (int x = 0; x < horizontalFrames; ++x) {
            for (int y = 0; y < verticalFrames; ++y) {
                Frames[(y * horizontalFrames) + x] = new(x, y);
            }
        }
    }
}