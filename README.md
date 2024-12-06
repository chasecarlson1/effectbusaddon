# Godot C# EffectBus 2D Addon

[The GitHub Repository](https://github.com/chasecarlson1/effectbusaddon)

**Made for Godot v4.3 stable, mono/C# edition.**

MIT licensed by u/GamingGuitarControlr on Reddit.


## Table of Contents
1. [Overview](#overview)
2. [How to Set-Up an `EffectBus` Node](#how-to-set-up-an-effectbus-node)
3. [How to Play Effects](#how-to-play-effects)
4. [Example](#example)

## Overview

### An `EffectBus` node manages object pools of `AnimatedSprite2D`-based effects for performance optimization.

#### The best use case of an `EffectBus` node is to pool hundreds (or even thousands) of `AnimatedSprite2D` flipbook effects in a scene where the effects can to be y-sorted, unlike using a regular particle system.

#### *Common effects include bullet hits, footsteps, blood, explosions, decals, etc...*

An `EffectBus` is a node you instantiate in a scene to play `SpriteFrames`-based effects. The `EffectBus` pools `AnimatedSprite2D` nodes and manages them to reuse them each time an effect is called from anywhere using a static method: `EffectBus.PlayEffect(EffectData data)`.

In the scene tree, the effects will be a sibling of the `EffectBus` node, which means you can add an `EffectBus` as a child of a `Node2D` with y-sorting enabled, and the effects you then create will be y-sorted! *(unless you set an explicit z-index and set it to not be relative for the effects)*.

## How to Set-Up an `EffectBus` Node
When you add an `EffectBus` node to your scene, you will see one exported property: Sprite Frame Data.

With this property, you can add new `SpriteFramesData` resource instances to this exported `Array` to set up what effects can be played.

### These `SpriteFrameData` resources have several exported properties to set up...
- **Name** is the unique ID or name of this effect.
    - It is a StringName.
    - and you must pass the exact name when calling for the effect.
- **Frames** is the underlying SpriteFrames resource for the effect.
    - Animation names do not matter.
    - Animations must loop.
    - More animations = more random variety to play.
- **Count** is the number of instances in this pool.
    - More instances means more nodes in the scene.
    - The instances will be *siblings* of the EffectBus.
    - Defaults to 20, but could be much, much more.
    - Once the 20th one has played, the 0th one will play, and the cycle continues to reuse the AnimatedSprite2D nodes in the pool.
- **Offset Centered** means the instances (AnimatedSprite2Ds) of this effect will have the origin of the sprite be Centered.
    - If you turn it off, it will be centered at the top left corner of the frame.
- **Offset** is the center/anchor offset of the instances
    - This is the "Offset" property of the AnimatedSprite2D instances.
- **HideOnFinish**, if true, will hide the AnimatedSprite2D of an effect instance that has finished playing.
    - This doesn't matter if your last frame in your effect is blank.
    - If you have a blood splatter, you might want this to be turned off to allow the last frame to stay put until the instance is recycled in the object pool.
- **Z Index Relative** will make the instances have this be true
    - Leave this on if you want y-sorting to work.
- **Z Index** is the z-index of the instances
    - Leave this at zero if you want y-sorting to work.

### Note that the names of the animations in the SpriteFrames resource do not matter.
Animations will be played at random.

You include multiple animations in the SpriteFrames resource to allow for random variations. 

Make sure the animations in the SpriteFrames resource do NOT loop.

***tl;dr...***

***Instantiate an EffectBus node and add effect resources to its only exported variable, which is an Array of EffectData.***

## How to Play Effects
If I have an effect with the StringName Name of "step", I can call that effect to play by using the following static function...
```cs
EffectBus.EffectBus.PlayEffect(EffectData data);
```
`EffectData` is a struct that contains the Name of the effect you are trying to play, the global Transform2D of the effect (where to play it), the self-modulate color of the effect, and an optional speed modifier for adjusting the playback speed (defaults to 1f).

*An actual method call would look something like this:*
```cs
EffectBus.EffectBus.PlayEffect(new("StringName of the effect", globalTransformToUse, Colors.RosyBrown));
```
*Or this if you want to have a non-1x playback speed modifier:*
```cs
EffectBus.EffectBus.PlayEffect(new("StringName of the effect", globalTransformToUse, Colors.RosyBrown, 2f)); // 2x speed
```
***tl;dr...***

***Safely call a static method, "EffectBus.PlayEffect(data)", from any node or script, and if an EffectBus is in the SceneTree, it will respond and play it.***

## Example

You don't need to get a reference to the actual `EffectBus` node that will manage the effects you are calling.

If the effect doesn't exist in an EffectBus in the scene, nothing happens.

*Here is an example of a script in a scene that calls an effect named "test"...*
```cs
using Godot;

public partial class Test : Node2D
{
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            Transform2D xform = new(0f, new(1f, 1f), 0f, GetGlobalMousePosition());
            EffectBus.EffectBus.PlayEffect(new("test", xform, Colors.RosyBrown)); // defaults to a speed of 1f
            // EffectBus is the namespace, and EffectBus is the name of the class inside it that you will use, which can be a little confusing
        }
    }
}
```
