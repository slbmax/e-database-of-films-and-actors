namespace ClassLib
{
    public class Actor
    {
        public int id;
        public string fullname;
        public string country;
        public int age;
        public Film[] films;
        public Actor()
        {
            this.id = 0;
            this.fullname = "";
            this.country = "";
            this.age = 0;
        }
        public Actor(string fullName, string country, int age)
        {
            this.fullname = fullName;
            this.country = country;
            this.age = age;
        }
        public override string ToString()
        {
            return $"[{id}] {fullname}, {age} -- {country}";
        }
    }
}