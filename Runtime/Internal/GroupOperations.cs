namespace TinyNaCl.Internal
{
    internal static class GroupOperations
    {
        public static void ge_p2_0(out GroupElementP2 h)
        {
            h.X = default;
            FieldOperations.fe_1(out h.Y);
            FieldOperations.fe_1(out h.Z);
        }

        /// <summary>
        /// r = p + q
        /// </summary>
        internal static void ge_add(out GroupElementP1P1 r, ref GroupElementP3 p, ref GroupElementCached q)
        {
            FieldOperations.fe_add(out r.X, ref p.Y, ref p.X);
            FieldOperations.fe_sub(out r.Y, ref p.Y, ref p.X);
            FieldOperations.fe_mul(out r.Z, ref r.X, ref q.YplusX);
            FieldOperations.fe_mul(out r.Y, ref r.Y, ref q.YminusX);
            FieldOperations.fe_mul(out r.T, ref q.T2d, ref p.T);
            FieldOperations.fe_mul(out r.X, ref p.Z, ref q.Z);
            FieldOperations.fe_add(out FieldElement t0, ref r.X, ref r.X);
            FieldOperations.fe_sub(out r.X, ref r.Z, ref r.Y);
            FieldOperations.fe_add(out r.Y, ref r.Z, ref r.Y);
            FieldOperations.fe_add(out r.Z, ref t0, ref r.T);
            FieldOperations.fe_sub(out r.T, ref t0, ref r.T);
        }

        private static void slide(sbyte[] r, byte[] a)
        {
            for (int i = 0; i < 256; ++i)
                r[i] = (sbyte)(1 & (a[i >> 3] >> (i & 7)));

            for (int i = 0; i < 256; ++i)
            {
                if (r[i] != 0)
                {
                    for (int b = 1; b <= 6 && (i + b) < 256; ++b)
                    {
                        if (r[i + b] != 0)
                        {
                            if (r[i] + (r[i + b] << b) <= 15)
                            {
                                r[i] += (sbyte)(r[i + b] << b);
                                r[i + b] = 0;
                            }
                            else if (r[i] - (r[i + b] << b) >= -15)
                            {
                                r[i] -= (sbyte)(r[i + b] << b);
                                for (int k = i + b; k < 256; ++k)
                                {
                                    if (r[k] == 0)
                                    {
                                        r[k] = 1;
                                        break;
                                    }
                                    r[k] = 0;
                                }
                            }
                            else
                                break;
                        }
                    }
                }
            }
        }

        public static void ge_double_scalarmult_vartime(out GroupElementP2 r, byte[] a, ref GroupElementP3 A, byte[] b)
        {
            GroupElementPreComp[] Bi = LookupTables.Base2;
            // todo: Perhaps remove these allocations?
            sbyte[] aslide = new sbyte[256];
            sbyte[] bslide = new sbyte[256];
            GroupElementCached[] Ai = new GroupElementCached[8];
            GroupElementP1P1 t;
            GroupElementP3 u;
            GroupElementP3 A2;
            int i;

            slide(aslide, a);
            slide(bslide, b);

            ge_p3_to_cached(out Ai[0], ref A);
            ge_p3_dbl(out t, ref A);
            ge_p1p1_to_p3(out A2, ref t);
            ge_add(out t, ref A2, ref Ai[0]);
            ge_p1p1_to_p3(out u, ref t);
            ge_p3_to_cached(out Ai[1], ref u);
            ge_add(out t, ref A2, ref Ai[1]);
            ge_p1p1_to_p3(out u, ref t);
            ge_p3_to_cached(out Ai[2], ref u);
            ge_add(out t, ref A2, ref Ai[2]);
            ge_p1p1_to_p3(out u, ref t);
            ge_p3_to_cached(out Ai[3], ref u);
            ge_add(out t, ref A2, ref Ai[3]);
            ge_p1p1_to_p3(out u, ref t);
            ge_p3_to_cached(out Ai[4], ref u);
            ge_add(out t, ref A2, ref Ai[4]);
            ge_p1p1_to_p3(out u, ref t);
            ge_p3_to_cached(out Ai[5], ref u);
            ge_add(out t, ref A2, ref Ai[5]);
            ge_p1p1_to_p3(out u, ref t);
            ge_p3_to_cached(out Ai[6], ref u);
            ge_add(out t, ref A2, ref Ai[6]);
            ge_p1p1_to_p3(out u, ref t);
            ge_p3_to_cached(out Ai[7], ref u);

            ge_p2_0(out r);

            for (i = 255; i >= 0; --i)
            {
                if ((aslide[i] != 0) || (bslide[i] != 0))
                    break;
            }

            for (; i >= 0; --i)
            {
                ge_p2_dbl(out t, ref r);

                if (aslide[i] > 0)
                {
                    ge_p1p1_to_p3(out u, ref t);
                    ge_add(out t, ref u, ref Ai[aslide[i] / 2]);
                }
                else if (aslide[i] < 0)
                {
                    ge_p1p1_to_p3(out u, ref t);
                    ge_sub(out t, ref u, ref Ai[(-aslide[i]) / 2]);
                }

                if (bslide[i] > 0)
                {
                    ge_p1p1_to_p3(out u, ref t);
                    ge_madd(out t, ref u, ref Bi[bslide[i] / 2]);
                }
                else if (bslide[i] < 0)
                {
                    ge_p1p1_to_p3(out u, ref t);
                    ge_msub(out t, ref u, ref Bi[(-bslide[i]) / 2]);
                }

                ge_p1p1_to_p2(out r, ref t);
            }
        }

        public static int ge_frombytes_negate_vartime(out GroupElementP3 h, byte[] data, int offset)
        {
            FieldOperations.fe_frombytes(out h.Y, data, offset);
            FieldOperations.fe_1(out h.Z);
            FieldOperations.fe_sq(out FieldElement u, ref h.Y);
            FieldOperations.fe_mul(out FieldElement v, ref u, ref LookupTables.d);
            FieldOperations.fe_sub(out u, ref u, ref h.Z);
            FieldOperations.fe_add(out v, ref v, ref h.Z);
            FieldOperations.fe_sq(out FieldElement v3, ref v);
            FieldOperations.fe_mul(out v3, ref v3, ref v);
            FieldOperations.fe_sq(out h.X, ref v3);
            FieldOperations.fe_mul(out h.X, ref h.X, ref v);
            FieldOperations.fe_mul(out h.X, ref h.X, ref u);
            FieldOperations.fe_pow22523(out h.X, ref h.X);
            FieldOperations.fe_mul(out h.X, ref h.X, ref v3);
            FieldOperations.fe_mul(out h.X, ref h.X, ref u);
            FieldOperations.fe_sq(out FieldElement vxx, ref h.X);
            FieldOperations.fe_mul(out vxx, ref vxx, ref v);
            FieldOperations.fe_sub(out FieldElement check, ref vxx, ref u);
            if (FieldOperations.fe_isnonzero(ref check) != 0)
            {
                FieldOperations.fe_add(out check, ref vxx, ref u);
                if (FieldOperations.fe_isnonzero(ref check) != 0)
                {
                    h = default;
                    return -1;
                }
                FieldOperations.fe_mul(out h.X, ref h.X, ref LookupTables.sqrtm1);
            }

            if (FieldOperations.fe_isnegative(ref h.X) == (data[offset + 31] >> 7))
                FieldOperations.fe_neg(out h.X, ref h.X);

            FieldOperations.fe_mul(out h.T, ref h.X, ref h.Y);
            return 0;
        }

        /// <summary>
        /// r = p + q
        /// </summary>
        public static void ge_madd(out GroupElementP1P1 r, ref GroupElementP3 p, ref GroupElementPreComp q)
        {
            FieldOperations.fe_add(out r.X, ref p.Y, ref p.X);
            FieldOperations.fe_sub(out r.Y, ref p.Y, ref p.X);
            FieldOperations.fe_mul(out r.Z, ref r.X, ref q.yplusx);
            FieldOperations.fe_mul(out r.Y, ref r.Y, ref q.yminusx);
            FieldOperations.fe_mul(out r.T, ref q.xy2d, ref p.T);
            FieldOperations.fe_add(out FieldElement t0, ref p.Z, ref p.Z);
            FieldOperations.fe_sub(out r.X, ref r.Z, ref r.Y);
            FieldOperations.fe_add(out r.Y, ref r.Z, ref r.Y);
            FieldOperations.fe_add(out r.Z, ref t0, ref r.T);
            FieldOperations.fe_sub(out r.T, ref t0, ref r.T);
        }

        /// <summary>
        /// r = p - q
        /// </summary>
        public static void ge_msub(out GroupElementP1P1 r, ref GroupElementP3 p, ref GroupElementPreComp q)
        {
            FieldOperations.fe_add(out r.X, ref p.Y, ref p.X);
            FieldOperations.fe_sub(out r.Y, ref p.Y, ref p.X);
            FieldOperations.fe_mul(out r.Z, ref r.X, ref q.yminusx);
            FieldOperations.fe_mul(out r.Y, ref r.Y, ref q.yplusx);
            FieldOperations.fe_mul(out r.T, ref q.xy2d, ref p.T);
            FieldOperations.fe_add(out FieldElement t0, ref p.Z, ref p.Z);
            FieldOperations.fe_sub(out r.X, ref r.Z, ref r.Y);
            FieldOperations.fe_add(out r.Y, ref r.Z, ref r.Y);
            FieldOperations.fe_sub(out r.Z, ref t0, ref r.T);
            FieldOperations.fe_add(out r.T, ref t0, ref r.T);
        }

        /// <summary>
        /// r = p
        /// </summary>
        public static void ge_p1p1_to_p2(out GroupElementP2 r, ref GroupElementP1P1 p)
        {
            FieldOperations.fe_mul(out r.X, ref p.X, ref p.T);
            FieldOperations.fe_mul(out r.Y, ref p.Y, ref p.Z);
            FieldOperations.fe_mul(out r.Z, ref p.Z, ref p.T);
        }

        /// <summary>
        /// r = p
        /// </summary>
        public static void ge_p1p1_to_p3(out GroupElementP3 r, ref GroupElementP1P1 p)
        {
            FieldOperations.fe_mul(out r.X, ref p.X, ref p.T);
            FieldOperations.fe_mul(out r.Y, ref p.Y, ref p.Z);
            FieldOperations.fe_mul(out r.Z, ref p.Z, ref p.T);
            FieldOperations.fe_mul(out r.T, ref p.X, ref p.Y);
        }

        ///<summary>
        /// r = 2 * p
        ///</summary>
        public static void ge_p2_dbl(out GroupElementP1P1 r, ref GroupElementP2 p)
        {
            FieldOperations.fe_sq(out r.X, ref p.X);
            FieldOperations.fe_sq(out r.Z, ref p.Y);
            FieldOperations.fe_sq2(out r.T, ref p.Z);
            FieldOperations.fe_add(out r.Y, ref p.X, ref p.Y);
            FieldOperations.fe_sq(out FieldElement t0, ref r.Y);
            FieldOperations.fe_add(out r.Y, ref r.Z, ref r.X);
            FieldOperations.fe_sub(out r.Z, ref r.Z, ref r.X);
            FieldOperations.fe_sub(out r.X, ref t0, ref r.Y);
            FieldOperations.fe_sub(out r.T, ref r.T, ref r.Z);
        }

        ///<summary>
        /// r = 2 * p
        ///</summary>
        public static void ge_p3_dbl(out GroupElementP1P1 r, ref GroupElementP3 p)
        {
            ge_p3_to_p2(out GroupElementP2 q, ref p);
            ge_p2_dbl(out r, ref q);
        }

        ///<summary>
        /// r = p
        ///</summary>
        public static void ge_p3_to_cached(out GroupElementCached r, ref GroupElementP3 p)
        {
            FieldOperations.fe_add(out r.YplusX, ref p.Y, ref p.X);
            FieldOperations.fe_sub(out r.YminusX, ref p.Y, ref p.X);
            r.Z = p.Z;
            FieldOperations.fe_mul(out r.T2d, ref p.T, ref LookupTables.d2);
        }

        ///<summary>
        /// r = p
        ///</summary>
        public static void ge_p3_to_p2(out GroupElementP2 r, ref GroupElementP3 p)
        {
            r.X = p.X;
            r.Y = p.Y;
            r.Z = p.Z;
        }

        ///<summary>
        /// r = p - q
        ///</summary>
        public static void ge_sub(out GroupElementP1P1 r, ref GroupElementP3 p, ref GroupElementCached q)
        {
            FieldOperations.fe_add(out r.X, ref p.Y, ref p.X);
            FieldOperations.fe_sub(out r.Y, ref p.Y, ref p.X);
            FieldOperations.fe_mul(out r.Z, ref r.X, ref q.YminusX);
            FieldOperations.fe_mul(out r.Y, ref r.Y, ref q.YplusX);
            FieldOperations.fe_mul(out r.T, ref q.T2d, ref p.T);
            FieldOperations.fe_mul(out r.X, ref p.Z, ref q.Z);
            FieldOperations.fe_add(out FieldElement t0, ref r.X, ref r.X);
            FieldOperations.fe_sub(out r.X, ref r.Z, ref r.Y);
            FieldOperations.fe_add(out r.Y, ref r.Z, ref r.Y);
            FieldOperations.fe_sub(out r.Z, ref t0, ref r.T);
            FieldOperations.fe_add(out r.T, ref t0, ref r.T);
        }

        public static void ge_tobytes(byte[] s, int offset, ref GroupElementP2 h)
        {
            FieldOperations.fe_invert(out FieldElement recip, ref h.Z);
            FieldOperations.fe_mul(out FieldElement x, ref h.X, ref recip);
            FieldOperations.fe_mul(out FieldElement y, ref h.Y, ref recip);
            FieldOperations.fe_tobytes(s, offset, ref y);
            s[offset + 31] ^= (byte)(FieldOperations.fe_isnegative(ref x) << 7);
        }
    }
}