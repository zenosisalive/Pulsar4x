﻿using System;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;
using Eto.Serialization.Xaml;
using Pulsar4X.ECSLib;
using Pulsar4X.ViewModel.SystemView;
using Pulsar4X.ViewModel;

namespace Pulsar4X.CrossPlatformUI.Views
{
    public class SystemMap_DrawableView : Drawable
    {
        SystemMap_DrawableVM _viewModel;
        private List<DrawableObject> _shapesList = new List<DrawableObject>();
        private List<OrbitRing> _orbitRings = new List<OrbitRing>();
        private Camera2D _camera;
        private bool IsMouseDown;
        private Point LastLoc;
        public Point LastOffset;
        public SystemMap_DrawableView()
        {
            XamlReader.Load(this);
            _camera = new Camera2D(this);
            this.MouseDown += SystemMap_DrawableView_MouseDown;
            this.MouseUp += SystemMap_DrawableView_MouseUp;
            this.MouseWheel += SystemMap_DrawableView_MouseWheel;
            this.MouseMove += SystemMap_DrawableView_MouseMove;

            IsMouseDown = false;
            LastLoc.X = -1;
            LastLoc.Y = -1;
        }

        private void SystemMap_DrawableView_MouseMove(object sender, MouseEventArgs e)
        {
            if(IsMouseDown == true)
            {
                //Point loc = (Point)e.Location - Size / 2;
                Point loc = (Point)(e.Location - LastLoc);
                LastOffset = loc;
                //_camera.ViewPortCenter += loc;
                //_camera.CenterOn(e);
                _camera.CenterOn(loc);
                //_camera.Offset(loc);
                Invalidate();
            }

            LastLoc = (Point)e.Location;
        }

        private void SystemMap_DrawableView_MouseWheel(object sender, MouseEventArgs e)
        {
            if ((int)e.Delta.Height == 1)
            {
                _camera.ZoomIn(Size);
                Invalidate();
            }
            else if ((int)e.Delta.Height == -1)
            {
                _camera.ZoomOut(Size);
                Invalidate();
            }
        }

        private void SystemMap_DrawableView_MouseUp(object sender, MouseEventArgs e)
        {
            IsMouseDown = false;
        }

        public void SetViewmodel(SystemMap_DrawableVM viewModel) 
        {
            _viewModel = viewModel;
            viewModel.PropertyChanged += ViewModel_PropertyChanged;

            _shapesList.Add(new DrawableObject(this, viewModel.BackGroundHud, _camera));


            SystemBodies_CollectionChanged();
        }

        private void SystemMap_DrawableView_MouseDown(object sender, MouseEventArgs e)
        {
            IsMouseDown = true;
        }

