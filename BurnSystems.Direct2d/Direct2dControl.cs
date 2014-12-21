﻿// Copyright (c) 2010-2012 SharpDX - Alexandre Mutel
// 
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

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D10;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D10.Device1;
using FeatureLevel = SharpDX.Direct3D10.FeatureLevel;

namespace BurnSystems.Direct2d
{

    public class DPFCanvas : System.Windows.Controls.Image
    {
        private Device Device;
        private Texture2D RenderTarget;
        private DX10ImageSource D3DSurface;
        private readonly Stopwatch RenderTimer;
        private RenderTarget m_d2dRenderTarget;
        private SharpDX.Direct2D1.Factory m_d2dFactory;


        public DPFCanvas()
        {
            this.RenderTimer = new Stopwatch();
            this.Loaded += this.Window_Loaded;
            this.Unloaded += this.Window_Closing;
            this.Stretch = System.Windows.Media.Stretch.Fill;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DPFCanvas.IsInDesignMode)
                return;

            this.StartD3D();
            this.StartRendering();
        }

        private void Window_Closing(object sender, RoutedEventArgs e)
        {
            if (DPFCanvas.IsInDesignMode)
                return;

            this.StopRendering();
            this.EndD3D();
        }

        private void StartD3D()
        {
            this.Device = new Device(DriverType.Hardware, DeviceCreationFlags.BgraSupport, FeatureLevel.Level_10_0);

            this.D3DSurface = new DX10ImageSource();
            this.D3DSurface.IsFrontBufferAvailableChanged += OnIsFrontBufferAvailableChanged;

            this.CreateAndBindTargets();

            this.Source = this.D3DSurface;
        }

        private void EndD3D()
        {
            this.D3DSurface.IsFrontBufferAvailableChanged -= OnIsFrontBufferAvailableChanged;
            this.Source = null;

            Disposer.SafeDispose(ref this.m_d2dRenderTarget);
            Disposer.SafeDispose(ref this.m_d2dFactory);
            Disposer.SafeDispose(ref this.D3DSurface);
            Disposer.SafeDispose(ref this.RenderTarget);
            Disposer.SafeDispose(ref this.Device);
        }

        private void CreateAndBindTargets()
        {
            this.D3DSurface.SetRenderTargetDX10(null);

            Disposer.SafeDispose(ref this.m_d2dRenderTarget);
            Disposer.SafeDispose(ref this.m_d2dFactory);
            Disposer.SafeDispose(ref this.RenderTarget);

            int width = Math.Max((int)this.ActualWidth, 100);
            int height = Math.Max((int)this.ActualHeight, 100);

            Texture2DDescription colordesc = new Texture2DDescription
            {
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                Format = Format.B8G8R8A8_UNorm,
                Width = width,
                Height = height,
                MipLevels = 1,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                OptionFlags = ResourceOptionFlags.Shared,
                CpuAccessFlags = CpuAccessFlags.None,
                ArraySize = 1
            };

            this.RenderTarget = new Texture2D(this.Device, colordesc);

            Surface surface = this.RenderTarget.QueryInterface<Surface>();


            m_d2dFactory = new SharpDX.Direct2D1.Factory();
            var rtp = new RenderTargetProperties(new PixelFormat(Format.Unknown, AlphaMode.Premultiplied));
            m_d2dRenderTarget = new RenderTarget(m_d2dFactory, surface, rtp);

            this.D3DSurface.SetRenderTargetDX10(this.RenderTarget);
        }

        private void StartRendering()
        {
            if (this.RenderTimer.IsRunning)
                return;

            System.Windows.Media.CompositionTarget.Rendering += OnRendering;
            this.RenderTimer.Start();
        }

        private void StopRendering()
        {
            if (!this.RenderTimer.IsRunning)
                return;

            System.Windows.Media.CompositionTarget.Rendering -= OnRendering;
            this.RenderTimer.Stop();
        }

        private void OnRendering(object sender, EventArgs e)
        {
            if (!this.RenderTimer.IsRunning)
                return;

            this.Render();
            this.D3DSurface.InvalidateD3DImage();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            this.CreateAndBindTargets();
            base.OnRenderSizeChanged(sizeInfo);
        }

        private void Render()
        {
            SharpDX.Direct3D10.Device device = this.Device;
            if (device == null)
                return;

            Texture2D renderTarget = this.RenderTarget;
            if (renderTarget == null)
                return;

            int targetWidth = renderTarget.Description.Width;
            int targetHeight = renderTarget.Description.Height;

            device.Rasterizer.SetViewports(new Viewport(0, 0, targetWidth, targetHeight, 0.0f, 1.0f));

            var solidColorBrush = new SharpDX.Direct2D1.SolidColorBrush(m_d2dRenderTarget, SharpDX.Color.White);

            m_d2dRenderTarget.BeginDraw();

            m_d2dRenderTarget.Clear(Color.Black);

            var rnd = new Random();
            for (var n = 0; n < 5000; n++)
            {
                var p0 = new Vector2((float)rnd.NextDouble() * 500, (float)rnd.NextDouble() * 500);
                var p1 = new Vector2((float)rnd.NextDouble() * 500, (float)rnd.NextDouble() * 500);
                m_d2dRenderTarget.DrawLine(
                    p0, p1, solidColorBrush);
            }

            m_d2dRenderTarget.EndDraw();

            f++;
            if ((DateTime.Now - last).TotalSeconds > 1)
            {
                System.Diagnostics.Debug.WriteLine(f);
                f = 0;
                last = DateTime.Now;
            }

            device.Flush();
        }

        private DateTime last;
        private int f;

        private void OnIsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // this fires when the screensaver kicks in, the machine goes into sleep or hibernate
            // and any other catastrophic losses of the d3d device from WPF's point of view
            if (this.D3DSurface.IsFrontBufferAvailable)
                this.StartRendering();
            else
                this.StopRendering();
        }

        /// <summary>
        /// Gets a value indicating whether the control is in design mode
        /// (running in Blend or Visual Studio).
        /// </summary>
        public static bool IsInDesignMode
        {
            get
            {
                DependencyProperty prop = DesignerProperties.IsInDesignModeProperty;
                bool isDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;
                return isDesignMode;
            }
        }
    }
}