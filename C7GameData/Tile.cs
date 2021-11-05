namespace C7GameData
{
    public class Tile
    {
        int xCoordinate;
        int yCoordinate;
        TerrainType terrainType;

        //One thing to decide is do we want to have a tile have a list of units on it,
        //or a unit have reference to the tile it is on, or both?
        //The downside of both is that both have to be updated (and it uses a miniscule amount
        //of memory for pointers), but I'm inclined to go with both since it makes it easy and
        //efficient to perform calculations, whether you need to know which unit on a tile
        //has the best defense, or which tile a unit is on when viewing the Military Advisor.
        Array<MapUnit> unitsOnTile;
    }
}