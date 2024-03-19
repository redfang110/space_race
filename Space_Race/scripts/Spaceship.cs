using System;
using System.Collections.Generic;
using System.Reflection;

// using System.Numerics;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using BEPUphysics.Entities;
using BEPUphysics.CollisionShapes.ConvexShapes;

namespace Space_Race
{
    public class Spaceship : DrawableGameComponent
    {
        public Vector3 Position;
        public Vector3 ForwardVector;
        public Vector3 UpVector;
        public Vector3 LookAt;
        private float LookAtDistance;
        private Model model, target;
        private Game1 _game;
        public Box collider;
        public bool hit;
        private float maxMagnitude = 100;

        public bool currentTarget;


        public Spaceship(Game1 game, Vector3 Position) : base(game)
        {
            _game = game;
            this.Position = Position;
            this.LookAt = Position + new Vector3(0, 0, 3);
            this.LookAtDistance = Math.Abs(Vector3.Distance(Position, LookAt));
            currentTarget = false;
            hit = false;
            collider = new Box(new BEPUutilities.Vector3(Position.X, Position.Y, Position.Z), 8, 4, 8, 1);
            collider.Material.Bounciness = 1;
            _game.space.Add(collider);
        }

        public override void Update(GameTime gameTime)
        {
            var keyStates = Keyboard.GetState();

            // Set AngularVelocity to zero 
            // - This ensures yaw and pitch are only adjusted during user input
            // Decriment LinearVelocity if thruster is not on
            // - This gives a more natural feel to thruster input
            collider.AngularVelocity = BEPUutilities.Vector3.Zero;
            if (!keyStates.IsKeyDown(Keys.Space))
            {
                collider.LinearVelocity -= collider.LinearVelocity / 100;
            }

            // foreach loop handles user input logic
            foreach (Keys key in keyStates.GetPressedKeys())
            {
                // Variables used for WASD input ("* 2" for Y rotation since it rotates at half speed)
                BEPUutilities.Vector3 rotation;
                float rotationSensativity = 15f;
                collider.ActivityInformation.Activate();
                switch (key)
                {
                    case Keys.A:
                        // Rotates collider (left) about the Y axis
                        rotation = BEPUutilities.Vector3.UnitY * rotationSensativity * 2;
                        collider.ApplyAngularImpulse(ref rotation);
                        break;
                    case Keys.D:
                        // Rotates collider (right) about the Y axis
                        rotation = BEPUutilities.Vector3.UnitY * -rotationSensativity * 2;
                        collider.ApplyAngularImpulse(ref rotation);
                        break;
                    case Keys.W:
                        // Rotates collider (up) about the X axis
                        rotation = BEPUutilities.Vector3.UnitX * -rotationSensativity;
                        collider.ApplyAngularImpulse(ref rotation);
                        break;
                    case Keys.S:
                        // Rotates collider (down) about the X axis
                        rotation = BEPUutilities.Vector3.UnitX * rotationSensativity;
                        collider.ApplyAngularImpulse(ref rotation);
                        break;
                    case Keys.Space:
                        BEPUutilities.Vector3 bepuDir = new BEPUutilities.Vector3(ForwardVector.X, ForwardVector.Y, ForwardVector.Z);
                        collider.ApplyLinearImpulse(ref bepuDir);

                        if (collider.LinearVelocity.LengthSquared() > maxMagnitude)
                        {
                            collider.LinearVelocity *= maxMagnitude / collider.LinearVelocity.LengthSquared();
                        }
                        break;
                }
            }

            Matrix RotationMatrix = Matrix.CreateFromQuaternion(new Quaternion(collider.Orientation.X, collider.Orientation.Y, collider.Orientation.Z, collider.Orientation.W));
            Position = new Vector3(collider.Position.X, collider.Position.Y, collider.Position.Z);
            ForwardVector = -Vector3.Transform(Vector3.Forward, RotationMatrix);
            UpVector = Vector3.Transform(Vector3.Up, RotationMatrix);
            LookAt = Position + (LookAtDistance * ForwardVector);

            // Console.WriteLine("Position: " + Position);
            // Console.WriteLine("LookAt: " + LookAt);
            // Console.WriteLine("LookAtDistance: " + Math.Abs(Vector3.Distance(Position, LookAt)));
            // Console.WriteLine("Rotation" + RotationMatrix);
            // Console.WriteLine("Direction" + directionVector);
            // Console.WriteLine();

            // TODO: Add your update logic here
            base.Update(gameTime);
        }

        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here
            model = _game.Content.Load<Model>("donut");
            target = _game.Content.Load<Model>("donut");
        }

        

        public override void Draw(GameTime gameTime)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    Matrix rotationFixer = Matrix.CreateRotationY(MathHelper.ToRadians(0)) * Matrix.CreateRotationX(MathHelper.ToRadians(0)) * Matrix.CreateRotationZ(MathHelper.ToRadians(0));
                    // Vector3 forwardDirection = -Vector3.Transform(Vector3.Forward, RotationMatrix);
                    // Vector3 upDirection = Vector3.Transform(Vector3.Up, RotationMatrix);

                    effect.EnableDefaultLighting();
                    effect.World = rotationFixer * Matrix.CreateWorld(Position, ForwardVector, UpVector);
                    effect.View = _game.viewMatrix;
                    effect.Projection = _game.projectionMatrix;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }

            foreach (ModelMesh mesh in target.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = Matrix.CreateWorld(LookAt, -ForwardVector, UpVector) * Matrix.CreateScale(new Vector3(0.25f, 0.25f, 0.25f));
                    effect.View = _game.viewMatrix;
                    effect.Projection = _game.projectionMatrix;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }



            base.Draw(gameTime);
        }
    }
}