# BoomForce - BroForce Clone

A recreation of *Broforce*, developed as a project for the Game Engines course of my Software Engineering Bachelor studies. Built in Unity, the project emphasizes destructible environments, complex chain reactions, and physics-based object interactions, replicating the core gameplay mechanics of the original game.

## ⚠️ Current Status & Installation

**Please Note:** The project currently cannot be built into a standalone executable.
To play the game, you must run it inside the **Unity Editor**.

1. Clone this repository.
2. Open the project via Unity Hub.
3. Open the main scene.
4. Press the **Play** button in the Editor.

## 🎮 Controls

| Action | Input | Notes |
| :--- | :--- | :--- |
| **Move Left/Right** | `A` / `D` or `Arrow Keys` | |
| **Climb Ladder** | `W` / `S` | (Preferred over Arrow Keys for smoother input) |
| **Shoot** | `Left Mouse Button` / `Touchpad` | |

## ⚙️ Game Mechanics & Physics

The core challenge of this project was programming the physics and interactions for movable and destructible objects to create satisfying chaos.

### Destructible Terrain (Tile System)
The environment consists of tiles with specific health points. Damage is calculated based on direct hits and fire damage over time.
* **Grass Tiles:** 8 HP
* **Hard Tiles:** 10 HP
* **Soft Tiles:** 4 HP
* **Damage Logic:** One shot deals **2 damage**. Burning deals **1 damage** per second.

### Object Interactions & Chain Reactions
A significant amount of development time went into handling the logic for explosions, fire propagation, and physics triggers.

* **Barrels:**
    * **Red Barrel:** Explodes 0.2 seconds after triggering.
    * **Black Barrel:** Explodes 4 seconds after triggering.
    * **Explosion Logic:**
        * *Radius 1:* Instant destruction of blocks and triggering of other explosives.
        * *Radius 2:* Sets blocks on fire (burns for 4s), triggers other barrels, and dislodges falling stones.

* **Flybarrel:**
    * Once triggered, it launches after a short delay, traveling 18 tiles (upwards or rightwards depending on orientation).
    * Explodes on impact with any object (except other Flybarrels).
    * **Chain Reaction:** If it hits another Flybarrel, it triggers it immediately.

* **Falling Stones:**
    * **Triggers:** Shots, fire, or player collision.
    * **Physics Logic:** Falls if there is no support underneath. It also includes neighbor-checking logic (e.g., falls automatically if unsupported and only connected to one other stone on specific sides).

## 👨‍💻 Technical Implementation & Key Learnings

Developing *BoomForce* provided deep insights into Unity's core systems:

* **TileMap & Grid System:** Learned how to effectively manipulate the grid for dynamic, destructible environments.
* **Complex Physics:** Managed physical states (falling, stationary, exploding) and optimized collision detection.
* **Recursive Logic:** Implemented chain reactions (explosions triggering other explosions), requiring careful state management to avoid infinite loops or multi-triggering.
* **Input Handling:** Implemented cross-device input support (Keyboard/Mouse/Touchpad).
* **Debugging:** Solved complex edge cases where objects wouldn't react correctly within the chain of events (e.g., balancing burn times vs. explosion triggers).

## 🎨 Assets & Credits

While the game logic and physics programming are original, visual and audio assets were sourced from the community.

**Visual Assets:**
* **Sunny Land** by ANSIMUZ
* **Terrainify 2D** by Robronic Games
* **Too Cube Forest** by NEMO-MK2
* Broforce sprites via [The Spriters Resource](https://www.spriters-resource.com/pc_computer/broforce/)
* Additional assets from [Itch.io](https://itch.io/)

**Audio:**
* Jumping/Landing sounds created by the developer.
* Additional SFX from [Videvo](https://www.videvo.net/) and [Mixkit](https://mixkit.co/).

**Tutorials & References:**
* 2D Movement based on the tutorial by [Brackeys](https://youtu.be/dwcT-Dch0bA?si=PwcWeVFtiC-wRh2_).
