// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

namespace BurnSystems.Direct2d.Example
{
    using SharpDX;
    using SharpDX.Direct2D1;
    using System;

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
