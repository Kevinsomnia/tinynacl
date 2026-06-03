using System.Runtime.CompilerServices;

namespace TinyNaCl.Internal
{
    internal static class GroupOperations
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        private static void slide(sbyte[] r, byte[] a, int aOffset)
        {
            for (int i = 0; i < 256; ++i)
                r[i] = (sbyte)(1 & (a[aOffset + (i >> 3)] >> (i & 7)));

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

        public static void ge_double_scalarmult_vartime(out GroupElementP2 r, byte[] a, ref GroupElementP3 A, byte[] b, int bOffset)
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

            slide(aslide, a, 0);
            slide(bslide, b, bOffset);

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ge_p1p1_to_p2(out GroupElementP2 r, ref GroupElementP1P1 p)
        {
            FieldOperations.fe_mul(out r.X, ref p.X, ref p.T);
            FieldOperations.fe_mul(out r.Y, ref p.Y, ref p.Z);
            FieldOperations.fe_mul(out r.Z, ref p.Z, ref p.T);
        }

        /// <summary>
        /// r = p
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        public static void ge_p3_tobytes(byte[] s, int offset, ref GroupElementP3 h)
        {
            FieldOperations.fe_invert(out FieldElement recip, ref h.Z);
            FieldOperations.fe_mul(out FieldElement x, ref h.X, ref recip);
            FieldOperations.fe_mul(out FieldElement y, ref h.Y, ref recip);
            FieldOperations.fe_tobytes(s, offset, ref y);
            s[offset + 31] ^= (byte)(FieldOperations.fe_isnegative(ref x) << 7);
        }

        public static void ge_p3_0(out GroupElementP3 h)
        {
            h.X = default;
            FieldOperations.fe_1(out h.Y);
            FieldOperations.fe_1(out h.Z);
            h.T = default;
        }

        internal static void ge_precomp_0(out GroupElementPreComp h)
        {
            FieldOperations.fe_1(out h.yplusx);
            FieldOperations.fe_1(out h.yminusx);
            h.xy2d = default;
        }

        private static void cmov(ref GroupElementPreComp t, ref GroupElementPreComp u, int b)
        {
            FieldOperations.fe_cmov(ref t.yplusx, ref u.yplusx, b);
            FieldOperations.fe_cmov(ref t.yminusx, ref u.yminusx, b);
            FieldOperations.fe_cmov(ref t.xy2d, ref u.xy2d, b);
        }

        // Returns 1 if b == c, else 0. Inputs are signed bytes in range [-8, 8].
        private static byte equal(int b, int c)
        {
            uint x = (uint)(b ^ c);
            x -= 1;
            return (byte)((x >> 31) & 1);
        }

        // Returns 1 if b < 0, else 0.
        private static byte negative(int b)
        {
            return (byte)(((uint)b >> 31) & 1);
        }

        // Constant-time selection of LookupTables.Base[pos][|b|-1], with conditional negation if b<0.
        // b is in [-8, 8]. Result is identity when b == 0.
        private static void select(out GroupElementPreComp t, int pos, int b)
        {
            int bnegative = negative(b);
            int babs = b - (((-bnegative) & b) << 1);

            ge_precomp_0(out t);
            var row = LookupTables.Base[pos];
            cmov(ref t, ref row[0], equal(babs, 1));
            cmov(ref t, ref row[1], equal(babs, 2));
            cmov(ref t, ref row[2], equal(babs, 3));
            cmov(ref t, ref row[3], equal(babs, 4));
            cmov(ref t, ref row[4], equal(babs, 5));
            cmov(ref t, ref row[5], equal(babs, 6));
            cmov(ref t, ref row[6], equal(babs, 7));
            cmov(ref t, ref row[7], equal(babs, 8));

            GroupElementPreComp minust;
            minust.yplusx = t.yminusx;
            minust.yminusx = t.yplusx;
            FieldOperations.fe_neg(out minust.xy2d, ref t.xy2d);
            cmov(ref t, ref minust, bnegative);
        }

        /// <summary>
        /// h = a * B, where B is the Ed25519 base point and a is a 32-byte little-endian scalar.
        /// Constant-time with respect to a.
        /// </summary>
        public static void ge_scalarmult_base(out GroupElementP3 h, byte[] a, int aOffset)
        {
            // Decompose a into 64 signed 4-bit digits, e[i] in [-8, 7].
            sbyte[] e = new sbyte[64];
            for (int i = 0; i < 32; i++)
            {
                e[2 * i] = (sbyte)(a[aOffset + i] & 15);
                e[2 * i + 1] = (sbyte)((a[aOffset + i] >> 4) & 15);
            }
            sbyte carry = 0;
            for (int i = 0; i < 63; i++)
            {
                e[i] += carry;
                carry = (sbyte)((e[i] + 8) >> 4);
                e[i] -= (sbyte)(carry << 4);
            }
            e[63] += carry;

            GroupElementP1P1 r;
            GroupElementP2 s;
            GroupElementPreComp t;

            ge_p3_0(out h);

            // Odd digits: accumulate 16^(2i+1) * e[2i+1] * B.
            for (int i = 1; i < 64; i += 2)
            {
                select(out t, i / 2, e[i]);
                ge_madd(out r, ref h, ref t);
                ge_p1p1_to_p3(out h, ref r);
            }

            // h *= 16, so odd-digit contributions land at the right place.
            ge_p3_dbl(out r, ref h);
            ge_p1p1_to_p2(out s, ref r);
            ge_p2_dbl(out r, ref s);
            ge_p1p1_to_p2(out s, ref r);
            ge_p2_dbl(out r, ref s);
            ge_p1p1_to_p2(out s, ref r);
            ge_p2_dbl(out r, ref s);
            ge_p1p1_to_p3(out h, ref r);

            // Even digits.
            for (int i = 0; i < 64; i += 2)
            {
                select(out t, i / 2, e[i]);
                ge_madd(out r, ref h, ref t);
                ge_p1p1_to_p3(out h, ref r);
            }
        }
    }
}