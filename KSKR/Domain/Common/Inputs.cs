using System;
using System.IO;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Newtonsoft.Json;

namespace Domain.Common
{
    public delegate void InputsUpdate();

    [JsonObject(MemberSerialization.OptIn)]
    public class Inputs
    {
        [JsonProperty]
        private LoadsVector _r;
        [JsonProperty]
        private double[] _u_, _u;
        [JsonProperty]
        private double _dt, _tk;
        [JsonProperty]
        private double[][] _m, _k;

        public Matrix<double> M
        {
            get { return _m == null ? null : DenseMatrix.OfRows(_m); }
            set
            {
                _m = value == null ? null : value.ToColumnArrays();
                if (OnInputsUpdate != null)
                    OnInputsUpdate();
            }
        }

        public Matrix<double> K
        {
            get { return _k == null ? null : DenseMatrix.OfRows(_k); }
            set
            {
                _k = value == null ? null : value.ToColumnArrays();
                if (OnInputsUpdate != null)
                    OnInputsUpdate();
            }
        }

        [JsonProperty]
        public double T0 { get; set; }

        public double Tk
        {
            get { return _tk; }
            set
            {
                _tk = value;
                if (OnInputsUpdate != null)
                    OnInputsUpdate();
            }
        }

        public double DeltaT
        {
            get { return _dt; }
            set
            {
                _dt = value;
                if (OnInputsUpdate != null)
                    OnInputsUpdate();
            }
        }

        [JsonProperty]
        public double Alpha { get; set; }

        [JsonProperty]
        public double Beta { get; set; }

        [JsonProperty]
        public double Teta { get; set; }

        public Matrix<double> C
        {
            get { return _m != null && _k != null ? Alpha * K + Beta * M : null; }
        }

        public Vector<double> MovementU
        {
            get { return _u == null ? null : DenseVector.OfEnumerable(_u); }
            set
            {
                _u = value == null ? null : value.ToArray();
                if (OnInputsUpdate != null)
                    OnInputsUpdate();
            }
        }

        public Vector<double> SpeedU
        {
            get { return _u_ == null ? null : DenseVector.OfEnumerable(_u_); }
            set
            {
                _u_ = value == null ? null : value.ToArray();
                if (OnInputsUpdate != null)
                    OnInputsUpdate();
            }
        }

        public LoadsVector R
        {
            get { return _r; }
            set
            {
                _r = value;
                if (OnInputsUpdate != null)
                    OnInputsUpdate();
            }
        }

        public event InputsUpdate OnInputsUpdate;

        public void Serrialize(string path)
        {
            var inpStr = JsonConvert.SerializeObject(this);

            File.WriteAllText(path, inpStr);
        }

        public static Inputs LoadObject(string path)
        {
            try
            {
                var objJson = File.ReadAllText(path);
                return string.IsNullOrEmpty(objJson) ? null : JsonConvert.DeserializeObject<Inputs>(objJson);
            }
            catch (Exception)
            {

                return null;
            }
        }
    }
}
