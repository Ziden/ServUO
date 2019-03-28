using System;
using Server.Network;

namespace Server.Items
{
    public abstract class FarmableCrop : Item
    {
        private bool m_Picked;
        public FarmableCrop(int itemID)
            : base(itemID)
        {
            this.Movable = false;
        }

        public virtual int GetMinSkill()
        {
            return 0;
        }

        public virtual int GetMaxSkill()
        {
            return 30;
        }

        public FarmableCrop(Serial serial)
            : base(serial)
        {
        }

        public abstract Item GetCropObject();

        public abstract int GetPickedID();

        public override void OnDoubleClick(Mobile from)
        {
            Map map = this.Map;
            Point3D loc = this.Location;

            if (this.Parent != null || this.Movable || this.IsLockedDown || this.IsSecure || map == null || map == Map.Internal)
                return;

            if (!from.InRange(loc, 2) || !from.InLOS(this))
                from.NonlocalOverheadMessage(MessageType.Regular, 0x3B2, true, "Estou muito longe"); // I can't reach that.
            else if (!this.m_Picked)
                this.OnPicked(from, loc, map);
        }

        public virtual void OnPicked(Mobile from, Point3D loc, Map map)
        {
            this.ItemID = 0x1014;

            Item spawn = this.GetCropObject();

            //if (spawn != null)
            //     spawn.MoveToWorld(loc, map);

            var Skill = (int)from.Skills[SkillName.Herding].Value;
            var amt = new Random().Next(5) + Skill / 5;
            spawn.Amount = amt;

            var check = from.CheckSkill(SkillName.Herding, GetMinSkill(), GetMaxSkill());

            var faiou = false;
            if(!check)
            {
                if(Utility.Random(3)==1)
                {
                    from.Emote("* tentou colher e estragou *");
                    Effects.PlaySound(this.Location, this.Map, 0x12E);
                    this.m_Picked = true;
                    this.Unlink();
                    Timer.DelayCall(TimeSpan.FromMinutes(5.0), new TimerCallback(Delete));
                } else
                {
                    faiou = true;
                    spawn.Amount = 1;
                }
            }

            from.Emote("* colhendo *");
            if (Skill < 40 || faiou)
            {
                from.SendMessage("Por ser inexperiente voce retirou a planta de forma errada");
                spawn.MoveToWorld(loc, map);
            } else
            {
                from.AddToBackpack(spawn);
            }

            Effects.PlaySound(this.Location, this.Map, 0x12E);
            this.m_Picked = true;
            this.Unlink();
            Timer.DelayCall(TimeSpan.FromMinutes(5.0), new TimerCallback(Delete));
        }

        public void Unlink()
        {
            ISpawner se = this.Spawner;

            if (se != null)
            {
                this.Spawner.Remove(this);
                this.Spawner = null;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write(this.m_Picked);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            switch ( version )
            {
                case 0:
                    this.m_Picked = reader.ReadBool();
                    break;
            }
            if (this.m_Picked)
            {
                this.Unlink();
                this.Delete();
            }
        }
    }
}
