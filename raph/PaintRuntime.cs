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
                _Runtime.ExecBlock(Block);
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
            // 注册函数
            _Runtime.RegisterIdentifier("print", (Runtime.NativeCallHandler)delegate(Runtime Context, object[] Args, int LineNumber)
            {
                foreach (object o in Args)
                {
                    if (OnOutputText != null)
                        OnOutputText(this, o.ToString());
                }

                return new Language.Runtime.None();
            });
            _Runtime.RegisterIdentifier("draw", (Runtime.NativeCallHandler)delegate(Runtime Context, object[] Args, int LineNumber)
            {
                Runtime.ArgCountCheckHelper("draw", Args, 2, LineNumber);
                double tX = Runtime.ArgCheckHelper<double>("draw", Args, 0, LineNumber);
                double tY = Runtime.ArgCheckHelper<double>("draw", Args, 1, LineNumber);

                Runtime.Vector2 tOrigin = Context.FetchIdentifier<Runtime.Vector2>("origin");
                Runtime.Vector2 tScale = Context.FetchIdentifier<Runtime.Vector2>("scale");
                double tRotation = Context.FetchIdentifier<double>("rot");
                tX *= tScale.x;
                tY *= tScale.y;
                if (tRotation != 0)
                {
                    double tAfterRotX = tX * Math.Cos(tRotation) + tY * Math.Sin(tRotation);
                    double tAfterRotY = tY * Math.Cos(tRotation) - tX * Math.Sin(tRotation);
                    tX = tAfterRotX;
                    tY = tAfterRotY;
                }
                tX += tOrigin.x;
                tY += tOrigin.y;

                if (!(tX >= _TargetBuffer.Width || tX < 0 || tY < 0 || tY >= _TargetBuffer.Height))
                {
                    _TargetBuffer.SetPixel((int)tX, (int)tY, _PixelColor);
                }
                return new Runtime.None();
            });
            _Runtime.RegisterIdentifier("setPixelAlpha", (Runtime.NativeCallHandler)delegate(Runtime Context, object[] Args, int LineNumber)
            {
                Runtime.ArgCountCheckHelper("setPixelAlpha", Args, 1, LineNumber);
                double tArg = Runtime.ArgCheckHelper<double>("setPixelAlpha", Args, 0, LineNumber);

                _PixelColor = Color.FromArgb((byte)Math.Min(tArg, 255), _PixelColor.R, _PixelColor.G, _PixelColor.B);
                return new Runtime.None();
            });
            _Runtime.RegisterIdentifier("setPixelRed", (Runtime.NativeCallHandler)delegate(Runtime Context, object[] Args, int LineNumber)
            {
                Runtime.ArgCountCheckHelper("setPixelRed", Args, 1, LineNumber);
                double tArg = Runtime.ArgCheckHelper<double>("setPixelRed", Args, 0, LineNumber);

                _PixelColor = Color.FromArgb(_PixelColor.A, (byte)Math.Min(tArg, 255), _PixelColor.G, _PixelColor.B);
                return new Runtime.None();
            });
            _Runtime.RegisterIdentifier("setPixelGreen", (Runtime.NativeCallHandler)delegate(Runtime Context, object[] Args, int LineNumber)
            {
                Runtime.ArgCountCheckHelper("setPixelGreen", Args, 1, LineNumber);
                double tArg = Runtime.ArgCheckHelper<double>("setPixelGreen", Args, 0, LineNumber);

                _PixelColor = Color.FromArgb(_PixelColor.A, _PixelColor.R, (byte)Math.Min(tArg, 255), _PixelColor.B);
                return new Runtime.None();
            });
            _Runtime.RegisterIdentifier("setPixelBlue", (Runtime.NativeCallHandler)delegate(Runtime Context, object[] Args, int LineNumber)
            {
                Runtime.ArgCountCheckHelper("setPixelBlue", Args, 1, LineNumber);
                double tArg = Runtime.ArgCheckHelper<double>("setPixelBlue", Args, 0, LineNumber);

                _PixelColor = Color.FromArgb(_PixelColor.A, _PixelColor.R, _PixelColor.G, (byte)Math.Min(tArg, 255));
                return new Runtime.None();
            });
            _Runtime.RegisterIdentifier("origin", new Runtime.Vector2 { x = 0, y = 0 });
            _Runtime.RegisterIdentifier("scale", new Runtime.Vector2 { x = 1, y = 1 });
            _Runtime.RegisterIdentifier("rot", 0.0);
        }
    }
}
