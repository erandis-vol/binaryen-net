namespace Binaryen
{
    public enum UnaryOperator
    {
        ClzInt32,
        ClzInt64,
        CtzInt32,
        CtzInt64,
        PopcntInt32,
        PopcntInt64,

        NegFloat32,
        NegFloat64,
        AbsFloat32,
        AbsFloat64,
        CeilFloat32,
        CeilFloat64,
        FloorFloat32,
        FloorFloat64,
        TruncFloat32,
        TruncFloat64,
        NearestFloat32,
        NearestFloat64,
        SqrtFloat32,
        SqrtFloat64,
                                                                                                                                                                                                    // relational
        EqZInt32,
        EqZInt64,

        ExtendSInt32,
        ExtendUInt32,

        WrapInt64,

        TruncSFloat32ToInt32,
        TruncSFloat32ToInt64,
        TruncUFloat32ToInt32,
        TruncUFloat32ToInt64,
        TruncSFloat64ToInt32,
        TruncSFloat64ToInt64,
        TruncUFloat64ToInt32,
        TruncUFloat64ToInt64,

        ReinterpretFloat32,
        ReinterpretFloat64,

        ConvertSInt32ToFloat32,
        ConvertSInt32ToFloat64,
        ConvertUInt32ToFloat32,
        ConvertUInt32ToFloat64,
        ConvertSInt64ToFloat32,
        ConvertSInt64ToFloat64,
        ConvertUInt64ToFloat32,
        ConvertUInt64ToFloat64,

        PromoteFloat32,

        DemoteFloat64,

        ReinterpretInt32,
        ReinterpretInt64,

        ExtendS8Int32,
        ExtendS16Int32,
        ExtendS8Int64,
        ExtendS16Int64,
        ExtendS32Int64,
    }

    public enum BinaryOperator
    {
        AddInt32,
        SubInt32,
        MulInt32,

        DivSInt32,
        DivUInt32,
        RemSInt32,
        RemUInt32,
        AndInt32,
        OrInt32,
        XorInt32,
        ShlInt32,
        ShrUInt32,
        ShrSInt32,
        RotLInt32,
        RotRInt32,

        EqInt32,
        NeInt32,

        LtSInt32,
        LtUInt32,
        LeSInt32,
        LeUInt32,
        GtSInt32,
        GtUInt32,
        GeSInt32,
        GeUInt32,

        AddInt64,
        SubInt64,
        MulInt64,

        DivSInt64,
        DivUInt64,
        RemSInt64,
        RemUInt64,
        AndInt64,
        OrInt64,
        XorInt64,
        ShlInt64,
        ShrUInt64,
        ShrSInt64,
        RotLInt64,
        RotRInt64,

        EqInt64,
        NeInt64,

        LtSInt64,
        LtUInt64,
        LeSInt64,
        LeUInt64,
        GtSInt64,
        GtUInt64,
        GeSInt64,
        GeUInt64,

        AddFloat32,
        SubFloat32,
        MulFloat32,

        DivFloat32,
        CopySignFloat32,
        MinFloat32,
        MaxFloat32,

        EqFloat32,
        NeFloat32,

        LtFloat32,
        LeFloat32,
        GtFloat32,
        GeFloat32,

        AddFloat64,
        SubFloat64,
        MulFloat64,

        DivFloat64,
        CopySignFloat64,
        MinFloat64,
        MaxFloat64,

        EqFloat64,
        NeFloat64,

        LtFloat64,
        LeFloat64,
        GtFloat64,
        GeFloat64,
    }

    public enum HostOperator
    {
        PageSize,
        CurrentMemory,
        GrowMemory,
        HasFeature,
    }

    public enum AtomicOperator
    {
        Add,
        Sub,
        And,
        Or,
        Xor,
        Xchg
    }
}
