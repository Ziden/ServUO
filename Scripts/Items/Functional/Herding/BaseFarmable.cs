using System;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public abstract class BaseFarmable : Item
    {
        private bool m_Picked;

        public string nomeQuemPlantou;
        public long plantouQuando;

        public BaseFarmable(int itemID)
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

        public virtual BasePlantable GetSeed()
        {
            return null;
        }

        public BaseFarmable(Serial serial)
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
            if(this.nomeQuemPlantou != null && from.Name != this.nomeQuemPlantou)
            {
                var agora = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                var diferenca = (agora - plantouQuando) / 1000;



                if ( diferenca < 60 ) // nao passou X segundos
                {
                    from.SendGump(new ConfirmaRoubo((int)diferenca, from, loc, map, this));
                    return;
                } 
            }
            Colhe(from, loc, map);
        }

 
        public class ConfirmaRoubo : BaseConfirmGump
        {
            public int segundos;
            public Mobile from;
            public Point3D loc;
            public Map map;
            public BaseFarmable farm;

            public override string LabelString { get { return "Rouba-la o fara criminoso, ok ?"; } }
            public override string TitleString
            {
                get
                {
                    return "Esta planta ainda e protegida";
                }
            }

            public ConfirmaRoubo(int segundos, Mobile from, Point3D loc, Map map, BaseFarmable farm)
            {
                this.map = map;
                this.loc = loc;
                this.from = from;
                this.segundos = segundos;
                this.farm = farm;
            }

            public override void Confirm(Mobile from)
            {
                from.CriminalAction(false);
                farm.Colhe(from, loc, map);
            }
        }

        public void Colhe(Mobile from, Point3D loc, Map map)
        {
            this.ItemID = 0x1014;
            Item spawn = this.GetCropObject();

            var Skill = (int)from.Skills[SkillName.Herding].Value;
            var amt = new Random().Next(5) + Skill / 5;
            spawn.Amount = amt;

            var check = from.CheckSkill(SkillName.Herding, GetMinSkill(), GetMaxSkill());

            var faiou = false;
            if (!check)
            {
                if (Utility.Random(3) == 1)
                {
                    from.Emote("* tentou colher e estragou *");
                    Effects.PlaySound(this.Location, this.Map, 0x12E);
                    this.m_Picked = true;
                    this.Unlink();
                    Timer.DelayCall(TimeSpan.FromMinutes(5.0), new TimerCallback(Delete));
                }
                else
                {
                    faiou = true;
                    spawn.Amount = 1;
                }
            }

            from.Emote("* colhendo *");
            if (Skill < 35 || faiou)
            {
                from.SendMessage("Por ser inexperiente voce retirou a planta de forma errada");
                spawn.MoveToWorld(loc, map);
            }
            else
            {
                var seed = GetSeed();
                if(seed != null && Utility.Random(2)==1)
                {
                    from.AddToBackpack(seed);
                }
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

            writer.WriteEncodedInt(1); // version

            writer.Write(this.m_Picked);
            writer.Write(nomeQuemPlantou);
            writer.Write(plantouQuando);
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
                case 1:
                    this.m_Picked = reader.ReadBool();
                    this.nomeQuemPlantou = reader.ReadString();
                    this.plantouQuando = reader.ReadLong();
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
