using Avalonia.Controls;
using Mapsui;
using Mapsui.UI.Avalonia;
using Mapsui.Tiling.Layers;
using BruTile.Web;
using BruTile.Predefined;
using Mapsui.Projections;
using Mapsui.Extensions;

namespace GisApp;

public partial class MainWindow : Window
{
    private Map _map = new();
    private MapControl? _mapControl;
    private TextBlock? _coordinatesTextBlock;
    
    public MainWindow()
    {
        InitializeComponent();
        InitializeMap();
        BindEvents();
        InitializeCoordinatePicker();
    }

    private void InitializeMap()
    {
        var tileSource = new HttpTileSource(new GlobalSphericalMercator(),
            "https://tile.openstreetmap.org/{z}/{x}/{y}.png",
            new[] { "a", "b", "c" },
            "© OpenStreetMap contributors");

        var osmLayer = new TileLayer(tileSource)
        {
            Name = "OpenStreetMap"
        };
        
        _map.Layers.Add(osmLayer);
        
        var coords = SphericalMercator.FromLonLat(116.3, 39.9);
        var beijing = new MPoint(coords.x, coords.y);
        _map.Home = n => n.CenterOnAndZoomTo(beijing, 10);
        
        if(this.FindControl<MapControl>("MapControl") is MapControl mapControl)
        {
            mapControl.Map = _map;
            _mapControl = mapControl;
            _map.Navigator.CenterOnAndZoomTo(beijing, 10);
        }
    }

    private void BindEvents()
    {
        if (this.FindControl<CheckBox>("BaseMapCheckBox") is CheckBox baseMapCheckBox)
        {
            baseMapCheckBox.Click += (s, e) =>
            {
                if (_map.Layers.Count > 0)
                    _map.Layers[0].Enabled = baseMapCheckBox.IsChecked ?? true;
            };
        }

        if (this.FindControl<Button>("ZoomInButton") is Button zoomInButton)
        {
            zoomInButton.Click += (s, e) =>
            {
                if (_mapControl?.Map != null)
                {
                    _mapControl.Map.Navigator.ZoomIn();
                }
            };
        }

        if (this.FindControl<Button>("ZoomOutButton") is Button zoomOutButton)
        {
            zoomOutButton.Click += (s, e) =>
            {
                if (_mapControl?.Map != null)
                {
                    _mapControl.Map.Navigator.ZoomOut();
                }
            };
        }
    }

    private void InitializeCoordinatePicker()
    {
        _coordinatesTextBlock = this.FindControl<TextBlock>("CoordinatesTextBlock");
        
        if (_mapControl != null)
        {
            _mapControl.PointerMoved += (s, e) =>
            {
                var screenPosition = e.GetPosition(_mapControl);
                var worldPosition = _mapControl.Map.Navigator.Viewport.ScreenToWorld(screenPosition.X, screenPosition.Y);
                var wgs84 = SphericalMercator.ToLonLat(worldPosition.X, worldPosition.Y);
                
                if (_coordinatesTextBlock != null)
                {
                    _coordinatesTextBlock.Text = $"经度: {wgs84.lon:F6}, 纬度: {wgs84.lat:F6}";
                }
            };
        }
    }
}