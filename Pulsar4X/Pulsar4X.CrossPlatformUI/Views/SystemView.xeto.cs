﻿using Eto.Drawing;
using Eto.Forms;
using Eto.Serialization.Xaml;
using OpenTK;
using OpenTK.Graphics;
using Pulsar4X.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

//#TODO Move the actual rendering stuff out into a partial and separate it from the VMs
//I might also want to make windows render with DirectX so that it plays nicer with wpf, 
//so we'll have to see if we can abstract that too
namespace Pulsar4X.CrossPlatformUI.Views {
	public class SystemView : Panel {
		protected Panel RenderCanvasLocation;
		protected RenderCanvas RenderCanvas;
		private UITimer timDraw;

		#region Map Movement Buttons
		protected Button btn_up;
		protected Button btn_down;
		protected Button btn_left;
		protected Button btn_right;
		protected Button btn_zoom_in;
		protected Button btn_zoom_out;
		protected Button btn_min_zoom;
		#endregion

		protected ListBox systems;

		protected SystemVM CurrentSystem;
		public RenderVM RenderVM { get; set; }

		private bool mouse_held = false;
		private bool continue_drag = false;
		private Vector2 mouse_held_position;
		private Vector2 mouse_released_position;
		private const float mouse_move_threshold = 20f;
		
		private OpenGLRenderer Renderer;

		public SystemView(GameVM GameVM) {
			RenderVM = new RenderVM();
			Renderer = new OpenGLRenderer(RenderVM);
			DataContext = GameVM;
			RenderCanvas = new RenderCanvas(GraphicsMode.Default, 3, 3, GraphicsContextFlags.Default);
			XamlReader.Load(this);

			//SetupAllTheButtons();
			
			systems.BindDataContext(s => s.DataStore, (GameVM g) => g.StarSystems);
			systems.ItemTextBinding = Binding.Property((SystemVM vm) => vm.Name);
			systems.ItemKeyBinding = Binding.Property((SystemVM vm) => vm.ID).Convert((Guid ID) => ID.ToString());

			//direct binding - might need to be replaced later
			systems.Bind(s => s.SelectedValue, RenderVM, (RenderVM rvm) => rvm.ActiveSystem);

			RenderCanvas.GLInitalized += Initialize;
			RenderCanvas.GLDrawNow += DrawNow;
			RenderCanvas.GLShuttingDown += Teardown;
			RenderCanvas.GLResize += Resize;
			RenderCanvas.MouseMove += WhenMouseMove;
			RenderCanvas.MouseDown += WhenMouseDown;
			RenderCanvas.MouseUp += WhenMouseUp;
			RenderCanvas.MouseWheel += WhenMouseWheel;
			RenderCanvas.MouseLeave += WhenMouseLeave;

			RenderCanvasLocation.Content = RenderCanvas;

			Func<string,string> resource_for = s => $"Pulsar4X.CrossPlatformUI.Resources.{s}";

			btn_up.Image = Bitmap.FromResource(resource_for("AuroraButtons.up.png"));
			btn_up.Size = btn_up.Image.Size;

			btn_down.Image = Bitmap.FromResource(resource_for("AuroraButtons.down.png"));
			btn_down.Size = btn_down.Image.Size;

			btn_left.Image = Bitmap.FromResource(resource_for("AuroraButtons.left.png"));
			btn_left.Size = btn_left.Image.Size;

			btn_right.Image = Bitmap.FromResource(resource_for("AuroraButtons.right.png"));
			btn_right.Size = btn_right.Image.Size;

			btn_zoom_in.Image = Bitmap.FromResource(resource_for("AuroraButtons.zoom_in.png"));
			btn_zoom_in.Size = btn_zoom_in.Image.Size;

			btn_zoom_out.Image = Bitmap.FromResource(resource_for("AuroraButtons.zoom_out.png"));
			btn_zoom_out.Size = btn_zoom_out.Image.Size;
		}

		private void WhenMouseLeave(object sender, MouseEventArgs e) {
			e.Handled = true;
			mouse_held = false;
			continue_drag = false;
		}

		private void WhenMouseWheel(object sender, MouseEventArgs e) {
			e.Handled = true;
			RenderVM.UpdateCameraZoom((int)e.Delta.Height);
		}

		private void WhenMouseUp(object sender, MouseEventArgs e) {
			e.Handled = true;
			mouse_held = false;
			continue_drag = false;
		}

		private void WhenMouseDown(object sender, MouseEventArgs e) {
			e.Handled = true;
			mouse_held = true;
			mouse_held_position.X = e.Location.X;
			mouse_held_position.Y = e.Location.Y;
		}

		private void WhenMouseMove(object sender, MouseEventArgs e) {
			e.Handled = true;
			if (mouse_held) {
				Vector2 mouse_pos = new Vector2(e.Location.X, e.Location.Y);
				var delta = mouse_pos - mouse_held_position;
				if (delta.Length > mouse_move_threshold || continue_drag) {
					continue_drag = true;
					RenderVM.UpdateCameraPosition(delta);
					mouse_held_position = mouse_pos;
				}
			}
		}

		private void Draw() {
			RenderVM.drawPending = true;
		}

		public void Initialize(object sender, EventArgs e) {
			RenderCanvas.MakeCurrent();
			var bounds = RenderCanvas.Bounds;
			Renderer.Initialize(bounds.X, bounds.Y, bounds.Width, bounds.Height);

			//we need this to run on its own because we cant have rendering blocked by the
			//the rest of the system or waiting for an advance time command
			timDraw = new UITimer { Interval = 0.013 }; // Every Millisecond.
			timDraw.Elapsed += timDraw_Elapsed;
			timDraw.Start();
		}

		private void timDraw_Elapsed(object sender, EventArgs e) {
			if (!RenderVM.drawPending || !RenderCanvas.IsInitialized) {
				return;
			}

			RenderCanvas.MakeCurrent();

			Renderer.Draw(RenderVM);

			RenderCanvas.SwapBuffers();

			RenderVM.drawPending = false;
		}

		public void DrawNow(object sender, EventArgs e) {
			Draw();
			
		}

		public void Resize(object sender, EventArgs e) {
			RenderCanvas.MakeCurrent();
			var bounds = RenderCanvas.Bounds;
			RenderVM.Resize(bounds.Width, bounds.Height);
			Renderer.Resize(bounds.X, bounds.Y, bounds.Width, bounds.Height);
			RenderVM.drawPending = true;
		}

		public void Teardown(object sender, EventArgs e) {
			Renderer.Destroy();
		}
	}
}
