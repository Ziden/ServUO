using System;

namespace Server.Items
{
    public class FarmableCotton : BaseFarmable
    {
        [Constructable]
        public FarmableCotton()
            : base(GetCropID())
        {
            this.Name = "Planta de Algodao";
        }

        public FarmableCotton(Serial serial)
            : base(serial)
        {
        }

        public override BasePlantable GetSeed()
        {
            return new CottonSeeds();
        }

        public static int GetCropID()
        {
            return Utility.Random(3153, 4);
        }

        public override Item GetCropObject()
        {
            return new Cotton();
        }

        public override int GetPickedID()
        {
            return 3254;
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
