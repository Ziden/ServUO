using System;

namespace Server.Items
{
    public class FarmableNightShade : FarmableCrop
    {
        [Constructable]
        public FarmableNightShade()
            : base(GetCropID())
        {
            this.Name = "Planta de Nightshade";
        }

        public FarmableNightShade(Serial serial)
            : base(serial)
        {
        }

        public static int GetCropID()
        {
            return 0x18E5;
        }

        public override Item GetCropObject()
        {
            return new Nightshade();
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
