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
            string newCont = fullname;
            if(newCont.Length > 55)
            {
                newCont = fullname.Substring(0, 55) + "...";
            }
            return $"[{id}] {newCont}, {age} -- {country}";
        }
        public string ActorCon()
        {
            string sep = "!";
            string con = id+sep+fullname+sep+country+sep+age;
            return con;
        }
        public static Actor Parse(string userToParse)
        {
            Actor actor = new Actor();
            string[] arr = userToParse.Split("!");
            actor.id = int.Parse(arr[0]);
            actor.fullname=arr[1];
            actor.country=arr[2];
            actor.age=int.Parse(arr[3]);
            return actor;
        }
    }
}