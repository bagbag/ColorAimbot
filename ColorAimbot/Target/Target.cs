using AForge.Imaging;

namespace ColorAimbot.Target
{
    public struct Target
    {
        public TargetDescriptor targetDescriptor;
        public Blob targetBlob;

        public Target(TargetDescriptor targetDescriptor, Blob targetBlob)
        {
            this.targetDescriptor = targetDescriptor;
            this.targetBlob = targetBlob;
        }
    }
}
