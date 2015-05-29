using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphicsPractical2
{
    public class Matrix3
    {
        private float m11, m12, m13, m21, m22, m23, m31, m32, m33;

        public Matrix3(float M11, float M12, float M13, float M21, float M22, float M23, float M31, float M32, float M33)
        {
            m11 = M11; m12 = M12; m13 = M13;
            m21 = M21; m22 = M22; m23 = M23;
            m31 = M31; m32 = M32; m33 = M33;
        }

        public float M11
        {
            get { return m11; }
            set { m11 = value; }
        }

        public float M12
        {
            get { return m12; }
            set { m12 = value; }
        }

        public float M13
        {
            get { return m13; }
            set { m13 = value; }
        }

        public float M21
        {
            get { return m21; }
            set { m21 = value; }
        }

        public float M22
        {
            get { return m22; }
            set { m22 = value; }
        }

        public float M23
        {
            get { return m23; }
            set { m23 = value; }
        }

        public float M31
        {
            get { return m31; }
            set { m31 = value; }
        }

        public float M32
        {
            get { return m32; }
            set { m32 = value; }
        }

        public float M33
        {
            get { return m33; }
            set { m33 = value; }
        }

        public static Matrix3 Transpose(Matrix3 i)
        {
            return new Matrix3(i.M11, i.M21, i.M31, i.M12, i.M22, i.M32, i.M13, i.M23, i.M33);
        }

        public static Matrix3 Inverse(Matrix3 i)
        {
            float abs = (i.M11 * i.M22 * i.M33 + i.M12 * i.M23 * i.M31 + i.M13 * i.M21 * i.M32) - (i.M13 * i.M22 * i.M31 + i.M12 * i.M21 * i.M33 + i.M11 * i.M23 * i.M32);
            float a,b,c,d,e,f,g,h,j;

            a = (i.M22*i.M33) - (i.M23*i.M32);
            b = -((i.M21*i.M33) - (i.M23*i.M31));
            c = (i.M21*i.M32) - (i.M22*i.M32);
            d = -((i.M12*i.M33) - (i.M13*i.M32));
            e = (i.M11*i.M33) - (i.M13*i.M31);
            f = -((i.M11*i.M32) - (i.M12*i.M31));
            g = (i.M12*i.M23) - (i.M13*i.M22);
            h = -((i.M11*i.M23) - (i.M13*i.M21));
            j = (i.M11*i.M22) - (i.M12*i.M21);

            return new Matrix3(a/abs, b/abs, c/abs, d/abs, e/abs, f/abs, g/abs, h/abs, j/abs);
        }
    }
}
