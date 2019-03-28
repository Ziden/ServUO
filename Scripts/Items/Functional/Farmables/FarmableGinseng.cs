using System;

namespace Server.Items
{
    public class FarmableGinseng : FarmableCrop
    {
        [Constructable]
        public FarmableGinseng()
            : base(GetCropID())
        {
            this.Name = "Planta de Ginseng";
        }

        public FarmableGinseng(Serial serial)
            : base(serial)
        {
        }

        public static int GetCropID()
        {
            return 0x18E9;
        }

        public override Item GetCropObject()
        {
            return new Ginseng();
        }

        public override int GetMaxSkill()
        {
            return 70;
        }

        public override int GetMinSkill()
        {
            return 40;
        }

        public override int GetPickedID()
        {
            return 0x1014;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}
