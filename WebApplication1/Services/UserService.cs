using Amazon.Util.Internal.PlatformServices;
using MongoDB.Driver;
using System.Xml.Linq;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class UserService
    {
        /*public User RegisterUser(string username, string password, string email)
        {
            User newUser = new User
            {
                Name = username,
                Password = password,
                Email = email
            };

            return newUser;
        }*/

        private readonly IMongoCollection<User> _users;

        public UserService(IUsersDatabaseSettings settings)
        {

            var client = new MongoClient();
            var database = client.GetDatabase();

            _users = database.GetCollection<User>();

        }

        //METODOS PADROES
            //READ
        public List<User> Get() => _users.Find(user => true).ToList();

        public User GetByName(string name) => _users.Find(user => user.Name == name).FirstOrDefault();

        public User GetByEmail(string email) => _users.Find(user => user.Email == email).FirstOrDefault();

        //CREATE
        public User Create(User user)
        {
            _users.InsertOne(user);
            return user;
        }

            //UPDATE

        public void Update(string name, User updatedUser) => _users.ReplaceOne(user => user.Name == name, updatedUser);

            //DELETE

        public void Delete(string name) => _users.DeleteOne(user => user.Name == name);

               
        //MEUS METODOS REGISTRATION E AUTH
            
            //Signup

        public User Signup(User thisUser)
        {
            thisUser.Password = BCrypt.Net.BCrypt.HashPassword(thisUser.Password);
            _users.InsertOne(thisUser);
            return thisUser;
        }

        // TODO [usersDTOs]
        public User Login(User thisUser)
        {
           
           var user = GetByEmail(thisUser.Email);
            bool isPasswordMatch = BCrypt.Net.BCrypt.Verify(thisUser.Password, user.Password);

            if(user == null || !isPasswordMatch)
            {
                return null;
            }
            
            return user;
            
        }


    }
}
