namespace Genetic;

public static class MyRand {
    public static readonly Random Rnd = new();
    public static int GetRand(int start, int end) {
        return start + Rnd.Next(end);
    }

    public static bool GetTrueWithProbability(int probability) {
        return GetRand(0, 100) < probability;
    }
}
