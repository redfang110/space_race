using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Space_Race.Example;

namespace Space_Race
{
    public class Ring : DrawableGameComponent
    {
        public Vector3 Position;
        public bool hit;
        public bool currentTarget;

        private Vector3 LookAt;
        private Model model;
        private Game1 _game;
        private Trigger trigger;

        public Ring(Game1 game, Vector3 Position, Vector3 LookAt) : base(game)
        {
            _game = game;
            this.Position = Position;
            this.LookAt = LookAt;
            currentTarget = false;
            hit = false;
            trigger = new Trigger(game, Position, new Vector3(1, 1, 1), CollisionLogic);
        }

        private void CollisionLogic()
        {
            hit = true;
        }

        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here
            model = _game.Content.Load<Model>("donut");
        }

        public override void Draw(GameTime gameTime)
        {
            foreach(ModelMesh mesh in model.Meshes) {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = Matrix.CreateRotationY(MathHelper.ToRadians(180)) * Matrix.CreateWorld(Position, LookAt - Position, Vector3.Up);
                    effect.View = _game.viewMatrix;
                    effect.Projection = _game.projectionMatrix;
                    // if (currentTarget) {
                    //     // Set the color properties of the effect
                    //     effect.DiffuseColor = Color.Green.ToVector3(); 
                    //     effect.SpecularColor = Color.Green.ToVector3(); 
                    //     effect.EmissiveColor = Color.Green.ToVector3(); 
                    //     effect.AmbientLightColor = Color.White.ToVector3(); 
                    // } else {
                    //     // Set the color properties of the effect
                    //     effect.DiffuseColor = Color.White.ToVector3(); 
                    //     effect.SpecularColor = Color.White.ToVector3(); 
                    //     effect.EmissiveColor = Color.White.ToVector3(); 
                    //     effect.AmbientLightColor = Color.White.ToVector3(); 
                    // }
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }

            base.Draw(gameTime);
        }
    }
}