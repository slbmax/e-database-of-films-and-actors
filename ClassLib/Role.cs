namespace ClassLib
{
    public class Role
    {
        public int id;
        public int actor_id;
        public int film_id;
        public Role()
        {
            this.id = 0;
            this.actor_id =0;
            this.film_id =0;
        }
        public string RoleCon()
        {
            string sep = "!-!-!-!-!";
            string con = id+sep+actor_id+sep+film_id;
            return con;
        }
        public static Role Parse(string userToParse)
        {
            string sep = "!-!-!-!-!";
            Role role = new Role();
            string[] arr = userToParse.Split(sep);
            role.id = int.Parse(arr[0]);
            role.actor_id = int.Parse(arr[1]);
            role.film_id = int.Parse(arr[2]);
            
            return role;
        }
    }
}