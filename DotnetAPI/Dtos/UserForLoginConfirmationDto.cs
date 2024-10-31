namespace DotnetAPI.Dtos
{
    public class UserForLoginConfirmationDto
    {
        public byte[] PasswordHash {get; set;}
        public byte[] PasswordSalt {get; set;}

        UserForLoginConfirmationDto() {
            if (PasswordHash == null) 
            {
                PasswordHash = new byte[0];
            }
            if (PasswordSalt == null) 
            {
                PasswordSalt = new byte[0];
            }

        }
    }
}