namespace DotnetAPI.Dtos
{
    class UserForLoginDto
    {
        string Email {get; set;}
        string Password {get; set;}

        public UserForLoginDto() {
            if (Email == null) {
                Email = "";
            }
            if (Password == null) {
                Password = "";
            }
        }
    }
}