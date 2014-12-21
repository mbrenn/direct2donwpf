using SharpDX;
using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BurnSystems.Direct2d.Example
{
    public class RandomLinesControl : Direct2dControl
    {

        /// <summary>
        /// Does the actual rendering. 
        /// BeginDraw and EndDraw are already called by the caller. 
        /// </summary>
        public override void Render(RenderTarget target)
        {
            var width = this.ActualWidth;
            var height = this.ActualHeight;
            target.Clear(Color.Black);
            var solidColorBrush1 = new SharpDX.Direct2D1.SolidColorBrush(target, SharpDX.Color.Blue);
            var solidColorBrush2 = new SharpDX.Direct2D1.SolidColorBrush(target, SharpDX.Color.Red);
            var rnd = new Random();
            for (var n = 0; n < 5000; n++)
            {
                var p0 = new Vector2((float)(rnd.NextDouble() * width), (float)(rnd.NextDouble() * height));
                var p1 = new Vector2((float)(rnd.NextDouble() * width), (float)(rnd.NextDouble() * height));
                target.DrawLine(
                    p0, 
                    p1, 
                    n % 2 == 1 ? solidColorBrush1 : solidColorBrush2, 
                    2);
            }
        }
    }
}