        private void SystemBodies_CollectionChanged()
        {
            List<DrawableObject> newShapelist = new List<DrawableObject>();

            foreach (var item in _viewModel.SystemBodies)
            {
                item.Icon.PropertyChanged += ViewModel_PropertyChanged;
                newShapelist.Add(new DrawableObject(this, item.Icon, _camera));

                //if (item.OrbitEllipse != null)
                //{
                //item.OrbitEllipse.PropertyChanged += ViewModel_PropertyChanged;
                //_shapesList.Add(new DrawableObject(this, item.OrbitEllipse, _camera));
                //}
                //if (item.SimpleOrbitEllipse != null)
                //{
                //    newShapelist.Add(new DrawableObject(this, item.SimpleOrbitEllipse, _camera));
                //}


                if (item.SimpleOrbitEllipseFading != null)
                {
                    item.SimpleOrbitEllipseFading.PropertyChanged += ViewModel_PropertyChanged;
                    newShapelist.Add(new DrawableObject(this, item.SimpleOrbitEllipseFading, _camera));
                }
            }
            _shapesList = newShapelist;

            Color ringColor = Colors.Wheat;
            List<OrbitRing> newOrbitList = new List<OrbitRing>();
            foreach (var item in _viewModel.OrbitalEntities)
            {
                OrbitRing ring = new OrbitRing(item, _camera);
                ring.PenColor = ringColor;
                PercentValue percent = new PercentValue();
                percent.Percent = 1.0f;
                ring.OrbitPercent = percent;
                newOrbitList.Add(ring);
            }
            _orbitRings = newOrbitList;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SystemMap_DrawableVM.SystemBodies))
                SystemBodies_CollectionChanged();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Colors.DarkBlue, e.ClipRectangle);

            foreach (var item in _shapesList)
            {
                item.DrawMe(e.Graphics);
            }
            foreach (var item in _orbitRings)
            {
                item.DrawMe(e.Graphics);
            }
        }
    }

    public class PathData
    {
        public Pen EtoPen { get; set; }
        public PenData PenData { get; set; }
        public GraphicsPath EtoPath { get; set; }
        public PathData(Pen etoPen, PenData penData, GraphicsPath etoPath)
        {
            EtoPen = etoPen;
            PenData = penData;
            EtoPath = etoPath;
        }
    }

    public class DrawableObject
    {

        private Drawable _parent;
        float _scale { get { return _objectData.Scale; } }
        private VectorGraphicDataBase _objectData;
        private Camera2D _camera;
        private List<PathData> _pathDataList = new List<PathData>();
        private List<TextData> _textData = new List<TextData>();
        public DrawableObject(Drawable parent, VectorGraphicDataBase objectInfo, Camera2D camera)
        {
            _parent = parent;
            _objectData = objectInfo;
            _camera = camera;
            foreach (var pathPenDataPair in _objectData.PathList)
            {
                GraphicsPath path = new GraphicsPath();



                if (_objectData is OrbitEllipseFading)
                {
                    ArcData arcData = (ArcData)pathPenDataPair.VectorShapes[0];
                    path.AddArc(arcData.X1, arcData.X2, arcData.Width * _scale, arcData.Height * _scale, arcData.StartAngle, arcData.SweepAngle);

                }

                else
                {
                    foreach (var shape in pathPenDataPair.VectorShapes)
                    {
                        if (shape is EllipseData)
                            path.AddEllipse(shape.X1, shape.Y1, shape.X2, shape.Y2);
                        else if (shape is LineData)
                            path.AddLine(shape.X1 * _scale, shape.Y1 * _scale, shape.X2 * _scale, shape.Y2 * _scale);
                        else if (shape is RectangleData)
                            path.AddRectangle(shape.X1, shape.Y1, shape.X2, shape.Y2);
                        else if (shape is ArcData)
                        {
                            ArcData arcData = (ArcData)shape;
                            path.AddArc(shape.X1, shape.Y1, shape.X2, shape.Y2, arcData.StartAngle, arcData.SweepAngle);
                        }
                        else if (shape is BezierData)
                        {
                            BezierData bezData = (BezierData)shape;
                            PointF start = new PointF(bezData.X1, bezData.Y1);
                            PointF end = new PointF(bezData.X2, bezData.Y2);
                            PointF control1 = new PointF(bezData.ControlX1, bezData.ControlY1);
                            PointF control2 = new PointF(bezData.ControlX2, bezData.ControlY2);
                            path.AddBezier(start, control1, control2, end);
                        }
                        else if (shape is TextData)
                        {
                            _textData.Add((TextData)shape);

                        }
                    }
                }


                Color iconcolor = new Color();
                iconcolor.Ab = pathPenDataPair.Pen.Alpha;
                iconcolor.Rb = pathPenDataPair.Pen.Red;
                iconcolor.Bb = pathPenDataPair.Pen.Blue;
                iconcolor.Gb = pathPenDataPair.Pen.Green;

                Pen pen = new Pen(iconcolor, pathPenDataPair.Pen.Thickness);

                PathData pathData = new PathData(pen, pathPenDataPair.Pen, path);
                _pathDataList.Add(pathData);
            }
        }


        private Pen UpdatePen(PenData penData, Pen penEto)
        {
            Color newColor = new Color();
            newColor.Ab = penData.Alpha;
            newColor.Rb = penData.Red;
            newColor.Bb = penData.Blue;
            newColor.Gb = penData.Green;

            penEto.Color = newColor;

            penEto.Color = newColor;
            penEto.Thickness = penData.Thickness;
            return penEto;
        }

        public void DrawMe(Graphics g)
        {
            foreach (var pathData in _pathDataList)
            {
                UpdatePen(pathData.PenData, pathData.EtoPen);

                g.SaveTransform();
                g.MultiplyTransform(_camera.GetViewProjectionMatrix());
                //g.MultiplyTransform(Matrix.FromRotationAt(_objectData.Rotation, _parent.Width * 0.5f, _parent.Height * 0.5f));
                //g.TranslateTransform(PosXViewAdjusted, PosYViewAdjusted);
                g.TranslateTransform(_objectData.PosX * _scale, _objectData.PosY * _scale);


                g.DrawPath(pathData.EtoPen, pathData.EtoPath);
                g.RestoreTransform();
            }
            Font lastFont = null;
            foreach (var item in _textData)
            {
                g.SaveTransform();
                g.MultiplyTransform(_camera.GetViewProjectionMatrix());
                //g.TranslateTransform(PosXViewAdjusted, PosYViewAdjusted);
                g.TranslateTransform(_objectData.PosX * _scale, _objectData.PosY * _scale);
                Font font = new Font(item.Font.FontFamily.ToString(), item.Y2);
                Color color = new Color(item.Color.R, item.Color.G, item.Color.B);
                g.DrawText(font, color, item.X1, item.X2, item.Text);

                g.RestoreTransform();

                lastFont = font;
            }
            if (lastFont != null)
            {
               g.SaveTransform();
               String Entry = String.Format("{0} {1}", _camera.WorldPosition.X, _camera.WorldPosition.Y);
               
               g.DrawText(lastFont, Colors.White, 10, 10, Entry);

                Entry = String.Format("{0} {1}", (_parent as SystemMap_DrawableView).LastOffset.X, (_parent as SystemMap_DrawableView).LastOffset.Y);
                g.DrawText(lastFont, Colors.White, 10, 30, Entry);
                g.RestoreTransform();
            }

        }
    }
}
