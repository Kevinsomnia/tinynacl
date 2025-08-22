namespace TinyNaCl.Internal
{
    internal struct GroupElementP2
    {
        public FieldElement X;
        public FieldElement Y;
        public FieldElement Z;
    };

    internal struct GroupElementP3
    {
        public FieldElement X;
        public FieldElement Y;
        public FieldElement Z;
        public FieldElement T;
    };

    internal struct GroupElementP1P1
    {
        public FieldElement X;
        public FieldElement Y;
        public FieldElement Z;
        public FieldElement T;
    };

    internal struct GroupElementPreComp
    {
        public FieldElement yplusx;
        public FieldElement yminusx;
        public FieldElement xy2d;

        public GroupElementPreComp(FieldElement yplusx, FieldElement yminusx, FieldElement xy2d)
        {
            this.yplusx = yplusx;
            this.yminusx = yminusx;
            this.xy2d = xy2d;
        }
    };

    internal struct GroupElementCached
    {
        public FieldElement YplusX;
        public FieldElement YminusX;
        public FieldElement Z;
        public FieldElement T2d;
    };
}
