namespace Contact.Domain.ValueTypes
{
    public class Person
    {
        private readonly string _identifier;
        private readonly string _name;

        public string Identifier { get { return _identifier; } }
        public string Name { get { return _name; } }

        public Person(string identifier, string name)
        {
            _identifier = identifier;
            _name = name;
        }
    }
}
