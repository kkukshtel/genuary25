//ported from https://github.com/lorenSchmidt/fractal_cell_noise
/*

    use like:
    var noise = new OctaviaNoise(seed: 88883);
    double noiseSample = noise.CellNoiseXY(xsize: 100, ysize:100, x: x, y: y);
*/
public class OctaviaNoise
{
    private readonly int[] noiseTable;
    private readonly int ns = 256;
    private readonly int ntSize;
    private readonly int ntSizeMinus1;
    private readonly int pcIncrement = 101159;
    private int pcSeed = 0;
    private double cHeight = 0;
    private int noiseSeed = 0;

    public OctaviaNoise(int seed = 88883)
    {
        ntSize = ns * ns;
        ntSizeMinus1 = ntSize - 1;
        noiseTable = InitializeNoiseTable(seed);
    }

    private int[] InitializeNoiseTable(int seed)
    {
        Random random = new Random(seed);
        List<int> list = new List<int>();
        int[] table = new int[ntSize];

        // Initialize list with sequential numbers
        for (int i = 0; i < ntSize; i++)
        {
            list.Add(i);
        }

        // Fill table with randomly drawn numbers
        for (int i = 0; i < ntSize; i++)
        {
            table[i] = DrawCard(list, random);
        }

        return table;
    }

    private int DrawCard(List<int> list, Random random)
    {
        if (list.Count == 0)
        {
            throw new InvalidOperationException("Cannot draw from empty list");
        }

        int index = random.Next(list.Count);
        int value = list[index];
        list.RemoveAt(index);
        return value;
    }

    private int Pos3Int(int x, int y, int seed)
    {
        long linear = ((x % ns) + (y % ns) * (long)ns + seed) % noiseTable.Length;
        return noiseTable[linear];
    }

    private int PrimeCycle()
    {
        int result = noiseTable[pcSeed % ntSize];
        pcSeed += pcIncrement;
        return result;
    }

    public double CurveStack2x2XY(
        double x, 
        double y, 
        int xsize = 256, 
        int ysize = 256, 
        int d = 1, 
        int seed = 0, 
        double softness = 1, 
        int samples = 4, 
        double bias = 0, 
        double range = 1)
    {
        x /= xsize;
        y /= xsize;
        
        int ix = (int)Math.Floor(x * d);
        int iy = (int)Math.Floor(y * d);
        int ti = 0; // random number table index
        int dm1 = d - 1; // for the bitwise & instead of % range trick

        double sum = 0;

        // Calculate quadrant-based sampling
        int left = ix - 1 + ((int)Math.Floor(x * 2 * d) & 1);
        int top = iy - 1 + ((int)Math.Floor(y * 2 * d) & 1);
        int right = left + 1;
        int bottom = top + 1;

        for (int cy = top; cy <= bottom; cy++)
        {
            for (int cx = left; cx <= right; cx++)
            {
                // Deterministic noise based on position
                ti = Pos3Int((cx + d) & dm1, (cy + d) & dm1, noiseSeed);

                for (int a = 0; a < samples; a++)
                {
                    double px = cx / (double)d + noiseTable[ti++ & ntSizeMinus1] / (double)ntSize / d;
                    double py = cy / (double)d + noiseTable[ti++ & ntSizeMinus1] / (double)ntSize / d;
                    double distanceSquared = d * d * (Math.Pow(x - px, 2) + Math.Pow(y - py, 2)) * 4;

                    double h = bias + -range + 2 * range * noiseTable[ti++ % ntSize] / (double)ntSize;

                    if (distanceSquared < 1.0)
                    {
                        double amp = (softness * (1 - distanceSquared) / (softness + distanceSquared));
                        amp *= amp;
                        sum += h * amp;
                    }
                }
            }
        }

        return sum;
    }

    public double CellNoiseXY(
        double x, 
        double y, 
        int xsize = 256, 
        int ysize = 256, 
        int density = 4, 
        int seed = 0,
        int octaves = 2, 
        double amplitudeRatio = 0.5, 
        double softness = 1, 
        int samples = 4, 
        double bias = 0, 
        double range = 1)
    {
        double surface = 0;
        
        for (int a = 0; a < octaves; a++)
        {
            int octaveSeed = noiseTable[seed % ntSize];
            seed += pcIncrement;
            
            double layer = CurveStack2x2XY(
                x, y, xsize, ysize, 
                density * (int)Math.Pow(2, a), 
                octaveSeed, softness, samples, bias, range);

            surface += Math.Pow(amplitudeRatio, a) * layer;
        }

        return 0.5 * surface;
    }
}