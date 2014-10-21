namespace no.miles.at.Backend.Domain.ValueTypes
{
    public class Person
    {
        public readonly string Identifier;
        public readonly string Name;

        public Person(string identifier, string name)
        {
            Identifier = identifier;
            Name = name;
        }
    }
}
