<p align="center">
  <img width="150" height="263" src="Assets/readme_logo.png">
</p>

A collection of C# libraries designed for creating games and developer tools. As this is currently still in development, these sections will be used for describing our plans for each.

---

<p><a href="Rise.Numerics"><img width="265" height="32" src="Assets/header_numerics.png"></a></p>

A math library with vectors, matrices, helper functions, shapes, and useful geometry algorithms.

### Notes
- `RectanglePacker` should go in here
- Do we want different matrix sizes provided?
- Maybe a 5x5 `ColorMatrix` for color transformations
- More organized splines, and a linear extrapolator
- It might be nice to move angles into a single value struct, with handy shortcuts for radians/degrees/turns/etc. and other functions
- A generic `MatrixStack` could potentially go in here


---

<p><a href="Rise.Imaging"><img width="265" height="32" src="Assets/header_imaging.png"></a></p>

An image processing library with bitmaps and encoders/decoders for various image formats.

### Notes
- Possible formats to support?
    - TGA, BMP, JPG, GIF, WEBP, FLIF, BPG, ICO
    - RIF (_Rise Image Format_)
- Decoders and encoders both should be thread-safe, so asset processing can be threaded
- Algorithms for processing images? Bitmap drawing, scaling, rotating, blitting tools, noise generation, etc.?

---

<p><a href="Rise.Framework"><img width="265" height="32" src="Assets/header_framework.png"></a></p>

The core framework for managing windows, events, input, and low-level graphics rendering.

### Notes
- This serves only as a foundation and should avoid having game or implementation-specific stuff as much as possible (maybe even call it _Foundation_?)
- Should this framework also be in charge of managing assets? Should there be some sort of general asset manager built into all this? This might be nice going forward, having all assets streamed through and tracked accordingly.
- Maybe some sort of thread pooling system, which the asset manager and other tasks can use to run and report on their completion state.
- Currently only OpenGL, but in the future might want to support multiple renderers: OpenGL, DirectX, Vulkan, Metal, etc.
- Logging and error reporting should be built into the app loop, so errors can be handled gracefully and appropriately for different builds/states.

---

<p><a href="Rise.Engine"><img width="265" height="32" src="Assets/header_engine.png"></a></p>

A robust and extensible engine for running games and apps.

### Notes
- Should be modular and reusable design (components, etc.)
- Should be a foundation only, designed to be extended and built on top of, not solving every problem.
- Actual tools/components/renderers/etc. built on top of the game engine should get their own modules.
- It should be structured, so everything can be expected to work a certain way and can thus be optimized, but never at a loss of convenience or speed when coding games. (eg. Unity is too restrictive)
- Parallelism/multi-threading? Which parts/etc. use the thread pooling and are able to split off their tasks. If not here, then it should be easy for sub-modules to be able to parallelize themselves.
- Data-driven Entity-Component system?
    - `Entity` are in a big list and can have multiple `Component` attached.
    - Component are pure data, and `Manager` handle the actual logic of components, executing them in array batches.
    - Maybe there is a built-in `Script` and `ScriptManager` that are for attaching custom C# scripts as components, kind of like how non-data game engines work.
    - These custom Scripts should know how to serialize/deserialize, since that comes built-in with the data-driven component types.
- Object pooling? It'll work by default with data-driven components, but scripts might have to be disallowed constructors... if this is done, it should be trivial and user-friendly to create handy factory methods for creating scripts with custom parameters.
- Entire engine state should be serializable.
    - It should be possible to load/save states directly. This is difficult to have work efficiently, but if constructors are removed and data-driven types are prominent, it might be simpler.
    - This provides huge benefits to testing and debugging (eg. can see the game states leading up to a crash, can create jump points, easy undo/redo support, etc.)

---

<p><a href="Rise.Toolkit"><img width="265" height="32" src="Assets/header_toolkit.png"></a></p>

A robust library to streamline development of apps and game development tools.

---

<p><a href="Rise.Studio"><img width="265" height="32" src="Assets/header_studio.png"></a></p>

A visual development studio for developing games.

---

### Features and Tools Ideas

Some of these features it is yet-unclear if they should be part of the above libraries or in their own. This is where we can just list some things to consider or research if we want to add them.

- Collision (basic, polygonal, physics)
- Spacial partitioning?
- Virtual Machine (to hook up scripting languages)
- Command Console overlay
- Object pooling and reusability
- Texture packer tools
- Extensible scene editor
- Image editor
- Aseprite importing tools
- Animation format(s) for sprites?
- Basic sprite editor
- Part-based sprite editor
- Skeletal/puppet animation
- Particle system / particle editor?
- Rendering effects
    - Tinting
    - Color matrix
    - Gaussian blur
    - Outline / drop shadow
    - Bloom
    - Color grading
    - Antialiasing
    - Mesh deforming
- Tilemap rendering
