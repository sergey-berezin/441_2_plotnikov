namespace Genetic;

public static class MyRand {
    public static int GetRand(int start, int end) {
        Random rnd = new();
        return start + rnd.Next(end);
    }

    public static bool GetTrueWithProbability(int probability) {
        return GetRand(0, 100) < probability;
    }
}
