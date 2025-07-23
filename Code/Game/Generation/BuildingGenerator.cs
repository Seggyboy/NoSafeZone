public class BuildingGenerator
{
    private readonly Dictionary<int, Tile[,]> _world;
    private readonly int _width, _length, _height;
    public bool MapLoaded = false;

    public BuildingGenerator(int width, int length, int height, Dictionary<int, Tile[,]> world)
    {
        _width = width;
        _world = world;
        _length = length;
        _height = height;
    }

}