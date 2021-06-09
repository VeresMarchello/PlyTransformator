using Media3D = System.Windows.Media.Media3D;
using SharpDX;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Windows.Input;
using System.IO;
using System.Windows.Media.Imaging;
using HelixToolkit.Wpf.SharpDX;
using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelixToolkit.Wpf.SharpDX.Assimp;
using System.Windows.Media;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using HelixToolkit.Wpf.SharpDX.Model;

namespace PlyTransformator
{
    class MainViewModel : INotifyPropertyChanged
    {
        private EffectsManager _effectsManager;
        private Camera _camera;

        private Media3D.Vector3D _directionalLightDirection;
        private Color4 _directionalLightColor;
        private Color4 _ambientLightColor;

        private MeshGeometry3D _ply1;
        private MeshGeometry3D _ply2;
        private MeshGeometry3D _ply3;

        private bool _isLoading;


        public MainViewModel()
        {
            _effectsManager = new DefaultEffectsManager();
            _camera = new PerspectiveCamera()
            {
                LookDirection = new Media3D.Vector3D(0, 0, -10),
                Position = new Media3D.Point3D(0, 0, 10),
                UpDirection = new Media3D.Vector3D(0, 1, 0),
                FarPlaneDistance = 10000000000000,
                NearPlaneDistance = 0.01f
            };

            _directionalLightDirection = new Media3D.Vector3D(-0, -0, -10);
            _directionalLightColor = SharpDX.Color.White;
            _ambientLightColor = new Color4(0f, 0f, 0f, 0f);

            var lineBuilder = new LineBuilder();
            lineBuilder.AddLine(new Vector3(0, 0, 0), new Vector3(1000, 0, 0));
            lineBuilder.AddLine(new Vector3(0, 0, 0), new Vector3(0, 1000, 0));
            lineBuilder.AddLine(new Vector3(0, 0, 0), new Vector3(0, 0, 1000));

            AxisModel = lineBuilder.ToLineGeometry3D();
            AxisModel.Colors = new Color4Collection(AxisModel.Positions.Count)
            {
                Colors.Red.ToColor4(),
                Colors.Red.ToColor4(),
                Colors.Green.ToColor4(),
                Colors.Green.ToColor4(),
                Colors.Blue.ToColor4(),
                Colors.Blue.ToColor4()
            };

            IsLoading = true;
            _ply1 = ImportSTL("08315-11069_000");
            _ply2 = ImportSTL("part1_plexi");
            //_ply3 = ImportSTL("Madi_Cloud_mesh_center");
            IsLoading = false;

        }


        public EffectsManager EffectsManager
        {
            get { return _effectsManager; }
            set
            {
                _effectsManager = value;
                NotifyPropertyChanged();
            }
        }
        public Camera Camera
        {
            get { return _camera; }
            set
            {
                _camera = value;
                NotifyPropertyChanged();
            }
        }

        public Media3D.Vector3D DirectionalLightDirection
        {
            get { return _directionalLightDirection; }
            set
            {
                _directionalLightDirection = value;
                NotifyPropertyChanged();
            }
        }
        public Color4 DirectionalLightColor
        {
            get { return _directionalLightColor; }
            set
            {
                _directionalLightColor = value;
                NotifyPropertyChanged();
            }
        }
        public Color4 AmbientLightColor
        {
            get { return _ambientLightColor; }
            set
            {
                _ambientLightColor = value;
                NotifyPropertyChanged();
            }
        }

        public MeshGeometry3D Ply1
        {
            get { return _ply1; }
            set
            {
                _ply1 = value;
                NotifyPropertyChanged();
            }
        }
        public MeshGeometry3D Ply2
        {
            get { return _ply2; }
            set
            {
                _ply2 = value;
                NotifyPropertyChanged();
            }
        }
        public MeshGeometry3D Ply3
        {
            get { return _ply3; }
            set
            {
                _ply3 = value;
                NotifyPropertyChanged();
            }
        }

