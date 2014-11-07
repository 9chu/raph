using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using raph.Language;

namespace raph
{
    /// <summary>
    /// 实现一个运行环境
    /// </summary>
    public class PaintRuntime
    {
        private Runtime _Runtime = new Runtime();

        private Bitmap _TargetBuffer = new Bitmap(640, 480);
        private Color _PixelColor = Color.Black;

        public delegate void OnRuntimeExceptionHandler(PaintRuntime Sender, RuntimeException e);
        public delegate void OnOutputTextHandler(PaintRuntime Sender, string Content);

        public event OnRuntimeExceptionHandler OnRuntimeException;
        public event OnOutputTextHandler OnOutputText;

        private void castTuple2ToFloat2(RuntimeValue Input, out double X, out double Y)
        {
            if (Input.ValueType != RuntimeValueType.Tuple)
                throw new ArgumentException();
            RuntimeValue.Tuple tTuple = (RuntimeValue.Tuple)Input;
            if (tTuple.Value.Length != 2)
                throw new ArgumentException();
            X = tTuple.Value[0].CastTo<double>();
            Y = tTuple.Value[1].CastTo<double>();
        }

        /// <summary>
        /// 目标缓冲区
        /// </summary>
        public Bitmap TargetBuffer
        {
            get
            {
                return _TargetBuffer;
            }
        }

        public void RunAST(ASTNode.StatementList Block)
        {
            try
            {
                _Runtime.ExecAST(Block);
            }
            catch (RuntimeException e)
            {
                if (OnRuntimeException != null)
                {
                    OnRuntimeException(this, e);
                }
            }
        }

        public PaintRuntime()
        {
            RuntimeContext tContext = _Runtime.RootContext;

            // 注册常量
            tContext.Register("origin", new RuntimeValue.Tuple(new RuntimeValue[] { new RuntimeValue.Digit(0), new RuntimeValue.Digit(0) }));
            tContext.Register("scale", new RuntimeValue.Tuple(new RuntimeValue[] { new RuntimeValue.Digit(1), new RuntimeValue.Digit(1) }));
            tContext.Register("rot", 0.0);

            // 注册函数
            tContext.Register("print", (ExternalFunctionHandler)delegate(RuntimeContext Context, RuntimeValue[] Args)
            {
                foreach (RuntimeValue o in Args)
                {
                    if (OnOutputText != null)
                        OnOutputText(this, o.DataToString());
                }
                return new RuntimeValue.None();
            });
            tContext.Register("draw", (ExternalFunctionHandler)delegate(RuntimeContext Context, RuntimeValue[] Args)
            {
                double tX = Args[0].CastTo<double>();
                double tY = Args[1].CastTo<double>();

                double tOriginX, tOriginY;
                double tScaleX, tScaleY;
                double tRotation = Context["rot"].CastTo<double>();

                // 获取环境变量
                try
                {
                    castTuple2ToFloat2(Context["origin"], out tOriginX, out tOriginY);
                }
                catch (ArgumentException)
                {
                    throw new InvalidCastException("local var \"origin\" must be a tuple(2).");
                }
                try
                {
                    castTuple2ToFloat2(Context["scale"], out tScaleX, out tScaleY);
                }
                catch (ArgumentException)
                {
                    throw new InvalidCastException("local var \"scale\" must be a tuple(2).");
                }

                // 计算
                tX *= tScaleX;
                tY *= tScaleY;
                if (tRotation != 0)
                {
                    double tAfterRotX = tX * Math.Cos(tRotation) + tY * Math.Sin(tRotation);
                    double tAfterRotY = tY * Math.Cos(tRotation) - tX * Math.Sin(tRotation);
                    tX = tAfterRotX;
                    tY = tAfterRotY;
                }
                tX += tOriginX;
                tY += tOriginY;

                if (!(tX >= _TargetBuffer.Width || tX < 0 || tY < 0 || tY >= _TargetBuffer.Height))
                    _TargetBuffer.SetPixel((int)tX, (int)tY, _PixelColor);
                return new RuntimeValue.None();
            }, 2);
            tContext.Register("setPixelAlpha", (ExternalFunctionHandler)delegate(RuntimeContext Context, RuntimeValue[] Args)
            {
                double tArg = Args[0].CastTo<double>();
                _PixelColor = Color.FromArgb((byte)Math.Min(tArg, 255), _PixelColor.R, _PixelColor.G, _PixelColor.B);
                return new RuntimeValue.None();
            }, 1);
            tContext.Register("setPixelRed", (ExternalFunctionHandler)delegate(RuntimeContext Context, RuntimeValue[] Args)
            {
                double tArg = Args[0].CastTo<double>();
                _PixelColor = Color.FromArgb(_PixelColor.A, (byte)Math.Min(tArg, 255), _PixelColor.G, _PixelColor.B);
                return new RuntimeValue.None();
            }, 1);
            tContext.Register("setPixelGreen", (ExternalFunctionHandler)delegate(RuntimeContext Context, RuntimeValue[] Args)
            {
                double tArg = Args[0].CastTo<double>();
                _PixelColor = Color.FromArgb(_PixelColor.A, _PixelColor.R, (byte)Math.Min(tArg, 255), _PixelColor.B);
                return new RuntimeValue.None();
            }, 1);
            tContext.Register("setPixelBlue", (ExternalFunctionHandler)delegate(RuntimeContext Context, RuntimeValue[] Args)
            {
                double tArg = Args[0].CastTo<double>();
                _PixelColor = Color.FromArgb(_PixelColor.A, _PixelColor.R, _PixelColor.G, (byte)Math.Min(tArg, 255));
                return new RuntimeValue.None();
            }, 1);
        }
    }
}
