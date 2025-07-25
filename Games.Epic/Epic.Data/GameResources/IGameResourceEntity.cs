using System;

namespace Epic.Data.GameResources
{
    public interface IGameResourceEntity
    {
        Guid Id { get; }
        string Key { get; }
        string Name { get; }
        string IconUrl { get; }
        int Price { get; }
    }

    internal class MutableGameResourceEntity : IGameResourceEntity
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string IconUrl { get; set; }
        public int Price { get; set; } = 1;
    }
}
