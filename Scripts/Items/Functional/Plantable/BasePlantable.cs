using System;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
    public abstract class BasePlantable : Item
    {
        public class GrowTimer : Timer
        {

            private BasePlantable plantable;

            private int count;

            public GrowTimer(BasePlantable plantable) : base(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10), 2)
            {
                this.plantable = plantable;
            }

            protected override void OnTick()
            {
                count++;
                if (count == 1)
                {
                    plantable.ItemID = 3254;
                }
                if (count == 2)
                {
                    plantable.GetToPlant().MoveToWorld(plantable.Location, plantable.Map);
                    plantable.Consume();

                }
            }
        }

        public BasePlantable(int itemID)
            : base(itemID)
        {
        }

        public virtual int GetMinSkill()
        {
            return 0;
        }

        public virtual int GetMaxSkill()
        {
            return 30;
        }

        public BasePlantable(Serial serial)
            : base(serial)
        {
        }

        public abstract Item GetToPlant();


        private class PlantTarget : Target
        {
            private BasePlantable toPlant;
            public PlantTarget(BasePlantable toPlant) : base(6, true, TargetFlags.None)
            {
                this.toPlant = toPlant;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                IPoint3D p = targeted as IPoint3D;

                if (p == null || from.Map == null)
                    return;

                if (p is LandTarget)
                {
                    var target = (LandTarget)p;

                    if (target.TileID != 9)
                    {
                        from.SendMessage("Voce apenas pode plantar isto em fazendas");
                        return;
                    }

                    if (from.CheckSkill(SkillName.Herding, toPlant.GetMinSkill(), toPlant.GetMaxSkill()))
                    {
                        from.Emote("* plantando *");
                        from.Animate(AnimationType.Attack, 3);
                        toPlant.DropToWorld(from, target.Location);
                        toPlant.Movable = false;
                        new GrowTimer(toPlant).Start();
                        Effects.PlaySound(target.Location, from.Map, 0x12E);
                    }
                    else
                    {
                        from.SendMessage("Voce nao conseguiu colocar a planta corretamente");
                    }
                }
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.Target = new PlantTarget(this);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
        }
    }
}