        public PhongMaterial Material { get; private set; } = PhongMaterials.Gray;
        private Media3D.Transform3D _ply1Transform;
        private Media3D.Transform3D _ply2Transform;
        private Media3D.Transform3D _ply3Transform;
        public Media3D.Transform3D Ply1Transform
        {
            get { return _ply1Transform; }
            set
            {
                _ply1Transform = value;
                NotifyPropertyChanged();
            }
        }
        public Media3D.Transform3D Ply2Transform
        {
            get { return _ply2Transform; }
            set
            {
                _ply2Transform = value;
                NotifyPropertyChanged();
            }
        }
        public Media3D.Transform3D Ply3Transform
        {
            get { return _ply3Transform; }
            set
            {
                _ply3Transform = value;
                NotifyPropertyChanged();
            }
        }

        private Element3D _target;
        public Element3D Target
        {
            get { return _target; }
            set
            {
                _target = value;
                NotifyPropertyChanged();
            }
        }
        private Vector3 _centerOffset;
        public Vector3 CenterOffset
        {
            get { return _centerOffset; }
            set
            {
                _centerOffset = value;
                NotifyPropertyChanged();
            }
        }

        public LineGeometry3D AxisModel { get; private set; }

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                NotifyPropertyChanged();
            }
        }


        public MeshGeometry3D ImportSTL(string fileName)
        {
            var list = new List<Object3D>();

            //Task.Run(() =>
            //{
            var reader = new StLReader();
            list = reader.Read($"../../PLYs/{ fileName }.stl");
            //var loader = new Importer();
            //return loader.Load($"../../PLYs/{ fileName }.stl");
            //}, TaskScheduler.FromCurrentSynchronizationContext());

            //Task.Run(() =>
            //{
            //    var reader = new StLReader();
            //    return reader.Read($"../../PLYs/{ fileName }.stl");
            //    //var loader = new Importer();
            //    //return loader.Load($"../../PLYs/{ fileName }.stl");
            //}).ContinueWith((result) =>
            //{
            //    IsLoading = false;
            //    var scene = result.Result;
            //    if (result.IsCompleted)
            //    {
            //        foreach (var node in scene.Root.Traverse())
            //        {
            //            Console.WriteLine("item");
            //            if (node is MeshNode m)
            //            {
            //                Console.WriteLine(true);
            //                var object3D = new Object3D() { Geometry = m.Geometry, Material = m.Material, Transform = new List<SharpDX.Matrix>() };
            //                object3D.Transform.Add(m.ModelMatrix);
            //                list.Add(object3D);
            //                Console.WriteLine("-----" + list.Count);
            //            }
            //        }
            //    }

            //}, TaskScheduler.FromCurrentSynchronizationContext());

            return list[0].Geometry as MeshGeometry3D;
        }

        public void MouseDown3DHandler(object sender, MouseDown3DEventArgs e)
        {
            var originalEvent = e.OriginalInputEventArgs as MouseButtonEventArgs;
            var pressedMouseButton = originalEvent.ChangedButton;
            if (e.HitTestResult != null && e.HitTestResult.ModelHit is MeshGeometryModel3D m && (m.Geometry == Ply1 || m.Geometry == Ply2 || m.Geometry == Ply3))
            {
                if (pressedMouseButton != MouseButton.Left)
                {
                    return;
                }
                Target = null;
                CenterOffset = m.Bounds.Center;
                var centerOffsetWithTransform = m.BoundsWithTransform.Center;
                Target = e.HitTestResult.ModelHit as Element3D;
                Camera.Position = new Media3D.Point3D(centerOffsetWithTransform.X, centerOffsetWithTransform.Y, centerOffsetWithTransform.Z + 100);
                Camera.LookDirection = new Media3D.Vector3D(0, 0, -100);
                NotifyPropertyChanged("Camera");
            }
        }

        //public void MouseUp3DHandler(object sender, MouseUp3DEventArgs e)
        //{
        //    var originalEvent = e.OriginalInputEventArgs as MouseButtonEventArgs;
        //    var pressedMouseButton = originalEvent.ChangedButton;
        //    if (pressedMouseButton != MouseButton.Left)
        //    {
        //        return;
        //    }
        //}

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string info = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
