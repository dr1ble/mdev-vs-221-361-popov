namespace ColorBox.Core
{
    public class GameColor
    {
        public int Id { get; }
        public string Name { get; }
        public ShapeType Shape { get; } // Свойство для типа фигуры

        public GameColor(int id, string name, ShapeType shape)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Shape = shape; // Инициализация фигуры
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is GameColor other && Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}