# Simple Mouse Look
A really quick and easy "Mouse Look" system for the new input system. Will make an object look at the mouse position.

## Setup
You'll want a Mouse Position binding in your input actions. As always, **USE THE PLAYERINPUT COMPONENT** - it will make the new input system much easier for you.

## With Aim Assist
Grab the aim assist package, add a serialized or public reference to AimAssistInput, and set `transform.up` to `aimAssist.TransformUpDirection((_inputWorldPos - transform.position).normalized);` 