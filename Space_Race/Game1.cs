using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BEPUphysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
// using BEPUphysics.Entities.Prefabs;
// using BEPUphysics;
// using ConversionHelper;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// using BEPUphysics.CollisionRuleManagement;
// using BEPUphysics.Entities;

namespace Space_Race;
// dotnet mgcb-editor
// dotnet run Program.cs

// Resources
// https://www.youtube.com/watch?v=OWrBLS7HO0A for general monogame 3d tutorial and code
// http://rbwhitaker.wikidot.com for skybox and skybox tutorial and code

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    public Space space;
    
    private Vector3 camTarget;
    private Vector3 camPosition;
    // private Matrix camRotationMatrix;
    public Matrix projectionMatrix;
    public Matrix viewMatrix;
    public Matrix worldMatrix;
    private List<Ring> rings;
    private Spaceship spaceship;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        space = new Space();

        camTarget = new Vector3(0, 0, 0);
        camPosition = new Vector3(0, 30, -5);
        projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), GraphicsDevice.DisplayMode.AspectRatio, 1f, 1000f);
        viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, Vector3.Up);
        worldMatrix = Matrix.CreateWorld(new Vector3(0, 0, 0), Vector3.Forward, Vector3.Up);

        rings = new List<Ring>
        {
            new Ring(this, new Vector3(0, 0, 0), new Vector3(0, 0, -10)),
            new Ring(this, new Vector3(-5, 5, 10), new Vector3(0, 0, 0)),
            new Ring(this, new Vector3(0, 10, 20), new Vector3(-5, 5, 10)),
            new Ring(this, new Vector3(5, 5, 30), new Vector3(0, 10, 20)),
            new Ring(this, new Vector3(0, 0, 40), new Vector3(5, 5, 30)),
            new Ring(this, new Vector3(-5, -5, 50), new Vector3(0, 0, 40)),
            new Ring(this, new Vector3(0, -10, 60), new Vector3(-5, -5, 50))
        }; 
        rings[0].currentTarget = true;
        
        foreach(Ring ring in rings) {
            Components.Add(ring);
        }
        spaceship = new Spaceship(this, new Vector3(0, 0, -5));
        Components.Add(spaceship);

        AddDebugRings();

        base.Initialize();
    }

    private void AddDebugRings()
    {
        float radius = 75f;
        int numberOfPoints = 100;
        Vector3 center = new Vector3(0, 0, 0);
        for (int i = 1; i < numberOfPoints + 1; i++) {
            float angle = (float)i / numberOfPoints * MathHelper.ToRadians(360);
            float x = (float)(center.X + radius * MathF.Cos(angle));
            float z = (float)(center.Z + radius * MathF.Sin(angle));
            Vector3 point = new Vector3(x, center.Y, z);
            Components.Add(new Ring(this, point, point - (center - point)));
        }
        for (int i = 1; i < numberOfPoints + 1; i++) {
            float angle = (float)i / numberOfPoints * MathHelper.ToRadians(360);
            float y = (float)(center.Y + radius * MathF.Cos(angle));
            float z = (float)(center.Z + radius * MathF.Sin(angle));
            Vector3 point = new Vector3(center.X, y, z);
            Components.Add(new Ring(this, point, point - (center - point)));
        }
        for (int i = 1; i < numberOfPoints + 1; i++) {
            float angle = (float)i / numberOfPoints * MathHelper.ToRadians(360);
            float y = (float)(center.Y + radius * MathF.Sin(angle));
            float x = (float)(center.X + radius * MathF.Cos(angle));
            Vector3 point = new Vector3(x, y, center.Z);
            Components.Add(new Ring(this, point, point - (center - point)));
        }
    }

    protected override void LoadContent()
    {
        // TODO: use this.Content to load your game content here
    }

    private void UpdateRings()
    {
        int index = 0;
        for(int i = 0; i < rings.Count; i++) {
            if (rings[i].hit) {
                index = i;
                rings[i].currentTarget = false;
            }
        }

        if (index == 0) {
            rings[0].currentTarget = rings[0].hit ? false : true;
            rings[1].currentTarget = rings[0].hit ? true : false;
        } else {
            int currentInd = index + 1 > rings.Count - 1 ? index : index + 1;
            rings[currentInd].currentTarget = index != currentInd || !rings[index].hit;
        }
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        space.Update();
        UpdateRings();

        camPosition = spaceship.Position;
        camTarget = spaceship.LookAt;

        viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, Vector3.Up);

        // TODO: Add your update logic here
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        base.Draw(gameTime);
    }
}
