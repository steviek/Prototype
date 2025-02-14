namespace C7GameData
{
    using System;
    using System.Collections.Generic;
    /**
     * The game map, at the top level.
     */
    public class GameMap
    {
        public int numTilesWide { get; private set; }
        public int numTilesTall { get; private set; }
        bool wrapHorizontally, wrapVertically;

        public List<Tile> tiles {get;}

        public GameMap()
        {
            this.tiles = new List<Tile>();
        }

        public int tileCoordsToIndex(int x, int y)
        {
            return y * numTilesWide/2 + (y%2 == 0 ? x/2 : (x-1)/2);
        }

        public void tileIndexToCoords(int index, out int x, out int y)
        {
            int doubleRow = index / numTilesWide;
            int doubleRowRem = index % numTilesWide;
            if (doubleRowRem < numTilesWide/2) {
                x = 2 * doubleRowRem;
                y = 2 * doubleRow;
            } else {
                x = 1 + 2 * (doubleRowRem - numTilesWide/2);
                y = 2 * doubleRow + 1;
            }
        }

        // This method verifies that the conversion between tile index and coords is consistent for all possible valid inputs. It's not called
        // anywhere but I'm keeping it around in case we ever need to work on the conversion methods again.
        public void testTileIndexComputation()
        {
            for (int y = 0; y < numTilesTall; y++)
                for (int x = y%2; x < numTilesWide; x += 2) {
                    int rx, ry;
                    int index = tileCoordsToIndex(x, y);
                    tileIndexToCoords(index, out rx, out ry);
                    if ((rx != x) || (ry != y))
                        throw new System.Exception(String.Format("Error computing tile index/coords: ({0}, {1}) -> {2} -> ({3}, {4})", x, y, index, rx, ry));
                }

            for (int i = 0; i < numTilesWide * numTilesTall / 2; i++) {
                int x, y;
                tileIndexToCoords(i, out x, out y);
                int ri = tileCoordsToIndex(x, y);
                if (ri != i)
                    throw new System.Exception(String.Format("Error computing tile index/coords: {0} -> ({1}, {2}) -> {3}", i, x, y, ri));
            }
        }

        public bool isTileAt(int x, int y)
        {
            bool evenRow = y%2 == 0;
            bool xInBounds; {
                if (wrapHorizontally)
                    xInBounds = true;
                else if (evenRow)
                    xInBounds = (x >= 0) && (x <= numTilesWide - 2);
                else
                    xInBounds = (x >= 1) && (x <= numTilesWide - 1);
            }
            bool yInBounds = wrapVertically || ((y >= 0) && (y < numTilesTall));
            return xInBounds && yInBounds && (evenRow ? (x%2 == 0) : (x%2 != 0));
        }

        public Tile tileAt(int x, int y)
        {
            if (isTileAt(x, y))
                return tiles[tileCoordsToIndex(x, y)];
            else
                return null; // TODO: Consider using empty tile object instead of null
        }

        /**
         * Another temporary method.  Puppeteer has a better map in the UI.  This just generates a boring, but functional, map.
         **/
        public static GameMap generateDummyGameMap()
        {
            TerrainType grassland = new TerrainType();
            grassland.name = "Grassland";
            grassland.baseFoodProduction = 2;
            grassland.baseShieldProduction = 1; //with only one terrain type, it needs to be > 0
            grassland.baseCommerceProduction = 1;   //same as above
            grassland.movementCost = 1;

            GameMap dummyMap = new GameMap();
            dummyMap.numTilesTall = 80;
            dummyMap.numTilesWide = 80;

            //Uh, right, isometic.  That means we have to stagger things.
            //Also I forget how to do ranges in C#, oh well.
            for (int y = 0; y < dummyMap.numTilesTall; y++)
            {
                int firstXCoordinate = 0;
                if (y % 2 == 1)
                {
                    firstXCoordinate = 1;
                }
                for (int x = firstXCoordinate; x < dummyMap.numTilesWide; x += 2)
                {
                    Tile newTile = new Tile();
                    newTile.xCoordinate = x;
                    newTile.yCoordinate = y;
                    newTile.terrainType = grassland;
                    dummyMap.tiles.Add(newTile);
                }
            }
            return dummyMap;
        }
        // Inputs: noise field width and height, bool whether noise should smoothly wrap X or Y
        // Actual fake-isometric map will have different shape, but for noise we'll go straight 2d matrix
        // NOTE: Apparently this OpenSimplex implementation doesn't do octaves, including persistance or lacunarity
        //  Might be able to implement them, use https://www.youtube.com/watch?v=MRNFcywkUSA&list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3&index=4 as reference
        // TODO: Parameterize octaves, persistence, scale/period; compare this generator to Godot's
        // NOTE: Godot's OpenSimplexNoise returns -1 to 1; this one seems to be from 0 to 1 like most Simplex/Perlin implementations
        public static double[,] tempMapGenPrototyping(int width, int height, bool wrapX = true, bool wrapY = false)
        {
            // TODO: I think my octaves implementation is broken; specifically it needs normalizing I think as additional octaves drive more extreme values
            int octaves = 1;
            double persistence = 0.5;
            // The public domain OpenSiplex implementation always
            //   seems to be 0 at 0,0, so let's offset from it.
            double originOffset = 1000;
            double scale = 0.03;
            double xRadius = (double)width / (System.Math.PI * 2);
            double yRadius = (double)height / (System.Math.PI * 2);
            OpenSimplexNoise noise = new OpenSimplexNoise();
            double[,] noiseField = new double[width, height];

            for (int x=0; x < width; x++)
            {
                double oX = originOffset + (scale * x);
                // Set up cX,cY to make one circle as a function of x
                double theta = ((double)x / (double)width) * (System.Math.PI * 2);
                double cX = originOffset + (scale * xRadius * System.Math.Sin(theta));
                double cY = originOffset + (scale * xRadius * System.Math.Cos(theta));
                for (int y=0; y < height; y++)
                {
                    double oY = originOffset + (scale * y);
                    // Set up ycX,ycY to make one circle as a function of y
                    double yTheta = ((double)y / (double)height) * (System.Math.PI * 2);
                    double ycX = originOffset + (scale * yRadius * System.Math.Sin(yTheta));
                    double ycY = originOffset + (scale * yRadius * System.Math.Cos(yTheta));

                    // No wrapping, just yoink values at scaled coordinates
                    if (!(wrapX || wrapY))
                    {
                        // noiseField[x,y] = noise.Evaluate(oX, oY);
                        for (int i=0;i<octaves;i++)
                        {
                            double offset = i * 1.5 * System.Math.Max(width, height) * scale;
                            noiseField[x,y] += (octaves - i) * persistence * noise.Evaluate(oX + offset, oY + offset);
                        }
                        continue;
                    }
                    // Bi-axis wrapping requires two extra dimensions and circling through each
                    if (wrapX && wrapY)
                    {
                        for (int i=0;i<octaves;i++)
                        {
                            double offset = i * 1.5 * System.Math.Max(width, height) * scale;
                            double a = cX + offset;
                            double b = cY + offset;
                            double c = ycX + offset;
                            double d = ycY + offset;
                            noiseField[x,y] += (octaves - i) * persistence * noise.Evaluate(a, b, c, d);
                        }
                        // Skip the below tests, go to next loop iteration
                        continue;
                    }
                    // Y wrapping as Y increments it instead traces a circle in a third dimension to match up its ends
                    if (wrapY)
                    {
                        for (int i=0;i<octaves;i++)
                        {
                            double offset = i * 1.5 * System.Math.Max(width, height) * scale;
                            double a = ycX + offset;
                            double b = ycY + offset;
                            double c = oX + offset;
                            noiseField[x,y] += (octaves - i) * persistence * noise.Evaluate(a, b, c);
                        }
                        continue;
                    }
                    // Similar to Y wrapping
                    if (wrapX)
                    {
                        for (int i=0;i<octaves;i++)
                        {
                            double offset = i * 1.5 * System.Math.Max(width, height) * scale;
                            double a = cX + offset;
                            double b = cY + offset;
                            double c = oY + offset;
                            noiseField[x,y] += (octaves - i) * persistence * noise.Evaluate(a, b, c);
                        }
                    }
                }
            }
            return noiseField;
        }
    }
}
