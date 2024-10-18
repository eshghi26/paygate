namespace Common.Helper.Helper
{
    public static class RandomProvider
    {
        private static readonly ThreadLocal<Random> Random =
            new(() => new Random(Interlocked.Increment(ref _seed)));

        private static int _seed = Environment.TickCount;

        public static int Next(int minValue, int maxValue)
        {
            if (Random.Value == null)
                return Math.Abs(new Guid().GetHashCode());

            return Random.Value.Next(minValue, maxValue);
        }
    }
}
