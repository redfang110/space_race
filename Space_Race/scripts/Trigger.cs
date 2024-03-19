using BEPUphysics.Entities.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using BEPUutilities;
using Vector3 = BEPUutilities.Vector3;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.CollisionTests;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using Microsoft.Xna.Framework;

namespace Space_Race.Example
{
    public delegate void CollisionLogic();
    public class Trigger
    {
        public Box box;
        public Vector3 Position;
        private Vector3 Size;
        public CollisionLogic collisionFunction;
        public Trigger(Game1 game, Microsoft.Xna.Framework.Vector3 MonoPosition, Microsoft.Xna.Framework.Vector3 MonoSize, CollisionLogic collisionFunction)
        {
            //Create static box of size 1x1x1 at origin
            this.Position = new Vector3(MonoPosition.X, MonoPosition.Y, MonoPosition.Z);
            this.Size = new Vector3(MonoSize.X, MonoSize.Y, MonoSize.Z);
            this.collisionFunction = collisionFunction;
            box = new Box(Position, Size.X, Size.Y, Size.Z);

            //Add Box to physics space
            game.space.Add(box);
            //Disable solver to make box generate collision events but no affect physics (like a trigger in unity)
            //More about collision rules: https://github.com/bepu/bepuphysics1/blob/master/Documentation/CollisionRules.md
            box.CollisionInformation.CollisionRules.Personal = CollisionRule.NoSolver;
            //Add collision start listener
            //More about collision events: https://github.com/bepu/bepuphysics1/blob/master/Documentation/CollisionEvents.md
            box.CollisionInformation.Events.ContactCreated += CollisionHappened;
        }

        //Handle collision events
        void CollisionHappened(EntityCollidable sender, Collidable other, CollidablePairHandler pair, ContactData contact)
        {
            collisionFunction();
        }
    }
}
