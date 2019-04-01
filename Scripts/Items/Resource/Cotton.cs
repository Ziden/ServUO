using System;
using Server.Targeting;

namespace Server.Items
{
    public class Cotton : Item, IDyable, ICommodity
    {
        [Constructable]
        public Cotton()
            : this(1)
        {
        }

        [Constructable]
        public Cotton(int amount)
            : base(0xDF9)
        {
            this.Stackable = true;
            this.Weight = 0.8;
            this.Amount = amount;
            this.Name = "Bolas de Algodao Fofinhas";
        }

        public Cotton(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description { get { return LabelNumber; } }
        bool ICommodity.IsDeedable { get { return true; } }

        public static void OnSpun(ISpinningWheel wheel, Mobile from, int hue)
        {
            Item item = new SpoolOfThread(6);
            item.Hue = hue;
            from.AddToBackpack(item);
            from.SendMessage("Voce colocou linho sua mochila");
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public bool Dye(Mobile from, DyeTub sender)
        {
            if (this.Deleted)
                return false;

            this.Hue = sender.DyedHue;

            return true;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.IsChildOf(from.Backpack))
            {
                from.SendMessage("Escolha uma roda de tecer");
                from.Target = new PickWheelTarget(this);
            }
        }

        private class PickWheelTarget : Target
        {
            private readonly Cotton m_Cotton;
            public PickWheelTarget(Cotton cotton)
                : base(3, false, TargetFlags.None)
            {
                this.m_Cotton = cotton;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (this.m_Cotton.Deleted)
                    return;

                if(this.m_Cotton.Amount < 5)
                {
                    from.SendMessage("Junte pelo menos 5 bolas de algodao");
                    return;
                }

                ISpinningWheel wheel = targeted as ISpinningWheel;

                if (wheel == null && targeted is AddonComponent)
                    wheel = ((AddonComponent)targeted).Addon as ISpinningWheel;

                if (wheel is Item)
                {
                    Item item = (Item)wheel;

                    if (!this.m_Cotton.IsChildOf(from.Backpack))
                    {
                        from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                    }
                    else if (wheel.Spinning)
                    {
                        from.SendMessage("Esta roda ja esta sendo usada");
                    }
                    else
                    {
                        this.m_Cotton.Consume(5);
                        wheel.BeginSpin(new SpinCallback(Cotton.OnSpun), from, this.m_Cotton.Hue);
                    }
                }
                else
                {
                    from.SendMessage("Use isto em uma roda de tecer"); // Use that on a spinning wheel.
                }
            }
        }
    }
}
