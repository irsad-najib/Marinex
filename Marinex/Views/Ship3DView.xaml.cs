using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Marinex.Models;

namespace Marinex.Views
{
    public partial class Ship3DView : Window
    {
        private Point _lastMousePosition;
        private bool _isRotating;
        private ShipPosition _shipData;

        public Ship3DView(ShipPosition shipData)
        {
            InitializeComponent();
            _shipData = shipData;
            
            if (_shipData != null)
            {
                TxtShipName.Text = _shipData.ShipName?.ToUpper() ?? "UNKNOWN VESSEL";
            }

            BuildShipModel();
        }

        private void BuildShipModel()
        {
            var modelGroup = new Model3DGroup();

            // Material Definitions
            var hullMaterial = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(45, 45, 45))); // Dark Grey Hull
            var deckMaterial = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(100, 20, 20))); // Red Deck
            var bridgeMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.WhiteSmoke)); // White Bridge
            var containerMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.Orange)); // Orange Containers
            var waterMaterial = new DiffuseMaterial(new SolidColorBrush(Color.FromArgb(100, 0, 100, 255))); // Transparent Blue Water

            // 1. Create Hull (Main Body)
            // Simple box: Length 20, Width 4, Height 3
            AddBox(modelGroup, new Point3D(0, 0, 0), 20, 3, 4, hullMaterial);

            // 2. Create Deck (Top of Hull)
            AddBox(modelGroup, new Point3D(0, 1.6, 0), 19.5, 0.2, 3.8, deckMaterial);

            // 3. Create Bridge/Superstructure (Rear)
            // Positioned at x=6 (back), y=3 (up)
            AddBox(modelGroup, new Point3D(6, 3, 0), 3, 4, 3.5, bridgeMaterial);
            // Bridge Window area (darker)
            AddBox(modelGroup, new Point3D(6, 4, 0), 3.1, 0.5, 3.6, new DiffuseMaterial(Brushes.Black));

            // 4. Create Cargo Containers (Front)
            // Stack 1
            AddBox(modelGroup, new Point3D(-2, 2.5, 0), 4, 2, 3, containerMaterial);
            // Stack 2
            AddBox(modelGroup, new Point3D(-7, 2.5, 0), 4, 2, 3, new DiffuseMaterial(Brushes.SteelBlue));

            // 5. Water Plane (Sea Surface)
            AddBox(modelGroup, new Point3D(0, -2, 0), 60, 0.1, 60, waterMaterial);

            // Add the whole group to the container
            var modelVisual = new ModelVisual3D();
            modelVisual.Content = modelGroup;
            ShipModelContainer.Children.Add(modelVisual);
        }

        // Helper to add a box to the model group
        private void AddBox(Model3DGroup group, Point3D center, double length, double height, double width, Material material)
        {
            var mesh = new MeshGeometry3D();
            
            double l = length / 2;
            double h = height / 2;
            double w = width / 2;

            Point3D[] corners = {
                new Point3D(-l, -h, -w), new Point3D(l, -h, -w), new Point3D(l, -h, w), new Point3D(-l, -h, w), // Bottom
                new Point3D(-l, h, -w), new Point3D(l, h, -w), new Point3D(l, h, w), new Point3D(-l, h, w)     // Top
            };

            // Define triangles for 6 faces
            // Top
            AddTriangle(mesh, corners[7], corners[6], corners[5]);
            AddTriangle(mesh, corners[5], corners[4], corners[7]);
            // Bottom
            AddTriangle(mesh, corners[0], corners[1], corners[2]);
            AddTriangle(mesh, corners[2], corners[3], corners[0]);
            // Front
            AddTriangle(mesh, corners[7], corners[4], corners[0]);
            AddTriangle(mesh, corners[0], corners[3], corners[7]);
            // Back
            AddTriangle(mesh, corners[5], corners[6], corners[2]);
            AddTriangle(mesh, corners[2], corners[1], corners[5]);
            // Left
            AddTriangle(mesh, corners[4], corners[5], corners[1]);
            AddTriangle(mesh, corners[1], corners[0], corners[4]);
            // Right
            AddTriangle(mesh, corners[6], corners[7], corners[3]);
            AddTriangle(mesh, corners[3], corners[2], corners[6]);

            // Center the geometry
            for (int i = 0; i < mesh.Positions.Count; i++)
            {
                mesh.Positions[i] = new Point3D(
                    mesh.Positions[i].X + center.X,
                    mesh.Positions[i].Y + center.Y,
                    mesh.Positions[i].Z + center.Z);
            }

            group.Children.Add(new GeometryModel3D(mesh, material));
        }

        private void AddTriangle(MeshGeometry3D mesh, Point3D p1, Point3D p2, Point3D p3)
        {
            int index1 = mesh.Positions.Count;
            mesh.Positions.Add(p1);
            mesh.Positions.Add(p2);
            mesh.Positions.Add(p3);
            
            mesh.TriangleIndices.Add(index1);
            mesh.TriangleIndices.Add(index1 + 1);
            mesh.TriangleIndices.Add(index1 + 2);
        }

        // Mouse Interaction for Rotation
        private void Viewport3D_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _isRotating = true;
                _lastMousePosition = e.GetPosition(MainViewport);
                MainViewport.CaptureMouse();
            }
        }

        private void Viewport3D_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isRotating)
            {
                Point currentPos = e.GetPosition(MainViewport);
                double dx = currentPos.X - _lastMousePosition.X;
                double dy = currentPos.Y - _lastMousePosition.Y;

                RotationY.Angle += dx * 0.5;
                RotationX.Angle += dy * 0.5;

                _lastMousePosition = currentPos;
            }
        }

        private void Viewport3D_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isRotating = false;
            MainViewport.ReleaseMouseCapture();
        }
    }
}

